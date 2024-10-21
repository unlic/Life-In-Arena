using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button soloPlay;  
    
    void Start()
    {
        soloPlay.onClick.AddListener(OpenGameScene);
    }


    private void OpenGameScene()
    {
        SceneManager.LoadScene("Game");
    }
}
