using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ErrorManager : EUS.Cat_Systems.Singleton<ErrorManager>
{

    ApiCall.ApiTypes sceneType;
    Text errorText;

    protected override void Awake()
    {
        isDestroyable = false;
        base.Awake();
    }

    private void Start()
    {
        SetErrorTextField();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("OVERRIDE SUCCESSFUL");
        sceneType = ApiCall.Instance.apiType;

        //return;

        SetErrorTextField();

        //try
        //{
        //    throw new Exception("This is an exception for testing purposes.");
        //}
        //catch (Exception ex)
        //{
        //    Debug.Log("Caught");
        //    LogExceptionToEndUser(ex);
        //}
    }

    private void SetErrorTextField()
    {
        switch (sceneType)
        {
            case ApiCall.ApiTypes.CARD_INFO:

                errorText = ApiCall.Instance.cardInfo.errorText;

                break;
            case ApiCall.ApiTypes.CARD_SEARCH:

                errorText = ApiCall.Instance.cardSearch.errorText;

                break;
            case ApiCall.ApiTypes.CARD_RANDOM:
                break;
            default:
                break;
        }
    }
     
    public void SetError(string errorMessage)
    {
        ErrorPreparation();

        errorText.text = errorMessage;
        errorText.enabled = true;
    }

    public void ClearError()
    {
        ClearErrorPreparation();

        errorText.text = "";
        errorText.enabled = false;
    }


    private void ErrorPreparation()
    {
        switch (sceneType)
        {
            case ApiCall.ApiTypes.CARD_INFO:

                ClearCardInfo(ApiCall.Instance.cardInfo);

                break;

            case ApiCall.ApiTypes.CARD_SEARCH:

                ApiCall.Instance.cardSearch.DestroySearchResults();

                break;

            case ApiCall.ApiTypes.CARD_RANDOM:
                
                //ClearCardInfo(ApiCall.Instance.cardInfo);
                
                break;
            
            default:
                break;
        }
    }

    private void ClearErrorPreparation()
    {

        switch (sceneType)
        {
            case ApiCall.ApiTypes.CARD_INFO:

                ApiCall.Instance.cardInfo.ResetImageIndex();

                break;
            case ApiCall.ApiTypes.CARD_SEARCH:

                break;
            case ApiCall.ApiTypes.CARD_RANDOM:
                break;
            default:
                break;
        }
    }

    private void ClearCardInfo(CardInfo cardInfo)
    {
        cardInfo.ClearTextInfo(new TextExtension[] {
                    cardInfo.id, cardInfo.cardName, cardInfo.cardType,cardInfo.monsterType,
                    cardInfo.atk, cardInfo.def, cardInfo.level, cardInfo.attribute,
                    cardInfo.pendulumScale, cardInfo.archetype, cardInfo.desc });

        cardInfo.ResetImageIndex();

        cardInfo.HideImageButtons();
        cardInfo.HideImage();

        cardInfo.ShowCardSetButton(false);
    }


    public static void ThrowException(Exception exception)
    {
        throw exception;
    }
    public static void LogException(Exception exception)
    {
#if UNITY_EDITOR
        Debug.LogError(exception.Message + "\n\n" + exception.InnerException);
#endif
    }
    public static void ThrowExceptionToEndUser(Exception exception)
    {
        Instance.SetError(exception.Message + "\n\n" + exception.InnerException);

        ThrowException(exception);

    }
    public static void LogExceptionToEndUser(Exception exception)
    {
        Instance.SetError(exception.Message + "\n\n" + exception.InnerException);

        LogException(exception);
    }


}
