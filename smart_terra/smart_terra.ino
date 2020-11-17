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
bool is_liquid_level_sufficient=true;
bool is_job_done;
//mode data:
float mode_humidity;
float mode_temperature;
const char* mode_twilight_hour;
const char* mode_hour_of_dawn;
//real data:
float heat_index_celsius=2;//dane do testow
float soil_moisture_percentage=1;
float humidity=3;
float temperature=3;
String LED_hex_color="#ffffff";//domyslnie bedzie bialy
float LED_brightness=100;
unsigned long period;
unsigned long end_task_time;
unsigned long start_task_time;

int device_id=1;
const char* ssid = "UPCEA1369B";
const char* password = "uckKvpbZfzu3";
/*const char* ssid = "Redmi";
const char* password = "12345678";*/

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
  //192.168.43.186- tel
  //192.168.0.164- dom
  if (WiFi.status() == WL_CONNECTED) 
  {
    //get_mode_data();
    //fetch_and_send_terrarium_data();
    //check_liquid_level();
    //checkTerrariumData(modeData, terrariumData);
    //przez czas dzialania np pompki wszytskie inne funkcje beda wylaczone- poprawic
    check_device_job_data();//sprawdzaj czy uzytkownik nie dodał ktoregos joba
  }
  delay(1000);// czekamy sekundę
}

void turnoff_water_pump(unsigned long wait_time);
void turnon_water_pump(unsigned long period, const unsigned long end_task_time, unsigned long start_task_time );
byte* color_converter(String hex_value);
void set_LED_color(String hex_value);
void turnoff_LED();
void fetch_dht22_sensor_data();
void fetch_soil_moisture_sensor_data();

void get_mode_data(){
  HTTPClient http;
  //ip domowe:192.168.0.164
  http.begin("http://192.168.0.164:5000/api/devices/"+ String(device_id)+"/modes");
  int httpCode = http.GET();
  Serial.print("httpCode: ");
  Serial.println(httpCode);
  
  if (httpCode > 0) 
    {
       String payload = http.getString();
       Serial.println(payload);
      const size_t bufferSize = JSON_OBJECT_SIZE(3) + JSON_OBJECT_SIZE(4) + JSON_OBJECT_SIZE(5) + JSON_OBJECT_SIZE(6) + 370;
      //ArduinoJson5:
      //DynamicJsonBuffer jsonBuffer(bufferSize);
      //JsonObject& root = jsonBuffer.parseObject(http.getString());
      DynamicJsonDocument doc(bufferSize);
      deserializeJson(doc, http.getString());
      
      mode_temperature = doc["temperature"]; 
      mode_humidity = doc["humidity"]; 
      mode_twilight_hour = doc["twilightHour"]; 
      mode_hour_of_dawn = doc["hourOfDawn"]; 

      Serial.print("mode_humidity:");
      Serial.println(mode_humidity);
      Serial.print("mode_temperature:");
      Serial.println(temperature);
      Serial.print("mode_twilight_hour:");
      Serial.println(mode_twilight_hour);
      Serial.print("mode_hour_of_dawn:");
      Serial.println(mode_hour_of_dawn);
    }
  http.end(); //Close connection
  //delay(30UL * 60UL * 1000UL); //30 minutes each of 60 seconds each of 1000 milliseconds all unsigned long
}

void fetch_and_send_terrarium_data(){
  //fetch_dht22_sensor_data();//temp, humidity
  //fetch_soil_moisture_sensor_data();//soil moisture percentage
  
  String data = "{\"isLiquidLevelSufficient\":" + String(is_liquid_level_sufficient) 
  + " ,\"temperature\": " + String(temperature) + ",\"humidity\": " + String(humidity) 
  + ",\"heatIndex\": " + String(heat_index_celsius) + ",\"soilMoisturePercentage\": " 
  + String(soil_moisture_percentage) + ",\"ledHexColor\": \"" + String(LED_hex_color) + "\",\"ledBrightness\": " + String(LED_brightness) + "}";

  HTTPClient http;    //Declare object of class HTTPClient
  //DevicePropertiesId tez powinno byc pobierane z bazy- poprawic
  http.begin("http://192.168.0.164:5000/api/DeviceProperties/1");//Specify request destination
  http.addHeader("Content-Type", "application/json-patch+json");  //Specify content-type header
  
  int httpCode = http.PATCH(data);   //Send the request
  String payload = http.getString(); //Get the response payload
  
  Serial.println(httpCode);   //Print HTTP return code
  Serial.println(payload);    //Print request response payload
  http.end();
}

void set_job_done_property(int dj_id, bool done_value){
  HTTPClient http;    //Declare object of class HTTPClient
  http.begin("http://192.168.0.164:5000/api/DeviceJobs/"+ String(dj_id)+"");//Specify request destination
  http.addHeader("Content-Type", "application/json-patch+json");  //Specify content-type header

  int httpCode = http.PATCH("{\"done\": "+ String(done_value)+ " }");   //Send the request
  String payload = http.getString(); //Get the response payload

  Serial.println(httpCode);   //Print HTTP return code
  Serial.println(payload);    //Print request response payload
  http.end();
}

void check_device_job_data(){
  /*1. odpytuj baze danych czy uzytkownik dodal joba, czyli sprawdz tabele deviceJobs->wywolaj metode GET: GetDeviceJobs(deviceId)
      1.1. jezeli jest null-> nie rob nic
      1.2. jezeli nie jest null:
        1.2.1. sprawdz Id joba
        1.2.2. w zaleznosci od Id joba wlacz odpowiednie narzedzia lub ustaw odpowiednie dane(kolor, pompka etc)
        1.2.3. przemyslec co w przypadku kiedy np wilgotnosc po wlaczeniu pompki bedzie za duza, albo powinna byc noc a ktos ustawi jasnosc na 100%?
              -> na czas trwania taska, wykonac bezwzglednie
              -> fukncja odpowiedzialna za wlaczanie pompki musi sprawdzac poziom wody, plus obliczac czas dzialania i czas przerw
   */
   
  //po stronie API bedzie zadanie zeby wysylac pierwszego joba z flaga done ustawiona na false-> wtedy zawsze doc[0][...]
  HTTPClient http;
  //ip domowe:192.168.0.164
  http.begin("http://192.168.0.164:5000/api/DeviceJobs/deviceId="+ String(device_id)+ "");
  Serial.println("http://192.168.0.164:5000/api/DeviceJobs/deviceId="+ String(device_id)+ "");
  int httpCode = http.GET();
  Serial.print("httpCode: ");
  Serial.println(httpCode);
  
  if (httpCode > 0) 
    {
      const size_t bufferSize = JSON_OBJECT_SIZE(3) + JSON_OBJECT_SIZE(4) + JSON_OBJECT_SIZE(5) + JSON_OBJECT_SIZE(6) + 700;
      DynamicJsonDocument doc(bufferSize);
      deserializeJson(doc, http.getString());
      
      //doc[i]: 0=TurnOnLED, 1=TurnOffLED, 2=TurnOnWaterPump
      int device_job_id= doc[0]["id"];
      const char* job_type = doc[0]["job"]["type"];//lista/tablica
      const char* job_name = doc[0]["job"]["name"];
      unsigned long jobBodyPump = doc[2]["body"];
      Serial.println((String)device_job_id);
      Serial.println((String)job_type);
      Serial.println((String)job_name);
      Serial.println((String)jobBodyPump);

      if((String)job_type=="LED"){
        
        if((String)job_name=="TurnOnLED"){
          //dla testow job bedzie dzialal na 10sek, pozniej wywolujemy turnoff_LED()
          //pozniej uzytkownik bedzie decydowal czy chce wylaczyc juz joba
          //domyslnie to bedzie tylko ustawienie koloru
          const char* job_body = doc[0]["body"];
          LED_hex_color= (String)job_body;
          set_LED_color((String)job_body);
          Serial.println((String)job_body);
          
          delay(10000);
          turnoff_LED();
          
          is_job_done=true;
          set_job_done_property(device_job_id, is_job_done);
        }
        if((String)job_name=="TurnOffLED"){
          //doc[1]["body"] body i tak bedzie nullem
          turnoff_LED();
          is_job_done=true;
          set_job_done_property(device_job_id, is_job_done);
        }
      }
      if((String)job_type=="PUMP" && (String)job_name=="TurnOnWaterPump"){
        //check_liquid_level(); if(is_liquid_level_sufficient==1){}=>
        unsigned long job_body = doc[2]["body"];//min
        //Serial.print("jobBody: "); Serial.println(job_body);
        
        period = job_body * 60UL * 1000UL;//min->millis
        //Serial.print("period: "); Serial.println(period);
        
        end_task_time= millis()+period;//tutaj podmienic na executionTime+period
        //millis()- Number of milliseconds passed since the program started. Data type: unsigned long
        start_task_time= millis();//tutaj podmienic na executionTime
        
        //trzeba obliczyc wait time
        turnon_water_pump(period, end_task_time,start_task_time);
        
        //patch done property
        is_job_done=true;
        set_job_done_property(device_job_id, is_job_done);
        //else{...}
      }
    }
  http.end();
}

//wlacz odpowiednie urzadzenia w zaleznosci od wymaganych danych
void checkTerrariumData(bool modeData, bool terrariumDara){
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
      Wazne: przy kazdej zmianie koloru/jasnosci ustawiamy pola: LED_hex_color i LED_brightness!!!
    */
}
void send_liquid_level_sensor_data(bool sensor_data){
  HTTPClient http;    //Declare object of class HTTPClient
  http.begin("http://192.192.168.0.164:5000/api/DeviceProperties/1/LiquidLevel");//Specify request destination
  http.addHeader("Content-Type", "application/json-patch+json");  //Specify content-type header

  int httpCode = http.PATCH("{\"isLiquidLevelSufficient\": " + String(sensor_data) + "}");   //Send the request
  String payload = http.getString(); //Get the response payload
  
  Serial.println(httpCode);   //Print HTTP return code
  Serial.println(payload);    //Print request response payload
  http.end(); 
}

void check_liquid_level(){
  is_liquid_level_sufficient=digitalRead(LIQUID_LEVEL_SENSOR);
  //dopoki poziom wody nie bedzie wystarczajacy to pompka bedzie wylaczona
  while(is_liquid_level_sufficient==0){
    
    digitalWrite(WATER_PUMP_PIN, HIGH);//OFF dla NO, pompka nie dziala dla NO
    send_liquid_level_sensor_data(false);//wyslij info do uzytkownika
    
    if(is_liquid_level_sufficient==1){
      //kiedy poziom wody bedzie odpowiedni wychodzimy z funkcji i wysyłamy dane
      send_liquid_level_sensor_data(true);
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
  //check_liquid_level();
  if(is_liquid_level_sufficient==1){
    while(start_task_time < end_task_time)  //test whether the period has elapsed
    {
      digitalWrite(WATER_PUMP_PIN, LOW);// ON, pompka dziala dla NO
      
      //powiedzmy ze niezaleznie od czasu jaki wybral uzytkownik, pompka ma byc 5sek onn i 5 sek off
      unsigned long wait_time= 5000;
      delay(5000);//przekaznik ma byc ON przez okreslony czas
      turnoff_water_pump(5000);//przekaznik ma byc OFF przez okreslony czas, nastepnie znowu wlaczamy
      start_task_time=millis();
      if(start_task_time>=end_task_time){
        break;
      }
    }
    digitalWrite(WATER_PUMP_PIN, HIGH);// OFF dla NO, pompka nie dziala dla NO
  }
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

void turnoff_LED(){
  for(int i=0; i<strip.numPixels(); i++) {
    //pozniej nalezy przywrocic dane sprzed wlaczenia joba
    strip.setPixelColor(i, strip.Color(0,0,0));
    strip.show();
  }
  //default brightness:
  strip.setBrightness(70); // Set BRIGHTNESS to about 1/5 (max = 255)
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

byte* color_converter(String hex_value)
{
  //https://forum.arduino.cc/index.php?topic=162792.30
  String value;
  value=hex_value;
  
  char charbuf[8];
  value.toCharArray(charbuf,8);
  long int rgb=strtol(charbuf,0,16); //=>rgb=0x001234FE;
  static byte rgb_value[3];
  rgb_value[0]=(byte)(rgb>>16); //r
  rgb_value[1]=(byte)(rgb>>8);  //g
  rgb_value[2]=(byte)(rgb);     //b

  return rgb_value;
}
