[System.Serializable]
//Class to parse Json data to
public class CardInfoParse
{
    public int id;
    public string name;
    public string type; //Card Type
    public string desc;
    public int atk;
    public int def;
    public int level;   //Level & Rank
    public string race; //Monster/Spell/Trap Type
    public string attribute;
    public string archetype;    //The archetype a card belongs to
    public int linkval; //Link Rating
    public string[] linkmarkers;    //Link Arrows
    public CardSetTemp[] card_sets;
    public CardImageTemp[] card_images;
    public CardPriceTemp[] card_prices;
    public int scale;   //Pendulum Scale
    public string archetype_name;   //The name of an archetype
}