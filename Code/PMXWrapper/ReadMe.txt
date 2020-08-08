Due to the vague legal status of distributing the driver and dll, 
we won't be able to include the files in the public repo, 
but we can tell you where you might be able to find them.

Where can you obtain the pmx driver?
- first, you will need to obtain a copy of some of the Intel tools prior to August 2019 (Pre-ScrewedDrivers publication)
- Second, You will need to use a tool like ResourceHacker to extract the internal resources in the tool, they include x86 and x64 drivers.
- third, you will end up finding the occompanying dlls Pmxdll32e.dll and Idrvdll32e.dll

One sources for the required files:
http://ftp.hp.com/pub/softpaq/sp82501-83000/sp82644.exe
if you unzip the file ,you might be able to find the needed files
you can use resource hacker to extract the 32/64 pmxdrv driver from an executable and rename it to pmxdrv.sys


Code tested with the following files:

Name: Pmxdll32e.dll SHA256: 1882C65036142FEE6B89042E2B7BF383AAA8E151B65942F5B74C03AB5CC4E5F9

Name: Idrvdll32e.dll SHA256: 65A34921EDE066338D9717DDFA134B54294BB81509D40061D2F4BC441FC392C5

pmxdrv.sys | PCIUTIL SHA256: 82B30461DBF40AC15FCE6A83B9BAD2EBD05B27DEA1B784EAA096422FE8927B7B  


