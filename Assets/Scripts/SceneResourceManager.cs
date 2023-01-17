using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneResourceManager : MonoBehaviour
{

    void Start()
    {
        if (EUS.sceneIndex == 2)
        {
            ApiCall.Instance.SetID();
            if (ApiCall.Instance.cardID != "" || ApiCall.Instance.dropdownUrlMod != "")
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
