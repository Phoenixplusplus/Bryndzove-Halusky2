#include "ChatMain.h"
#include <string>

enum MessageType
{
	PLAYER_INPUT,
	PLAYER_JOINED,
	PLAYER_LEFT,
	PLAYER_KICKED,
	MESSAGE_TYPE_ID_OUT_OF_RANGE
};

static std::string errorMessage = "BHChat.DLL does not containt any errors";
static bool _errorCheck = false;

#define NUMBER_OF_JOKES 10

// All jokes included bellow comes from https://short-funny.com/funniest-jokes-7.php 
static const char * jokesArray[NUMBER_OF_JOKES] = {
/* Joke 0 */ "Send nudes!",
/* Joke 1 */ "Anton, do you think I am a bad mother? My name is Paul.",
/* Joke 2 */ "Mom, where do tampons go? Where the babies come from, darling. In the stork?",
/* Joke 3 */ "My dog used to chase people on a bike a lot. It got so bad, finally I had to take his bike away.",
/* Joke 4 */ "A wife is like a hand grenade. Take off the ring and say good bye to your house.",
/* Joke 5 */ "I was making Russian tea. Unfortunately I cannot fish the teabag out of the vodka bottle.",
/* Joke 6 */ "What goes up and down but never moves? The stairs!",
/* Joke 7 */ "I can not believe I forgot to go to the gym today. That is 7 years in a row now.",
/* Joke 8 */ "Men 1845: I just killed a buffalo.   Men 1952: I just fixed the roof.   Men 2017: I just shaved my legs.",
/* Joke 9 */ "Daddy what is a transvestite? Ask Mommy, he knows."};

static Color colorJoined(0, 255, 0);
static Color colorLeft(255, 0, 0);
static Color colorKicked(255, 128, 0);
static Color colorText(255, 255, 255);

extern "C"
{
	// Return Error message
	const char * GetErrorMessage()				{ return errorMessage.c_str(); }
	// Return Error status
	bool ErrorCheck()							{ return _errorCheck; }

	// Return joke by ID, if the ID is out of range, return random joke
	const char * GetJokeByID(int ID)
	{
		if (ID >= NUMBER_OF_JOKES)
		{	// Assign error message			
			errorMessage = "ERROR::Array out of range, joke ID is higher than array range. Observed ID is " + std::to_string(ID) + 
			", maximal allowed ID is " + std::to_string(NUMBER_OF_JOKES) + ". Returning randomly generated joke instead.";						
			// Set error check to true
			_errorCheck = true;
			// Return random joke, in purpose to do not crash unity chat.
			return GetRandomJoke();
		}
		// Return requested joke
		else return jokesArray[ID];
	}
	// Return the jokes Lenght
	int GetJokesCount()								{ return NUMBER_OF_JOKES; }
	// Return random joke from joke array
	const char * GetRandomJoke()					{ return jokesArray[rand() % NUMBER_OF_JOKES]; }
	// Return the color of text depends on message type
	Color GetTextColor(int messageType)
	{
		switch (messageType)
		{
			case 0: return colorText;		// Default message type, player has typed message and pressed enter
			case 1: return colorJoined;		// Player has joined room message type, auto generated when player join room
			case 2: return colorLeft;		// Player has left room message type, auto generated when player left room
			case 3: return colorKicked;		// Player has been kicked message type, auto generated when master client kick player
			default:						// ERROR - Use default text color, code should not get here.
			{	// Assign error message				
				errorMessage = "ERROR::Given invalid messageType number. Observed number is " + std::to_string(messageType) +
				", maximal allowed number is " + std::to_string(MessageType::MESSAGE_TYPE_ID_OUT_OF_RANGE - 1) + ". Default color has been returned.";								
				// Set error check to true
				_errorCheck = true;
				// Return defaul color
				return colorText;
			}
		}
	}
}