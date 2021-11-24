/*
main.cpp
*/
#include <cctype>
#include <iostream>
#include <string>
using namespace std;

#include "Flight.h"

void readFile(const string fileName, Flight& flight);
/*REQUIRES: fileName is the string file path of a readable file.
* This file must follow the format as instructed in the Term Project Instruction Document.
* Also expects flight to be a properly constructed Flight object.
*
* PROMISES: If there are any issues reading the file of the file path fileName
* due to poor formatting or if the file simply cannot be read, the function
* will terminate the program.
* If the reading is successful, the name of the flight, the rows, the columns,
* all the passengers in the file will be copied into variables located in the
* flight object.
*/
void saveFile(const string fileName, Flight& flight);
/*REQUIRES: Expects flight to have appropriate values for the variables within
* its Passenger object vector and its rows and columns as these values will be
* used to write and save to the file at the path of fileName.
*
* PROMISES: If the file at file path fileName does not exist, a file of that file path
* will be created. Otherwise, this file will be overwritten.
* The function will write into the file the contents of the Flight object flight in a
* format that can be read by the program at a later time.
*/
void println();
//PROMISES: Prints a line of dashes followed by two line returns.
void promptEnter();
/*PROMISES: Prompts the user to press Enter to proceed
* and cleans input stream (using cleanInputStream())
*/
void cleanInputStream();
/*PROMISES: Cleans the input stream until it reaches '\n' character or end of file (EOF).
* It also clears any flags in the cin Standard Input Steam object.
*/
void promptSelect(const string& file, Flight& flight);
/*PROMISES: Prompts the user to input a number corresponding to the selection of
* one of six options: Display Flight Seat Map, Display Passengers Information,
* Add a New Passenger, Remove an Existing Passenger, Save Data, or Quit.
* Once an option is selected, the selected process will be performed.
* If the user inputs an invalid number or character, an error message is displayed
* and the input stream is cleared.
*/
void addPassenger(Flight& flight);
/*REQUIRES: Expects flight to be a properly constructed Flight object.
*
* PROMISES: Prompts the user to create a new Passenger object to add to the Passenger object
* vector of flight. This is done through prompting a series of inputs corresponding to the
* personal information of this passenger. This information includes the firstName, lastName,
* phone, row and col of seat, and id of the Passenger object. If any of this information is input
* incorrectly, the function will display an error message and return. If the information is filled in
* successfully and the new passenger does not have the same ID or seat as a pre-existing passenger,
* the Passenger object will be added to the Passenger obeject vector of flight.
*/
void removePassenger(Flight& flight);
/*PROMISES: Prompts the user for the ID of the passenger to be removed from the
* Flight object flight. If the Flight object flight is successfully able to remove a 
* Passenger object with an id value equal to the value input by the user (ie: remove() == 1),
* a success message is output to the terminal. Otherwise, an error message is displayed.
*/

int Exit = 0;

int main() {
	
	Flight flight;
	const string file("C:\\cygwin64\\home\\Eman2\\ENSF 337\\final\\flight_info.txt");
	
	//Start Statement:
	cout << '\n' << "Version: 1.0" << '\n'
		 << "Term Project - Flight Management Program in C++" << '\n'
		 << "Produced by: Emmanuel Omari-Osei" << '\n';
	
	//Read File:
	readFile(file,flight);
	
	//Repeating Selection:
	while (Exit == 0) {
		promptEnter();
		println();
		promptSelect(file,flight);
	}
	
	exit(1);
	
	return 0;
}

void println() {
	for (int i = 0; i < 20; i++) cout << "---";
	cout << "\n\n";
}

void promptSelect(const string& file, Flight& flight) {
	int s = 0;
	cout << "Please select one of the following options:\n"
		 << "1. Display Flight Seat Map\n"
		 << "2. Display Passengers Information\n"
		 << "3. Add a New Passenger\n"
		 << "4. Remove an Existing Passenger\n"
		 << "5. Save Data\n"
		 << "6. Quit\n\n"
		 << "Enter your choice: (1, 2, 3, 4, 5, or 6) ";
	cin >> s;
	cout << "\n";
	cleanInputStream();
	switch (s) {
		case 1:
			flight.printSeatMap();
		break;
		case 2:
			flight.printPassengers();
		break;
		case 3:
			addPassenger(flight);
		break;
		case 4:
			removePassenger(flight);
		break;
		case 5:
			saveFile(file,flight);
		break;
		case 6:
			cout << "Ending program...\n";
			Exit = 1;
		break;
		default:
			cout << "Invalid Input! Returning to Selection Menu...\n";
			cleanInputStream();
		break;
	}
}

void promptEnter() {
	cout << '\n' << "<<< Press Return to Continue >>>" << '\n';
	cleanInputStream();
}

void cleanInputStream() {
	char c = cin.get();
	while (c != '\n' && c != EOF) 
		c = cin.get();
	cin.clear();
}

void removePassenger(Flight& flight) {
	
	int ID;
	cout << "Please enter the ID of the passenger you would like to remove: ";
	cin >> ID;
	cleanInputStream();
	cout << "\n\n";
	
	cout << "Attempting to remove passenger... : ID: " << ID << "\n\n";
	if (flight.remove(ID) == 1) cout << "Passenger successfully removed!\n";
	else cout << "Could not remove passenger.\nIt is possible the passenger with this ID does not exist.\n"
			  << "Returning to Selection Menu...\n";
}

void addPassenger(Flight& flight) {
	int done = 0;
	Passenger newby;
	
	while (!done) {
		//First Name Input:
		char fname[22];
		int sf=0;
		cout << "Please enter passenger first name: ";
		cin.get(fname,21);
		cleanInputStream();
		while (fname[sf]) sf++;
		if (sf > 19) {
			cout << "The name typed was too long, so it was shortened to \n" << fname << " .\n";
		}
		if (sf == 0) {
			newby.setFirstName("<no name>");
			cleanInputStream();
			cout << "No name was input.\n";
		}
		else newby.setFirstName(fname);
		cout << "\n";
		
		//Last Name Input:
		char lname[22];
		int sl=0;
		cout << "Please enter passenger last name: ";
		cin.get(lname,21);
		cleanInputStream();
		while (lname[sl]) sl++;
		if (sl > 19) {
			cout << "The name typed was too long, so it was shortened to \n" << lname << " .";
		}
		if (sl == 0) {
			newby.setFirstName("<no name>");
			cleanInputStream();
			cout << "No name was input.\n";
		}
		else newby.setLastName(lname);
		cout << "\n";
	
		//Phone Number Input:
		char inp[15];
		string phone = "";
		cout << "Please enter passenger phone number: ";
		cin.get(inp,14);
		cleanInputStream();
		int dcount = 0;
		
		for (int i = 0; inp[i] && (int)phone.size() < 12; i++) {
			if (isdigit(inp[i])) {
				phone += inp[i];
				dcount++;
			}
			else if (inp[i] == ' ' || inp[i] == '-');
			else {
				cout << "\nInvalid phone number input!!\nReturning to Selection Menu...\n";
				done = 1;
				break;
			}
		
			if (dcount > 2 && (int)phone.size() < 10) {
				phone+='-';
				dcount = 0;
			}
		}
		if (done) continue;
		
		int j = 0;
		while (inp[j]) j++;
		
		if (j > 12) {
			cout << "\nPhone number longer than proper length!\n Phone number taken: " << phone << "\n";
		}
		if ((int)phone.size() != 12) {
			cout << "\nInvalid phone number input!!\nReturning to Selection Menu...\n";
			done = 1;
			continue;
		}
		newby.setPhone(phone);
		cout << "\n";
	
	
		//ID Input:
		int ID = 0;
		cout << "Please enter passenger ID: ";
		cin >> ID;
		cleanInputStream();
		if (ID <= 0) {
			cout << "\nInvalid ID number!!\nReturning to Selection Menu...\n";
			done = 1;
			continue;
		}
		newby.setId(ID);
		cout << "\n";
	
		//Row Input:
		int row = 0;
		cout << "Please enter passenger row number: ";
		cin >> row;
		cleanInputStream();
		if (row <= 0) {
			cout << "\nInvalid row number!!\nReturning to Selection Menu...\n";
			done = 1;
			continue;
		}
		newby.setRow(row);
		cout << "\n";
	
		//Column Input:
		char col = 0;
		cout << "Please enter passenger seat letter: ";
		cin >> col;
		cleanInputStream();
		if (!isalpha(col)) {
			cout << "\nInvalid seat letter!!\nReturning to Selection Menu...\n";
			done = 1;
			continue;
		}
		newby.setCol(toupper(col));
		cout << "\n";
	
		if (flight.add(newby) == 1) {
			cout << "Passenger, " << fname << " " << lname << ", successfully added!\n";
			done = 1;
		}
		else {
			cout << "\nCould not add passenger, " << fname << " " << lname << "!!\n"
				<< "It could be that a passenger with the same ID already exists,\nor "
				<< "that the desired seat for this passenger is unavailable.\n"
				<< "\nReturning to Selection Menu...\n";
			done = 1;
		}
	}
}