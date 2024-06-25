using UnityEngine;
using TMPro;

public class Fast : MonoBehaviour
{
    public FuelingStation fuelingStation; // Reference to the FuelingStation
    public GameObject Stantion1botum; // Reference to the UI button or game object
    public TMP_Text currentRateText; // UI text to display current fuelingRate
    public TMP_Text futureRateText; // UI text to display future fuelingRate
    public TMP_Text futureCostText; // UI text to display future cost
    private MoneyManager moneyManager; // Reference to the MoneyManager
    public ParticleSystem clickParticles; 
    private bool bloc;

    private int cost; // Cost to upgrade

    private void Start()
    {
        bloc = true;
        moneyManager = MoneyManager.instance;
        cost = fuelingStation.fastUpgradeCost; // Initialize cost from fuelingStation
        UpdateRateAndCostText();
    }

    void OnMouseUpAsButton()
    {
        if (moneyManager.GetMoney() >= cost && bloc)
        {
            bloc = false;
            if (clickParticles != null)
        {
            clickParticles.Play();
        }
            moneyManager.RemoveMoney(cost);
            fuelingStation.fuelingRate *= 1.05f; // Increase fuelingRate by 20%
            cost = Mathf.CeilToInt(cost * 1.2f); // Increase cost by 20%
            fuelingStation.fastUpgradeCost = cost; // Update the cost in fuelingStation
            //Stantion1botum.SetActive(false);
            UpdateRateAndCostText();
            bloc = true;
        }
    }

    private void UpdateRateAndCostText()
    {
        currentRateText.text = "Current Fueling: " + fuelingStation.fuelingRate.ToString("F2");
        futureRateText.text = "Next: " + (fuelingStation.fuelingRate * 1.05f).ToString("F2");
        futureCostText.text = "$" + Mathf.CeilToInt(cost * 1.2f).ToString();
    }
}
