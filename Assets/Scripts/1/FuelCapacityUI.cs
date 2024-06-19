using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FuelCapacityUI : MonoBehaviour
{
    public enum DisplayType
    {
        FuelStation,
        Car
    }

    public DisplayType displayType;

    public TMP_Text currentFuelText; // Використовуємо TMP_Text для відображення тексту
    public Image fuelCapacityRenderer; // Image для відображення прогресу ємності палива

    public FuelingStation fuelingStation; // Посилання на скрипт FuelingStation
    public CarController carController; // Посилання на скрипт CarController

    void Start()
    {
        UpdateUI(); // Оновлюємо відображення ємності палива
    }

    void Update()
    {
        if (displayType == DisplayType.FuelStation && fuelingStation != null)
        {
            if (int.TryParse(currentFuelText.text.Split('/')[0].Trim(), out int currentFuel) && fuelingStation.GetCurrentFuel() != currentFuel)
            {
                UpdateUI();
            }
        }
        else if (displayType == DisplayType.Car && carController != null)
        {
            if (int.TryParse(currentFuelText.text.Split('/')[0].Trim(), out int currentFuel) && carController.GetCurrentFuel() != currentFuel)
            {
                UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        int currentFuel = 0;
        int maxFuel = 1;

        if (displayType == DisplayType.FuelStation && fuelingStation != null)
        {
            currentFuel = Mathf.FloorToInt(fuelingStation.GetCurrentFuel());
            maxFuel = Mathf.FloorToInt(fuelingStation.GetMaxFuel());
        }
        else if (displayType == DisplayType.Car && carController != null)
        {
            currentFuel = Mathf.FloorToInt(carController.GetCurrentFuel());
            maxFuel = Mathf.FloorToInt(carController.GetMaxFuel());
        }

        currentFuelText.text = $"{currentFuel} / {maxFuel}";

        // Обчислюємо прогрес від 0 до 1
        float fillAmount = (float)currentFuel / maxFuel;

        // Переконайтеся, що fuelCapacityRenderer не є null
        if (fuelCapacityRenderer != null)
        {
            // Змінюємо fillAmount Image зліва на право
            fuelCapacityRenderer.fillAmount = fillAmount;
        }
    }
}
