Coursework for ARU Software Implementation Module

Task as follows:

A MARINA BERTH BOOKING SYSTEM

A new coastal marina has been built. The marina is a man-made single narrow canal in which boats can be moored one-in-front of another, but they cannot pass each other (this means that boats berthed nearer the entrance must be temporarily moved into a separate holding bay to allow a boat that wishes to leave access out of the marina).

The marina manager has contracted you to write a console-based program that will record berth bookings for the marina as each new boat arrives. On execution the program’s main menu should first give the user four options;

	1. record a new booking
	2. delete a record
	3. display all records (and available marina space) and
	4. exit the program.

For new bookings you will have to check that the boat length is short enough (maximum length 15m) and that it is shallow enough (maximum draft 5m) and that there is sufficient room in the marina (the length of the marina is 150m). If the boat is too big or there is insufficient room you should terminate this transaction with an appropriate message and return to the main option menu. If the booking can proceed then the duration of stay in months should be requested and this is used to display the full price of the berth which is based on the length of the boat and onthe duration of stay (10 poundsper meter per month). The user then has the choice to reject and quit the program or accept the offer. If the user is happy with the price the following information should be requested (stored in an object, which should also include appropriate methods);Name of owner (single string)Name of boat (single string)Type of boat (single string –narrow, sailing, or motor)Boat length ( int )The object should then be added to the list of records, and the program returned to the main option menu.