#Change for arduino
import urllib
#Change for VS
#import urllib.request
#import urllib.parse
#Change
import datetime
import os
import sys
import time
import traceback
########################## METHODS ##########################################
def internet_on(logsFolder):
	try:
		#Change for arduino
		response=urllib.urlopen("http://contactmanager5810.azurewebsites.net/api/contacts")
		#Change for VS
		#response=urllib.request.urlopen("http://contactmanager5810.azurewebsites.net/api/contacts")
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
			os.remove(os.path.join(path, f))

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
			iterator = 0

			for original in array:
				writeLog(logFile, "_________________________")
				iterator = iterator + 1
				if original.startswith("$GPGGA"):
					writeLog(logFile, "Row found starting with $GPGGA: " + original)
					longitude =  [x.strip() for x in original.split(',')][2]
					latitude =  [x.strip() for x in original.split(',')][4]
					writeLog(logFile, "longitude: " + longitude)
					writeLog(logFile, "latitude: " + latitude)
					if (len(longitude) > 0 and len(latitude) > 0):
						longitudePrecise = int([x.strip() for x in longitude.split('.')][1])
						latitudePrecise = int([x.strip() for x in latitude.split('.')][1])
						writeLog(logFile, "longitudePrecise: " + str(longitudePrecise))
						writeLog(logFile, "latitudePrecise: " + str(latitudePrecise))
						writeLog(logFile, "oldLongitudePrecise: " + str(oldLongitudePrecise))
						writeLog(logFile, "oldLatitudePrecise: " + str(oldLatitudePrecise))
						if((oldLongitudePrecise - precision > longitudePrecise or oldLongitudePrecise + precision < longitudePrecise) or(oldLatitudePrecise - precision > latitudePrecise or oldLatitudePrecise + precision < latitudePrecise) or iterator > thresholdSameCount):
							writeLog(logFile, "Sending data up...")
							#Change for arduino
							data = urllib.urlencode({'Name': 'x', 'Address': original.strip('\n'), 'City': 'd', 'Zip': 'e', 'Email': 'f', 'Twitter': 'y'})
							#Change for VS
							#data = urllib.parse.urlencode({'Name': 'x', 'Address': original.strip('\n'), 'City': 'd', 'Zip': 'e', 'Email': 'f', 'Twitter': 'y'})
							#Change
							writeLog(logFile, "data: " + str(data))
							binary_data = data.encode('utf8')
							#Change for arduino
							f = urllib.urlopen("http://contactmanager5810.azurewebsites.net/api/contacts", binary_data)
							#Change for VS
							#f = urllib.request.urlopen("http://contactmanager5810.azurewebsites.net/api/contacts", binary_data)
							#Change
							writeLog(logFile, f.read())
							writeLog(logFile, "Sent data up...")
							iterator = 0
						else:
							writeLog(logFile, "No changes detected...")
						oldLongitudePrecise = longitudePrecise
						oldLatitudePrecise = latitudePrecise

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
