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
#define DHT_PIN D8
#define SOIL_MOISTURE_SENSOR_PIN A0
#define LED_PIN D6 
#define LED_COUNT 11
#define WATER_PUMP_PIN D5 
#define LIQUID_LEVEL_SENSOR D7
#define PROPELLER_PIN D2

bool is_liquid_level_sufficient=true;
bool is_job_done;
//mode data:
bool mode_is_on;
float mode_humidity;
float mode_temperature;
const char* mode_twilight_hour;
const char* mode_hour_of_dawn;
//real data:
float heat_index_celsius=2;//dane do testow
float soil_moisture_percentage=1;
float humidity=3;
float temperature=3;
String LED_hex_color;
float actual_LED_brightness;
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
String IP="192.168.43.186";
int device_id=1013;
int dev_prop_id;
/*const char* ssid = "UPCEA1369B";
const char* password = "uckKvpbZfzu3";*/
const char* ssid = "Redmi";
const char* password = "12345678";

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
}


void loop() {
  if (WiFi.status() == WL_CONNECTED) 
  { 
    get_mode_data();
    
    if(mode_is_on==1){
      fetch_terrarium_data();
      check_terrarium_data();
    }
    else{
      check_device_job_data();
    }

    if(counter%10==0){
      send_terrarium_data();
    }
    delay(5000);
    counter++;
  }
}
//declarations of request functions:
void send_liquid_level_sensor_data(bool sensor_data);
void set_job_done_property(int dj_id, bool done_value);
void get_latest_device_property_id();
void get_actual_time();
//declarations of functions controlling terrarium environment:
void check_liquid_level();
void fetch_dht22_sensor_data();
void fetch_soil_moisture_sensor_data();
//declarations of functions which manages the devices:
void tunon_propeller();
void turnoff_water_pump(unsigned long wait_time);
void turnon_water_pump(unsigned long period, const unsigned long end_task_time, unsigned long start_task_time );
void change_brightness(int period_h, float beginning_brightness, float final_brightness);
void simulate_day_night_mode();
//declarations of LED functions:
void turnoff_LED();
void get_LED_brightness();
void set_LED_brightness(float brightness);
void set_LED_color(String hex_value);
byte* color_converter(String hex_value);

void get_mode_data(){
  //declare object of class HTTPClient
  HTTPClient http;
  //specify request destination
  http.begin("http://"+ String(IP)+":5000/api/devices/"+ String(device_id)+"/modes");
  //send the request
  int httpCode = http.GET();
  //print HTTP return code
  //Serial.print("httpCode: ");
  //Serial.println(httpCode);
  
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
    }
  http.end(); //close connection
}

void fetch_terrarium_data(){
  //check_liquid_level();
  //fetch_dht22_sensor_data();//temp, humidity
  //fetch_soil_moisture_sensor_data();//soil moisture percentage
  //get_LED_brightness();
}

void check_terrarium_data(){
  
  if(humidity-3 < mode_humidity){
    
    while(1){
      float start_water_pump = millis();
      float working_time= 50000;
      //turn on water pump for 5 min
      turnon_water_pump(working_time, start_water_pump+working_time, start_water_pump);
      fetch_soil_moisture_sensor_data();
      
      if(humidity >= mode_humidity){
        break;
      }
    }
  }
        
  else if(humidity > mode_humidity+3){
    
    while(1){
      //turn on the propeller for 5 sec
      tunon_propeller();
      
      if(humidity >= mode_humidity){
          break;
        }
    }
  }
        
  if(temperature-3 < mode_temperature){
    
    while(1){
      /*turn on lamp for 5 min*/
      fetch_dht22_sensor_data();
      
      if(temperature >= mode_temperature){
        break;
      }
    }
  }
    
  else if(temperature > mode_temperature+3){
    
    while(1){
    //turn on the propeller for 5 sec
      tunon_propeller();
      
      if(temperature >= mode_temperature){
          break;
        }
    }
  }

  simulate_day_night_mode();
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
          
          is_job_done=true;
          set_job_done_property(device_job_id, is_job_done);
        }
        
        if((String)job_name=="TurnOffLED"){
          turnoff_LED();
          
          is_job_done=true;
          set_job_done_property(device_job_id, is_job_done);
        }
      }
      
      if((String)job_type=="PUMP" && (String)job_name=="TurnOnWaterPump"){
        check_liquid_level(); 
        if(is_liquid_level_sufficient==1){
          unsigned long job_body = doc["body"];//min
        
          period = job_body * 60UL * 1000UL;//min->millis
          
          end_task_time= millis()+period;
          start_task_time= millis();
          
          turnon_water_pump(period, end_task_time,start_task_time);
          
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
  + String(soil_moisture_percentage) + ",\"ledHexColor\": \"" + String(LED_hex_color) + "\",\"ledBrightness\": " + String(actual_LED_brightness) + "}";

  HTTPClient http;    
  http.begin("http://"+ String(IP)+":5000/api/Devices/"+ String(device_id)+"/deviceProperties");
  //specify content-type header
  http.addHeader("Content-Type", "application/json-patch+json");  
  
  int httpCode = http.PATCH(data);   
  String payload = http.getString(); 
   
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

void get_actual_time(){
  HTTPClient http;
  http.begin("http://worldclockapi.com/api/json/utc/now");
  int httpCode = http.GET();
  
  if (httpCode > 0) 
    {
       String payload = http.getString();
      const size_t bufferSize = 370;
      DynamicJsonDocument doc(bufferSize);
      deserializeJson(doc, http.getString());
      String current_date_time = doc["currentDateTime"];
      int time_index= current_date_time.indexOf("T");
      actual_time_h= current_date_time.substring(time_index+1, time_index+3).toInt()+1;
      actual_time_m= current_date_time.substring(time_index+4, time_index+6).toInt();
    }

   http.end();
}

void check_liquid_level(){
  is_liquid_level_sufficient=digitalRead(LIQUID_LEVEL_SENSOR);
  //while liquid level is not sufficient water pump will be disabled
  while(is_liquid_level_sufficient==0){
    
    digitalWrite(WATER_PUMP_PIN, HIGH);//OFF for NO pin
    send_liquid_level_sensor_data(false);

    is_liquid_level_sufficient=digitalRead(LIQUID_LEVEL_SENSOR);
    //when minimum liquid level will be appropriate send info and exit function
    if(is_liquid_level_sufficient==1){
      send_liquid_level_sensor_data(true);
      break;
    }
    delay(1000);
  }
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

void tunon_propeller(){
  digitalWrite(PROPELLER_PIN, HIGH);
  delay(5000);
  digitalWrite(PROPELLER_PIN, LOW);
  delay(1000);
}

void turnoff_water_pump(unsigned long wait_time){
  digitalWrite(WATER_PUMP_PIN, HIGH); //OFF for NO pin
  delay(wait_time);
  digitalWrite(WATER_PUMP_PIN, LOW);  //ON for NO pin
}

void turnon_water_pump(unsigned long period, const unsigned long end_task_time, unsigned long start_task_time ){
  check_liquid_level();
  
  if(is_liquid_level_sufficient==1){
    //test whether the period has elapsed
    while(start_task_time < end_task_time)  
    {
      digitalWrite(WATER_PUMP_PIN, LOW);  //ON for NO pin, water pump is working
      check_liquid_level();
      analogWrite(WATER_PUMP_PIN,225);
      
      check_liquid_level();
      start_task_time=millis();
      if(start_task_time>=end_task_time || is_liquid_level_sufficient==0){
        break;
      }
    }
    digitalWrite(WATER_PUMP_PIN, HIGH); //OFF for NO pin
  }
}

void change_brightness(int period_h, float beginning_brightness, float final_brightness)
{
  
  Serial.println("change brightness here!");
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
  
  if (time_now_minutes - previous_minute >= interval && simulation_brightness <=final_brightness) {
    previous_minute = time_now_minutes;
    set_LED_brightness(simulation_brightness);
    simulation_brightness+=diff;
    Serial.print("simulation_brightness:");
    Serial.println(simulation_brightness);
    Serial.println("One minutes has passed");
    delay(5000);
  }
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
  int night_h = twilight_hour_h +(night_hours_h/2);;
  
  get_actual_time();
  
  if(actual_time_h>=hour_of_dawn_h && actual_time_h<midday_h){
    int period_h= abs(midday_h-hour_of_dawn_h);
    change_brightness(period_h, 0, 255);
  }
  
  if(actual_time_h>=midday_h && actual_time_h<twilight_hour_h){
    int period_h= abs(twilight_hour_h-midday_h);
    change_brightness(period_h, 255, 127);
  }

  if(actual_time_h>=twilight_hour_h && actual_time_h<night_h){
    int period_h= abs(night_h-twilight_hour_h);
    change_brightness(period_h, 127, 0);
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
  actual_LED_brightness= strip.getBrightness();
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
  strip.setBrightness(255);
  actual_LED_brightness = 255;
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
