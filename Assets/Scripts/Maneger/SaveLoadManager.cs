using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public FuelingStation[] fuelingStations; // Array of fueling stations
    public string saveFileName = "gameData.json";
    public GameObject[] objectsToSave; // Array of objects whose state needs to be saved

    public void SaveGame()
    {
        GameData data = new GameData();
        data.money = MoneyManager.instance.GetMoney();

        // Save the state of each fueling station
        data.stationsData = new StationData[fuelingStations.Length];
        for (int i = 0; i < fuelingStations.Length; i++)
        {
            StationData stationData = new StationData
            {
                fuelingRate = fuelingStations[i].fuelingRate,
                fuelingRateStantion = fuelingStations[i].fuelingRateStantion,
                maxFuel = fuelingStations[i].maxFuel,
                currentFuel = fuelingStations[i].currentFuel,
                bagUpgradeCost = fuelingStations[i].bagUpgradeCost, // Cost for Bag upgrade
                fastUpgradeCost = fuelingStations[i].fastUpgradeCost // Cost for Fast upgrade
            };
            data.stationsData[i] = stationData;
        }

        // Save the state of each object
        data.objectsState = new bool[objectsToSave.Length];
        for (int i = 0; i < objectsToSave.Length; i++)
        {
            data.objectsState[i] = objectsToSave[i].activeSelf;
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);
        //Debug.Log("Game Saved: " + json);
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/" + saveFileName;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            MoneyManager.instance.RemoveMoney(MoneyManager.instance.GetMoney()); // Remove current money
            MoneyManager.instance.AddMoney(data.money); // Add saved money

            // Restore the state of each fueling station
            for (int i = 0; i < data.stationsData.Length; i++)
            {
                if (i < fuelingStations.Length)
                {
                    fuelingStations[i].fuelingRate = data.stationsData[i].fuelingRate;
                    fuelingStations[i].fuelingRateStantion = data.stationsData[i].fuelingRateStantion;
                    fuelingStations[i].maxFuel = data.stationsData[i].maxFuel;
                    fuelingStations[i].currentFuel = data.stationsData[i].currentFuel;
                    fuelingStations[i].bagUpgradeCost = data.stationsData[i].bagUpgradeCost; // Restore Bag upgrade cost
                    fuelingStations[i].fastUpgradeCost = data.stationsData[i].fastUpgradeCost; // Restore Fast upgrade cost
                }
            }

            // Restore the state of each object
            for (int i = 0; i < data.objectsState.Length; i++)
            {
                if (i < objectsToSave.Length)
                {
                    objectsToSave[i].SetActive(data.objectsState[i]);
                }
            }

            MoneyManager.instance.UpdateMoneyUI();
            Debug.Log("Game Loaded: " + json);
        }
        else
        {
            Debug.Log("No save file found");
        }
    }
}