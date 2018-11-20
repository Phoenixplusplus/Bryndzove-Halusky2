#pragma once
#include "Color.h"

#ifdef BHCHATDLL_EXPORT
#define BHCHATDLL_API __declspec(dllexport)
#else
#define BHCHATDLL_API __declspec(dllexport)
#endif

extern "C"
{
	BHCHATDLL_API int GetJokesCount();
	BHCHATDLL_API const char * GetJokeByID(int jokeID);
	BHCHATDLL_API const char * GetRandomJoke();
	BHCHATDLL_API const char * GetErrorMessage();
	BHCHATDLL_API bool ErrorCheck();
	BHCHATDLL_API Color GetTextColor(int messageType);
}