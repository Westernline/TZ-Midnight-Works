using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerUpgr : MonoBehaviour
{
    public FuelingStation fuelingStations;
    public GameObject Stantion1botum;
    private MoneyManager moneyManager; 
    private bool bloc ;

    public int cost ;

    private void Start()
    {
        bloc = true;
        moneyManager = MoneyManager.instance; 
    }


    void OnMouseUpAsButton()
    {
        if (moneyManager.GetMoney() >= cost && bloc)
            {
                bloc = false;
                moneyManager.RemoveMoney(cost);
                fuelingStations.AddWorker();
                Stantion1botum.SetActive(false);
            }
    }
}
