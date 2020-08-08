# Bytes In Disguise (⌐■_■)
# DEF CON 28 Safe mode talk
Non-Volatile Memory. EVERY computer has it, from the chip that stores your BIOS to the controller that runs your laptop trackpad and even your new fancy USB-C monitor. These small nooks of storage can be (ab)used by anyone to store data or code without causing any side effects and none would be the wiser. We will show you more than one example of how this is possible and walk through everything you need to know to do it, too.

In this talk, we will describe how to hide persistence in these obscure memory chips using simple tools that we are releasing as open source. We will show multiple ways to accomplish this without detection. On the defensive front, we’ll discuss what can be done to detect and lock

## Speakers:
@HackingThings & @JesseMichael
  
## Content of this repository
### Slides
- Located in \Slides - only a single PDF file.
### Videos and Audio from the slides
- AUDIO - Topher and Mike Recapping Hide Yo Sh!t
- DEMO 1 - CMOS RW 
- DEMO 2 - ASMedia meterpreter
- DEMO 3 - GBe
### Code
- CMOS_RW
  - Code showing reading and writing to unsused CMOS regions
- ASMediaFWDemo
  - Code showing downloading and executing shellcode hidden inside ASMedia firmware.
- SuperSneakyExec
  - Example code in C# to download and execute shellcode from the internet 
