# Yu-Gi-Oh! Card Explorer

Interface for searching through YGOPRODeck's database of Yu-Gi-Oh! cards, using their API
</br>
Developed for android, in **Unity 2021.1.3f1**

All code can be found in Assets/Scripts
</br>

### Challenges
* Learning how to work with an API
* Adapting to the latest API version due to earlier versions being completely deprecated
* Having to work around the inconsistencies of the API
* Making the UI good for as many Aspect Ratios as possible
* Refactoring
* Changing UI scaling to layout groups

### Future plans
* Add error messages
* Update Random Card scene
* Make data persist through scene changes
* Check with YGOPRODeck how they handle image changes, i.e. card erratas, and how I should handle updating images
* Completely remove Random Archetype scene
* Possibly turn into a companion app, if the want/need arises

## INSTALLATION INSTRUCTIONS
1. Download the .apk file from **Releases**, to an android device
2. Make sure "install from unknown sources" is allowed on said android device
3. Open the .apk to install the app

## USAGE INSTRUCTIONS
**Card Information:** Gives information of a card, such as its card type, which archetype it belongs to, and what effect it has.
A list of all the sets the card has been printed in is also provided, as well as all artworks of the card.
A card can be found by providing either the exact card name, *case-insensitive*, or its ID. Further down, I list a handful of card names and IDs that can be used for testing purposes

<p align="middle">
  <img src="/Screenshots/Blue%20Eyes%20White%20Dragon%20-%20Info.jpg" width="33%" height="33%" />

  <img src="/Screenshots/Ghostrick%20-%20Search.jpg" width="33%" height="33%" />
</p>

**Card Search:** Lists all cards in the database, based on different search filters. You can i.e. find all *Dark Magician* cards by choosing the Archetype parameter and choosing Dark Magician. The card image of each result can now be resized by clicking on it.
Some other examples of filter combinations: 
* All monsters with Light attribute - *Attribute: Light*
* All cards from the set *Savage Strike* - *Card Set: Savage Strike*
* All cards currently on the TCG banlist in some form - *Banlist: TCG*
* All field spells - *Card Type: Spell Card* & *M/S/T Type: Field*
* All Fire attribute Effect monsters with 200 ATK - *Card Type: Effect Monster*, *ATK = 200* & *Attribute: Fire*

**Side Menu** Menu for updating data, slides in from the left side.
Currently the data that can be updated includes cards, search data, archetypes, & card sets.
In order for the **Card Search** scene to function properly, both archetypes and card sets need to be updated, since new card sets and archetypes are introduced regularly.
The need for this is due to limiting the amount of API calls needed.
If this is done from within the **Card Search** scene, a scene reload is needed for the updates to take effect.


**Random Card:** *Currently Disabled* ~~Gives information of a random card. The data returned from the API is **not** identical to what is returned in Card Information~~

**Random Archetype:** *Currently Disabled, will be scrapped* ~~It's supposed to return 3 archetypes at random. Might end up being scrapped, since the reason it was added was because of an idea between my friends and I.~~

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
 
### All data fetched is from YGOPRODeck's API: https://db.ygoprodeck.com/api-guide/

### No license provided
