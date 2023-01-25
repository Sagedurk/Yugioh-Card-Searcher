using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonManager: MonoBehaviour
{
    public Button loadCardInfoButton;
    public Button loadCardSearchButton;
    public Button loadRandomButton;
    public Button loadArchetypeButton;


    private void Start()
    {
        loadCardInfoButton.onClick.AddListener(delegate { SetButtonLoad(SceneNames.CARD_INFO); });
        loadCardSearchButton.onClick.AddListener(delegate { SetButtonLoad(SceneNames.CARD_SEARCH); });
        loadRandomButton.onClick.AddListener(delegate { SetButtonLoad(SceneNames.RANDOM_CARD); });
        loadArchetypeButton.onClick.AddListener(delegate { SetButtonLoad(SceneNames.ARCHETYPE); });
    }

    public void SetButtonLoad(SceneNames sceneName)
    {
        EUS.Cat_Scene.LoadScene(sceneName, EUS.Cat_Scene.LoadType.SINGLE);
    }

}
