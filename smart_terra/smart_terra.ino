#include "DHT.h"
#include <Adafruit_NeoPixel.h>
#ifdef __AVR__
 #include <avr/power.h> // Required for 16 MHz Adafruit Trinket
#endif
#include <String.h>

#define DHT_TYPE DHT22
#define DHT_PIN 5
#define SOIL_MOISTURE_PIN A0
#define LED_PIN D6
#define LED_COUNT 60
#define WATER_PUMP_PIN D5

String hex_value= "#f7f700";
bool if_water_pump_job_is_done;
float moisture_percentage;
float humidity;
float temperature;
float heat_index_celsius;
unsigned long period;
unsigned long end_task_time;
unsigned long start_task_time;
  
DHT dht(DHT_PIN, DHT_TYPE);
Adafruit_NeoPixel strip(LED_COUNT, LED_PIN, NEO_GRB + NEO_KHZ800);

//uruchamia się raz przy uruchomieniu
void setup() {
  Serial.begin(300);
  dht.begin();
  strip.begin();           // INITIALIZE NeoPixel strip object (REQUIRED)
  pinMode(LED_PIN, OUTPUT);    // sets the digital pin D6 as output
  pinMode(WATER_PUMP_PIN, OUTPUT);    // sets the digital pin D5 as output
  if_water_pump_job_is_done= false;
  //dlaczego nie moge nic wypisac w setup:
  //https://forum.arduino.cc/index.php?topic=171889.0
}

//uruchamia się w nieskończonej pętli
void loop() {
  //print_soil_moisture_sensor_data();
  //print_dht22_sensor_data();
  period = 10000;//10sek
  end_task_time= millis()+period;
  //millis()- Number of milliseconds passed since the program started. Data type: unsigned long.
  start_task_time= millis();
  if(if_water_pump_job_is_done==false){
    turnon_water_pump(period, end_task_time, start_task_time );
  }
  //set_LED_color(hex_value);
  delay(1000);// czekamy sekundę
}

void wait(unsigned long wait_time){
  digitalWrite(WATER_PUMP_PIN, HIGH);// OFF dla NO, pompka nie dziala dla NO
  delay(wait_time);
  digitalWrite(WATER_PUMP_PIN, LOW);// ON dla NO, pompka dziala dla NO
}

void turnon_water_pump(unsigned long period, const unsigned long end_task_time, unsigned long start_task_time ){
  //https://www.programmingelectronics.com/automated-plant-watering-with-arduino-nano/
  
  while(start_task_time < end_task_time)  //test whether the period has elapsed
  {
    digitalWrite(WATER_PUMP_PIN, LOW);// ON, pompka dziala dla NO
    //trzeba potestowac na ile czasu ma byc ON/OFF
    delay(period/10);//przekaznik ma byc ON przez okreslony czas
    wait(period/100);//przekaznik ma byc OFF przez okreslony czas, nastepnie znowu wlaczamy
    start_task_time=millis();
    if(start_task_time>=end_task_time){
      break;
    }
  }
  digitalWrite(WATER_PUMP_PIN, HIGH);// OFF dla NO, pompka nie dziala dla NO
  if_water_pump_job_is_done= true;
}


void print_dht22_sensor_data(){
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
  
  Serial.print(F("Humidity: "));
  Serial.print(humidity);
  Serial.print(F("%  Temperature: "));
  Serial.print(temperature);
  Serial.print(F("°C "));
  Serial.print(F(" Heat index: "));
  Serial.print(heat_index_celsius);
  Serial.println(F("°C "));
}

void print_soil_moisture_sensor_data() {
  int sensor_analog;
  sensor_analog = analogRead(SOIL_MOISTURE_PIN);
  moisture_percentage = ( 100 - ( (sensor_analog / 1023.00) * 100 ) );
  Serial.print("Soil moisture Percentage = ");
  Serial.print(moisture_percentage);
  Serial.println(" %");
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
