using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.CompilerServices;
using System;
using Unity.Collections;
using UnityEngine.Networking;
//using UnityEditor.PackageManager.Requests;
//using UnityEngine.UIElements;

//Handles card information and API requests
public class CardInfo : MonoBehaviour
{   
    public const string URL = "db.ygoprodeck.com/api/", databaseVersion = "v7/", cInfo = "cardinfo.php?";
    public string _URLMod, cardID, jsonSaveData, fileName, nameMod;
    //Currently using UnityEngine.WWW cause it's easy to understand how to use
    //Change to UnityWebRequest in the near future
    public UnityWebRequest webRequest;
    GameObject prefab;
    float prefabHeight = 0;
    public InputField idInputField;
    public Text idInput, errorText, id, cardName, cardType, monsterType, atk, def, level, attribute, pendulumScale, archetype, desc, printAmount;
    public RawImage image;
    public RectTransform cardInfoTransform;
    public DropdownHandler dropDownMenu;
    public ScaleHandler scaleHandler;
    public SaveManager saveManager;
    public List<CardInfoParse> archetypeList = new List<CardInfoParse>();
    public CardInfoParse[] parseList, toJson;
    public ArchetypeParse[] parseArchList;
    public CardSetParse[] parseSetList;
    public GameObject cardSetContainer, artworkButtons;

    public int imageIndex, cardIDParsingResult;

    public Button nextImage, previousImage, showCardSets, cardSetCloseBtn;
    private void Start()
    {
        //Debug.Log(Application.persistentDataPath);
        //ClearCardInfo(id, cardName, cardType, monsterType, atk, def, level, attribute, pendulumScale, archetype, desc);
        
    }

    void Update()
    {
        //disable errorText when it's empty, so it doesn't block any UI interaction
        if (errorText != null) {
            if (errorText.text == "" && errorText.enabled)
            {
                errorText.enabled = false;
            }
            else if (errorText.text != "" && !errorText.enabled)
            {
                errorText.enabled = true;
            }
        }
    }

    //Called by button
    public void RequestCheck()
    {
        SetID();

        if (cardID != "")
        {
            if (cardID != null)
            {
                //Card Information
                if (EUS.sceneIndex == 1)
                {
                    //Local File
                    PrepToFileName(cardID);
                    
                    if (System.IO.File.Exists(Application.persistentDataPath + "/" + fileName.ToLower() + ".txt"))
                    {
                        Debug.Log(fileName);
                        saveManager.ReadFile(fileName);
                        ClearCardInfo(true, errorText);
                        StartCoroutine(LoadCardInfo(null,saveManager.fileData));
                    }
                  
                    //API Request
                    else
                    {
                        //request = new WWW(URL + cInfo + nameMod + cardID);          //Card Information
                        if (int.TryParse(cardID, out cardIDParsingResult))
                            nameMod = "id=";
                        else
                            nameMod = "name=";
                        Debug.Log(nameMod);
                        webRequest = UnityWebRequest.Get(URL + databaseVersion + cInfo + nameMod + cardID);          //Card Information
                        StartCoroutine(APIrequest());
                    }
                }
                else if (EUS.sceneIndex == 2)
                {
                    PrepToFileName(cardID);
                    if (System.IO.File.Exists(Application.persistentDataPath + "/" + fileName.ToLower() + _URLMod.ToLower() + " search" + ".txt"))
                    {
                        saveManager.ReadFile(fileName + _URLMod + " search");
                        ResetPrefab();
                        ClearCardInfo(true, errorText);
                        StartCoroutine(LoadCardInfo(null, saveManager.fileData));
                    }
                    else
                    {   if(!saveManager.CheckIfOnlySpaces(cardID) || !saveManager.CheckIfOnlySpaces(_URLMod))
                        {
                        webRequest = UnityWebRequest.Get(URL + databaseVersion + cInfo + "fname=" + cardID + _URLMod);         //General Search
                        StartCoroutine(APIrequest());
                        }
                        else
                        {
                            //Add errortext
                        }
                    }
                }
            }
            /*else
            {
                
                else if (EUS.sceneIndex == 4)
                {
                    saveManager.WriteFile("scene4","yes");
                    Debug.Log("Sen API request");
                    if (System.IO.File.Exists(Application.persistentDataPath + "/archetypes.txt"))
                    {
                        saveManager.ReadFile("archetypes");
                        StartCoroutine(LoadCardInfo(null, saveManager.fileData));
                    }
                    else
                    {
                    //CardID = null
                    archetypeList.Clear();
                    request = new WWW(URL + "archetypes.php");                  //Archetype
                    StartCoroutine(APIrequest());
                    }
                }
            }*/
        }
        else
        {
            if (EUS.sceneIndex == 1)
            {
                /*
                ClearCardInfo(true, id, cardName, cardType, monsterType, atk, def, level, attribute, pendulumScale, archetype, desc);
                errorText.text = "No matching card was found.";
                image.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                 */

            }
            else if (EUS.sceneIndex == 2)
            {
                if (System.IO.File.Exists(Application.persistentDataPath + "/" + fileName.ToLower() + _URLMod.ToLower() + " search" + ".txt"))
                {
                    saveManager.ReadFile(fileName + _URLMod + " search");
                    ResetPrefab();
                    ClearCardInfo(true, errorText);
                    StartCoroutine(LoadCardInfo(null, saveManager.fileData));
                }
                else
                {
                    if (!saveManager.CheckIfOnlySpaces(_URLMod))
                    {
                        webRequest = UnityWebRequest.Get(URL + databaseVersion + cInfo + "fname=" + cardID + _URLMod);
                        StartCoroutine(APIrequest());
                    }
                    else
                    {
                        //Add errortext
                    }
                }
            }
            ClearCardInfo(true, id, cardName, cardType, monsterType, atk, def, level, attribute, pendulumScale, archetype, desc);
        }
        if (EUS.sceneIndex == 3)
        {
            //CardID = null
            webRequest = UnityWebRequest.Get(URL + databaseVersion + "randomcard.php");                  //Randomcard
            StartCoroutine(APIrequest(false));
        }
        if (EUS.sceneIndex == 4)
        {
            //saveManager.WriteFile("scene4outside","yes");
            Debug.Log("Sen API request");
            if (System.IO.File.Exists(Application.persistentDataPath + "/archetypes.txt"))
            {
                saveManager.ReadFile("archetypes");
                StartCoroutine(LoadCardInfo(null, saveManager.fileData));
            }
            else
            {
                //CardID = null
                archetypeList.Clear();
                webRequest = UnityWebRequest.Get(URL + databaseVersion + "archetypes.php");                  //Archetype
                StartCoroutine(ArchetypeRequest());
            }
        }
    }

    public IEnumerator APIrequest(bool resetButtons = true)
    {
        Debug.Log("Requesting from API");
        yield return webRequest.SendWebRequest();
        if (!webRequest.isNetworkError)
        { 
            if (webRequest.downloadHandler.text.Contains("No card matching your query was found in the database."))
            {
                ResetPrefab();
                ClearCardInfo(resetButtons, id, cardName, cardType, monsterType, atk, def, level, attribute, pendulumScale, archetype, desc);
                errorText.text = "No matching card was found.";

                image.color = Vector4.zero;
                showCardSets.interactable = false;
                showCardSets.gameObject.SetActive(false);
                artworkButtons.SetActive(false);
            }
            else if (webRequest.downloadHandler.text.Contains("\"error\":"))
            {
                ResetPrefab();
                ClearCardInfo(resetButtons, id, cardName, cardType, monsterType, atk, def, level, attribute, pendulumScale, archetype, desc);
                errorText.text = "An error has occurred.";
            }
            else
            {
                if (errorText != null)
                    ClearCardInfo(resetButtons, errorText);
                ResetPrefab();
                StartCoroutine(LoadCardInfo(webRequest));
            }
        }
    }

    public void ClearCardInfo(bool ResetIndex, Text textField1, Text textField2 = null, Text textField3 = null, Text textField4 = null, Text textField5 = null, Text textField6 = null, Text textField7 = null, Text textField8 = null, Text textField9 = null, Text textField10 = null, Text textField11 = null)
    {
        //Rename to textField? e.g. textField1, textField2
        textField1.text = "";
        if (textField2 != null)
            textField2.text = "";
        if (textField3 != null)
            textField3.text = "";
        if (textField4 != null)
            textField4.text = "";
        if (textField5 != null)
            textField5.text = "";
        if (textField6 != null)
            textField6.text = "";
        if (textField7 != null)
            textField7.text = "";
        if (textField8 != null)
            textField8.text = "";
        if (textField9 != null)
            textField9.text = "";
        if (textField10 != null)
            textField10.text = "";
        if (textField11 != null)
            textField11.text = "";
        if(ResetIndex)
            ResetImageIndex();
    }

    private void ResetImageIndex()
    {
        imageIndex = 0;
        if (nextImage != null)
        nextImage.interactable = true;
        if (previousImage != null)
            previousImage.interactable = false;
        Debug.Log("artwork arrows reset");
    }
    public void SetID()
    {
        if (idInput != null)
            cardID = idInput.text;
    }

    public void ResetPrefab()
    {
        EUS.DestroyAll("PrefabSearch");
        prefabHeight = 0;


    }

    IEnumerator SetImage(string url, int id, RawImage img, string additionalName = "")
    {
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(url);
        yield return imageRequest.SendWebRequest();
        //Vector3 imgLocation = img.gameObject.transform.parent.GetComponent<RectTransform>().anchoredPosition;
        //img.gameObject.transform.parent.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(imageRequest);
        img.texture = downloadedTexture;
        byte[] image_b = downloadedTexture.EncodeToJPG(100);
        File.WriteAllBytes(Application.persistentDataPath + "/" + id + additionalName, image_b);
        //if(img.gameObject.GetComponent<Image>() != null)
           // img.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        //else if (img.gameObject.GetComponent<RawImage>() != null)
            img.color = new Color(1, 1, 1, 1);
        //img.gameObject.transform.parent.GetComponent<RectTransform>().anchoredPosition = imgLocation;
    }

    public IEnumerator LoadImage(int id, RawImage img, bool imgSmall = false)    //REMEMBER: LoadImage is an IEnumerator, USE STARTCOROUTINE!
    {
        yield return id;
        byte[] image_b;
        if (File.Exists(Application.persistentDataPath + "/" + id))
        {
            image_b = File.ReadAllBytes(Application.persistentDataPath + "/" + id);
            Texture2D tex = new Texture2D(1,1);
            tex.LoadImage(image_b);
            img.texture = tex;
            img.color = new Color(1, 1, 1, 1);
        }
        else
        {
            if (!imgSmall)
            {
            StartCoroutine(SetImage("https://storage.googleapis.com/ygoprodeck.com/pics/" + id.ToString() + ".jpg", id, img));
            }
            else
            {
            StartCoroutine(SetImage("https://storage.googleapis.com/ygoprodeck.com/pics_small/" + id.ToString() + ".jpg", id, img, " small"));
            }
        }
        
        if(EUS.sceneIndex == 1)// || EUS.sceneIndex == 3)
            artworkButtons.SetActive(true);
    }
    public IEnumerator CardInfoPosition()
    {
        yield return desc.rectTransform.sizeDelta.y;
        cardInfoTransform.sizeDelta = new Vector2(cardInfoTransform.sizeDelta.x, (desc.rectTransform.sizeDelta.y - desc.rectTransform.anchoredPosition.y));
    }

    public IEnumerator LoadCardInfo(UnityWebRequest req = null, string cachedData = "")
    {
        string json;
        Debug.Log("Initialize Card Loading");
        if (req != null)
        {
            yield return req.downloadHandler.text;
           
            json =  req.downloadHandler.text;
            if (EUS.sceneIndex == 3)
                json = "{ \"data\":[" + req.downloadHandler.text + "]}";
           
            parseList = JsonHelper.FromJson<CardInfoParse>(json); 
            Debug.Log("Fetched Data from API");
            Debug.Log(parseList.Length);
           
        }
        else if (cachedData != "")
        {
            yield return cachedData;
            json = cachedData;
            if (EUS.sceneIndex == 3)
                json = "{ \"data\":[" + cachedData + "]}";
            parseList = JsonHelper.FromJson<CardInfoParse>(json);
            Debug.Log("Fetched Data from local file");
            Debug.Log(parseList.Length);
        }
        for (int i = 0; i < parseList.Length; i++)
        {           
            if (EUS.sceneIndex == 1 || EUS.sceneIndex == 3)
            {
                if (req != null)
                {

                    PrepToFileName(parseList[i].name);
                    if(EUS.sceneIndex == 1)
                    {
                        saveManager.WriteFile(fileName, req.downloadHandler.text);
                        saveManager.WriteFile(parseList[i].id.ToString(), req.downloadHandler.text);
                    }
                    else if(EUS.sceneIndex == 3)
                    {
                    //TROUBLESHOOTING: randomcard endpoint returns wrong data
                        //saveManager.WriteFile(fileName, "{ \"data\":[" + req.downloadHandler.text);
                        //saveManager.WriteFile(parseList[i].id.ToString(), req.downloadHandler.text);
                    }
                }

                if (image != null) { 
                    if (req != null)
                    {
                   
                        if (EUS.sceneIndex == 1)
                        StartCoroutine(SetImage("https://storage.googleapis.com/ygoprodeck.com/pics/" + parseList[i].card_images[imageIndex].id.ToString() + ".jpg", parseList[i].card_images[imageIndex].id, image));
                        else if (EUS.sceneIndex == 3)
                            StartCoroutine(SetImage("https://storage.googleapis.com/ygoprodeck.com/pics/" + parseList[i].id.ToString() + ".jpg", parseList[i].id,image));
                    }
                    else 
                    { 
                        if (EUS.sceneIndex == 1)
                            StartCoroutine(LoadImage(parseList[i].card_images[imageIndex].id, image));
                        else if (EUS.sceneIndex == 3)
                            StartCoroutine(LoadImage(parseList[i].id, image));
                    }
                    if (EUS.sceneIndex == 1)
                    {
                    artworkButtons.SetActive(true);   //Is this the real one or should a different one exist?
                    }

                }

                id.text = "ID: " + parseList[i].id.ToString();
                cardName.text = "Name: " + parseList[i].name;
                cardType.text = "Card Type: " + parseList[i].type;
                desc.text = "Desc:\n" + parseList[i].desc;

                //-------------- Card Set Start --------------\\
                for (int j = 0; j < cardSetContainer.transform.childCount; j++)
                {

                    Destroy(cardSetContainer.transform.GetChild(j).gameObject);
                }
                if (parseList[i].card_sets != null)
                {
                    for (int j = 0; j < parseList[i].card_sets.Length; j++)
                    {
                        SpawnSets(i, j);
                        //Debug.Log(parseList[i].card_sets[j].set_name + " : " + parseList[i].card_sets[j].set_code + " : " + parseList[i].card_sets[j].set_rarity + " : " + parseList[i].card_sets[j].set_price);
                    }
                    cardSetContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, parseList[i].card_sets.Length * scaleHandler.menu.sizeDelta.y);
                    RectTransform scrollContainer = cardSetContainer.transform.parent.GetComponent<RectTransform>();
                
                    scrollContainer.anchoredPosition = new Vector2(0, -scaleHandler.menu.sizeDelta.y * 1.25f);
                    scrollContainer.sizeDelta = new Vector2(scaleHandler.canvas.sizeDelta.x, scaleHandler.canvas.sizeDelta.y + scrollContainer.anchoredPosition.y - scaleHandler.menu.sizeDelta.y);

                     printAmount.text = "Total amount of prints: " + parseList[i].card_sets.Length;
                    showCardSets.gameObject.SetActive(true);
                     showCardSets.interactable = true;
                    RectTransform closeBtn = cardSetCloseBtn.GetComponent<RectTransform>();
                    closeBtn.sizeDelta = new Vector2(scaleHandler.menu.sizeDelta.y * 1.25f/2, scaleHandler.menu.sizeDelta.y * 1.25f/2);
                    closeBtn.anchoredPosition = new Vector2(closeBtn.sizeDelta.x/2, -closeBtn.sizeDelta.x/2);
            
                    Debug.Log("ARTWORK " + parseList[i].card_images.Length);
                    if (parseList[i].card_images.Length < 2)
                    {
                        nextImage.interactable = false;
                        Debug.Log("no alt artwork");
                    }
                }
                else
                {
                    showCardSets.interactable = false;
                }

                //-------------- Card Set End --------------\\

                //-------------- Image Start --------------\\
                //-------------- Image End --------------\\

                //Prices Start
                //for (int j = 0; j < parseList[i].card_prices.Length; j++)
                //{

                    //Debug.Log("CM Price: " + parseList[i].card_prices[i].cardmarket_price);
                    //Debug.Log("TCG Price: " + parseList[i].card_prices[i].tcgplayer_price);
                    //Debug.Log("Coolstuff Price: " + parseList[i].card_prices[i].coolstuffinc_price);
                    //Debug.Log("Ebay Price: " + parseList[i].card_prices[i].ebay_price);
                    //Debug.Log("A-Z Price: " + parseList[i].card_prices[i].amazon_price);
                //}
                //Prices End

          //General info end
                if (parseList[i].type == "Spell Card")
                    monsterType.text = "Spell Type: " + parseList[i].race;
                else if (parseList[i].type == "Trap Card")
                    monsterType.text = "Trap Type: " + parseList[i].race;
                else
                    monsterType.text = "Monster Type: " + parseList[i].race;

                //monster specific
                if (parseList[i].type != "Spell Card" && parseList[i].type != "Trap Card")
                {
                    atk.text = "ATK: " + parseList[i].atk.ToString();
                    attribute.text = "Attribute: " + parseList[i].attribute;
                    //monster specific, !link
                    if (parseList[i].type != "Link Monster")
                    {
                        def.text = "DEF: " + parseList[i].def.ToString();
                        level.text = "Level: " + parseList[i].level.ToString();
                    }

                    //link specific
                    else
                    {
                        def.text = "Link Rating: " + parseList[i].linkval.ToString();
                        //Add link markers to Card Info
                        string linkMarkers = "";
                        for (int j = 0; j < parseList[i].linkmarkers.Length; j++)
                        {
                            if (j != 0)
                            {
                                linkMarkers += ", " + parseList[i].linkmarkers[j];
                            }
                            else
                            {
                                linkMarkers += parseList[i].linkmarkers[j];
                            }
                        }
                        level.text = "Link Markers: " + linkMarkers;
                    }

                    //pendulum specific
                    if (parseList[i].type.Contains("Pendulum"))
                    {
                        archetype.rectTransform.anchoredPosition = new Vector2(0, cardName.rectTransform.anchoredPosition.y * 9);
                        pendulumScale.text = "Pendulum Scale: " + parseList[i].scale.ToString();
                    }
                    else
                    {
                        //If not pendulum
                        ClearCardInfo(false, pendulumScale);
                        archetype.rectTransform.anchoredPosition = new Vector2(0, cardName.rectTransform.anchoredPosition.y * 8);
                    }
                }
                else
                {
                    //If Spell or Trap
                    archetype.rectTransform.anchoredPosition = new Vector2(0, cardName.rectTransform.anchoredPosition.y * 4);
                    ClearCardInfo(false, atk, def, level, pendulumScale, attribute);
                }
                //misc
                //Makes sure to only show archetype if the card belongs to one
                if (parseList[i].archetype != null)
                {
                    archetype.text = "Archetype: " + parseList[i].archetype;
                    desc.rectTransform.anchoredPosition = archetype.rectTransform.anchoredPosition - new Vector2(0, -cardName.rectTransform.anchoredPosition.y);
                }
                else
                {
                    ClearCardInfo(false, archetype);
                    desc.rectTransform.anchoredPosition = archetype.rectTransform.anchoredPosition;
                }
                //Make Card Info end where desc ends, to not cut out any information
                StartCoroutine(CardInfoPosition());
            }
            
            else if (EUS.sceneIndex == 2)
            {
                Debug.Log("iteration #" + (i+1));
                prefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Search Result"), GameObject.FindGameObjectWithTag("Result").transform) as GameObject;
                RectTransform prefabRT = prefab.GetComponent<RectTransform>();
                prefabRT.sizeDelta = new Vector2(0,scaleHandler.scrollField.sizeDelta.y/20);   //Make X entries fit on screen at the same time (Make 1 entry 1/20 of the viewport height)
                prefabRT.anchoredPosition = new Vector2(0, prefabHeight);
                prefab.GetComponent<Text>().fontSize = scaleHandler.fontSize;
                prefab.GetComponentInChildren<Text>().text = parseList[i].name;

                StartCoroutine(LoadImage(parseList[i].id, prefab.transform.GetChild(1).GetComponent<RawImage>(), true));
                

                Button btn = prefab.GetComponentInChildren<Button>();
                RectTransform rect = btn.GetComponent<RectTransform>();
                btn.name = parseList[i].name;       
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x , -(prefabRT.sizeDelta.y * 0.025f));
                rect.sizeDelta = new Vector2(scaleHandler.canvas.sizeDelta.x * 0.25f, prefabRT.sizeDelta.y * 0.95f);
                btn.GetComponentInChildren<Text>().fontSize = scaleHandler.fontSize;

                //StartCoroutine(SetImage2("https://storage.googleapis.com/ygoprodeck.com/pics_small/" + parseList[i].id.ToString() + ".jpg", parseList[i].id, btn.GetComponent<CanvasRenderer>()));

                prefab.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(-rect.sizeDelta.x + rect.anchoredPosition.y, 0);
                prefab.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(prefabRT.sizeDelta.y * 0.68493f, prefabRT.sizeDelta.y);

                StartCoroutine(SetCardSearchHeight());
                prefabHeight -= prefabRT.sizeDelta.y;
                toJson = new CardInfoParse[1];
                toJson[0] = parseList[i];
               
                jsonSaveData = JsonHelper.ToJson<CardInfoParse>(toJson);
                Debug.Log(jsonSaveData);

            //TROUBLESHOOTING: Make sure the saved data is valid to be read later
                //jsonSaveData = jsonSaveData.Replace("{\"Items\":","");
                //jsonSaveData = jsonSaveData.Substring(0, jsonSaveData.Length - 1);

                if (i == 0) //Only check once
                {
                    if (req != null)
                    {
                        //save the search results
                        saveManager.WriteFile(fileName + _URLMod + " search" , req.downloadHandler.text);
                    }
                }
                
                if (req != null)
                {
                    PrepToFileName(parseList[i].name);
                    saveManager.WriteFile(fileName, jsonSaveData);
                    // saveManager.WriteFile(parseList[i].id.ToString(), jsonSaveData);
                }
            }
            else if (EUS.sceneIndex == 4)
            {
                if (i < 3)
                {
                    archetypeList.Add(parseList[UnityEngine.Random.Range(0,parseList.Length)]);
                }
                if (i == 3)
                {
                    id.text = archetypeList[0].archetype_name;
                    cardName.text = archetypeList[1].archetype_name;
                    cardType.text = archetypeList[2].archetype_name;
                    archetypeList.Clear();
                    if(req != null)
                        saveManager.WriteFile("archetypes", req.downloadHandler.text);
                }
            }
        }
    }

    public void SpawnSets(int i, int j)
    {
        float cardSetHeight = scaleHandler.menu.sizeDelta.y;
        string cardSetText = parseList[i].card_sets[j].set_name + ", \n" + parseList[i].card_sets[j].set_code + ", " + parseList[i].card_sets[j].set_rarity;

        prefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Card Set"), cardSetContainer.transform) as GameObject;
        RectTransform prefabRT = prefab.GetComponent<RectTransform>();
        prefabRT.sizeDelta = new Vector2(0, scaleHandler.scrollField.sizeDelta.y/10);   //Make 20 entries fit on screen at the same time (Make 1 entry 1/20 of the viewport height)
        prefabRT.anchoredPosition = new Vector2(0, -cardSetHeight * j);
        prefab.GetComponent<Text>().fontSize = scaleHandler.fontSize;
        prefab.GetComponentInChildren<Text>().text = cardSetText;
        printAmount.fontSize = scaleHandler.fontSize;
        printAmount.rectTransform.sizeDelta = new Vector2(cardSetHeight * 1.25f * 2.8916667f, cardSetHeight * 1.25f);

    }

    IEnumerator SetCardSearchHeight()
    {
        //Sets the height of the scrollable object so that the generated text and buttons doesn't get cut off too early
        yield return prefab.GetComponent<RectTransform>().sizeDelta;
        cardInfoTransform.sizeDelta = new Vector2(cardInfoTransform.sizeDelta.x, (prefab.GetComponent<RectTransform>().sizeDelta.y - prefab.GetComponent<RectTransform>().anchoredPosition.y));
    }

    //Called by button
    public void CopyCardName()
    {
        //copies the card name to the clipboard
        GUIUtility.systemCopyBuffer = gameObject.name;
    }

    public IEnumerator LoadArchetypeInfo(UnityWebRequest req = null, string cachedData = "")
    {
        string json;

        if(req != null)
        {
            yield return req.downloadHandler.text;
            json = "{ \"data\":" + req.downloadHandler.text + "}";
            parseArchList = JsonHelper.FromJson<ArchetypeParse>(json);
        }
        else if (cachedData != "")
        {
            yield return cachedData;
            json = "{ \"data\":" + cachedData + "}";
            parseArchList = JsonHelper.FromJson<ArchetypeParse>(json);
        }
        
        if(dropDownMenu != null)
        {
            dropDownMenu.archetype.Clear();
            dropDownMenu.archetype.Add(new Dropdown.OptionData(""));
        }

        for (int i = 0; i < parseArchList.Length; i++)
        {
            dropDownMenu.archetype.Add(new Dropdown.OptionData(parseArchList[i].archetype_name));
        }
        if (req != null)
            saveManager.WriteFile("archetypes", req.downloadHandler.text);
    }

    public IEnumerator GetArchetypes()
    {
        webRequest = UnityWebRequest.Get(URL + databaseVersion + "archetypes.php");
        yield return StartCoroutine(ArchetypeRequest());
    }

    private IEnumerator ArchetypeRequest()
    {
        yield return webRequest.SendWebRequest();

        if (errorText != null)
                ClearCardInfo(false, errorText);
        if (System.IO.File.Exists(Application.persistentDataPath + "/archetypes.txt"))
        {
            saveManager.ReadFile("archetypes");
            yield return StartCoroutine(LoadArchetypeInfo(null, saveManager.fileData));
        }
        else
        {
            yield return StartCoroutine(LoadArchetypeInfo(webRequest));
        }
    }


    public IEnumerator LoadCardSetInfo(UnityWebRequest req = null, string cachedData = "")
    {

        string json;
        if (req != null) { 
            yield return req.downloadHandler.text;
            json = "{ \"Items\":" + req.downloadHandler.text + "}";
            parseSetList = JsonHelper.FromJson<CardSetParse>(json);
        }
        else if (cachedData != "")
        {
            yield return cachedData;
            json = "{ \"Items\":" + cachedData + "}";
            parseSetList = JsonHelper.FromJson<CardSetParse>(json);
        }

        if (parseSetList == null)
            yield break;

        dropDownMenu.cardset.Clear();
        dropDownMenu.cardset.Add(new Dropdown.OptionData(""));
        for (int i = 0; i < parseSetList.Length; i++)
        {

            dropDownMenu.cardset.Add(new Dropdown.OptionData(parseSetList[i].set_name));
        }
     
        if (req != null)
            saveManager.WriteFile("cardsets", req.downloadHandler.text);
    }

    public IEnumerator GetCardSet()
    {
        webRequest = UnityWebRequest.Get("db.ygoprodeck.com/api/v7/cardsets.php");                  //Randomcard
        yield return StartCoroutine(CardSetRequest());
    }

    private IEnumerator CardSetRequest()
    {
        yield return webRequest.SendWebRequest();

        if (errorText != null)
            ClearCardInfo(false, errorText);
        if (System.IO.File.Exists(Application.persistentDataPath + "/cardsets.txt"))
        {
            saveManager.ReadFile("cardsets");
            yield return StartCoroutine(LoadCardSetInfo(null, saveManager.fileData));
        }
        else
        {
            yield return StartCoroutine(LoadCardSetInfo(webRequest));
        }
    }

    private void ReplaceToValidFileName(string toReplace, string replaceTo)
    {
        if (fileName.Contains(toReplace))
        {
            fileName = fileName.Replace(toReplace, replaceTo);
        }
    }

    public void PrepToFileName(string fileNameInsert)
    {
        fileName = fileNameInsert;
        ReplaceToValidFileName(":", "_");
        ReplaceToValidFileName("/", "=");
        ReplaceToValidFileName("?","ʔ");
        ReplaceToValidFileName("%","¤");
        ReplaceToValidFileName("\"","^");
    }
    
    public void PrepFromFileName()
    {
        //fileName = cardID;
        ReplaceToValidFileName("_", ":");
        ReplaceToValidFileName("=", "/");
        ReplaceToValidFileName("ʔ","?");
        ReplaceToValidFileName("¤","%");
        ReplaceToValidFileName("^","\"");
    }

    public void NextImage()
    {
            Debug.Log(imageIndex + " : " + (parseList[0].card_images.Length-1));
        if (imageIndex < (parseList[0].card_images.Length - 1))
        {
            imageIndex++;
            StartCoroutine(LoadImage(parseList[0].card_images[imageIndex].id, image));
            if (previousImage.interactable == false)
            {
                previousImage.interactable = true;
            }
        }
        if (imageIndex >= (parseList[0].card_images.Length-1))
        {
            nextImage.GetComponent<Button>().interactable = false;
        }
        //Debug.Log(imageIndex + " : " + (parseList[0].card_images.Length - 1));
    }

    public void PreviousImage()
    {
        Debug.Log("Previous Image");
        Debug.Log(parseList[0].name + " : " + parseList[0].card_images[imageIndex].id);
        imageIndex--;
        StartCoroutine(LoadImage(parseList[0].card_images[imageIndex].id, image));
        if (nextImage.interactable == false)
        {
            nextImage.interactable = true;
        }
        if (imageIndex == 0)
        {
            previousImage.GetComponent<Button>().interactable = false;
        }
    }
}