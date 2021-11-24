/*
read_file.cpp
*/
#include <fstream>
#include <iostream>
#include <string>
#include <cctype>
using namespace std;

#include "Flight.h"

void cleanInputStream();
/*PROMISES: Cleans the input stream until it reaches '\n' character or end of file (EOF).
* It also clears any flags in the cin Standard Input Steam object.
* (Also used in main.cpp)
*/
void errorRead(ifstream& r);
//PROMISES: Clears the flag of ifstream r and outputs an error message on the terminal.
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
* (Also used in main.cpp)
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
* (Also used in main.cpp)
*/

void errorRead(ifstream& r) {
	cout << "\nError: In-File could not be read!!\nTerminating Program...\n";
	r.clear();
	exit(1);
}

void readFile(const string fileName, Flight& flight) {
	ifstream read;
	
	read.open(fileName);
	if (read.fail()) {
		errorRead(read);
	}
	
	//Read First Line: (Flight Name, Rows, and Columns)
	char flightName[8];
	read.get(flightName,7);
	if (read.fail()) {
		cout << "\n Here! \n";
		errorRead(read);
	}
	flight.setName(flightName);
	
	int rows = 0;
	read >> rows;
	if (rows <= 0) errorRead(read);
	flight.setRows(rows);
	
	int cols = 0;
	read >> cols;
	if (cols < 0) errorRead(read);
	flight.setCols(cols);
	char junk[100];
	read.getline(junk, 100);
	
	//Read Passenger Rows: (First Name, Last Name, Phone, ID, Row, and Column)

	while (!read.eof()) {
	
		Passenger nebby;
		
		//First Name Input:
		char fname[22];
		read.get(fname,21);
		if (read.fail()) {
			read.clear();
			break;
		}
		nebby.setFirstName(fname);
	
		//Last Name Input:
		char lname[22];
		read.get(lname,21);
		if (read.fail()) {
			read.clear();
			break;
		}
		nebby.setLastName(lname);
	
		//Phone Number Input:
		char phone[16];
		read.get(phone,15);
		if (read.fail()) {
			read.clear();
			break;
		}
		nebby.setPhone(phone);
		
		//Row Input:
		int row = 0;
		read >> row;
		if (read.fail()) {
			read.clear();
			break;
		}
		nebby.setRow(row);
	
		//Column Input:
		char col = 0;
		read >> col;
		if (read.fail()) {
			read.clear();
			break;
		}
		nebby.setCol(col);
		
		//ID Input:
		int ID = 0;
		read >> ID;
		nebby.setId(ID);
		
		//Add to Flight:
		flight.add(nebby);
		
		char junk[100];
		read.getline(junk, 100);
		read.clear();
	
	}
	
	read.close();
}

void saveFile(const string fileName, Flight& flight) {
	char choice = 0;
		
	cout << "Are you sure you want to save your progress?\n(Prexisting save data will be overwritten!)\nYes or No? (Y / N) : ";
	cin >> choice;
	cleanInputStream();
	choice = toupper(choice);
	if (choice != 'Y' && choice != 'N') {
		cout << "\nInvalid Input!!\nReturning to Selection Menu...\n";
		return;
	}
	cout << "\n";
	
	if (choice == 'N')
		cout << "Save Cancelled!\nReturning to Selection Menu...\n";
	else {
		ofstream out;
		out.open(fileName);
		cout << "Saving to File: " << fileName << " ...\n";
		
		out << left << setw(9) << flight.getName() << setw(6) << flight.getRows() << setw(6) << flight.getCols() << "\n";
		
		for (int i = 0; i < flight.size(); i++) {
			string seat = "";
			seat+= to_string(flight.getPassenger(i).getRow());
			seat+= flight.getPassenger(i).getCol();
			
			out  << left << setw(20) << flight.getPassenger(i).getFirstName() << setw(20) << flight.getPassenger(i).getLastName()
				 << setw(20) << flight.getPassenger(i).getPhone()
				 << setw(4) << seat << setw(0) << flight.getPassenger(i).getId() << "\n";
		}
		cout << "Data Saved!\n";
	}
}