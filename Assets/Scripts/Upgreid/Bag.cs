using UnityEngine;
using TMPro;

public class Bag : MonoBehaviour
{
    public FuelingStation fuelingStation; // Reference to the FuelingStation
    public GameObject Stantion1botum; // Reference to the UI button or game object
    public TMP_Text currentFuelText; // UI text to display current maxFuel
    public TMP_Text futureFuelText; // UI text to display future maxFuel
    public TMP_Text futureCostText; // UI text to display future cost
    private MoneyManager moneyManager; // Reference to the MoneyManager
    public ParticleSystem clickParticles; 
    private bool bloc;

    private int cost; // Cost to upgrade

    private void Start()
    {
        bloc = true;
        moneyManager = MoneyManager.instance;
        cost = fuelingStation.bagUpgradeCost; // Initialize cost from fuelingStation
        UpdateFuelAndCostText();
    }

    void OnMouseUpAsButton()
    {
        if (moneyManager.GetMoney() >= cost && bloc)
        {
            
            bloc = false;
            
        // Запуск партиклів під час кліку
        if (clickParticles != null)
        {
            clickParticles.Play();
        }
            moneyManager.RemoveMoney(cost);
            fuelingStation.maxFuel *= 1.2f; // Increase maxFuel by 20%
            cost = Mathf.CeilToInt(cost * 1.2f); // Increase cost by 20%
            fuelingStation.bagUpgradeCost = cost; // Update the cost in fuelingStation
            //Stantion1botum.SetActive(false);
            UpdateFuelAndCostText();
            
            bloc = true;
        }
    }

    private void UpdateFuelAndCostText()
    {
        currentFuelText.text = "Current Max Fuel: " + fuelingStation.maxFuel.ToString("F2");
        futureFuelText.text = "Next: " + (fuelingStation.maxFuel * 1.2f).ToString("F2");
        futureCostText.text = "$" + Mathf.CeilToInt(cost * 1.2f).ToString();
    }
}
