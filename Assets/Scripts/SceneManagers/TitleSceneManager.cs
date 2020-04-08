using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    public Button button_AcceptAndStartPrediction;

    void Start()
    {
        
    }

    void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyUp (KeyCode.Escape)) {
            Application.Quit();
            return;
        }
#endif
    }

    public void LoadCaliculationScene() {
        SceneManager.LoadScene("CaliculationScene");
    }
}
