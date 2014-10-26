import urllib
import os

test = open("/mnt/sd/arduino/www/mysketch/start.txt", "w")
test.write("nonkey")
test.close()

file = open("/mnt/sd/arduino/www/mysketch/test.txt", "w")
ins = open( "/mnt/sd/arduino/www/mysketch/datalog.txt", "r" )
array = []
for line in ins:
	array.append(line)
	file.write(line)
ins.close()
file.close()

for x in array:
	data = urllib.urlencode({'Name': 'x', 'Address': x.strip('\n'), 'City': 'd', 'Zip': 'e', 'Email': 'f', 'Twitter': 'y'})
	print (data)
	binary_data = data.encode('utf8')
	f = urllib.urlopen("http://contactmanager5810.azurewebsites.net/api/contacts", binary_data)
	print (f.read())

os.remove("/mnt/sd/arduino/www/mysketch/datalog.txt")	
end = open("/mnt/sd/arduino/www/mysketch/end.txt", "w")
end.write("nonkey")
end.close()