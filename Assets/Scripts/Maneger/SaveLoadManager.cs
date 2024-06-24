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
        data.refueledCarCount = MoneyManager.instance.GetRefueledCarCount();
        data.targetRefueledCars = MoneyManager.instance.GetTargetRefueledCars(); // Зберегти ціль

        data.stationsData = new StationData[fuelingStations.Length];
        for (int i = 0; i < fuelingStations.Length; i++)
        {
            StationData stationData = new StationData
            {
                fuelingRate = fuelingStations[i].fuelingRate,
                fuelingRateStantion = fuelingStations[i].fuelingRateStantion,
                maxFuel = fuelingStations[i].maxFuel,
                currentFuel = fuelingStations[i].currentFuel,
                bagUpgradeCost = fuelingStations[i].bagUpgradeCost,
                fastUpgradeCost = fuelingStations[i].fastUpgradeCost,
                isAutomatic = fuelingStations[i].isAutomatic // Save automatic state
            };
            data.stationsData[i] = stationData;
        }

        data.objectsState = new bool[objectsToSave.Length];
        for (int i = 0; i < objectsToSave.Length; i++)
        {
            data.objectsState[i] = objectsToSave[i].activeSelf;
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/" + saveFileName;
    if (File.Exists(path))
    {
        string json = File.ReadAllText(path);
        GameData data = JsonUtility.FromJson<GameData>(json);
        MoneyManager.instance.RemoveMoney(MoneyManager.instance.GetMoney());
        MoneyManager.instance.AddMoney(data.money);
        MoneyManager.instance.SetRefueledCarCount(data.refueledCarCount);
        MoneyManager.instance.SetTargetRefueledCars(data.targetRefueledCars); // Завантажити ціль

            for (int i = 0; i < data.stationsData.Length; i++)
            {
                if (i < fuelingStations.Length)
                {
                    fuelingStations[i].fuelingRate = data.stationsData[i].fuelingRate;
                    fuelingStations[i].fuelingRateStantion = data.stationsData[i].fuelingRateStantion;
                    fuelingStations[i].maxFuel = data.stationsData[i].maxFuel;
                    fuelingStations[i].currentFuel = data.stationsData[i].currentFuel;
                    fuelingStations[i].bagUpgradeCost = data.stationsData[i].bagUpgradeCost;
                    fuelingStations[i].fastUpgradeCost = data.stationsData[i].fastUpgradeCost;
                    fuelingStations[i].isAutomatic = data.stationsData[i].isAutomatic; // Restore automatic state

                    if (fuelingStations[i].isAutomatic)
                    {
                        fuelingStations[i].RemoveAllWorkersAndSetAutomatic(); // Set station to automatic mode if needed
                    }
                }
            }

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
