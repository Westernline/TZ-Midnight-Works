using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UphreidStantion : MonoBehaviour
{
    public GameObject Stantion1;
    public GameObject Stantion1botum;
    public GameObject Stantion1botumTrue;
    private MoneyManager moneyManager; // Reference to the MoneyManager
    public int cost;
    private bool bloc;
    private SaveLoadManager saveLoadManager; // Reference to the SaveLoadManager

    private void Start()
    {
        bloc = true;
        moneyManager = MoneyManager.instance; 
        saveLoadManager = FindObjectOfType<SaveLoadManager>(); // Get the SaveLoadManager instance
    }

    void OnMouseUpAsButton()
    {
        if (moneyManager.GetMoney() >= cost && bloc)
        {
            bloc = false;
            moneyManager.RemoveMoney(cost);
            Stantion1.SetActive(true);

            if (Stantion1botumTrue)
            {
                Stantion1botumTrue.SetActive(true);
            }

            Stantion1botum.SetActive(false);

            // Save the game after upgrading the station
            if (saveLoadManager != null)
            {
                saveLoadManager.SaveGame();
            }
            else
            {
                Debug.LogWarning("SaveLoadManager reference not set!");
            }
        }
        else
        {
            // Debug.LogWarning("Not enough money or station already upgraded!");
        }
    }
}
