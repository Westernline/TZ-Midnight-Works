using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameUI;
    //public GameObject upgradeMenu;
    public Button saveButton;
    public Button loadButton;
    public Button exitButton; // Reference to the exit button
    public SaveLoadManager saveLoadManager;
    private MoneyManager moneyManager; 
    public TMP_Text moneyText;

    void Start()
    {
        moneyManager = MoneyManager.instance; 
        // Initialize buttons with corresponding methods
        saveButton.onClick.AddListener(saveLoadManager.SaveGame);
        loadButton.onClick.AddListener(saveLoadManager.LoadGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        // Update the money text to reflect the current money value from MoneyManager
        moneyText.text = "$" + moneyManager.GetMoney().ToString();
    }

    public void ExitGame()
    {
       // Debug.Log("Exiting Game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Other methods for menu navigation can be uncommented and implemented as needed

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        gameUI.SetActive(false);
        //upgradeMenu.SetActive(false);
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        gameUI.SetActive(true);
        //upgradeMenu.SetActive(false);
    }

    public void ShowUpgradeMenu()
    {
        mainMenu.SetActive(false);
        gameUI.SetActive(false);
        //upgradeMenu.SetActive(true);
    }
}
