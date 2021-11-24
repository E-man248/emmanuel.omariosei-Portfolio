/*
Flight.h
*/

#include <vector>
using namespace std;

#include "Passenger.h"

class Flight {
	
	private:
		vector<Passenger> passengers;
		string name;
		int contains(int pId) const;
		/* Overloaded with 1 other function
		* REQUIRES:
		* pId to be equivalent to the value of an id of a prexisting
		* Passenger element in the vector passengers.
		*
		* PROMISES:
		* If pId not equal to the id of any of the Passenger objects
		* in the vector passengers, returns -1.
		* If vector passengers contains a Passenger object
		* with the id of pId, returns the index of that object
		* in the vector.
		*/
		int contains(int r, char c) const;
		/* Overloaded with 1 other function
		* REQUIRES:
		* r and c to be equivalent to the row and col value (respectively)
		* of a prexisting Passenger element's Seat object in the vector passengers.
		*
		* PROMISES:
		* If r and c not equal to the row and col of any of the Passenger objects'
		* Seat object in the vector passengers, returns -1.
		* If vector passengers contains a Passenger object
		* with the Seat object with row == r and col == c,
		* returns the index of that object in the vector.
		*/
		int rows;
		int cols;
	
	public:
		Flight();
		/*PROMISES:
		* Default Flight construct:
		* Rows and cols set to 0
		*/
		Flight(const string& n, int r, int c);
		/*REQUIRES:
		* Expects c <= 26
		* PROMISES:
		* Flight construct:
		* Rows and cols set to r and c respectively.
		* if c > 26, cols set to 26
		*/
		Passenger& getPassenger(int i);
		/*REQUIRES:
		* i < passengers.size()-1
		*
		* PROMISES:
		* if i < passengers.size()-1 return element at index passengers.size()-1.
		* returns element at i.
		*/
		
		const string& getName() const { return name; }
		//PROMISES: To return rows
		int getRows() const { return rows; }
		//PROMISES: To return rows
		int getCols() const { return cols; }
		//PROMISES: To return cols
		int size() const { return passengers.size(); }
		
		void setName(const string& n) {	name = n; }
		//PROMISES: To set name to n
		void setRows(int r) { rows = r; }
		//PROMISES: To set rows to r
		void setCols(int c) {
			if (c < 26) cols = c;
			else cols = 26;
		}
		/*REQUIRES:
		* Expects c <= 26
		* PROMISES:
		* To set cols to c
		* if c > 26, cols set to 26
		*/
		
		int add(Passenger& p);
		/*REQUIRES:
		* Passenger p has a Seat object with row > 0 && row <= rows,
		* Passenger p has a Seat object with col >= 0 && col < cols,
		* the vector passengers does not contain a Passenger object
		* with Passenger p's id value (ie: helper function contains() == -1),
		* and the vector passengers does not contain a Passenger object
		* with a Seat object whose row and col match that of Passenger p's
		* Seat object's row and col values (ie: helper function contains() == -1).
		*
		* PROMISES:
		* If any of the above requirements are not valid, the function returns 0,
		* otherwise Passenger p will be added to the vector passengers and the
		* function will return 1.
		*/
		
		int remove(int i);
		/*REQUIRES:
		* Expects i to be the id value of a Passenger object in the vector passengers.
		*
		* PROMISES:
		* If i is not the id value of any Passenger object in the vector passengers
		* (ie: helper function contains() == -1), the function returns 0.
		* Otherwise, the Passenger object in vector passengers with id value i
		* (index found using helper function contains), is erased from the vector.
		*/
		
		void printPassengers() const;
		/*PROMISES:
		* To print to the terminal the personal information of each Passenger object in the vector passengers.
		* This personal information includes the firstName, lastName, phone, row and col of seat, and id
		* of each Passenger object.
		*/
		
		void printSeatMap() const;
		/*PROMISES:
		* To print to the terminal a Seat Map displaying all the rows and columns of
		* the Flight and an 'X' at a row-column spot that correlates to the row and col
		* of a Seat object contained by any Passenger object in the vector passengers,
		* representing their spot on the plane in reference to other passengers.
		*/
};