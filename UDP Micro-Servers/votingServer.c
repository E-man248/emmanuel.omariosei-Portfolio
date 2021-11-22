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
#define MAX_CANDIDATES 100
#define DEBUG 0

int serverSocket;

void exitCode();
void clearStrings();
void addCandidate(const char* candidate, int id);
int getVoteFromMessage(const char* message);

int serverPortNumber = 8003;

char messageIn[MAX_MESSAGE_LENGTH];
char messageReply[MAX_MESSAGE_LENGTH+3];
char serverIPAddress[32] = "136.159.5.25";

int numberOfCandidates = 4;
char* candidates[MAX_CANDIDATES];
int candidateIDs[MAX_CANDIDATES];
int candidateVotes[MAX_CANDIDATES];

int hasVoted = 0;
char prevClientIP[32] = "\0";
int prevClientPortNumber = 0;

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
    
    candidates[0] = "George";
    candidates[1] = "William";
    candidates[2] = "Jeff";
    candidates[3] = "Amy";

    candidateIDs[0] = 1;
    candidateIDs[1] = 2;
    candidateIDs[2] = 3;
    candidateIDs[3] = 4;

    candidateVotes[0] = 5;
    candidateVotes[1] = 5;
    candidateVotes[2] = 5;
    candidateVotes[3] = 5;

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

        // Change Message to Upper Case:
        int index = 0;
        while(index < strlen(messageIn) && !isspace(messageIn[index]))
        {
            messageIn[index] = toupper(messageIn[index]);
            index++;
        }
        
        int currentClientPortNumber = ntohs(clientSocketInfo.sin_port);
        char currentClientIP[32]  = inet_ntoa(clientSocketInfo.sin_addr);

        if (strncmp(messageIn, "START", 5) == 0 || strncmp(messageIn, "MENU", 4) == 0) // Menu
        {
            strcpy(messageReply, "Voting Server:\n\n");
            strcat(messageReply, "   Please Input the Number for the Associated Service:\n");
            strcat(messageReply, "\tMENU  -  See all available commands\n");
            strcat(messageReply, "\tLISTING  -  Show all the Candidates in the running\n");
            strcat(messageReply, "\tVOTE <Candidate ID>  -  Request to secure your vote\n");
            strcat(messageReply, "\tRESULTS  -  Show the full voting summary\n");
            strcat(messageReply, "\tKEY <Voting Encryption>  -  Place your vote with a vote encrypted code\n");
            strcat(messageReply, "\tEND  -  End Voting Server Session\n");
            strcat(messageReply, "\n\n");
        }
        else if (strncmp(messageIn, "LISTING", 7) == 0) // Listing
        {
            strcpy(messageReply, "   Candidate Listing:\n");
            for (int i = 0; i < numberOfCandidates; i++)
            {
                char listing[100];
                sprintf(listing, "\t%s\t\t| ID: %d\n", candidates[i], candidateIDs[i]);
                strcat(messageReply, listing);
            }
            strcat(messageReply, "\n");
        }
        else if (strncmp(messageIn, "VOTE", 4) == 0) // Voting
        {
            if (hasVoted && currentClientPortNumber == prevClientPortNumber && strcmp(currentClientIP, prevClientIP) == 0)
            {
                strcpy(messageReply, "   You have already placed your vote...\n\n");
            }
            else
            {
                int vote = getVoteFromMessage(messageIn);
                if (candidates[vote-1] != 0 && vote > 0 && vote <= MAX_CANDIDATES)
                {
                    char reply[1000];
                    int encryptionKey = random()%10;

                    printf("   Voting for %s! | ID: %d\n\n", candidates[vote-1], vote);
                    sprintf(reply, "   Voting for %s! | ID: %d\n\n", candidates[vote-1], vote);
                    strcpy(messageReply, reply);

                    char keySend[100];
                    printf("   Please Enter the Command: \n   KEY %d\n   In order to place your vote!\nKEY<%d>", vote*encryptionKey, encryptionKey);
                    sprintf(keySend, "   Please Enter the Command: \n   KEY %d\n   In order to place your vote!\nKEY<%d>", vote*encryptionKey, encryptionKey);
                    strcat(messageReply, keySend);

                    printf("\nSending Voting Encryption Key: %d\n", encryptionKey);
                }
                else
                {
                    char reply[1000];
                    printf("   Invalid Candidate ID... | ID: %d", vote);
                    sprintf(reply, "   Invalid Candidate ID... | ID: %d", vote);
                    strcpy(messageReply, reply);
                    strcat(messageReply, "\n\n");
                }
            }
        }
        else if (strncmp(messageIn, "RESULTS", 7) == 0) // Results
        {
            if (hasVoted && currentClientPortNumber == prevClientPortNumber && strcmp(currentClientIP, prevClientIP) == 0)
            {
                strcpy(messageReply, "   Showing Voting Results:\n");
                for (int i = 0; i < numberOfCandidates; i++)
                {
                    char listing[100];
                    sprintf(listing, "\t%s\t\t| Votes: %d\n", candidates[i], candidateVotes[i]);
                    strcat(messageReply, listing);
                }
                strcat(messageReply, "\n\n");
            }
            else
            {
                hasVoted = 0;
                strcpy(messageReply, "   You must VOTE first before seeing the results...\n\n");
            }  
        }
        else if (strncmp(messageIn, "KEY", 3) == 0) // Key
        {
            if (hasVoted && currentClientPortNumber == prevClientPortNumber && strcmp(currentClientIP, prevClientIP) == 0)
            {
                strcpy(messageReply, "   You have already placed your vote...\n\n");
            }
            else
            {
                printf("Processing Encrypted Vote:\n\n");

                int vote = getVoteFromMessage(messageIn) / (messageIn[strlen(messageIn)-1] - '0');
                candidateVotes[vote-1]++;

                printf("Encrypted Vote: %d\n", getVoteFromMessage(messageIn));
                printf("Encryption Key: %d\n", messageIn[strlen(messageIn)-1] - '0');
                printf("Real Vote: %d\n", vote);
                printf("Voted Candidate: %s\n", candidates[vote-1]);

                prevClientPortNumber = ntohs(clientSocketInfo.sin_port);
                prevClientIP = inet_ntoa(clientSocketInfo.sin_addr);
                hasVoted = 1;
                strcpy(messageReply, "   Vote Submitted!");
                strcat(messageReply, "\n\n");
            }
        }
        else if (strncmp(messageIn, "END", 3) == 0) // End
        {
            prevClientPortNumber = ntohs(clientSocketInfo.sin_port);
            prevClientIP = inet_ntoa(clientSocketInfo.sin_addr);
            strcpy(messageReply, "END");
        }
        else // Invalid Input
        {
            strcpy(messageReply, "   Invalid Voting Input!\n\n   Options:\n");
            strcat(messageReply, "\tMENU  -  See all available commands\n");
            strcat(messageReply, "\tLISTING  -  Show all the Candidates in the running\n");
            strcat(messageReply, "\tVOTE  -  Secure your vote\n");
            strcat(messageReply, "\tRESULTS  -  Show the full voting summary\n");
            strcat(messageReply, "\tEND  -  End Voting Server Session\n");
            strcat(messageReply, "\n\n");
        }

        fprintf(stdout, "\nSending Message to Client | IP: %s, Port: %d\n", inet_ntoa(clientSocketInfo.sin_addr), ntohs(clientSocketInfo.sin_port));
        sendto(serverSocket, messageReply, strlen(messageReply) , 0, client, clientSocketInfoSize);
        printf("\nMessage Sent to Client!\n\n");
        
        clearStrings();          
    }
    
    exitCode();
}

int getVoteFromMessage(const char* message)
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

    char dest[MAX_MESSAGE_LENGTH];
    strncpy(dest, message + start, end - start);
    char zero = '\0';
    strcat(dest, &zero);

    return atoi(dest);
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