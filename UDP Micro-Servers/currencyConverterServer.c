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
int getCurrencyFromMessage(char* dest, const char* message);
double getAmountFromMessage(const char* message);

int serverPortNumber = 8002;

double CNDToUSDRatio = 0.79;
double CNDToEuroRatio = 0.70;
double CNDToPoundRatio = 0.59;
double CNDToBitcoinRatio = 0.000013;

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
        
        if (strncmp(messageIn, "START", 5) == 0)
        {
            strcpy(messageReply, "Currency Converter:\n");
            strcat(messageReply, "Currencies: usd, euros, pounds, bitcoin\n");
            strcat(messageReply, "Format: $5.00 cad euros\n");
        }
        else
        {
            // Get Currency Type:
            char currency[MAX_MESSAGE_LENGTH];
            memset(&currency, '\0', sizeof(currency));
            getCurrencyFromMessage(currency, messageIn);
            
            // Set to LowerCase
            int index = 0;
            while(index < strlen(currency))
            {
                currency[index] = tolower(currency[index]);
                index++;
            }
            strcpy(messageReply, currency);
            double result = getAmountFromMessage(messageIn);

            // Get Currency Conversion:
            if (strncmp(currency, "usd", 3) == 0) result *= CNDToUSDRatio;
            else if (strncmp(currency, "euros", 5) == 0) result *= CNDToEuroRatio;
            else if (strncmp(currency, "pounds", 6) == 0) result *= CNDToPoundRatio;
            else if (strncmp(currency, "bitcoin", 7) == 0) result *= CNDToBitcoinRatio;
            else result = -1;

            if (result == -1)
            {
                strcpy(messageReply, "Invalid Currency Input!");
                strcat(messageReply, "\nEND");
            }
            else
            {
                char strVal[MAX_MESSAGE_LENGTH];
                sprintf(strVal, "%lf", result);

                strcpy(messageReply, "Conversion: ");
                strcat(messageReply, strVal);
                strcat(messageReply, " ");
                strcat(messageReply, currency);
                strcat(messageReply, "\nEND");
            }
        }

        fprintf(stdout, "\nSending Message to Client | IP: %s, Port: %d\n", inet_ntoa(clientSocketInfo.sin_addr), ntohs(clientSocketInfo.sin_port));
        sendto(serverSocket, messageReply, strlen(messageReply) , 0, client, clientSocketInfoSize);
        printf("\nMessage Sent to Client!\n\n");
        
        clearStrings();          
    }
    
    exitCode();
}

double getAmountFromMessage(const char* message)
{
    int start = 0;
    while(start < strlen(message) && !isdigit(message[start]))
    {
        start++;
    }

    int end = start+1;
    while(end < strlen(message) && !isspace(message[end]))
    {
        end++;
    }

    char dest[MAX_MESSAGE_LENGTH];
    strncpy(dest, message + start, end - start);
    char zero = '\0';
    strcat(dest, &zero);

    return atof(dest);
}

int getCurrencyFromMessage(char* dest, const char* message)
{
    int start = 0;
    while(start < strlen(message) && !isspace(message[start]))
    {
        start++;
    }
    start++;

    while(start < strlen(message) && !isspace(message[start]))
    {
        start++;
    }
    start++;

    int end = start+1;
    while(end < strlen(message) && !isspace(message[end]))
    {
        end++;
    }

    strncpy(dest, message + start, end - start);
    char zero = '\0';
    strcat(dest, &zero);

    return ++end;
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