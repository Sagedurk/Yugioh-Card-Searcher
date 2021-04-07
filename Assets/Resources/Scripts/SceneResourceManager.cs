using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SceneResourceManager : MonoBehaviour
{
    public CardInfo cardInfo;
    void Start()
    {
        cardInfo = this.gameObject.GetComponent<CardInfo>();

        if (EUS.sceneIndex == 2)
        {
            cardInfo.SetID();
            if (cardInfo.cardID != "" || cardInfo._URLMod != "")
            {
                /*if (System.IO.File.Exists(Application.persistentDataPath + "/" + cardInfo.cardID.ToLower() + cardInfo._URLMod.ToLower() + "_search" + ".txt"))
                {
                    cardInfo.saveManager.ReadFile(cardInfo.cardID + cardInfo._URLMod + "_search");
                    cardInfo.ResetPrefab();
                    cardInfo.ClearCardInfo(cardInfo.errorText);
                    StartCoroutine(cardInfo.LoadCardInfo(null, cardInfo.saveManager.fileData));
                }*/
            }
        }
    }
}
