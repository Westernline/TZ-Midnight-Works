using UnityEngine;

public class AutomaticStantion : MonoBehaviour
{
    public FuelingStation fuelingStation; // Reference to the FuelingStation
    public GameObject Stantion1botum; // Reference to the UI button or game object
    private MoneyManager moneyManager; // Reference to the MoneyManager
    private bool bloc;

    private int cost = 10000; // Cost to upgrade

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
            fuelingStation.RemoveAllWorkersAndSetAutomatic(); // Correctly call the method
            Stantion1botum.SetActive(false);
            bloc = true;
        }
    }


}
