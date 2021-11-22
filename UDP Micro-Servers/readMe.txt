AUTHOR: Emmanuel Omari-Osei
UCID: 30092729


INSTRUCTIONS:

Files for this program should be found in a zip folder named:
"Emmanuel Omari-Osei - Assignment 2.zip"

Please unzip this folder and compile all the c program files in the folder:
1). indirectionServer.c
2). echoServer.c
3). translatorServer.c
4). currencyServer.c
5). votingServer.c

This must be done for each file in c file in the directory individually.

Ex: gcc -Wall -o indirectionServer indirectionServer.c

Once the all programs have been fully compiled, please locate the appropriate
executable files for each c program and run them individually as directed below.
This will require separate terminal windows (one for each program) to perform
correctly.

!! NOTE !!: The programs for the echoServer.c, translatorServer.c, currencyServer.c,
votingServer.c must be run on the IP 136.159.5.25 in order for the program
to run without modifying the code.
If this is not possible, please modify the "microServerIP" character array on line 26
in the indirectionServer.c file. The c-string should be set to the prefered
IP address. Following such, run the above four programs on that same IP address.
The IP address that the indirectionServer executable will be run on does not matter.

!! NOTE !!: The port numbers for all the executable files must be run set with a
specified scheme in mind. The port numbers must increment by one from the indirectionServer
port number in order to be correctly located by the indirectionServer executable.
The specific order of incrementation can be shown below with X representing
the port number of the indirectionServer executable:
1). indirectionServer -> Port: X
2). translatorServer -> Port: X+1
3). currencyServer -> Port: X+2
4). votingServer -> Port: X+3

!! NOTE !!: The echoServer is an optional executable and was primarily used for testing.
If you would like to run this server in the program with the same port number as the
indirectionServer (X in the example above)

The following ways are different ways to run the above program executable files.
(ALL executable files in this project use this format to be run)

1) ./proxyServer IP_Address Port_Number
	Runs the program on the IP_Address provided using the Port_Number
	Ex: ./indirectionServer 136.159.5.27 9000

2) ./proxyServer Port_Number
	Runs the program on the IP_Address 136.159.5.25 using the Port_Number
	Ex: ./indirectionServer 9000

3) ./proxyServer
	Runs the program on the IP Address 136.159.5.25 using the port 8000
	Ex: ./indirectionServer

One correct example to run the programs following the specified format above
would be below but is not limited to such:
(Each line is run on a separate terminal window)

Ex:
    ./indirectionServer 136.159.5.27 8000
    ./translatorServer 136.159.5.25 8001
    ./currencyServer 136.159.5.25 8002
    ./votingServer 136.159.5.25 8003
    ./echoServer 136.159.5.25 8000

Once all the servers are running correctly, the indirectionServer will be able
to receive requests from any other sockets through TCP when they by connect to
the specified IP_Address and Port_Number. One good way to connect to the server
is through Telnet, but other text clients can be used aswell.

Ex: telnet 136.159.5.27 8000 (for the example above)

Once your socket connects to the indirectionServer, a number of options will
be displayed for micro server services that will be performed by the
indirectionServer through a UDP request to the appropriate micro server.
These options can be selected by inputting the number correlating to the service
and following the directions displayed to the connecting socket's terminal.

If the program takes too long to connect to a specific micro server, it
will simply return to the selection menu for mirco services.

Special Voting Server Notes:

* Results can only be viewed after a vote has been placed.
* When placing a vote with the VOTE command, you will be sent an encrypted voting key.
  This must be input to the voting server with the KEY command to secure your vote.
* You can obtain a new encrypted voting key over and over with the VOTE command
  until you have secured the vote with the KEY command. ONCE YOU HAVE PLACED YOUR VOTE,
  YOU CANNOT VOTE AGAIN WITH THE SAME SOCKET. If you would like to vote again as a
  different user, you can exit the indirectionServer and reconnect to obtain a new socket.


TROUBLESHOOTING:

* Please ensure the programs are run in the correct format as instructed
  above to avoid error.
* If any server prints a binding error, please re-run the server on another port to correct
  the issue. Please ensure the new port follows the appropriate formate instructed above
  when doing so.


TESTING AND EXPERIMENTATION:

Most of the tests for these programs were completed using a local device
connected through SSH to the CPSC Linux servers. From there, the servers
were hosted on multiple terminals in the same way they are done in the
final program execution.

The echoServer executable is an example as to how some of the earlier testing
for the UDP connection was tested. In this testing server, the program
takes in the input the user types and displays it back to them. This proved
useful for making sure the data was correctly recieved and could be modified.

The WiFi networks the program has been tested on include the University of Calgary
WiFi network and the author's personal home network.


AUTHOR COMMENTS:

This assignment was fairly interesting to create. It really helped me understand
just how UDP differs from the TCP used in the previous assignment. One notable
point I came across is that the UDP data transfers actually send the entire socket
descriptor was used to send the data, which makes it easy to send informaion back.
It is nice to know there is a simple, while unreliable, data transfer protocol
available for use when trying to send a small amount of data.

Thank you for viewing my assignment!
