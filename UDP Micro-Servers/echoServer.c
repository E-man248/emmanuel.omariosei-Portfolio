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
#define MAX_MESSAGE_LENGTH 4096
#define DEBUG 0

int serverSocket;

void exitCode();
void clearStrings();

int serverPortNumber = 8000;

char messageIn[MAX_MESSAGE_LENGTH];
char messageReply[MAX_MESSAGE_LENGTH+3];
char serverIPAddress[32] = "136.159.5.25";

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
    printf("Using IP: %s on Port: %d", serverIPAddress, serverPortNumber);

    struct sockaddr_in serverSocketInfo, clientSocketInfo;
    int clientSocketInfoSize = sizeof(clientSocketInfo);
 
    /* Clean message strings (set to zero) and make them thread safe (null-terminated) */
    clearStrings();

    /* Initialize Server sockaddr Structure */
    memset(&serverSocketInfo, 0, sizeof(serverSocketInfo));
    serverSocketInfo.sin_family = AF_INET;
    serverSocketInfo.sin_port = htons(serverPortNumber);
    serverSocketInfo.sin_addr.s_addr = inet_addr(serverIPAddress);
    struct sockaddr * client = (struct sockaddr *)&clientSocketInfo;
    
    /* Set Up the Server Socket to use UDP */
    serverSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
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
        /* Clear Client sockaddr Structure */
        memset(&clientSocketInfo, 0, sizeof(clientSocketInfo));

        /* Recieve Message on Port */
        printf("\nReading on IP: %s on Port: %d \n", serverIPAddress, serverPortNumber);

        int readSize;
        if ((readSize = recvfrom(serverSocket, messageIn, MAX_MESSAGE_LENGTH, 0, client, (socklen_t*) &clientSocketInfoSize)) < 0)
        {
            fprintf(stderr, "Recieve Failed!\n");
            clearStrings();
            continue;
        }
        messageIn[readSize] = '\0';
        printf("\nClient on IP: %s on Port: %d \n", inet_ntoa(clientSocketInfo.sin_addr), ntohs(clientSocketInfo.sin_port));
        printf("Client Message: %s\n\n", messageIn);

        /* Special Client Requests */
        
        /* Hard EXIT */
        if (strncmp(messageIn, "EXIT", 4) == 0)
        {
            clearStrings();

            sprintf(messageReply, "\nDisconnecting Server!\n\n");
            printf("%s", messageReply);
            sendto(serverSocket, messageReply, strlen(messageReply) , 0, client, clientSocketInfoSize);

            clearStrings();
            break;
        }

        /* Send Message Reply to Client */

        strcpy(messageReply, "Echo: ");
        strcat(messageReply, messageIn);
        if (strncmp(messageIn, "END", 3) == 0)
        {
            strcat(messageReply, "END");
        }
        else
        {
            strcat(messageReply, "\n");
        }

        fprintf(stdout, "\nSending Message to Client | IP: %s, Port: %d\n", inet_ntoa(clientSocketInfo.sin_addr), ntohs(clientSocketInfo.sin_port));
        sendto(serverSocket, messageReply, strlen(messageReply) , 0, client, clientSocketInfoSize);
        printf("\nMessage Sent to Client!\n\n");
        
        clearStrings();          
    }
    
    exitCode();
}

void exitCode()
{   
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