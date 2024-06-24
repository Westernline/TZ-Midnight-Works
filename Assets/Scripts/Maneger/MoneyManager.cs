using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager instance;

    private int refueledCarCount = 0; // Кількість заправлених авто
    private int targetRefueledCars = 1; // Поточна ціль заправлених авто
    public TMP_Text moneyText;
    public TMP_Text refueledCarsText; // Текст для відображення кількості заправлених авто та цілі
    private int money = 1000;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncrementRefueledCarCount()
    {
        refueledCarCount++;
        UpdateRefueledCarsUI();

        if (refueledCarCount == 1 )
        {
            targetRefueledCars += 4; // Оновлення цілі
            UpdateRefueledCarsUI();
        }
        else if (refueledCarCount >= targetRefueledCars && refueledCarCount <= 100 )
        {
            targetRefueledCars += 5; // Оновлення цілі
            UpdateRefueledCarsUI();
        }
        else if (refueledCarCount >= targetRefueledCars && refueledCarCount >= 100 && refueledCarCount == 100 && refueledCarCount <= 1000 )
        {
            targetRefueledCars += 100; // Оновлення цілі
            UpdateRefueledCarsUI();
        }
        else if (refueledCarCount >= targetRefueledCars && refueledCarCount >= 1000 && refueledCarCount == 1000  && refueledCarCount <= 10000)
        {
            targetRefueledCars += 1000; // Оновлення цілі
            UpdateRefueledCarsUI();
        }
        else if (refueledCarCount >= targetRefueledCars && refueledCarCount >= 10000  && refueledCarCount == 10000 && refueledCarCount <= 100000)
        {
            targetRefueledCars += 10000; // Оновлення цілі
            UpdateRefueledCarsUI();
        }

    }

    public int GetRefueledCarCount()
    {
        return refueledCarCount;
    }

    public void SetRefueledCarCount(int count)
    {
        refueledCarCount = count;
        UpdateRefueledCarsUI();
    }

    public int GetMoney()
    {
        return money;
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    public void UpdateMoneyUI()
    {
        moneyText.text = "$" + money.ToString();
    }

    private void UpdateRefueledCarsUI()
    {
        refueledCarsText.text = $"{refueledCarCount}/{targetRefueledCars}";
    }

    public int GetTargetRefueledCars()
    {
        return targetRefueledCars;
    }

    public void SetTargetRefueledCars(int target)
    {
        targetRefueledCars = target;
    }
}
