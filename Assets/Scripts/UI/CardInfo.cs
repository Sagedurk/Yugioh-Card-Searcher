﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.CompilerServices;
using System;
using Unity.Collections;
using UnityEngine.Networking;
using static ApiCall;
using static RectTransformExt;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
//using UnityEditor.PackageManager.Requests;
//using UnityEngine.UIElements;

//Handles card information and API requests
public class CardInfo : MonoBehaviour
{
    GameObject prefab;
    [SerializeField] RectTransform viewport;
    [SerializeField] RectTransform menuButtons;

    [Header("Image Buttons")]
    public Button previousImageButton;
    public Button nextImageButton;
    public Button updateImagesButton;

    [Header("Card Set")]
    [SerializeField] int amountOfSetsDisplayed = 20;
    public RectTransform cardSetContainer;
    public Button  cardSetButton, cardSetCloseBtn;
    public TextExtension amountOfPrints;


    [Space(15)]
    public InputField idInputField;
    [HideInInspector] public Text idInput;
    public RawImage image;
    public RectTransform cardInfoTransform;
    public CardInfoParse[] fetchedCards;
    [SerializeField] Button submitButton;

    public int imageIndex, cardIDParsingResult;

    public TextExtension errorText, id, cardName, cardType, monsterType, atk, def, level, attribute, pendulumScale, archetype, desc;

    private void Awake()
    {
        idInput = idInputField.textComponent;
    }


   
    //Add visual content here:
    #region Visual Content

    #region Image
    
    #region Artwork Buttons

    public void HideImageButtons()
    {
        previousImageButton.gameObject.SetActive(false);
        nextImageButton.gameObject.SetActive(false);
    }

    public void ShowImageButtons()
    {
        previousImageButton.gameObject.SetActive(true);
        nextImageButton.gameObject.SetActive(true);
    }

    public void NextImage()
    {
        if (imageIndex < (fetchedCards[0].card_images.Length - 1))
        {
            imageIndex++;
            ApiCall.Instance.LoadImage(fetchedCards[0].card_images[imageIndex].id, image, ImageTypes.LARGE);
            if (previousImageButton.interactable == false)
            {
                previousImageButton.interactable = true;
            }
        }
        if (imageIndex >= (fetchedCards[0].card_images.Length - 1))
        {
            nextImageButton.GetComponent<Button>().interactable = false;
        }
    }

    public void PreviousImage()
    {
        imageIndex--;
        ApiCall.Instance.LoadImage(fetchedCards[0].card_images[imageIndex].id, image, ImageTypes.LARGE);
        if (nextImageButton.interactable == false)
        {
            nextImageButton.interactable = true;
        }
        if (imageIndex == 0)
        {
            previousImageButton.GetComponent<Button>().interactable = false;
        }
    }

    #endregion

    public void ResetImageIndex()
    {
        imageIndex = 0;

        if (nextImageButton != null)
            nextImageButton.interactable = true;

        if (previousImageButton != null)
            previousImageButton.interactable = false;
    }

    public void HideImage()
    {
        image.color = Color.clear;
        image.texture = null;
    }
    
    public void UpdateCardImages()
    {
        SaveManager.DeleteImages(fetchedCards[0]);
        StartCoroutine(SetImage(fetchedCards[0]));
    }

    private IEnumerator SetImage(CardInfoParse card)
    {
        yield return StartCoroutine(ApiCall.Instance.TryDownloadImages(ApiCall.imageURL, card));
        ApiCall.Instance.LoadImage(card.card_images[imageIndex].id, image, ApiCall.ImageTypes.LARGE);
    }

    #endregion

    #region Card Sets
    public void SpawnSets(CardInfoParse card, int j)
    {
        
        float cardSetHeight = menuButtons.rect.height;
        string cardSetText = card.card_sets[j].set_name + ", \n" + card.card_sets[j].set_code + ", " + card.card_sets[j].set_rarity;

        prefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Card Set"), cardSetContainer.transform) as GameObject;
        RectTransform prefabTransform = prefab.GetComponent<RectTransform>();

        prefabTransform.sizeDelta = new Vector2(0, viewport.rect.height / amountOfSetsDisplayed);
        prefabTransform.anchoredPosition = new Vector2(0, -prefabTransform.sizeDelta.y * j);
        prefab.GetComponent<Text>().fontSize = ScaleHandler.fontSize;
        prefab.GetComponentInChildren<Text>().text = cardSetText;
        amountOfPrints.fontSize = ScaleHandler.fontSize;
        amountOfPrints.rectTransform.sizeDelta = new Vector2(cardSetHeight * 1.25f * 2.8916667f, cardSetHeight * 1.25f);
    }

    public void ResetCardSetContainer()
    {
        for (int j = 0; j < cardSetContainer.childCount; j++)
        {
            Destroy(cardSetContainer.GetChild(j).gameObject);
        }
    }


    #endregion


    public IEnumerator ResizeTransform()
    {
        yield return desc.rectTransform.sizeDelta.y;
        cardInfoTransform.sizeDelta = new Vector2(cardInfoTransform.sizeDelta.x, (desc.rectTransform.sizeDelta.y - desc.rectTransform.anchoredPosition.y));
    }


    public void ShowCardSetButton(bool isShowing)
    {
        cardSetButton.gameObject.SetActive(isShowing);
        cardSetButton.interactable = isShowing;

        //updateImagesButton.gameObject.SetActive(isShowing);
        //updateImagesButton.interactable = isShowing;
    }

    public void ClearTextInfo(TextExtension[] textFields = null)
    {
        foreach (TextExtension text in textFields)
        {
            if (text != null)
                text.SetText("");
        }
    }

    public void ClearTextInfo(TextExtension textField)
    {
        if (textField != null)
            textField.SetText("");
    }


    #endregion



    #region Convertions

    private void SetUniversalCardInfo(CardInfoParse card)
    {
        id.SetText("ID: " + card.id.ToString());
        cardName.SetText("Name: " + card.name);
        cardType.SetText("Card Type: " + card.type);
        desc.SetText("Desc:\n" + card.desc);
    }

    private void SetType(CardInfoParse card)
    {
        if (card.type == "Spell Card")
            monsterType.SetText("Spell Type: ", card.race);
        else if (card.type == "Trap Card")
            monsterType.SetText("Trap Type: ", card.race);
        else
            monsterType.SetText("Monster Type: ", card.race);
    }

    #region Archetype Functions
    void TryShowArchetype(CardInfoParse card)
    {
        if (!ShowArchetype(card))
            HideArchetype();

    }

    bool ShowArchetype(CardInfoParse card)
    {
        if (card.archetype == null)
            return false;

        archetype.SetText("Archetype: " + card.archetype);

        Vector2 anchorPosition = archetype.rectTransform.anchoredPosition - new Vector2(0, -cardName.rectTransform.anchoredPosition.y);
        desc.SetAnchoredPosition(Directions2D.UP, anchorPosition);
        return true;

    }

    void HideArchetype()
    {
        ClearTextInfo(archetype);
        desc.SetAnchoredPosition(Directions2D.UP, archetype.rectTransform);
    }
    #endregion

    public IEnumerator ConvertData(CardInfoParse card)
    {
        #region Set Image
        //if (ApiCall.Instance.loadType == LoadTypes.API)
        yield return StartCoroutine(SetImage(card));
        ShowImageButtons();
        #endregion


        SetUniversalCardInfo(card);
        SetType(card);

        #region Card Set

        ResetCardSetContainer();

        if (card.card_sets != null)
        {
            for (int j = 0; j < card.card_sets.Length; j++)
            {
                SpawnSets(card, j);
            }

            Rect setSize = viewport.rect; 
            Vector2 canvasSize = ScaleHandler.GetCanvasSize();

            cardSetContainer.SetSize(Directions2D.UP, card.card_sets.Length * setSize.height / amountOfSetsDisplayed);

            RectTransform scrollContainer = cardSetContainer.parent.GetComponent<RectTransform>();

            scrollContainer.SetAnchoredPosition(Directions2D.DOWN, setSize.size / amountOfSetsDisplayed, 1.25f);
            scrollContainer.SetSize(Directions2D.UP_RIGHT, canvasSize);
            scrollContainer.AddSize(Directions2D.UP, (scrollContainer.anchoredPosition.y - setSize.height / amountOfSetsDisplayed));

            amountOfPrints.SetText("Total amount of prints: " + card.card_sets.Length);

            #region Buttons
            ShowCardSetButton(true);

            RectTransform closeButton = cardSetCloseBtn.GetComponent<RectTransform>();
            closeButton.SetSize(Directions2D.UP_RIGHT, setSize.height / amountOfSetsDisplayed, 0.625f);
            closeButton.SetAnchoredPosition(Directions2D.DOWN_RIGHT, closeButton.sizeDelta, 0.5f);
            #endregion

            if (card.card_images.Length < 2)
                nextImageButton.interactable = false;

        }
        else
            ShowCardSetButton(false);


        #endregion


        //monster specific
        #region Monster Specific
        if (card.type != "Spell Card" && card.type != "Trap Card")
        {
            atk.SetText("ATK: " + card.atk.ToString());
            attribute.SetText("Attribute: " + card.attribute);
            //monster specific, non-link
            if (card.type != "Link Monster")
            {
                def.SetText("DEF: " + card.def.ToString());
                level.SetText("Level: " + card.level.ToString());
            }

            //link specific
            else
            {
                def.SetText("Link Rating: " + card.linkval.ToString());
                //Add link markers to Card Info
                string linkMarkers = "";

                for (int j = 0; j < card.linkmarkers.Length; j++)
                {
                    if (j != 0)
                    {
                        linkMarkers += ", " + card.linkmarkers[j];
                    }
                    else
                    {
                        linkMarkers += card.linkmarkers[j];
                    }
                }
                level.SetText("Link Markers: " + linkMarkers);
            }

            //pendulum specific
            if (card.type.Contains("Pendulum"))
            {
                archetype.SetAnchoredPosition(Directions2D.UP, cardName.rectTransform, 9);
                pendulumScale.SetText("Pendulum Scale: " + card.pendScale.ToString());
            }
            else
            {
                //If not pendulum
                ClearTextInfo(pendulumScale);
                archetype.SetAnchoredPosition(Directions2D.UP, cardName.rectTransform, 8);
            }
        }
        #endregion

        else
        {
            //If Spell or Trap
            archetype.SetAnchoredPosition(Directions2D.UP, cardName.rectTransform, 4);
            ClearTextInfo(new TextExtension[] { atk, def, level, pendulumScale, attribute });
        }

        #region misc

        //Show archetype only if the card belongs to one
        TryShowArchetype(card);

        //Resize the container so it doesn't cut out any information
        StartCoroutine(ResizeTransform());
        #endregion

    }

    #endregion

    public void SetSubmitButtonListener()
    {
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(() => ApiCall.Instance.Execute());
    }
}