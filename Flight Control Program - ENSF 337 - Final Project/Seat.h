/*
Seat.h
*/

class Seat {
	
	private:
		int row;
		char col;
	public:
		Seat();
		/*PROMISES:
		* Default Seat construct:
		* Row and col set to 0 and 'A' respectively
		*/
		Seat(int r, char c);
		/*REQUIRES:
		* Expects c <= 'Z'
		*
		* PROMISES:
		* Seat construct:
		* Row and col set to r and c respectively
		* if c > 26, col set to 26
		*/
	
		int getRow() const {return row;}
		//PROMISES: To return row
		char getCol() const {return col;}
		//PROMISES: To return col
		
		void setRow(int r) { row = r; }
		//PROMISES: To set row to r
		void setCol(char c) {
			if (c < 'Z') col = c;
			else col = 'Z';
		}
		/*REQUIRES:
		* Expects c <= 'Z'
		*
		* PROMISES:
		* To set col to c
		* if c > 'Z', col set to 'Z'
		*/
};