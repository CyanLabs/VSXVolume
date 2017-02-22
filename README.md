VSXVolume
=========

Hijack keyboard volume keys

Hey guys here is a quick application release, it does one thing and one thing only, hijacks the volume control keys (Vol Up, Vol Down, Mute) to control your Pioneer VSX AVR via telnet instead of the computer volume.

V1.0.0
Launch the application with the first command line argument as the VSX IP address such as "VSX Volume.exe 192.168.0.1" if you launch it without arguments it will warn you and exit.

V1.1.0
Launch the application with the following parameter /ip=xxx.xxx.xxx.xxx such as "VSX Volume.exe /ip=192.168.0.1" if you launch it without arguments it will warn you and exit.

This application has no GUI and amy crash if it loses connection to the AVR.
