using UnityEngine;

public class FuelTank : MonoBehaviour
{
    public int maxFuel = 1000; // Початкова максимальна кількість палива
    public int currentFuel = 1000; // Поточна кількість палива
    public float refillRate = 100f; // Швидкість поповнення палива

    public void IncreaseMaxFuel(int fuelIncrease)
    {
        maxFuel += fuelIncrease;
        currentFuel += fuelIncrease;
    }

    void Update()
    {
        if (currentFuel < maxFuel)
        {
            currentFuel = Mathf.Min(currentFuel + Mathf.FloorToInt(refillRate * Time.deltaTime), maxFuel);
        }
    }
}
