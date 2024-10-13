using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 

public class MenuManager : MonoBehaviour
{
   
    public Button startButton;
    public Button menuButton;
    public Button instructionButton;
    public Button creditButton;
    public Button exitButton;

    void Start()
    {
        
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonPressed);
        }
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuButtonPressed);
        }
        if (instructionButton != null)
        {
            instructionButton.onClick.AddListener(OnInstructionButtonPressed);
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

    public void OnStartButtonPressed()
    {
        print("start works");
        SceneManager.LoadScene("Farm");
    }

    public void OnMenuButtonPressed()
    {
        print("menu works");
        SceneManager.LoadScene("Menu");
    }

    public void OnInstructionButtonPressed()
    {
        print("help works");
        SceneManager.LoadScene("Help");
    }

    public void OnCreditButtonPressed()
    {
        print("creditsworks");
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
