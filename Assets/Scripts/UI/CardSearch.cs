using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ApiCall;
using UnityEngine.UI;

public class CardSearch : MonoBehaviour
{
    public InputField idInputField;
    [HideInInspector] public Text idInput;

    GameObject prefab;
    public RectTransform cardSearchTransform;
    public DropdownHandler dropDownMenu;
    [SerializeField] Button submitButton;
    [HideInInspector] public CardInfoParse[] fetchedCards;

    private void Start()
    {
        idInput = idInputField.textComponent;
    }


    public IEnumerator ConvertData(CardInfoParse[] cards)
    {
        float prefabHeight = 0;

        for (int i = 0; i < cards.Length; i++)
        {
            CardInfoParse card = cards[i];

            prefab = Instantiate(Resources.Load<GameObject>("Prefabs/Search Result"), cardSearchTransform);
            RectTransform PrefabRT = prefab.GetComponent<RectTransform>();
            CardSearchResult result = prefab.GetComponent<CardSearchResult>();

            if (EUS.Cat_Math.IsDivisbleBy(i, 2))
                result.background.color = result.background.color.SetAlpha(0.5f);

            PrefabRT.anchoredPosition = Vector2.up * prefabHeight;
            
            result.cardName.text = card.name;
            result.imageID = card.id;

            cardSearchTransform.sizeDelta = new Vector2(cardSearchTransform.sizeDelta.x, (PrefabRT.sizeDelta.y - PrefabRT.anchoredPosition.y));
            prefabHeight -= PrefabRT.sizeDelta.y;
          
            yield return ApiCall.Instance.TryDownloadImages(ApiCall.imageURL, card);
            ApiCall.Instance.LoadImage(card.id, result.cardImage, ImageTypes.SMALL);

            if (ApiCall.Instance.loadType == LoadTypes.API)
                SaveManager.SaveCard(card, true);
        }
    
        ApiCall.Instance.webRequest.downloadHandler.text.WriteStringToFile(SaveManager.parameterDirectory, ApiCall.Instance.dropdownUrlMod + " search", SaveManager.parameterFileType);

    }


    public void ResetPrefab()
    {
        EUS.Cat_Object_Manipulation.DestroyAll("PrefabSearch");
    }

    public void SetSubmitButtonListener()
    {
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(() => ApiCall.Instance.Execute());
    }

}
