/*
Flight.h
*/

#include <vector>
using namespace std;

#include "Flight.h"

Flight::Flight() {
	name = "";
	passengers = vector<Passenger>(0);
	rows = 0;
	cols = 0;
}

Flight::Flight(const string& n, int r, int c) {
	name = n;
	passengers = vector<Passenger>(0);
	rows = r;
	if (c < 26) cols = c;
	else cols = 26;
}
		
Passenger& Flight::getPassenger(int i) {
	if (i < (int)passengers.size() - 1) {
		return passengers.at(i);
	}
	else return passengers[passengers.size() - 1];
}

int Flight::add(Passenger& p) {
	if (p.getRow() < 0 || p.getRow() > rows) return 0;
	if (p.getRow() < 0 || p.getCol()-65 >= cols) return 0;
	if (contains(p.getRow(), p.getCol()) > -1) return 0;
	if (contains(p.getId()) > -1) return 0;
	passengers.push_back(p);
	return 1;
}

int Flight::contains(int pId) const {
	
	for (int i = 0; i < (int)passengers.size(); i++) {
		if (passengers[i].getId() == pId) {
			return i;
		}
	}
	return -1;
}

int Flight::contains(int r, char c) const {
	
	for (int i = 0; i < (int)passengers.size(); i++) {
		if (passengers[i].getRow() == r && passengers[i].getCol() == c) {
			return i;
		}
	}
	return -1;
}

int Flight::remove(int i) {
	if (contains(i) > -1) {
		passengers.erase(passengers.begin() + contains(i));
		return 1;
	}
	else {
		return 0;
	}
}

void Flight::printSeatMap() const {
	cout << left << "Seat Map for Flight: " << name << "\n\n";
	cout << setw(5) << "";
	cout << "  " << (char)(65);
	for (int c = 1; c < cols; c++) cout << " - " << (char)(c+65);
	cout << "\n";
	cout << setw(5) << "";
	for (int c = 0; c < cols; c++) cout << "+---";
	cout << "+\n";
	
	for (int r = 1; r <= rows; r++) {
		cout << right << setw(5) << r << left <<  " ";
		
		for (int c = 0; c < cols; c++) {
			if (contains(r,c+65) > -1) cout << " X ";
			else cout << "   ";
			cout << "|";
		}
		
		cout << "\n" << setw(5) << "";
		for (int c = 0; c < cols; c++) cout << "+---";
		cout << "+\n";
	}
}
	
void Flight::printPassengers() const {
	cout << "Passengers of Flight: " << name << "\n\n";
	cout << left << setw(21) << " First Name" << setw(20) << "Last Name" << setw(20)
		 << "Phone" << setw(5) << "Row" << setw(6) << "Seat" << setw(7) << "ID";
	cout << "\n ";
	for (int i = 0; i < 26; i++) cout << "---";
	cout << "\n";
	for (int i = 0; i < (int)passengers.size(); i++) {
		cout << " ";
		passengers[i].print();
		cout << " ";
		for (int i = 0; i < 26; i++) cout << "---";
		cout << "\n";
	}
	cout << "\n";
}