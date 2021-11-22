#include <stdio.h>
#include <errno.h>
#include <stdlib.h>
#include <unistd.h>
#include <netinet/in.h>
#include <netdb.h>
#include <sys/socket.h>
#include <signal.h>
#include <string.h>
#include <ctype.h>
#include <arpa/inet.h>

/* Global manifest constants */
#define NUMBER_OF_OPTIONS 3
#define MAX_MESSAGE_LENGTH 4096
#define SOCKET_TIMEOUT_VAL 3
#define DEBUG 0

int serverSocket, clientSocket, senderSocket;

void exitCode();
void clearStrings();
void sendToTerminalAndSocket(int socket, const char* message);

char serverIPAddress[32] = "136.159.5.25";
char microServerIP[32] = "136.159.5.25";

int serverPortNumber = 8000;
int echoServerPortNumber = 8000;
int translatorServerPortNumber = 8001;
int currencyServerPortNumber = 8002;
int votingServerPortNumber = 8003;

char messageIn[MAX_MESSAGE_LENGTH];
char messageReply[MAX_MESSAGE_LENGTH+3];

int main(int argc, char *argv[])
{
    if(argc >= 3)
    {
        strcpy(serverIPAddress, argv[1]);
        serverPortNumber = atoi(argv[2]);
    }
    else if (argc == 2)
    {
        serverPortNumber = atoi(argv[1]);
    }

    /* Set Server Port Numbers (based on Indirection server port number) */
    echoServerPortNumber = serverPortNumber;
    translatorServerPortNumber = serverPortNumber+1;
    currencyServerPortNumber = serverPortNumber+2;
    votingServerPortNumber = serverPortNumber+3;

    printf("Using IP: %s on Port: %d", serverIPAddress, serverPortNumber);

    struct sockaddr_in serverSocketInfo, clientSocketInfo, microServerInfo;
    int microServerSocketInfoSize = sizeof(microServerInfo);
 
    /* Clean message strings (set to zero) and make them thread safe (null-terminated) */
    clearStrings();

    /* Initialize Server sockaddr Structure */
    memset(&serverSocketInfo, 0, sizeof(serverSocketInfo));
    serverSocketInfo.sin_family = AF_INET;
    serverSocketInfo.sin_port = htons(serverPortNumber);
    serverSocketInfo.sin_addr.s_addr = inet_addr(serverIPAddress);
    
    /* Set Up the Transport-Level End Point to use TCP */
    serverSocket = socket(AF_INET, SOCK_STREAM, 0);
    if(serverSocket == -1)
    {
        fprintf(stderr, "Server Socket Creation Failed!\n");
        exitCode();
    }
    printf("\nServer Socket Created!\n");

    /* Bind */
    if(bind(serverSocket, (struct sockaddr *)&serverSocketInfo, sizeof(serverSocketInfo)) < 0)
	{
		fprintf(stderr, "Binding Failed: %s !\n", strerror(errno));
        exitCode();
	}
	printf("Binding Successful!\n");
    
    /* Clean message strings (set to zero) and make them thread safe (null-terminated) */
    clearStrings();

    while (1)
    {
        /* Listen for socket connections */
        listen(serverSocket , 3);

        /* Accept Connecting Socket */
        printf("\nWaiting for a connection...\n");
        int clientInfoSize = sizeof(clientSocketInfo);
        clientSocket = accept(serverSocket, (struct sockaddr*)&clientSocketInfo, (socklen_t*)&clientInfoSize);
        if (clientSocket < 0)
        {
            fprintf(stderr, "Accept Failed!\n");
            exitCode();
        }
        printf("Connection Accepted!\n");

        /* Set Up the Sender Socket to use UDP */
        int voteEncryptionKey = 0; // Reset Vote Key
        if ((senderSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP))==-1)
        {
            fprintf(stderr, "Server Socket Creation Failed!\n");
            exitCode();
        }
        printf("\nSender Socket Created!\n");

        /* Set Up the Sender Socket Timeout */
        struct timeval timeoutInfo;
        timeoutInfo.tv_sec = SOCKET_TIMEOUT_VAL;
        timeoutInfo.tv_usec = 0;
        if ((setsockopt(senderSocket, SOL_SOCKET, SO_RCVTIMEO, &timeoutInfo, sizeof(timeoutInfo))) < 0)
        {
            fprintf(stderr, "Server Socket Timeout Setting Failed!\n");
            exitCode();
        }            
        printf("\nSender Socket Timeout Set!\n");

        while (1)
        {
            /* Prompt Client for Selection */
            
            int selectedNum = -1;

            /* Selection Loop */
            while(1)
            {
                clearStrings();
                strcpy(messageReply, "\nPlease Input the Number for the Associated Service:\n");
                strcat(messageReply, "E). Echo Service\n");
                strcat(messageReply, "1). Translator Service\n");
                strcat(messageReply, "2). Currency Converter Service\n");
                strcat(messageReply, "3). Voting Service\n");
                strcat(messageReply, "\n");
                sendToTerminalAndSocket(clientSocket, messageReply);

                // Recieve Client Message:
                if (recv(clientSocket, messageIn, MAX_MESSAGE_LENGTH, 0) < 0)
                {
                    fprintf(stderr, "Recieve from Client Failed!\n");
                    clearStrings();
                    continue;
                }
                strcat(messageIn, "\0");

                // Hard EXIT:
                if (strncmp(messageIn, "EXIT", 4) == 0 || strncmp(messageIn, "END", 3) == 0) break; 

                if (strncmp(messageIn, "E", 1) == 0)
                {
                    selectedNum = 101;
                    break;
                }

                // Check Input and Convert to Int:
                selectedNum = atoi(messageIn);

                if (selectedNum <= 0 || selectedNum > NUMBER_OF_OPTIONS)
                {
                    clearStrings();

                    strcpy(messageReply, "Invalid Input!\n\n");
                    sendToTerminalAndSocket(clientSocket, messageReply);

                    clearStrings();
                    continue;
                }

                break;
            }

            /* Client Request Interpretation */
            
            /* Hard EXIT */
            if (strncmp(messageIn, "EXIT", 4) == 0 || strncmp(messageIn, "END", 3) == 0)
            {
                clearStrings();

                sprintf(messageReply, "\nDisconnecting Server!\n\n");
                sendToTerminalAndSocket(clientSocket, messageReply);

                close(clientSocket);
                clearStrings();
                break;
            }

            /* Set Destination Server */
            char selectedServerName[MAX_MESSAGE_LENGTH/2];
            char selectedServerIP[MAX_MESSAGE_LENGTH];
            int selectedServerPortNumber;
            if (selectedNum == 1)
            {
                strcpy(selectedServerName, "Translator Server");
                strcpy(selectedServerIP, microServerIP);
                selectedServerPortNumber = translatorServerPortNumber;
            }
            else if (selectedNum == 2)
            {
                strcpy(selectedServerName, "Currency Converter Server");
                strcpy(selectedServerIP, microServerIP);
                selectedServerPortNumber = currencyServerPortNumber;
            }
            else if (selectedNum == 3)
            {
                strcpy(selectedServerName, "Voting Server");
                strcpy(selectedServerIP, microServerIP);
                selectedServerPortNumber = votingServerPortNumber;
            }
            else
            {
                strcpy(selectedServerName, "Echo Server");
                strcpy(selectedServerIP, microServerIP);
                selectedServerPortNumber = echoServerPortNumber;
            }

            printf("Client Message: %s\n", messageIn);
            printf("Selected Server IP: %s\n", selectedServerIP);
            printf("Selected Server Port Number: %d\n", selectedServerPortNumber);

            /* Prepare User Info Grab */

            /* Set Up Selected Destination Micro-Server Information */            
            memset((char *) &microServerInfo, 0, sizeof(microServerInfo));
            microServerInfo.sin_family = AF_INET;
            microServerInfo.sin_port = htons(selectedServerPortNumber);
            struct sockaddr * microServer = (struct sockaddr *) &microServerInfo;

            if (inet_pton(AF_INET, selectedServerIP, &microServerInfo.sin_addr)==0)
            {
                printf("inet_pton() failed\n");
                exitCode();
            }

            /* Start Client-Microserver Communications */
	        clearStrings();
            sprintf(messageReply, "Starting Communication with %s | IP: %s, Port: %d\n\n", selectedServerName, inet_ntoa(microServerInfo.sin_addr), ntohs(microServerInfo.sin_port));
            sendToTerminalAndSocket(clientSocket, messageReply);
            
            // Wake Up Server:
            clearStrings();
            strcpy(messageIn, "START");
            if (sendto(senderSocket, messageIn, strlen(messageIn), 0, microServer, microServerSocketInfoSize) == -1)
            {
                sprintf(messageReply, "Send Failed: %s !\n", strerror(errno));
                sendToTerminalAndSocket(clientSocket, messageReply);
                clearStrings();
                continue;
            }
            printf("Client Message Sent to %s!\n", selectedServerName);
            printf("\nReading on IP: %s on Port: %d \n", inet_ntoa(microServerInfo.sin_addr), ntohs(microServerInfo.sin_port));

            while (1)
            {
                // Recieve Reply from Micro Server:
                clearStrings();
                if (recvfrom(senderSocket, messageReply, MAX_MESSAGE_LENGTH, 0, microServer, (socklen_t*) &microServerSocketInfoSize) < 0)
                {
                    sprintf(messageReply, "Message Recieved Failed!\n");
                    sendToTerminalAndSocket(clientSocket, messageReply);
                    clearStrings();
                    break;
                }
                fprintf(stdout, "Recieved Reply from Micro Server:\n\n");

                // Voting Server Encryption KEY:
                if (selectedNum == 3 && strncmp(&messageReply[strlen(messageReply)-6], "KEY", 3) == 0)
                {
                    voteEncryptionKey = messageReply[strlen(messageReply)-2] - '0';
                    fprintf(stdout, "Obtained Voting Encryption Key: %d\n\n", voteEncryptionKey);
                    messageReply[strlen(messageReply)-6] = '\0';
                    strcat(messageReply, "\n");
                }

                // Print Message:
                sendToTerminalAndSocket(clientSocket, messageReply);

                // END Communication:
                if (strncmp(&messageReply[strlen(messageReply)-3], "END", 3) == 0)
                {
                    sendToTerminalAndSocket(clientSocket, "\n");
                    break;
                }

                // Get Message from Client:
                clearStrings();
                if (recv(clientSocket, messageIn, MAX_MESSAGE_LENGTH, 0) < 0)
                {
                    fprintf(stderr, "Recieve from Client Failed!\n");
                    clearStrings();
                    continue;
                }
                // Vote Key Encription Forwarding:
                if (strncmp(messageIn, "KEY", 3) == 0 && voteEncryptionKey)
                {
                    fprintf(stdout, "Forwarding Encryption Key: %d!\n", voteEncryptionKey);
                    char dest[MAX_MESSAGE_LENGTH];
                    sprintf(dest, " %d", voteEncryptionKey);
                    strcat(messageIn, dest);
                }
                // Send Message to Micro Server:
                if (sendto(senderSocket, messageIn, strlen(messageIn), 0, microServer, microServerSocketInfoSize) == -1)
                {
                    sprintf(messageReply, "Send Failed: %s !\n", strerror(errno));
                    sendToTerminalAndSocket(clientSocket, messageReply);
                    clearStrings();
                    continue;
                }
                printf("\nClient Message: %s\n", messageIn);
                printf("Sent to %s!\n", selectedServerName);
            }

            clearStrings();
        }
           
    }
    
    close(senderSocket);
    exitCode();
}

void sendToTerminalAndSocket(int socket, const char* message)
{
    printf("%s", message);
    send(socket, message, strlen(message), 0);
}

void exitCode()
{
    close(clientSocket);    
    close(serverSocket);
    clearStrings();
    exit(0);
}

void clearStrings()
{
    /* Clean message strings (set to zero) and make them thread safe (null-terminated) */
    memset(&messageIn, '\0', sizeof(messageIn));
    memset(&messageReply, '\0', sizeof(messageReply));
}