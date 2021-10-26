# Yu-Gi-Oh! Card Searcher

Interface for searching through YGOPRODeck's database of Yu-Gi-Oh! cards, using their API
</br>
Developed primarily for android, in **Unity 2020.2.7f1**

All code can be found in Assets/Resources/Scripts
</br>
*CardInfo*, *SaveManager*, *ScaleHandler*, *DropdownHandler*, and *DropdownOptionsHandler* are important scripts 

### Challenges
* Learning how to work with an API
* Adapting to the latest API version due to earlier versions being completely deprecated
* Having to work around the inconsistencies of the API
* Making the UI good for as many Aspect Ratios as possible

### Future plans
* Update the whole project to use the latest API version, making it fully functional again
* Refactor the code for better code structure
* Rework the instantiated results on *Card Search*, to not have a severe performance spike
* Fix the problem with *Card Search* appending save data to an already saved card, making the file unreadable when trying to load the data
* Including more

## INSTALLATION INSTRUCTIONS
1. Copy the .apk file, located in the root folder of the project, to an android device
2. Make sure "install from unknown sources" is allowed on said android device
3. Open the .apk to install the app 

If an android device is lacking, download Unity **2020.2.7f1** and open the project's root folder in Unity.
When running it inside the Unity editor, make sure the *Index* scene is opened.

## USAGE INSTRUCTIONS
**Card Information:** Gives information of a card, such as its card type, which archetype it belongs to, and what effect it has.
A list of all the sets the card has been printed in is also provided, as well as all artworks of the card.
A card can be found by providing either the exact card name, *case-insensitive*, or the ID. Further down, I list a handful of card names and IDs that can be used for testing purposes
However, if searched through ID, only 1 artwork will show up, due to how the API works.

<p align="middle">
  <img src="/Screenshots/Blue%20Eyes%20White%20Dragon%20-%20Info.jpg" width="33%" height="33%" />

  <img src="/Screenshots/Ghostrick%20-%20Search.jpg" width="33%" height="33%" />
</p>

**Card Search:** Lists all cards in the database, based on different search filters. You can for example find all *Dark Magician* cards by choosing the Archetype parameter and choosing Dark Magician.
Some other examples of filter combinations: 
* All monsters with Light attribute - *Attribute: Light*
* ~~All cards from the set *Savage Strike*~~ - Card Set parameter is currently not working
* All cards currently on the TCG banlist in some form - *Banlist: TCG*
* All field spells - *Card Type: Spell Card* & *Monster/S/T Type: Field*


**Random Card:** Gives information of a random card. The data returned from the API is **not** identical to what is returned in Card Information

**Random Archetype:** Not functional! It's supposed to return 3 archetypes at random. Might end up being scrapped, since the reason it was added was because of an idea between my friends and I.

| Card Name | Card ID | Card Type |
| --------- | :-------: | --------- |
| Dark Magician | 46986414 | Normal Monster |
| Trickstar Candina | 61283655 | Effect Monster |
| Bujinki Amaterasu | 68618157 | XYZ Monster |
| Dark Hole | 53129443 | Normal Spell Card |
| Mystical Space Typhoon | 63144961 | Quick-Play Spell Card |
| Magic Cylinder | 62279055 | Normal Trap Card |
| The Weather Thundery Canvas | 16849715 | Continuous Trap Card |

 </br>
 
### API documentation: https://db.ygoprodeck.com/api-guide/

### No license provided
