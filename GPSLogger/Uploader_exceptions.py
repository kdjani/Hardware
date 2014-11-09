#Change for arduino
import urllib
import httplib
#Change for VS
#import urllib.request
#import urllib.parse
#Change
import datetime
import os
import sys
import time
import traceback

######################### CONSTANTS #########################################

ADD_TEMPLATE = """<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
  <s:Body>
    <AddGpsData xmlns=\"http://tempuri.org/\">
      <userId>{0}</userId>
      <deviceId>{1}</deviceId>
      <time>{2}</time>
      <longitude>{3}</longitude>
      <latitude>{4}</latitude>
      <altitude>{5}</altitude>
    </AddGpsData>
  </s:Body>
</s:Envelope>"""

headers = {
    'Content-Type': 'text/xml; charset=utf-8',
    'SOAPAction': '"http://tempuri.org/ICustomerServices/AddGpsData"',
    'Accept-Encoding': 'gzip, deflate'
    }

site = "http://gpslogger.cloudapp.net/CustomerServices.svc"
url = "gpslogger.cloudapp.net"
userId = "75c022a7-757c-4fc9-9809-27092d4f4aef"
deviceId = "32e4a2b6-ec8a-4bf7-adee-d40759e818b5"

########################## METHODS ##########################################
def internet_on(logsFolder):
	try:
		#Change for arduino
		response=urllib.urlopen("http://gpslogger.cloudapp.net/CustomerServices.svc")
		#Change for VS
		#response=urllib.request.urlopen("http://gpslogger.cloudapp.net/CustomerServices.svc")
		#Change
		return True
	except:
		exc_type, exc_value, exc_traceback = sys.exc_info()
		lines = traceback.format_exception(exc_type, exc_value, exc_traceback)
		for line in lines:
			writeLog(logFile, line)
		e = sys.exc_info()[0]
		writeLog(logFile, "Internet exception: " + str(e))
		return False

def writeLog(logFile, logLine):
	logFile.write(str(datetime.datetime.now().time()) + " - " + str(logLine) + "\n")

##############################################################################

#Change for arduino
rootFolder = "/mnt/sd/arduino/www/mysketch/"
logsFolder = rootFolder + "Logs/"
#Change for VS
#rootFolder = "E:\python\\"
#logsFolder = rootFolder + "Logs\\"
#Change
if not os.path.exists(rootFolder):
    os.makedirs(rootFolder)
if not os.path.exists(logsFolder):
    os.makedirs(logsFolder)

bufsize = 1
logFile = open(logsFolder + "Log_" + str(datetime.datetime.now().time()).replace(":", "_") + ".log", "w", bufsize)
dataFile = "datalog.txt"

#delete old files
now = time.time()
for f in os.listdir(logsFolder):
	f = os.path.join(logsFolder, f)
	if os.stat(f).st_mtime < now - 86400:# one day old files
		if os.path.isfile(f):
			os.remove(f)

try:
	writeLog(logFile, "Starting script")
	if internet_on(logsFolder):
		writeLog(logFile, "Internet found")
		ins = open( rootFolder + dataFile, "r" )
		writeLog(logFile, "Data file found")
		array = []
		for line in ins:
			array.append(line)
		ins.close()

		if array.count == 0:
			writeLog(logFile, "Zero rows found in file")
		else:
			writeLog(logFile, "Non-zero rows found in file")
			#Precision of the minutes. If +-precision, data is not sent
			precision = 5
			#Even if coordinates are same, how many same till we send again
			thresholdSameCount = 10
			writeLog(logFile, "Precision: " + str(precision))
			writeLog(logFile, "ThresholdSameCount: " + str(thresholdSameCount))

			oldLongitudePrecise = -1
			oldLatitudePrecise = -1
			oldLongitudeDirection = "X"
			oldLatitudeDirection = "X"
			oldLongitude = -1
			oldLatitude = -1
			iterator = 0

			for original in array:
				writeLog(logFile, "_________________________")
				iterator = iterator + 1
				if original.startswith("$GPRMC"):
					writeLog(logFile, "Row found starting with $GPRMC: " + original)
					startTime =  [x.strip() for x in original.split(',')][1]
					startTime =  [x.strip() for x in startTime.split('.')][0]
					longitude =  [x.strip() for x in original.split(',')][3]
					longitudeDirection = [x.strip() for x in original.split(',')][4]
					latitude =  [x.strip() for x in original.split(',')][5]
					latitudeDirection = [x.strip() for x in original.split(',')][6]
					startDate = [x.strip() for x in original.split(',')][9]
					altitude = [x.strip() for x in original.split(',')][10]
					writeLog(logFile, "startTime: " + startTime)
					writeLog(logFile, "longitude: " + longitude)
					writeLog(logFile, "longitudeDirection: " + longitudeDirection)
					writeLog(logFile, "latitude: " + latitude)
					writeLog(logFile, "latitudeDirection: " + latitudeDirection)
					writeLog(logFile, "StartDate: " + startDate)
					writeLog(logFile, "altitude: " + altitude)
					if (len(longitude) > 0 and len(latitude) > 0):
						longitudePrecise = int([x.strip() for x in longitude.split('.')][1])
						latitudePrecise = int([x.strip() for x in latitude.split('.')][1])
						writeLog(logFile, "longitudePrecise: " + str(longitudePrecise))
						writeLog(logFile, "latitudePrecise: " + str(latitudePrecise))
						writeLog(logFile, "oldLongitudePrecise: " + str(oldLongitudePrecise))
						writeLog(logFile, "oldLatitudePrecise: " + str(oldLatitudePrecise))
						writeLog(logFile, "oldLongitudeDirection: " + oldLongitudeDirection)
						writeLog(logFile, "oldLatitudeDirection: " + oldLatitudeDirection)
						writeLog(logFile, "oldLongitude: " + str(oldLongitude))
						writeLog(logFile, "oldLatitude: " + str(oldLatitude))

						if((oldLongitudePrecise - precision > longitudePrecise or oldLongitudePrecise + precision < longitudePrecise) or(oldLatitudePrecise - precision > latitudePrecise or oldLatitudePrecise + precision < latitudePrecise) or iterator > thresholdSameCount or oldLongitudeDirection != longitudeDirection or oldLatitudeDirection != latitudeDirection or oldLongitude != longitude or oldLatitude != latitude):
							writeLog(logFile, "Sending data up...")

							longitudeStr = str(longitude) + "," + longitudeDirection
							latitudeStr = str(latitude) + "," + latitudeDirection
							startTime = startDate + startTime
							data = ADD_TEMPLATE.format(userId, deviceId, startTime, longitudeStr, latitudeStr, altitude)
							binary_data = data.encode('utf8')
							writeLog(logFile, "data: " + str(data))
							conn = httplib.HTTPConnection(url)
							conn.request("POST", "/CustomerServices.svc", binary_data, headers)
							f = conn.getresponse()

							writeLog(logFile, f.status)
							writeLog(logFile, f.reason)
							writeLog(logFile, f.read())

							#Change
							#Change for arduino
							#req = Request(site, binary_data, headers)
							#f = urllib.urlopen(req)
							#Change for VS
							#req = urllib.request.Request(site, binary_data, headers)
							#f = urllib.request.urlopen(req)
							#Change
							#writeLog(logFile, f.read())
							writeLog(logFile, "Sent data up...")
							iterator = 0
						else:
							writeLog(logFile, "No changes detected...")
						oldLongitudePrecise = longitudePrecise
						oldLatitudePrecise = latitudePrecise
						oldLongitudeDirection = longitudeDirection
						oldLatitudeDirection = latitudeDirection 
						oldLongitude = longitude
						oldLatitude = latitude

			#delete file since we are done with it		
			writeLog(logFile, "deleting file: " + dataFile)
			os.remove(rootFolder + dataFile)	

		#debug ending
		writeLog(logFile, "Ended...")
		logFile.close
	else:
		writeLog(logFile, "Internet not connected...ending...")
		logFile.close
except:# catch *all* exceptions
	exc_type, exc_value, exc_traceback = sys.exc_info()
	lines = traceback.format_exception(exc_type, exc_value, exc_traceback)
	for line in lines:
		writeLog(logFile, line)
	e = sys.exc_info()[0]
	writeLog(logFile, "Exception: " + str(e))
	logFile.close