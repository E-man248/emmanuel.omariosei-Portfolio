/*
Passenger.h
*/
#include <iomanip>
#include <iostream>
#include <string>
using namespace std;

#include "Seat.h"

class Passenger {
	
	private:
		string firstName;
		string lastName;
		string phone;
		Seat seat;
		int id;
		
	public:
		Passenger();
		/*PROMISES:
		* Default Passenger construct:
		* firstName and lastName set to empty strings.
		* phone set to "000-000-0000".
		* id set to 0.
		* Default constructor for Seat called to define seat.
		*/
		Passenger(const string& first, const string& last, int r = 0, int c = 0);
		/*PROMISES:
		* Passenger construct:
		* firstName and lastName set first and last respectively.
		* phone set to "000-000-0000".
		* Constructor for Seat called to define seat using r and c
		* (r and c defined as 0 if not defined)
		*/
	
		const string& getFirstName() const { return firstName; }
		//PROMISES: To return firstName
		const string& getLastName() const { return lastName; }
		//PROMISES: To return lastName
		const string& getPhone() const { return phone; }
		//PROMISES: To return phone
		int getRow() const { return seat.getRow(); }
		//PROMISES: To return row of seat using getRow()
		char getCol() const { return seat.getCol(); }
		//PROMISES: To return col of seat using getCol()
		int getId() const { return id; }
		//PROMISES: To return id
		
		void setFirstName(const string& name) { firstName = name; }
		//PROMISES: To set firstName to name
		void setLastName(const string& name) { lastName = name; }
		//PROMISES: To set lastName to name
		void setPhone(const string& p) { phone = p; }
		/*REQUIRES: Expects to recieve a string in the format: 000-000-0000
		* PROMISES: To set phone to p
		*/
		void setRow(int r) { seat.setRow(r); }
		//PROMISES: To set row of seat to r
		void setCol(char c) { seat.setCol(c); }
		//PROMISES: To set col of seat to c
		void setId(int i) { id = i; }
		//PROMISES: To set id to i
		
		void print() const;
		/*PROMISES: To print a line to the terminal containing information 
		* pertaining to the passenger. This includes firstName, lastName,
		* phone, row and col of seat, and id.
		*/
		
};