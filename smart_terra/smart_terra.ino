#include "DHT.h"
#include <Adafruit_NeoPixel.h>
#ifdef __AVR__
 #include <avr/power.h> // Required for 16 MHz Adafruit Trinket
#endif
#include <String.h>
#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <ArduinoJson.h>

#define DHT_TYPE DHT22
#define DHT_PIN 5
#define SOIL_MOISTURE_SENSOR_PIN A0
#define LED_PIN D6
#define LED_COUNT 60 
#define WATER_PUMP_PIN D5
#define LIQUID_LEVEL_SENSOR D7//?

String hex_value= "#f7f700";
bool if_water_pump_job_is_done;
bool is_liquid_level_sufficient;
//mode data:
const char* mode_humidity;
const char* mode_temperature;
const char* mode_twilight_hour;
const char* mode_hour_of_dawn;
//real data:
float heat_index_celsius;
float soil_moisture_percentage;
float humidity;
float temperature;
String LED_hex_color;
float LED_brightness;
unsigned long period;
unsigned long end_task_time;
unsigned long start_task_time;

int device_id=1;
const char* ssid = "UPCEA1369B";
const char* password = "uckKvpbZfzu3";

DHT dht(DHT_PIN, DHT_TYPE);
Adafruit_NeoPixel strip(LED_COUNT, LED_PIN, NEO_GRB + NEO_KHZ800);

//uruchamia się raz przy uruchomieniu
void setup() {
  Serial.begin(9600);
  delay(1000);
  Serial.println("Arduino started.");
  dht.begin();
  strip.begin();           // INITIALIZE NeoPixel strip object (REQUIRED)
  pinMode(LED_PIN, OUTPUT);    // sets the digital pin D6 as output
  pinMode(WATER_PUMP_PIN, OUTPUT);    // sets the digital pin D5 as output
  pinMode(LIQUID_LEVEL_SENSOR,INPUT);
  pinMode(SOIL_MOISTURE_SENSOR_PIN,INPUT);//nowe
  if_water_pump_job_is_done= false;
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(1000);
    Serial.println("Connecting...");
  }
  Serial.println("Connected");//->led on
  //dlaczego nie moge nic wypisac w setup:
  //https://forum.arduino.cc/index.php?topic=171889.0
}

//uruchamia się w nieskończonej pętli 
void loop() {
  /*
  period = 10000;//10sek
  end_task_time= millis()+period;
  //millis()- Number of milliseconds passed since the program started. Data type: unsigned long.
  start_task_time= millis();
  if(if_water_pump_job_is_done==false){
    turnon_water_pump(period, end_task_time, start_task_time );
  }*/
  //192.168.43.186
  if (WiFi.status() == WL_CONNECTED) 
  {
    get_mode_data();
    fetch_terrarium_data();
    check_liquid_level();
    //checkTerrariumData(modeData, terrariumData);
    //checkDeviceJobData();//sprawdzaj czy uzytkownik nie dodał ktoregos joba
  }
  delay(1000);// czekamy sekundę
}

void turnoff_water_pump(unsigned long wait_time);
void turnon_water_pump(unsigned long period, const unsigned long end_task_time, unsigned long start_task_time );
byte* color_converter(String hexValue);
void set_LED_color(String hex_value);
void fetch_dht22_sensor_data();
void fetch_soil_moisture_sensor_data();

void get_mode_data(){
  HTTPClient http;
  //http.begin("http://jsonplaceholder.typicode.com/users/1");
  //ip domowe:192.168.0.164
  http.begin("http://192.168.0.164:64624/api/devices/1/modes");
  int httpCode = http.GET();
  Serial.print("httpCode: ");
  Serial.println(httpCode);
  
  if (httpCode > 0) 
    {
      /*String payload = http.getString();
       Serial.println(payload);*/
      const size_t bufferSize = JSON_OBJECT_SIZE(3) + JSON_OBJECT_SIZE(4) + JSON_OBJECT_SIZE(5) + JSON_OBJECT_SIZE(6) + 370;
      /*ArduinoJson5:
      DynamicJsonBuffer jsonBuffer(bufferSize);
      JsonObject& root = jsonBuffer.parseObject(http.getString());*/
      DynamicJsonDocument doc(bufferSize);
      deserializeJson(doc, http.getString());
      //extract the data
      JsonObject root = doc.as<JsonObject>();
      
      mode_humidity = root["Humidity"]; 
      mode_temperature = root["Temperature"]; 
      mode_twilight_hour = root["TwilightHour"]; 
      mode_hour_of_dawn = root["HourOfDawn"]; 

      Serial.print("mode_humidity:");
      Serial.println(mode_humidity);
      Serial.print("mode_temperature:");
      Serial.println(mode_temperature);
      Serial.print("mode_twilight_hour:");
      Serial.println(mode_twilight_hour);
      Serial.print("mode_hour_of_dawn:");
      Serial.println(mode_hour_of_dawn);
    }
  http.end(); //Close connection
  delay(30UL * 60UL * 1000UL); //30 minutes each of 60 seconds each of 1000 milliseconds all unsigned long
}

void fetch_terrarium_data(){
  fetch_dht22_sensor_data();//temp, humidity
  fetch_soil_moisture_sensor_data();//soil moisture percentage
  String data;
  /*String data = "{\"Temperature\": " + String(temperature) + ", " + "\"Humidity\": " + String(humidity) + 
   "HeatIndex\": " + String(heat_index_celsius) + ", " + "\"SoilMoisturePercentage\": " + String(soil_moisture_percentage) +
   "LEDHexColor\": " + String(LED_hex_color) + ", " + "\"LEDBrightness\": " + String(LED_brightness)+ "}";*/
    
  //wywolaj funkcje PATCH: UpdateDeviceProperties(pobraneDane)
  HTTPClient http;    //Declare object of class HTTPClient
  http.begin("http://192.168.43.186:64624/api/DeviceProperties/1");//Specify request destination
  http.addHeader("Content-Type", "text/plain");  //Specify content-type header
  int httpCode = http.PATCH(data);   //Send the request
  String payload = http.getString(); //Get the response payload
  Serial.println(httpCode);   //Print HTTP return code
  Serial.println(payload);    //Print request response payload
  http.end();
}

void checkDeviceJobData(){
  /*1. odpytuj baze danych czy uzytkownik dodal joba, czyli sprawdz tabele deviceJobs->wywolaj metode GET: GetDeviceJobs(deviceId)
      1.1. jezeli jest null-> nie rob nic
      1.2. jezeli nie jest null:
        1.2.1. sprawdz Id joba
        1.2.2. w zaleznosci od Id joba wlacz odpowiednie narzedzia lub ustaw odpowiednie dane(kolor, pompka etc)
        1.2.3. przemyslec co w przypadku kiedy np wilgotnosc po wlaczeniu pompki bedzie za duza, albo powinna byc noc a ktos ustawi jasnosc na 100%?
              -> na czas trwania taska, wykonac bezwzglednie
              -> fukncja odpowiedzialna za wlaczanie pompki musi sprawdzac poziom wody, plus obliczac czas dzialania i czas przerw
   */
}

//wlacz odpowiednie urzadzenia w zaleznosci od wymaganych danych
void checkTerrariumData(bool modeData, bool terrariumDara){\
/*
    porownaj dane z tymi z moda->
      1. jezeli wilgotnosc jest za mala-> wlacz pompke
        { sprawdzaj co jakis czas czy wilgotnosc jest juz odpowiednia}
        1.1 jezeli za duza(dopuszczalna roznica, np +5%) -> w sumie lepiej zeby nie byla albo wlacz wiatraczki(dla lepszej cyrkulacji powietrza)*/
  
      /*2. jezeli temp za mala(dopuszczalna roznica, np -3st celsjusza) -> wlacz lampe grzewcza 
        { sprawdzaj co jakis czas czy temp jest juz odpwoiednia}
        2.2.1 jezeli za duza(dopuszczalna roznica, np +3st celsjusza) -> w sumie lepiej zeby nie byla albo wlacz wiatraczki(dla ochlodzenia i lepszej cyrkulacji powietrza)
      3. sprawdz dane odnosnie oswietlenia->
        swit ustawiony na godzine X(zaczyna sie rozjasniac) + zmierzch o godzinie Y(zaczyna sie sciemniac)
        3.1. trzeba wymyslic jak obliczyc jaka wartosc ma miec brightness w chwili wlaczenia terrarium!-> 
          3.1.1. sprawdzic czy jest wieksza czy mniejsza od X
          3.1.2 ...
        3.2. oblicz roznice bezwzgledna pomiedzy X a Y(Y-X) (dzien, brightness: 40%->100%)
          niech od godziny X->Y brightness zmienia sie co pol godziny o +[60/(godziny_dzienne*2)]
        3.3. oblicz ilosc godzin w nocy(24-dzien, brightness: 0%->40%)
          niech od godziny Y->X brightness zmienia sie co pol godziny o -[40/(godziny_wieczorne*2)]
    */
}
void send_liquid_level_sensor_data(){
  HTTPClient http;    //Declare object of class HTTPClient
  http.begin("http://192.168.43.186:64624/api/DeviceProperties/1/LiquidLevel");//Specify request destination
  http.addHeader("Content-Type", "text/plain");  //Specify content-type header
  int httpCode = http.PATCH("...");   //Send the request
  String payload = http.getString();                  //Get the response payload
  Serial.println(httpCode);   //Print HTTP return code
  Serial.println(payload);    //Print request response payload
  http.end();
}

void check_liquid_level(){
  is_liquid_level_sufficient=digitalRead(LIQUID_LEVEL_SENSOR);
  //dopoki poziom wody nie bedzie wystarczajacy to pompka bedzie wylaczona
  while(is_liquid_level_sufficient==0){
    digitalWrite(WATER_PUMP_PIN, HIGH);//OFF dla NO, pompka nie dziala dla NO
    send_liquid_level_sensor_data();//wyslij info do uzytkownika
    if(is_liquid_level_sufficient==1){
      //kiedy poziom wody bedzie odpowiedni wychodzimy z funkcji i wysyłamy dane
      send_liquid_level_sensor_data();
      break;
    }
    delay(10000);
  }
}

void turnoff_water_pump(unsigned long wait_time){
  digitalWrite(WATER_PUMP_PIN, HIGH);// OFF dla NO, pompka nie dziala dla NO
  delay(wait_time);
  digitalWrite(WATER_PUMP_PIN, LOW);// ON dla NO, pompka dziala dla NO
}

void turnon_water_pump(unsigned long period, const unsigned long end_task_time, unsigned long start_task_time ){
  //https://www.programmingelectronics.com/automated-plant-watering-with-arduino-nano/
  //sprawdz poziom wody w zbiorniku na samym poczatku 
  check_liquid_level();
  if(is_liquid_level_sufficient==1){//nowe
    while(start_task_time < end_task_time)  //test whether the period has elapsed
    {
      digitalWrite(WATER_PUMP_PIN, LOW);// ON, pompka dziala dla NO
      //trzeba potestowac na ile czasu ma byc ON/OFF
      delay(period/10);//przekaznik ma byc ON przez okreslony czas
      turnoff_water_pump(period/100);//przekaznik ma byc OFF przez okreslony czas, nastepnie znowu wlaczamy
      start_task_time=millis();
      if(start_task_time>=end_task_time){
        break;
      }
    }
    digitalWrite(WATER_PUMP_PIN, HIGH);// OFF dla NO, pompka nie dziala dla NO
  }
  if_water_pump_job_is_done= true;
}


void fetch_dht22_sensor_data(){
  humidity = dht.readHumidity();
  // Read temperature as Celsius (the default)
  temperature = dht.readTemperature();

  // Check if any reads failed and exit early (to try again)
    if (isnan(humidity) || isnan(temperature)) {
      Serial.println(F("Failed to read from DHT sensor!"));
      return;
    }
  // Compute heat index in Celsius (isFahreheit = false)
  heat_index_celsius = dht.computeHeatIndex(temperature, humidity, false);
  /*The Heat Index, sometimes referred to as the apparent temperature, is a measure of 
  how hot it really feels when relative humidity is factored with the actual air temperature.*/
  
  /*Serial.print(F("Humidity: "));
  Serial.print(humidity);
  Serial.print(F("%  Temperature: "));
  Serial.print(temperature);
  Serial.print(F("°C "));
  Serial.print(F(" Heat index: "));
  Serial.print(heat_index_celsius);
  Serial.println(F("°C "));*/
}

void fetch_soil_moisture_sensor_data() {
  int sensor_analog;
  sensor_analog = analogRead(SOIL_MOISTURE_SENSOR_PIN);
  soil_moisture_percentage = ( 100 - ( (sensor_analog / 1023.00) * 100 ) );
  /*Serial.print("Soil moisture Percentage = ");
  Serial.print(moisture_percentage);
  Serial.println(" %");*/
}

void set_LED_color(String hex_value){
  String string_color = hex_value.substring(1);
  byte* rgb_values;
  rgb_values = color_converter(string_color);
    
  for(int i=0; i<strip.numPixels(); i++) {
    //*(rgb_values + i) lub rgb_values[i]
    //zeby wylaczyc ledy wystarczy ustawic rgb na 0,0,0
    strip.setPixelColor(i, strip.Color(rgb_values[0], rgb_values[1], rgb_values[2]));
    strip.show();
  }
  //default brightness:
  strip.setBrightness(70); // Set BRIGHTNESS to about 1/5 (max = 255)
}

byte* color_converter(String hexValue)
{
  //https://forum.arduino.cc/index.php?topic=162792.30
  String value;
  value=hexValue;
  
  char charbuf[8];
  value.toCharArray(charbuf,8);
  long int rgb=strtol(charbuf,0,16); //=>rgb=0x001234FE;
  static byte rgb_value[3];
  rgb_value[0]=(byte)(rgb>>16); //r
  rgb_value[1]=(byte)(rgb>>8);  //g
  rgb_value[2]=(byte)(rgb);     //b

  return rgb_value;
}
