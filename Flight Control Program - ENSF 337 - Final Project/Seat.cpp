/*
Seat.cpp
*/

#include "Seat.h"

Seat::Seat() {
	row = 0;
	col = 'A';
}

Seat::Seat(int r, char c) {
	row = r;
	if (c < 'Z') col = c;
	else col = 'Z';
}
