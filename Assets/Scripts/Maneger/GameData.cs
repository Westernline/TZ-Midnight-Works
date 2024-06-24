using System;

[System.Serializable]
public class GameData
{
    public int money;
    public int refueledCarCount;
    public int targetRefueledCars; 
    public StationData[] stationsData;
    public bool[] objectsState;
}

[System.Serializable]
public class StationData
{
    public float fuelingRate;
    public float fuelingRateStantion;
    public float maxFuel;
    public float currentFuel;
    public int bagUpgradeCost; 
    public int fastUpgradeCost; 
    public bool isAutomatic;
}

