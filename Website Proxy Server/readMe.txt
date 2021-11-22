AUTHOR: Emmanuel Omari-Osei
UCID: 30092729

INSTRUCTIONS:

Files for this program should be found in a zip folder named:
"Emmanuel Omari-Osei - Assignment 1.zip"

Please unzip this folder and compile the c program named:
"Assignment 1 Proxy Server" in the program folder using a
gcc compiler or a compiler equivalent.

Ex: gcc -o proxyServer 'Assignment 1 Proxy Server'.c

Once the program has been fully compiled, please locate the executable file
("a.out" or "a.exe" by default, "proxyServer" in the example above)
In one of the following ways to start the proxy server:

1) ./proxyServer IP_Address Port_Number
	Runs the program on the IP_Address provided using the Port_Number
	Ex: ./proxyServer 136.159.5.41 8000

2) ./proxyServer Port_Number
	Runs the program on the IP address 127.0.0.1 using the Port_Number
	Ex: ./proxyServer 8000

3) ./proxyServer
	Runs the program on the IP address 127.0.0.1 using the port 8000
	Ex: ./proxyServer

Once the server is running, it will be able to receive requests from the browser
or any other sockets by connecting to the specified IP address and port number.

If you want to block sites that have an inappropriate word in their URL, you
can do so by connecting to the server using a program like Telnet, issue
the commands below:

1) BLOCK word
	Tells the proxy to block sites that have the specified word contained
	in their URL.
	Ex: BLOCK Hello

2) UNBLOCK word
	Tells the proxy to remove the specified word from the list of words
	it will try to block.
	Ex: UNBLOCK Hello

3) PRINTBLOCKED
	Tells the proxy to prints the current list of words
	it will try to block. This command also has short-hand versions
	such as PRINTBLOCK and PRINTBL.

After inputting one of the commands above, the program will display a message
about the command sent and close the connection with the Telnet program
(or any other connection program). From then on, the action will be reflected
in the server. More commands can be input at any time the server is waiting
on a connection to block and unblock site URLs dynamically.


TESTING AND EXPERIMENTATION:

Most of the tests for this program were completed using a local device
connected through SSH to the CPSC Linux servers. From there, the server
was hosted, and a local browser on the device used the proxy.

The WiFi networks the program has been tested on include the University of Calgary
WiFi network and the author's personal home network.

While the proxy server can successfully handle blocking sites that have inappropriate
words in their URL, it cannot handle these same words in the content body of the
site. Testing was performed to attempt this; however, these do not appear
in the final product.

The proxy server was not equipped to handle HTTPS requests, only those of HTTP.
The program will attempt to ignore most HTTPS requests some may cause the
program to crash. In the event of a program crash, please re-run the program.


AUTHOR COMMENTS:

This was an exciting assignment, and I learned a lot about sockets and how
useful they are for navigating the network. I greatly appreciate the fact
that I can now say I have coded a web proxy myself. Of course, there were
a lot of questions and queries made along the way, but I am satisfied with
the end product.

I hope you enjoy my proxy, and thank you for viewing my assignment.
