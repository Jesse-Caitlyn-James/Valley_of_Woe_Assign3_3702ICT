using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
   

    public Button backButton;

    void Start()
    {
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonPressed);
        }
        
    }

    public void OnBackButtonPressed()
    {
        
        SceneManager.LoadScene("Menu");
    }
   
}
