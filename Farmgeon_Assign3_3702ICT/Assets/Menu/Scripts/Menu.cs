using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
   
    public Button startButton;
    public Button creditButton;
    public Button exitButton;

    void Start()
    {
        
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonPressed);
        }
        if (creditButton != null)
        {
            creditButton.onClick.AddListener(OnCreditButtonPressed);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonPressed);
        }
    }

    void OnStartButtonPressed()
    {
        
        SceneManager.LoadScene("Farm");
    }
        void OnCreditButtonPressed(){
            SceneManager.LoadScene("Credits");
        }

        void OnExitButtonPressed()
    {
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(); // Quit 
        #endif
    }
}
