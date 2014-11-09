// Test code for Adafruit GPS modules using MTK3329/MTK3339 driver
//
// This code just echos whatever is coming from the GPS unit to the
// serial monitor, handy for debugging!
//
// Tested and works great with the Adafruit Ultimate GPS module
// using MTK33x9 chipset
//    ------> http://www.adafruit.com/products/746
// Pick one up today at the Adafruit electronics shop
// and help support open source hardware & software! -ada

//This code is intended for use with Arduino Leonardo and other ATmega32U4-based Arduinos

#include <Adafruit_GPS.h>
#include <SoftwareSerial.h>
#include <FileIO.h>

// Connect the GPS Power pin to 5V
// Connect the GPS Ground pin to ground
// If using software serial (sketch example default):
//   Connect the GPS TX (transmit) pin to Digital 8
//   Connect the GPS RX (receive) pin to Digital 7
// If using hardware serial:
//   Connect the GPS TX (transmit) pin to Arduino RX1 (Digital 0)
//   Connect the GPS RX (receive) pin to matching TX1 (Digital 1)

// If using software serial, keep these lines enabled
// (you can change the pin numbers to match your wiring):
SoftwareSerial mySerial(8, 7);
Adafruit_GPS GPS(&mySerial);

// If using hardware serial, comment
// out the above two lines and enable these two lines instead:
//HardwareSerial mySerial = Serial1;

#define PMTK_SET_NMEA_UPDATE_1HZ  "$PMTK220,1000*1F"
#define PMTK_SET_NMEA_UPDATE_5HZ  "$PMTK220,200*2C"
#define PMTK_SET_NMEA_UPDATE_10HZ "$PMTK220,100*2F"

// turn on only the second sentence (GPRMC)
#define PMTK_SET_NMEA_OUTPUT_RMCONLY "$PMTK314,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0*29"
// turn on GPRMC and GGA
#define PMTK_SET_NMEA_OUTPUT_RMCGGA "$PMTK314,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0*28"
// turn on ALL THE DATA
#define PMTK_SET_NMEA_OUTPUT_ALLDATA "$PMTK314,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0*28"
// turn off output
#define PMTK_SET_NMEA_OUTPUT_OFF "$PMTK314,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0*28"

#define PMTK_Q_RELEASE "$PMTK605*31"

#define GPSECHO  true

int uploadThresholdCount = 10;
int currentLoopCount = 0; 
bool alreadyUploading = false;
 
void setup() {
  while (!Serial); // wait for leo to be ready
  Bridge.begin();
  Serial.begin(57600); // this baud rate doesn't actually matter!
  //mySerial.begin(9600);
  GPS.begin(9600);
  FileSystem.begin();

  delay(2000);
  Serial.println("Get version!");
  

   GPS.sendCommand(PMTK_SET_NMEA_OUTPUT_RMCONLY);
   //GPS.sendCommand(PMTK_SET_NMEA_OUTPUT_ALLDATA);
   GPS.sendCommand(PMTK_SET_NMEA_UPDATE_1HZ);
   GPS.sendCommand(PGCMD_NOANTENNA);

  //mySerial.println(PMTK_Q_RELEASE);

  // you can send various commands to get it started
  //mySerial.println(PMTK_SET_NMEA_OUTPUT_RMCGGA);
  //mySerial.println(PMTK_SET_NMEA_OUTPUT_ALLDATA);`

  //mySerial.println(PMTK_SET_NMEA_UPDATE_1HZ);


 }


void loop() {
  /*
  if (Serial.available()) {
   char c = Serial.read();
   Serial.write(c);
   mySerial.write(c);
  }

  if (mySerial.available()) {
    char c = mySerial.read();
    Serial.write(c);
  }
  */
  
      if (currentLoopCount >= uploadThresholdCount && alreadyUploading == false) {
                alreadyUploading = true;
                Serial.println("Uploading - Start");
		Process p;
                p.begin("python");
                p.addParameter("/mnt/sd/arduino/www/mysketch/Uploader_exceptions.py");
                p.addParameter("foo"); // A command line parameter for the script
                p.addParameter("&2>1"); // pipe error output to stdout
                p.run(); // blocking call to run python; ATMega execution halts until complete
                
            
                // print to the serial port too:
                Serial.println("Uploaded data");
                // Ensure the last bit of data is sent.
                Serial.flush();
                currentLoopCount = 0;
                alreadyUploading = false;
	
	} else  {
                GPS.read();
		//char c = GPS.read();
		// if you want to debug, this is a good time to do it!
		//if (GPSECHO)
		  //if (c) Serial.print(c);

		if (GPS.newNMEAreceived())
		{
		  Serial.println("NMEA received");
		  char *stringptr = GPS.lastNMEA();
                  Serial.println(stringptr);
                  
                  Serial.println(GPS.fix);
                  
                  // this also sets the newNMEAreceived() flag to false 
		  if (!GPS.parse(stringptr))
                  {
  		    Serial.println("NMEA could not be parsed");
	            return;  // we can fail to parse a sentence in which case we should just wait for another
		  }
                  else
                  {
                    Serial.println("Successfully parsed.");
                  }

                  if (GPS.fix == 0) {
                          Serial.println("No GSPS fix. Sleeping ...");
                          delay(1000);
                          return;
                      }

                  Serial.println("GPS fix found!");
                  
		  File dataFile = FileSystem.open("/mnt/sd/arduino/www/mysketch/datalog.txt", FILE_APPEND);

                  if(strstr(stringptr, "RMC")){
                    Serial.print("Found RMC data.");
  		  // if the file is available, write to it:
  		  if (dataFile) {
  			uint8_t stringsize = strlen(stringptr);
  			dataFile.write((uint8_t *)stringptr, stringsize);
  			dataFile.close();
  
  			++currentLoopCount;
  			
  			// print to the serial port too:
  			Serial.println("Wrote gps data to file.");
  		  }
  		  // if the file isn't open, pop up an error:
  		  else {
  			Serial.println("error opening datalog.txt");
  		  }
                  }
		}
	}
}
