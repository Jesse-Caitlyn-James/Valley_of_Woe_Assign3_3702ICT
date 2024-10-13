using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    //objects in heigrachy
    public Button startButton;
    public Button creditButton;
    public Button instructionButton;
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
        if (instructionButton != null)
        {
            instructionButton.onClick.AddListener(OnInstructionButtonPressed);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonPressed);
        }
    }

    public void OnStartButtonPressed()
    {
        
        SceneManager.LoadScene("Farm");
    }

    public void OnInstructionButtonPressed()
    {
        
        SceneManager.LoadScene("Instructions");
    }

    public void OnCreditButtonPressed(){
        SceneManager.LoadScene("Credits");
    }

    public void OnExitButtonPressed()
    {
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(); // Quit 
        #endif
    }
}
