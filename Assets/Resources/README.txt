YGO Card Searcher v. 0.7

Developed in Unity 2019.2.12f1

API Documentation: https://db.ygoprodeck.com/api-guide-v5/


This program/app is a card searcher for the TCG Yu-Gi-Oh! (or YGO, for short).
The purpose of this app is to make it easier for my fellow YGO duelists to search for cards. 

It is primarily designed to be used on smartphones, but it is possible to use it on a computer as well, without any major problems. 


Currently on android, to my knowledge, you have to open up your browser, possibly open a new tab, 
search for the card you want information about, and either try to find a good enough picture on Google Images or find a link which has info about the card.

With this app, all you need to do is start it up, and from the get go, you can search for a card.



Explanation for the different menus:

	CARD INFORMATION:
	Returns the information of a specific card.
	The exact card name is needed, NOT case sensitive. (e.g. blue-eyes white dragon)
	It is also possible to search with a card's ID (found in the lower left corner of a card), which can be used to identify foreign cards, if the artwork is not recognizable.
	The API has more than 1 ID for a card if it has alternate artworks. (e.g. 89631139 returns the original artwork of Blue-Eyes White Dragon, while 89631140 returns an alternate artwork of it)


	CARD SEARCH:
	Returns the names of cards that fit the specified parameters (e.g. name, lvl, card type, format)
	A copy button is generated for each card, which copies the exact card name to the clipboard.


	RANDOM CARD: 
	Returns the information of a random card.


	RANDOM ARCHETYPE:
	Returns 3 random archetypes and series.
	This can be used if you want to find a new deck to play, however, the API does not contain every archetype in the game.
	An archetype is a group of cards, with a collective name (almost always as a prefix), that works in tandem with each other. (e.g. Trickstar, Volcanics, Cubic)


To use this on Windows, launch the -.exe, located in ---

To use this on Android, install the -.apk on your android device, located in ---, and launch the app. Permission is needed for it to store the data from the API locally.


The things I want to implement into this app are located in the to-do list

