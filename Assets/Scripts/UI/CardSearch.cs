using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ApiCall;
using UnityEngine.UI;

public class CardSearch : MonoBehaviour
{
    GameObject prefab;
    public ScaleHandler scaleHandler;
    public RectTransform cardSearchTransform;
    public DropdownHandler dropDownMenu;
    [SerializeField] Button submitButton;
    [HideInInspector] public float prefabHeight = 0;
    [HideInInspector] public CardInfoParse[] fetchedCards;


    public void ConvertData(CardInfoParse[] cards)
    {
        

        for (int i = 0; i < cards.Length; i++)
        {
            CardInfoParse card = cards[i];


            prefab = Instantiate(Resources.Load<GameObject>("Prefabs/Search Result"), cardSearchTransform);
            RectTransform prefabRT = prefab.GetComponent<RectTransform>();
            prefabRT.sizeDelta = new Vector2(0, scaleHandler.scrollField.sizeDelta.y / 20);   //Make X entries fit on screen at the same time (Make 1 entry 1/20 of the viewport height)
            prefabRT.anchoredPosition = new Vector2(0, prefabHeight);
            prefab.GetComponent<Text>().fontSize = scaleHandler.fontSize;
            prefab.GetComponentInChildren<Text>().text = card.name;

            ApiCall.Instance.LoadImage(card.id, prefab.transform.GetChild(1).GetComponent<RawImage>(), ImageTypes.SMALL);


            Button btn = prefab.GetComponentInChildren<Button>();
            RectTransform rect = btn.GetComponent<RectTransform>();
            btn.name = card.name;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -(prefabRT.sizeDelta.y * 0.025f));
            rect.sizeDelta = new Vector2(scaleHandler.canvas.sizeDelta.x * 0.25f, prefabRT.sizeDelta.y * 0.95f);
            btn.GetComponentInChildren<Text>().fontSize = scaleHandler.fontSize;

            //StartCoroutine(SetImage2("https://storage.googleapis.com/ygoprodeck.com/pics_small/" + parseList[i].id.ToString() + ".jpg", parseList[i].id, btn.GetComponent<CanvasRenderer>()));

            prefab.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(-rect.sizeDelta.x + rect.anchoredPosition.y, 0);
            prefab.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(prefabRT.sizeDelta.y * 0.68493f, prefabRT.sizeDelta.y);

            StartCoroutine(SetCardSearchHeight());
            prefabHeight -= prefabRT.sizeDelta.y;


            ApiCall.Instance.jsonSaveData = JsonParser.ToJson(card);


            if (i == 0) //Only check once
            {
                if (ApiCall.Instance.loadType == LoadTypes.API)
                {

                    //save the search results
                    ApiCall.Instance.webRequest.downloadHandler.text.WriteStringToFile(SaveManager.parameterDirectory, ApiCall.Instance.dropdownUrlMod + " search", SaveManager.parameterFileType);
                    //SaveManager.(ApiCall.Instance.fileName + ApiCall.Instance.dropdownUrlMod + " search", ApiCall.Instance.webRequest.downloadHandler.text);
                }
            }

            if (ApiCall.Instance.loadType == LoadTypes.API)
            {
                string cardFileName = card.name.ConvertToValidFileName();
                ApiCall.Instance.jsonSaveData.WriteStringToFile(SaveManager.parameterDirectory, cardFileName, SaveManager.cardFileType);
                //ApiCall.Instance.saveManager.WriteFile(ApiCall.Instance.fileName, ApiCall.Instance.jsonSaveData);
                // saveManager.WriteFile(parseList[i].id.ToString(), jsonSaveData);
            }

        }

    }



    public void ResetPrefab()
    {
        EUS.Cat_Object_Manipulation.DestroyAll("PrefabSearch");
        prefabHeight = 0;
    }

    public IEnumerator SetCardSearchHeight()
    {
        //Sets the height of the scrollable object so that the generated text and buttons doesn't get cut off too early
        yield return prefab.GetComponent<RectTransform>().sizeDelta;
        cardSearchTransform.sizeDelta = new Vector2(cardSearchTransform.sizeDelta.x, (prefab.GetComponent<RectTransform>().sizeDelta.y - prefab.GetComponent<RectTransform>().anchoredPosition.y));
    }

    public void SetSubmitButtonListener()
    {
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(() => ApiCall.Instance.Execute());
    }

}
