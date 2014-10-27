import urllib
import os

def internet_on():
	try:
		response=urllib.urlopen("http://contactmanager5810.azurewebsites.net/api/contacts")
		return True
	except: pass
	return False

#debug starting
test = open("/mnt/sd/arduino/www/mysketch/start.txt", "w")
test.write("monkey")
test.close()

# if connected to internet
	if internet_on():
	file = open("/mnt/sd/arduino/www/mysketch/test.txt", "w")
	ins = open( "/mnt/sd/arduino/www/mysketch/datalog.txt", "r" )
	array = []
	for line in ins:
		array.append(line)
		file.write(line)
	ins.close()
	file.close()

	#Precision of the minutes. If +-precision, data is not sent
	precision = 5
	#Even if coordinates are same, how many same till we send again
	thresholdSameCount = 10
	oldLongitudePrecise = -1
	oldLatitudePrecise = -1
	iterator = 0

	for original in array:
		iterator = iterator + 1
		if original.startswith("$GPGGA"):
			longitude =  [x.strip() for x in original.split(',')][2]
			latitude =  [x.strip() for x in original.split(',')][4]
			#print longitude
			#print latitude
			if (len(longitude) > 0 and len(latitude) > 0):
				longitudePrecise = int([x.strip() for x in longitude.split('.')][1])
				latitudePrecise = int([x.strip() for x in latitude.split('.')][1])
				if((oldLongitudePrecise - precision > longitudePrecise or oldLongitudePrecise + precision < longitudePrecise) or(oldLatitudePrecise - precision > latitudePrecise or oldLatitudePrecise + precision < latitudePrecise) or iterator > thresholdSameCount):
					#print str(longitudePrecise) + "," + str(latitudePrecise)
					#send data up
					data = urllib.urlencode({'Name': 'x', 'Address': original.strip('\n'), 'City': 'd', 'Zip': 'e', 'Email': 'f', 'Twitter': 'y'})
					#print (data)
					binary_data = data.encode('utf8')
					f = urllib.urlopen("http://contactmanager5810.azurewebsites.net/api/contacts", binary_data)
					print (f.read())
					iterator = 0
				oldLongitudePrecise = longitudePrecise
				oldLatitudePrecise = latitudePrecise

	#delete file since we are done with it		
	os.remove("/mnt/sd/arduino/www/mysketch/datalog.txt")	

#debug ending
end = open("/mnt/sd/arduino/www/mysketch/end.txt", "w")
end.write("monkey")
end.close()