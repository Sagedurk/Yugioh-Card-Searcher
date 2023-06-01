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
        base.Awake();
        sceneType = ApiCall.Instance.apiType;

    }

    private void Start()
    {
        SetErrorTextField();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetErrorTextField();

    }

    private void SetErrorTextField()
    {
        switch (sceneType)
        {
            case ApiCall.ApiTypes.CARD_INFO:

                errorText = ApiCall.Instance.cardInfo.errorText;

                break;
            case ApiCall.ApiTypes.CARD_SEARCH:

                //errorText = ApiCall.Instance.cardSearch.

                break;
            case ApiCall.ApiTypes.CARD_RANDOM:
                break;
            default:
                break;
        }
    }
     
    public void SetError(string errorMessage)
    {
        errorText.text = errorMessage;
        errorText.enabled = true;
    }

    public void ClearError()
    {
        errorText.text = "";
        errorText.enabled = false;
    }




    public static void ThrowException(Exception exception)
    {
        throw exception;
    }
    public static void LogException(Exception exception)
    {
#if UNITY_EDITOR
        Debug.LogError(exception.InnerException);
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
