using UnityEngine;
using TMPro;
using System.Collections;

public class Butom1 : MonoBehaviour
{
    [SerializeField] private FuelingStation fuelingStation; // Reference to the FuelingStation object
    [SerializeField] private TMP_Text costText; // Reference to the TextMeshPro text component to display cost

    void Start()
    {
        if (costText != null)
        {
            UpdateCostText(); // Initialize the cost text
            StartCoroutine(UpdateCostTextSec()); // Start the coroutine to update cost text periodically
        }
    }

    void OnMouseUpAsButton()
    {
        if (fuelingStation != null)
        {
            fuelingStation.OnRefuelingStationButtonClicked();
            UpdateCostText(); // Update the cost text after refueling
        }
        else
        {
            Debug.LogWarning("FuelingStation reference not set!");
        }
    }

    private void UpdateCostText()
    {
        if (fuelingStation != null && costText != null)
        {
            int litersToRefill = (int)(fuelingStation.GetMaxFuel() - fuelingStation.GetCurrentFuel());
            int costToRefill = litersToRefill * 5; // Assuming the cost is 5 money per liter
            costText.text = "$" + costToRefill.ToString();
        }
    }

    private IEnumerator UpdateCostTextSec()
    {
        while (true)
        {
            UpdateCostText(); // Update the cost text
            yield return new WaitForSeconds(1f); // Wait for 1 second before updating again
        }
    }
}
