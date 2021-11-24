/*
Passenger.cpp
*/
#include <iostream>
#include <string>
using namespace std;

#include "Passenger.h"

Passenger::Passenger() {
	firstName = "";
	lastName = "";
	phone = "000-000-0000";
	seat = Seat();
	id = 0;
}

Passenger::Passenger(const string& first, const string& last, int r, int c) {
	firstName = first;
	lastName = last;
	phone = "000-000-0000";
	seat = Seat();
	id = 0;
}

void Passenger::print() const {
	cout << left << setw(20) << firstName << setw(20) << lastName
		 << setw(20) << phone << setw(5) << seat.getRow()
		 << setw(6) << seat.getCol() << setw(5) << id << '\n';
}