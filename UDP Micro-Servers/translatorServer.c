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
#define MAX_WORD_LIST_SIZE 100
#define DEBUG 0

int serverSocket;

void exitCode();
void clearStrings();

int serverPortNumber = 8001;

char messageIn[MAX_MESSAGE_LENGTH];
char messageReply[MAX_MESSAGE_LENGTH+3];
char serverIPAddress[32] = "136.159.5.25";

const char* translateWord(const char* word);
int sizeOfWordList = 5;
const char* englishWords[MAX_WORD_LIST_SIZE];
const char* frenchWords[MAX_WORD_LIST_SIZE];

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

    englishWords[0] = "hello";
    englishWords[1] = "dog";
    englishWords[2] = "sugar";
    englishWords[3] = "cheese";
    englishWords[4] = "bye";
    
    frenchWords[0] = "Bonjour";
    frenchWords[1] = "Chien";
    frenchWords[2] = "Sucre";
    frenchWords[3] = "Fromage";
    frenchWords[4] = "Au Revoir";

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
            strcpy(messageReply, "Dictionary Words: Hello, Dog, Cheese, Sugar, Bye");
            strcat(messageReply, "\nEnter an English word: ");
        }
        else
        {
            // Clean Word and Make Word Lowercase:
            int index = 0;
            while(index < strlen(messageIn) && !isspace(messageIn[index]))
            {
                messageIn[index] = tolower(messageIn[index]);
                index++;
            }
            messageIn[index] = '\0';

            // Translate and Respond:
            const char* newWord = translateWord(messageIn);
            messageIn[0] = toupper(messageIn[0]);

            if (strncmp(newWord, "NONE", 4) == 0)
            {
                strcpy(messageReply, messageIn);
                strcat(messageReply, " is not in Dictionary!");
                strcat(messageReply, "\nEND");
            }
            else
            {
                strcpy(messageReply, "French translation: ");
                strcat(messageReply, newWord);
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

const char* translateWord(const char* word)
{
    int index = 0;
    while(index < sizeOfWordList)
    {
        if (strncmp(englishWords[index], word, strlen(word)) == 0)
        {
            return frenchWords[index];           
        }
        else index++;
    }
    return "NONE";
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