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
}
