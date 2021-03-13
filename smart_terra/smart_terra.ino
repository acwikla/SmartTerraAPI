#include "DHT.h"
#include <Adafruit_NeoPixel.h>
#ifdef __AVR__
 #include <avr/power.h> // Required for 16 MHz Adafruit Trinket
#endif
#include <String.h>
#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <ArduinoJson.h>
#include <stdlib.h>
#include <stdio.h>
#include <Time.h>

#define DHT_TYPE DHT22
#define LED_COUNT 12
#define SOIL_MOISTURE_SENSOR_PIN A0
#define LAMP_PIN D7
#define PROPELLER_PIN D5
#define WATER_PUMP_PIN D8
#define LED_PIN D6
#define LIQUID_LEVEL_SENSOR D2
#define DHT_PIN D1

bool is_liquid_level_sufficient=true;
bool is_job_done;
//mode data:
bool mode_is_on=true;
float mode_humidity=0;
float mode_temperature=0;
const char* mode_twilight_hour="09:00";
const char* mode_hour_of_dawn="19:00"; 
//real data:
float heat_index_celsius;
float soil_moisture_percentage;
float humidity=0;
float temperature=0;
String LED_hex_color;
float actual_LED_brightness;//?
float simulation_brightness=0;
unsigned long period;
unsigned long end_task_time;
unsigned long start_task_time;
int actual_time_h;
int actual_time_m;
int previous_hour=0;
int previous_minute=0; 
int counter=0;

//192.168.43.186- tel
//192.168.0.164- dom
String IP="192.168.0.6";
int device_id=1058;
int dev_prop_id;
/*const char* ssid = "UPCEA1369B";
const char* password = "uckKvpbZfzu3";*/
const char* ssid = "Creative";
const char* password = "Cre@tive";
/*const char* ssid = "Redmi";
const char* password = "12345678";*/

DHT dht(DHT_PIN, DHT_TYPE);
Adafruit_NeoPixel strip(LED_COUNT, LED_PIN, NEO_GRB + NEO_KHZ800);

void setup() {
  Serial.begin(9600);
  Serial.println("Arduino started.");
  dht.begin();                      //initialize DHT object
  strip.begin();                    //initialize NeoPixel strip object
  digitalWrite(WATER_PUMP_PIN, 0);
  pinMode(LED_PIN, OUTPUT);         //sets the digital LED_PIN as output
  pinMode(WATER_PUMP_PIN, OUTPUT);  
  pinMode(PROPELLER_PIN, OUTPUT);   
  pinMode(LAMP_PIN, OUTPUT); 
  pinMode(LIQUID_LEVEL_SENSOR,INPUT);
  pinMode(SOIL_MOISTURE_SENSOR_PIN,INPUT);

  WiFi.begin(ssid, password);
  
  delay(5000);
  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(1000);
    Serial.println("Connecting...");
  }
  Serial.println("Connected");//->led on
  set_LED_color("#ffffff");
}


void loop() {
  if (WiFi.status() == WL_CONNECTED) 
  {
    get_mode_data();
     
    if(mode_is_on==1){
      //fetch_terrarium_data();
      simulate_day_night_mode();
    }
  }
  
      
  /*if (WiFi.status() == WL_CONNECTED) 
  { 
    get_mode_data();
     
    if(mode_is_on==1){
      //fetch_terrarium_data();
      simulate_day_night_mode();
      //check_terrarium_data();
    }
    else{
      check_device_job_data();
    }

    if(counter%10==0){
      send_terrarium_data();
    }
    delay(5000);
    counter++; 
  }*/
}


void get_mode_data(){
  //declare object of class HTTPClient
  HTTPClient http;
  //specify request destination
  http.begin("http://"+ String(IP)+":5000/api/devices/"+ String(device_id)+"/modes");
  //send the request
  int httpCode = http.GET();
  //print HTTP return code
  Serial.print("httpCode: ");
  Serial.println(httpCode);
  
  if (httpCode > 0) 
    {
      //get the response payload
      String payload = http.getString();
      //print request response payload
      //Serial.println(payload);
      const size_t bufferSize = JSON_OBJECT_SIZE(3) + JSON_OBJECT_SIZE(4) + JSON_OBJECT_SIZE(5) + JSON_OBJECT_SIZE(6) + 370;
      DynamicJsonDocument doc(bufferSize);
      deserializeJson(doc, http.getString());
  
      mode_is_on = doc["isOn"];
      mode_temperature = doc["temperature"]; 
      mode_humidity = doc["humidity"]; 
      mode_twilight_hour = doc["twilightHour"]; 
      mode_hour_of_dawn = doc["hourOfDawn"]; 
      Serial.print("Mode has been successfully fetched. IsOn value:");
      Serial.println(mode_is_on);
      Serial.print("mode_twilight_hour: ");
      Serial.println(mode_twilight_hour);
      Serial.print("mode_hour_of_dawn: ");
      Serial.println(mode_hour_of_dawn);
    }
  http.end(); //close connection
}

void fetch_terrarium_data(){
  check_liquid_level();
  Serial.print("Liquid level: ");
  Serial.println(is_liquid_level_sufficient);
  
  fetch_dht22_sensor_data();//temp, humidity
  Serial.print("Temperature: ");
  Serial.println(temperature);

  Serial.print("Humidity: ");
  Serial.println(humidity);
  
  fetch_soil_moisture_sensor_data();//soil moisture percentage
  Serial.print("Soil moisture percentage: ");
  Serial.println(soil_moisture_percentage);

  get_LED_brightness();
  Serial.print("LED brightness: ");
  Serial.println(actual_LED_brightness);
  Serial.print("LED hex color: ");
  Serial.println(LED_hex_color);
}

void check_terrarium_data(){
  
  if(humidity-3 < mode_humidity){
    float start_water_pump = millis();
    float working_time= 60000;
    
    //turn on water pump for 1 min
    Serial.println("Water pump is working for 1 min.");
    turnon_water_pump(working_time, start_water_pump+working_time, start_water_pump);
    Serial.println("Water pump is off.");
    
    fetch_dht22_sensor_data();
  }
  else if(humidity > mode_humidity+3){
    //turn on the propeller for 10 sec
    Serial.println("Properller is working for 10 sec.");
    tunon_propeller(10000);
  }
        
  if(temperature-3 < mode_temperature){
    //turn on the lamp for 10 sec
    Serial.println("Lamp is working for 10 sec.");
    turnon_lamp(10000);
    
    fetch_dht22_sensor_data();
  }
  else if(temperature > mode_temperature+3){
    //turn on the propeller for 10 sec
    Serial.println("Properller is working for 10 sec.");
    tunon_propeller(10000);
  }

  //simulate_day_night_mode();
}

void check_device_job_data(){
  //API returns the first task with false done property, then changes its value to true.
  HTTPClient http;
  http.begin("http://"+ String(IP)+":5000/api/DeviceJobs/deviceId="+ String(device_id)+"/FalseDoneFlag");
  int httpCode = http.GET();
  String http_value= http.getString();

  if (httpCode > 0)
    {
      const size_t bufferSize = JSON_OBJECT_SIZE(3) + JSON_OBJECT_SIZE(4) + JSON_OBJECT_SIZE(5) + JSON_OBJECT_SIZE(6) + 700;
      DynamicJsonDocument doc(bufferSize);
      deserializeJson(doc, http.getString());

      int device_job_id= doc["id"];
      const char* job_type = doc["job"]["type"];
      const char* job_name = doc["job"]["name"];

      if((String)job_type=="LED"){
        
        if((String)job_name=="TurnOnLED"){
          const char* job_body = doc["body"];
          LED_hex_color= (String)job_body;
          set_LED_color((String)job_body);
          
          Serial.println("TurnOnLED job was executed with color: ."+ LED_hex_color);
          
          is_job_done=true;
          set_job_done_property(device_job_id, is_job_done);
        }
        
        if((String)job_name=="TurnOffLED"){
          turnoff_LED();
          Serial.println("TurnOffLED job was executed.");

          is_job_done=true;
          set_job_done_property(device_job_id, is_job_done);
        }
      }
      
      if((String)job_type=="PUMP" && (String)job_name=="TurnOnWaterPump"){
        check_liquid_level(); 
        if(is_liquid_level_sufficient==1){
          unsigned long job_body = doc["body"];//min
        
          period = job_body *1000UL;//min->millis
          
          end_task_time= millis()+period;
          start_task_time= millis();
          
          turnon_water_pump(period, end_task_time,start_task_time);
          Serial.println("TurnOnWaterPump job was executed.");
          
          is_job_done=true;
          set_job_done_property(device_job_id, is_job_done);
          }
      }
    }
  http.end();
  delay(1000);
}

void send_terrarium_data(){
  String data = "{\"isLiquidLevelSufficient\":" + String(is_liquid_level_sufficient) 
  + " ,\"temperature\": " + String(temperature) + ",\"humidity\": " + String(humidity) 
  + ",\"heatIndex\": " + String(heat_index_celsius) + ",\"soilMoisturePercentage\": " 
  + String(soil_moisture_percentage) + ",\"ledHexColor\": \"" + String(LED_hex_color) 
  + "\",\"ledBrightness\": " + String(actual_LED_brightness) + "}";

  HTTPClient http;    
  http.begin("http://"+ String(IP)+":5000/api/Devices/"+ String(device_id)+"/deviceProperties");
  //specify content-type header
  http.addHeader("Content-Type", "application/json-patch+json");  
  
  int httpCode = http.PATCH(data);   
  String payload = http.getString(); 

  Serial.print("Fetch deviceProperties payload: ");
  Serial.println(payload);

  http.end();
}

void send_liquid_level_sensor_data(bool sensor_data){
  get_latest_device_property_id();
  
  HTTPClient http;
  http.begin("http://"+ String(IP)+":5000/api/DeviceProperties/"+ String(dev_prop_id)+"/LiquidLevel");
  http.addHeader("Content-Type", "application/json-patch+json");
  int httpCode = http.PATCH("{\"isLiquidLevelSufficient\": " + String(sensor_data) + "}");
  String payload = http.getString();
  
  http.end(); 
}

void set_job_done_property(int dj_id, bool done_value){
  HTTPClient http;
  http.begin("http://"+ String(IP)+":5000/api/DeviceJobs/"+ String(dj_id)+"");
  http.addHeader("Content-Type", "application/json-patch+json");
 
  int httpCode = http.PATCH("{\"done\": "+ String(done_value)+ " }");
  String payload = http.getString();
  
  http.end();
}

void get_latest_device_property_id(){
  HTTPClient http;
  http.begin("http://"+ String(IP)+":5000/api/devices/"+ String(device_id)+"/latestDeviceProperties");
  int httpCode = http.GET();
  
  if (httpCode > 0) 
    {
      String payload = http.getString();
      const size_t bufferSize = JSON_OBJECT_SIZE(3) + JSON_OBJECT_SIZE(4) + JSON_OBJECT_SIZE(5) + JSON_OBJECT_SIZE(6) + 370;
      DynamicJsonDocument doc(bufferSize);
      
      deserializeJson(doc, http.getString());
      dev_prop_id = doc["id"];
  }
  
  http.end();
}

int temp=0;
void get_actual_time(){
  HTTPClient http;
  http.begin("http://worldclockapi.com/api/json/utc/now");
  int httpCode = http.GET();
  
  if (httpCode > 0) 
    {
      if(actual_time_h==24){
        temp=0;
      }
      temp++;
       String payload = http.getString();
      const size_t bufferSize = 370;
      DynamicJsonDocument doc(bufferSize);
      deserializeJson(doc, http.getString());
      String current_date_time = doc["currentDateTime"];
      int time_index= current_date_time.indexOf("T");
      actual_time_h= current_date_time.substring(time_index+1, time_index+3).toInt()+1;
      actual_time_m= current_date_time.substring(time_index+4, time_index+6).toInt();
      actual_time_h=temp;
    }

   http.end();
}

void check_liquid_level(){
  is_liquid_level_sufficient=digitalRead(LIQUID_LEVEL_SENSOR);
  
  //while liquid level is not sufficient water pump will be disabled
  if(is_liquid_level_sufficient==0){
    digitalWrite(WATER_PUMP_PIN, HIGH);//OFF for NO pin
    send_liquid_level_sensor_data(false);
  }
  
  is_liquid_level_sufficient=digitalRead(LIQUID_LEVEL_SENSOR);
  //when minimum liquid level will be appropriate send info
  if(is_liquid_level_sufficient==1){
    send_liquid_level_sensor_data(true);
  }
  delay(1000);
}

void fetch_dht22_sensor_data(){
  humidity = dht.readHumidity();
  //read temperature as Celsius (the default)
  temperature = dht.readTemperature();

  //check if any reads failed and exit early (to try again)
    if (isnan(humidity) || isnan(temperature)) {
      Serial.println(F("Failed to read from DHT sensor!"));
      return;
    }
  //compute heat index in Celsius (isFahreheit = false)
  heat_index_celsius = dht.computeHeatIndex(temperature, humidity, false);
  /*The Heat Index, sometimes referred to as the apparent temperature, is a measure of 
  how hot it really feels when relative humidity is factored with the actual air temperature.*/
}

void fetch_soil_moisture_sensor_data() {
  int sensor_analog;
  //read analog data
  sensor_analog = analogRead(SOIL_MOISTURE_SENSOR_PIN);
  //convert analog data to percent value
  soil_moisture_percentage = ( 100 - ( (sensor_analog / 1023.00) * 100 ) );
}

void tunon_propeller(unsigned long wait_time){
  digitalWrite(PROPELLER_PIN, HIGH);
  delay(wait_time);
  digitalWrite(PROPELLER_PIN, LOW);
  delay(1000);
}

void turnon_lamp(unsigned long wait_time){
  digitalWrite(LAMP_PIN, HIGH);
  delay(wait_time);
  digitalWrite(LAMP_PIN, LOW);
  delay(1000);
}

void turnoff_water_pump(){
  digitalWrite(WATER_PUMP_PIN, HIGH); //OFF for NO pin
  delay(1000);
}

void turnon_water_pump(unsigned long period, const unsigned long end_task_time, unsigned long start_task_time ){
  check_liquid_level();
  
  if(is_liquid_level_sufficient==1){
    //test whether the period has elapsed
    while(start_task_time < end_task_time)  
    {
      //digitalWrite(WATER_PUMP_PIN, HIGH);  //ON for NO pin, water pump is working
      check_liquid_level();
      analogWrite(WATER_PUMP_PIN,160);
      
      check_liquid_level();
      start_task_time=millis();
      if(start_task_time>=end_task_time || is_liquid_level_sufficient==0){
        break;
      }
      delay(1000);
    }
    digitalWrite(WATER_PUMP_PIN, LOW); //OFF for NO pin
  }
  else {
    Serial.println("Liquid level is insufficient!");
    return;
  }
}

void change_brightness(int period_h, float beginning_brightness, float final_brightness)
{
  Serial.println("change_brightness here!");
  Serial.print("actual_LED_brightness: ");
  Serial.println(actual_LED_brightness);
  Serial.print("beginning_brightness: ");
  Serial.println(beginning_brightness);
  Serial.print("final_brightness: ");
  Serial.println(final_brightness);
  get_LED_brightness();

  if(actual_LED_brightness<=beginning_brightness){
    simulation_brightness=beginning_brightness;
  }
  
  get_actual_time();
  
  int diff= abs(beginning_brightness-final_brightness)/period_h;
  
  unsigned long time_now = millis()/1000;
  int time_now_minutes = (time_now/60)%60;
  int time_now_hours = (time_now/3600)%24;
  int interval = 1;
  
  if (time_now_minutes - previous_minute >= interval) {
    previous_minute = time_now_minutes;
    set_LED_brightness(simulation_brightness);
    if(beginning_brightness> final_brightness){
      simulation_brightness-=diff;
    }
    if(beginning_brightness> final_brightness){
      simulation_brightness+=diff;
    }
    Serial.print("simulation_brightness:");
    Serial.println(simulation_brightness);
    delay(5000);
  }
}

void change_brightness(float final_brightness){
  set_LED_brightness(final_brightness);
  Serial.print("final_brightness: ");
  Serial.println(final_brightness);
  delay(500);
}

void simulate_day_night_mode()
{
  set_LED_color("#FFFFFF");
  
  String mode_hour_of_dawn_s= mode_hour_of_dawn; String mode_twilight_hour_s= mode_twilight_hour;
  int hour_of_dawn_h= mode_hour_of_dawn_s.substring(0,2).toInt();
  int hour_of_dawn_m= mode_hour_of_dawn_s.substring(3,5).toInt();
  int twilight_hour_h= mode_twilight_hour_s.substring(0,2).toInt();

  double day_hours_h= abs(hour_of_dawn_h-twilight_hour_h);
  double night_hours_h= abs(24-twilight_hour_h+hour_of_dawn_h);

  int midday_h = hour_of_dawn_h +(day_hours_h/2);
  Serial.print("midday_h: ");
  Serial.println(midday_h);
  
  get_actual_time();
  Serial.print("actual_time_h: ");
  Serial.println(actual_time_h);

  //Serial.print("actual_LED_brightness: ");
  //Serial.println(actual_LED_brightness);
  
  if(actual_time_h>=hour_of_dawn_h && actual_time_h<=midday_h){
    int brightness= map(actual_time_h, hour_of_dawn_h, midday_h, 0, 170);
    change_brightness(brightness);
    Serial.println("swit-poludnie");
  }
  
  else if(actual_time_h>midday_h && actual_time_h<=twilight_hour_h){
    int brightness= map(actual_time_h, midday_h, twilight_hour_h, 170, 0);
    change_brightness(brightness);
    Serial.println("poludnie-zmierzch");
  }
  
  else{
    change_brightness(0);
    Serial.println("zmierzch-swit");
  }
}

void turnoff_LED(){
  for(int i=0; i<strip.numPixels(); i++) {
    strip.setBrightness(0);
    strip.show();
  }
  actual_LED_brightness = 0;
}

void get_LED_brightness(){
  for(int i=0; i<strip.numPixels(); i++) {
    actual_LED_brightness= strip.getBrightness();
  }
}

void set_LED_brightness(float brightness){
  for(int i=0; i<strip.numPixels(); i++) {
    strip.setBrightness(brightness);
    strip.show();
  }
  actual_LED_brightness = brightness;
}

void set_LED_color(String hex_value){
  LED_hex_color = hex_value;
  String string_color = hex_value.substring(1);
  byte* rgb_values;
  rgb_values = color_converter(string_color);
    
  for(int i=0; i<strip.numPixels(); i++) {
    //*(rgb_values + i) or rgb_values[i]
    strip.setPixelColor(i, strip.Color(rgb_values[0], rgb_values[1], rgb_values[2]));
    strip.show();
  }
  //default brightness:
  strip.setBrightness(100);
  actual_LED_brightness = 100;
}

byte* color_converter(String hex_value)
{
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
