using System;

[System.Serializable]
public class GameData
{
    public int money;
    public StationData[] stationsData; // Array to store the state of each fueling station
    public bool[] objectsState; // Array to store the active state of each object
}

[System.Serializable]
public class StationData
{
    public float fuelingRate;
    public float fuelingRateStantion;
    public float maxFuel;
    public float currentFuel;
    public int bagUpgradeCost; // Add field for Bag upgrade cost
    public int fastUpgradeCost; // Add field for Fast upgrade cost
}
