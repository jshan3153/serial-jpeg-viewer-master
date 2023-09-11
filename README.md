serial-jpeg-viewer
==================

A viewer for text(ascii) jpeg data sent on the serial port. Just send a string called 'jpg\n' before each frame and send all the jpeg data in one line "ff,d8,32..." and with a '\n' at the end. Built as a quick hack since human readability of the bytes was a requirement. Else, sending the jpeg data as binary is a much faster alternative.

You'll need Visual Studio 10 atleast to compile this. Also before compiling set you baudrate and serial port number in the source. Since mine were constant, I did not provide a configurable setting.

Sample frames
```
jpg
ff,d8,2a,40,0,0,50,c0,7,d0,81,0,50,d4,0,18,0,30,c0,20,38,8,0,0,a2,0,15,70,
jpg
ff,d8,2a,40,0,0,50,c0,7,d0,81,0,50,d4,0,18,0,30,c0,20,38,8,0,0,a2,0,15,70,20,38,8,0,0,a2,0,15,0,18,0,30,c0,20,
```
