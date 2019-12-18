#include <Wire.h> 
#include <LiquidCrystal_I2C.h>

LiquidCrystal_I2C lcd(0x27,16,2);
int speakerPin = 8;;
/*////////////////////////////////////////
/                                        /
/         Sensor Variable                /
/////////////////////////////////////////*/
  int analogpin = A0;
  int sensorValue = 0;
  int outPutValue = 0;
  int counter =0;
    void setup() 
    {
      Serial.begin(9600);
//BUZZER or Warning Sound;
      pinMode(8,OUTPUT);
  
/*////////////////////////////////////////
/                                        /
/         LCD INITIALIZER                /
/////////////////////////////////////////*/
        
     //  Serial.begin(9600);
       lcd.init();             
       lcd.backlight();   
       lcd.print("Water Level");  

     }

//loop function 


void loop() {
      
 lcd.setCursor(0,1);
/*////////////////////////////////////////
/                                        /
/        For water sensor           /
/////////////////////////////////////////*/
  sensorValue  = analogRead(analogpin);
  outPutValue = map(sensorValue,0,1023,0,250);
  Serial.println(outPutValue);

/*////////////////////////////////////////
/                                        /
/         LCD CODE                       /
/////////////////////////////////////////*/
  if(outPutValue <=45)
  {
     lcd.print("Empty            ");  
  }
/*////////////////////////////////////////
/                                        /
/         Water beep                     /
/////////////////////////////////////////*/
    if(outPutValue <=70 && outPutValue >= 46 )
    {
       lcd.print("LOW            ");  
    }
    else if(outPutValue >70 && outPutValue <80)
    {
      lcd.print("AVERAGE        ");  

    }
    else if(outPutValue >80)
    {
      delay(1000);
    
       //Serial.println(counter);
      lcd.print("DANGER        ");  
    if(counter <=20)
    {
       counter+=1;
      tone(speakerPin,800,800);
      delay(200);
      tone(speakerPin,600,800);
    }
}


}
