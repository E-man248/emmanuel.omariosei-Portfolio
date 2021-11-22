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
#define MAX_BLOCKED_WORD_LIST_SIZE 100
#define DEBUG 0

int serverSocket, clientSocket, senderSocket;

void exitCode();
void clearStrings();
int getURLFromMessage(char* dest, const char* message);
int getMethodFromMessage(char* dest, const char* message);
int getHTTPVerFromMessage(char* dest, const char* message);
int getDomainNameFromURL(char* dest, const char* message);

int checkMessageForWord(const char* message, const char* word);
int blockWord(const char* word);
void unBlockWord(const char* word);
void getPrintBlockWords(char* word);

int serverPortNumber = 8000;
char messageIn[MAX_MESSAGE_LENGTH];
char messageReply[MAX_MESSAGE_LENGTH+3];
char serverIPAddress[32] = "127.0.0.1";

char hostName[MAX_MESSAGE_LENGTH];
const char* blockedWords[MAX_BLOCKED_WORD_LIST_SIZE] = {"\0"};
int sizeOfBlockedWordList = 0;

int main(int argc, char *argv[])
{
    memset(&blockedWords, 0, sizeof(blockedWords));
    
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

    int clientInfoSize;
    struct sockaddr_in serverSocketInfo, clientSocketInfo, webSocketInfo;
 
    /* Clean message strings (set to zero) and make them thread safe (null-terminated) */
    clearStrings();

    /* Initialize client sockaddr structure */
    memset(&serverSocketInfo, 0, sizeof(serverSocketInfo));
    serverSocketInfo.sin_family = AF_INET;
    serverSocketInfo.sin_port = htons(serverPortNumber);
    serverSocketInfo.sin_addr.s_addr = inet_addr(serverIPAddress);
    
    /* set up the transport-level end point to use TCP */
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
		fprintf(stderr, "Binding Failed: %d !\n", strerror(errno));
        exitCode();
	}
	printf("Binding Successful!\n");

    while (1)
    {
        /* Clean message strings (set to zero) and make them thread safe (null-terminated) */
        clearStrings();

        /* Listen for socket connections */
        listen(serverSocket , 3);

        /* Accept connecting socket */
        printf("\nWaiting for a connection...\n");
        clientInfoSize = sizeof(clientSocketInfo);
        clientSocket = accept(serverSocket, (struct sockaddr*)&clientSocketInfo, (socklen_t*)&clientInfoSize);
        if (clientSocket < 0)
        {
            fprintf(stderr, "Accept Failed!\n");
            exitCode();
        }
        printf("Connection Accepted!\n");

        /* Recieve client message */
        if (recv(clientSocket, messageIn, MAX_MESSAGE_LENGTH, 0) < 0)
        {
            fprintf(stderr, "Recieve Failed!\n");
            clearStrings();
            continue;
        }
        strcat(messageIn, "\0");

        /* Decifer Info About Client Request */

        char command[MAX_MESSAGE_LENGTH];
        memset(&command, '\0', sizeof(command));
        strcpy(command, messageIn);

        printf("Client Message Recieved: %s\n\n", messageIn);

        char method[MAX_MESSAGE_LENGTH];
        memset(&method, '\0', sizeof(method));
        getMethodFromMessage(method, command);

        char httpVer[MAX_MESSAGE_LENGTH];
        memset(&httpVer, '\0', sizeof(httpVer));
        getHTTPVerFromMessage(httpVer, command);

        char url[MAX_MESSAGE_LENGTH];
        memset(&url, '\0', sizeof(url));
        getURLFromMessage(url, command);

        /* Special User Requests */
        
        /* Hard EXIT */
        if (strncmp(messageIn, "EXIT", 4) == 0)
        {
            clearStrings();

            sprintf(messageReply, "\nDisconnecting Server!\n\n");
            printf("%s", messageReply);
            send(clientSocket , messageReply , strlen(messageReply) , 0);

            close(clientSocket);
            clearStrings();
            break;
        }

        /* BLOCK Word */
        if (strncmp(method, "BLOCK", 5) == 0)
        {
            strcat(url, "\0");
            int returnVal = blockWord(url);
            clearStrings();

            if (returnVal) sprintf(messageReply, "\n\"%s\" is now Blocked!\n\n", url);
            else sprintf(messageReply, "\nTOO MANY BLOCKED WORDS!!\n\n(Must Unblock with UNBLOCK)\n\n");

            printf("%s", messageReply);
            send(clientSocket , messageReply , strlen(messageReply) , 0);

            close(clientSocket);
            clearStrings();
            continue;
        }

        /* UNBLOCK Word */
        if (strncmp(method, "UNBLOCK", 7) == 0)
        {
            unBlockWord(url);
            clearStrings();

            sprintf(messageReply, "\n\"%s\" is now Unblocked!\n\n", url);

            printf("%s", messageReply);
            send(clientSocket , messageReply , strlen(messageReply) , 0);

            close(clientSocket);
            clearStrings();
            continue;
        }

        /* PRINT BLOCKED Words */
        if (strncmp(method, "PRINTBL", 7) == 0)
        {
            clearStrings();
            getPrintBlockWords(messageReply);
            printf("%s", messageReply);
            send(clientSocket , messageReply , strlen(messageReply) , 0);
            close(clientSocket);
            clearStrings();
            continue;
        }
        
        /* Ignore Non-GET Requests Other than Above (to avoid crash) */
        if (strncmp(method, "GET", 3) != 0)
        {
            printf("Method Error: %s\n", method);
            puts("Can Only Send GET Request! Ignored Message!\n");
            close(clientSocket);
            close(senderSocket);
            clearStrings();
            continue;
        }
        
        /* Check for BLOCKED Words */
        int index = 0;
        while (index < sizeOfBlockedWordList)
        {
            if (checkMessageForWord(url, blockedWords[index]))
            {
                printf("\nBLOCKED Url -> Redirecting Page...\n");
                strcpy(command, "GET http://pages.cpsc.ucalgary.ca/~carey/CPSC441/ass1/error.html HTTP/1.0");
                
                // Re-read method, url, and http:
                memset(&method, '\0', sizeof(method));
                getMethodFromMessage(method, command);

                memset(&httpVer, '\0', sizeof(httpVer));
                getHTTPVerFromMessage(httpVer, command);

                memset(&url, '\0', sizeof(url));
                getURLFromMessage(url, command);

                break;
            }
            else
            {
                index++;
            }
        }

        /* Prepare User Info Grab */

        /* Get sender IP from host name */
        memset(&hostName, '\0', sizeof(hostName));
        getDomainNameFromURL(hostName, url);

        struct hostent *hostInfo;
        struct in_addr **addr_list;
        char senderIP[32];

        hostInfo = gethostbyname(hostName);

        if ( hostInfo == (struct hostent*) NULL )
        {
            fprintf(stderr, "Host Name \"%s\" \nConversion Error!\n", hostName);
            herror("gethostname");
            clearStrings();
            continue;
        }
        
        addr_list = (struct in_addr **) hostInfo->h_addr_list;
        for(int i = 0; addr_list[i] != NULL; i++) 
        {
            strcpy(senderIP , inet_ntoa(*addr_list[i]));
        }

        printf("Host Name: \"%s\" | IP: %s\n", hostName, senderIP);

        memset(&webSocketInfo, 0, sizeof(webSocketInfo));
        webSocketInfo.sin_family = AF_INET;
        webSocketInfo.sin_port = htons (80);
        webSocketInfo.sin_addr.s_addr = inet_addr(senderIP);

        /* Create socket to send message */
        senderSocket = socket(AF_INET, SOCK_STREAM, 0);
        if(serverSocket == -1)
        {
            fprintf(stderr, "Sender Socket Creation Failed!\n");
            close(senderSocket);
            clearStrings();
            continue;
        }
        puts("Sender Socket Created!\n");
        
        /* Connecting to HTTP */
        puts("Creating Connection...");
        if (connect(senderSocket, (struct sockaddr *) &webSocketInfo, sizeof(webSocketInfo)) < 0)
        {
            fprintf(stderr, "Connection Error\n");
            close(senderSocket);
            clearStrings();
            continue;
        }
        puts("Connected!\n");

        clearStrings();

        sprintf(messageIn, "%s", command);
        strcat(messageIn, "\r\n\r\n");


        if(send(senderSocket , messageIn , strlen(messageIn) , 0) < 0)
        {
            puts("Send Failed\n");
            close(senderSocket);
            clearStrings();
            continue;
        }
        fprintf(stdout, "Sending to HTTP: %s at %s\n", hostName, senderIP);

        clearStrings();

        int count = 0;
        int sizeOfReply = 0;
        /*Recieve reply from server*/
        while((sizeOfReply = recv(senderSocket, messageReply, sizeof(messageReply) , 0)) > 0)
        {
            if (count == 0) printf("Message Recieved %d:\n%s ...\n", count, messageReply);

            /* Forward reply to client */
            int sizeOfSend = 0;
            if((sizeOfSend = send(clientSocket, messageReply, sizeOfReply, 0)) < 0)
            {
                fprintf(stderr, "Reply Failed!\n");
                close(senderSocket);
                clearStrings();
                break;
            }
            count++;

            clearStrings();
        }

        fprintf(stdout, "Full Reply Sent to User!\n");
        clearStrings();

        close(senderSocket);
    }
    
    exitCode();
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

int blockWord(const char* word)
{
    char* newElement = malloc(strlen(word));
    strncpy(newElement, word, strlen(word));

    if (sizeOfBlockedWordList + 1 > MAX_BLOCKED_WORD_LIST_SIZE)
    {
        return 0;
    }

    blockedWords[sizeOfBlockedWordList] = newElement;
    sizeOfBlockedWordList++;
    return 1;
}

void unBlockWord(const char* word)
{
    int index = 0;
    while(index < sizeOfBlockedWordList)
    {
        if (strncmp(blockedWords[index], word, strlen(word)) == 0)
        {
            if (index != sizeOfBlockedWordList-1)
            {
                // Swap Blocked Word to End of List
                const char* temp = blockedWords[index];
                blockedWords[index] = blockedWords[sizeOfBlockedWordList-1];
                blockedWords[sizeOfBlockedWordList-1] = temp;
            }

            char* doomedElement = (char*)blockedWords[sizeOfBlockedWordList-1];
            free(doomedElement);
            blockedWords[sizeOfBlockedWordList-1] = "\0";
            sizeOfBlockedWordList--;
        }
        else index++;
    }
}

void getPrintBlockWords(char* dest)
{
    char result[MAX_MESSAGE_LENGTH];
    memset(&result, '\0', sizeof(result));

    strcat(result, "\nBlocked Words:\n");
    int index = 0;
    while(index < sizeOfBlockedWordList)
    {
        strcat(result, "\n\t");
        strcat(result, blockedWords[index]);
        index++;
    }
    strcat(result, "\n\n");

    strcat(result, "Size of Blocked Word List = ");
    char size[10];
    sprintf(size,"%d / %d", sizeOfBlockedWordList, MAX_BLOCKED_WORD_LIST_SIZE);
    strcat(result, size);
    strcat(result, "\n\n");

    strncpy(dest, result, strlen(result));
}

int getMethodFromMessage(char* dest, const char* message)
{
    int end = 0;
    while(end < strlen(message) && !isspace(message[end]))
    {
        end++;
    }
    end++;

    strncpy(dest, message, end);
    char zero = '\0';
    strcat(dest, &zero);

    return ++end;
}

int getURLFromMessage(char* dest, const char* message)
{
    int start = 0;
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

int getDomainNameFromURL(char* dest, const char* message)
{
    int start = 0;
    char temp[MAX_MESSAGE_LENGTH+1];
    memset(&temp, '\0', sizeof(temp));
    while(message[start] != '/')
    {
        start++;
    }
    start++;
    while(message[start] != '/')
    {
        start++;
    }
    start++;

    int end = start;
    while(message[end] != '/')
    {
        end++;
    }

    strncpy(temp, message + start, end - start);
    strcpy(dest, temp);
    char zero = '\0';
    strcat(dest, &zero);

    return ++end;
}

int getHTTPVerFromMessage(char* dest, const char* message)
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

    int end = start;
    while(end < strlen(message) && !isspace(message[end]))
    {
        end++;
    }

    strncpy(dest, message + start, end - start);
    char zero = '\0';
    strcat(dest, &zero);

    return ++end;
}

int checkMessageForWord(const char* message, const char* word)
{
    int isCollecting = 0;
    int start = 0;
    int end = 0;
    while (start < strlen(message))
    {
        if (isCollecting)
        {
            end = start + strlen(word);
            if (end >= strlen(message) && end >= strlen(word))
            {
                isCollecting = 0;
                start++;
                continue;
            }
            char partialWord[strlen(word)];
            strncpy(partialWord, message + start, end - start);
            if (strncmp(partialWord, word, strlen(word)) == 0)
            {
                return 1;
            }
            else
            {
                start++;
                isCollecting = 0;
                continue;
            }
        }
        else
        {
            if (message[start] == word[0])
            {
                isCollecting = 1;
                continue;
            }
            start++;
        }
    }
    return 0;
}