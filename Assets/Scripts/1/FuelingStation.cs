using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FuelingStation : MonoBehaviour
{
    public float fuelingRate = 10f; // літрів за секунду
    public float fuelingRateStantion = 1000f; // літрів за секунду
    public Queue<GameObject> carQueue = new Queue<GameObject>(); // Черга автомобілів, що чекають на заправку
    public bool isAutomatic = true; // Чи є заправка автоматичною

    public float maxFuel = 1000f; // Максимальна кількість палива
    public float currentFuel = 1000f; // Поточна кількість палива
    public int numberOfPumps; // Кількість колонок

    private List<WorkerController> workers = new List<WorkerController>(); // Список працівників на заправці
    public Button moveToCarButton; // Кнопка для переміщення працівника до автомобіля
    public Button startRefuelButton; // Кнопка для початку заправки
    public Button refuelingStationButton; // Кнопка для заправки станції 

    private GameObject currentCar;
    private bool moveToCarClicked;
    private bool startRefuelClicked;
    private bool refuelingStationClicked;
    private bool isRefuelingInProgress;

    public GameObject workerPrefab; // Префаб працівника
    public GameObject moneyPrefab; // Префаб грошей
    // Масив точок для позицій працівників
    public Transform[] workerSpawnPoints;
    public Transform[] moneySpawnPoints;

    public float GetCurrentFuel()
    {
        return currentFuel;
    }

    public float GetMaxFuel()
    {
        return maxFuel;
    }

    void Start()
{
    // Додаємо підписки на події кнопок
    moveToCarButton.onClick.AddListener(OnMoveToCarButtonClicked);
    startRefuelButton.onClick.AddListener(OnStartRefuelButtonClicked);
    refuelingStationButton.onClick.AddListener(OnRefuelingStationButtonClicked);

    // Ініціалізуємо працівників, якщо станція не автоматична і є префаб працівника
    if (!isAutomatic && workerPrefab != null)
    {
        for (int i = 0; i < numberOfPumps; i++)
        {
            if (i < workerSpawnPoints.Length) // Переконуємося, що точка спавну існує
            {
                // Instantiate worker prefab
                GameObject workerInstance = Instantiate(workerPrefab, workerSpawnPoints[i].position, Quaternion.identity);
                WorkerController worker = workerInstance.GetComponent<WorkerController>();
                worker.transform.parent = transform;
                worker.SetWorkerSpawnPoints(workerSpawnPoints); // Set workerSpawnPoints on each worker
                workers.Add(worker);
            }
        }
    }
}


    public void AddCarToQueue(GameObject car)
    {
        carQueue.Enqueue(car);
        //Debug.Log($"Car {car.name} added to the queue. Queue length: {carQueue.Count}");
    }

    public bool IsCarNotInQueue(GameObject car)
    {
        return !carQueue.Contains(car);
    }

    public void StartRefueling(GameObject car)
    {
        if (carQueue.Count == 1)
        {
            StartCoroutine(FuelCar(car)); // Починаємо процес заправки
        }
    }


    IEnumerator FuelCar(GameObject car)
    {
        if (currentFuel <= 0)
        {
           yield return null; // Чекаємо наступного кадру на станції немає палива
        }

        CarController carController = car.GetComponent<CarController>();

        if (!isAutomatic && workers.Count > 0)
        {
            currentCar = car;
            moveToCarButton.gameObject.SetActive(true); // Активуємо кнопку переміщення до автомобіля

            // Очікуємо клік по кнопці moveToCarButton
            yield return new WaitUntil(() => moveToCarClicked);

            // Переміщуємо працівника до автомобіля
            WorkerController worker = workers[0]; // Вибираємо першого доступного працівника
            worker.MoveToCar(car);
            yield return new WaitUntil(() => Vector3.Distance(worker.transform.position, car.transform.position) <= 3.5f); // Чекаємо, поки працівник дійде до автомобіля

            moveToCarButton.gameObject.SetActive(false); // Деактивуємо кнопку переміщення до автомобіля

            startRefuelButton.gameObject.SetActive(true); // Активуємо кнопку початку заправки
            // Очікуємо клік по кнопці startRefuelButton
            yield return new WaitUntil(() => startRefuelClicked);

            startRefuelButton.gameObject.SetActive(false); // Деактивуємо кнопку початку заправки

            carController.StartRefueling(); // Починаємо заправку автомобіля

            moveToCarClicked = false; // Reset the flag
            startRefuelClicked = false; // Reset the flag
        }
        else if (isAutomatic && workers.Count < 0)
        {
            carController.StartRefueling(); // Починаємо автоматичну заправку
        }
        else 
        {
           yield return null;
        }

        float fuelAmount = 0f;
        float carMaxFuel = carController.GetMaxFuel();
        float carCurrentFuel = carController.GetCurrentFuel();
        float fuelNeeded = carMaxFuel - carCurrentFuel;

        while (fuelAmount < fuelNeeded && currentFuel > 0)
        {
            float fuelToAdd = fuelingRate * Time.deltaTime; // Обчислюємо кількість палива для додавання
            fuelAmount += fuelToAdd; // Накопичуємо кількість заправленого палива
            currentFuel -= fuelToAdd; // Зменшуємо кількість палива на станції
            carController.AddFuel(fuelToAdd); // Додаємо паливо до автомобіля
            yield return null; // Чекаємо наступного кадру
        }

        carController.FinishRefueling(); // Завершуємо заправку автомобіля
        carQueue.Dequeue(); // Видаляємо автомобіль з черги

        // Створюємо префаб грошей після завершення заправки
        CreateMoneyPrefab(fuelAmount);

        if (carQueue.Count > 0)
        {
            StartCoroutine(FuelCar(carQueue.Peek())); // Починаємо заправку наступного автомобіля у черзі
        }
    }

    void OnMoveToCarButtonClicked()
    {
        //Debug.Log("Move to Car button clicked");
        moveToCarClicked = true;
    }

    void OnStartRefuelButtonClicked()
    {
        //Debug.Log("Start Refuel button clicked");
        startRefuelClicked = true;
    }

    public void OnRefuelingStationButtonClicked()
    {
       // Debug.Log("Refueling Station button clicked");
        if (!isRefuelingInProgress)
        {
           // Debug.Log("Starting RefuelStationCoroutine");
            StartCoroutine(RefuelStationCoroutine());
        }
    }
    public int GetCarQueueLength()
    {
        return carQueue.Count; // Повертаємо довжину черги
    }

    public void UpgradeFuelingRate(float rateIncrease)
    {
        fuelingRate += rateIncrease; // Збільшуємо швидкість заправки
    }

    public void IncreaseMaxFuel(int fuelIncrease)
    {
        maxFuel += fuelIncrease; // Збільшуємо максимальну кількість палива
        currentFuel += fuelIncrease; // Збільшуємо поточну кількість палива
    }

    public void AddWorker()
    {
        if (!isAutomatic && workerPrefab != null)
        {
            // Instantiate worker prefab
                GameObject workerInstance = Instantiate(workerPrefab, workerSpawnPoints[0].position, Quaternion.identity);
                WorkerController worker = workerInstance.GetComponent<WorkerController>();
                worker.transform.parent = transform;
                worker.SetWorkerSpawnPoints(workerSpawnPoints); // Set workerSpawnPoints on each worker
                workers.Add(worker);
        }
    }

    public void WorkerReachedCar()
    {
        if (workers.Count > 0)
        {
            WorkerController worker = workers[0]; // Вибираємо працівника, який дійшов до автомобіля
            StartCoroutine(WaitAndReturnToStation(worker));
        }
        else
        {
            //Debug.LogWarning("Немає доступних працівників для заправки автомобіля.");
        }
    }

    private IEnumerator WaitAndReturnToStation(WorkerController worker)
    {
        yield return new WaitForSeconds(2f); // Чекаємо 2 секунди
        worker.ReturnToStation();
    }

    private void CreateMoneyPrefab(float fuelAmount)
    {
        if (moneyPrefab != null && moneySpawnPoints.Length > 0)
        {
            int spawnPointIndex = Random.Range(0, moneySpawnPoints.Length);
            Vector3 spawnPosition = moneySpawnPoints[spawnPointIndex].position;
            GameObject moneyInstance = Instantiate(moneyPrefab, spawnPosition, Quaternion.identity);
            Money moneyScript = moneyInstance.GetComponent<Money>();
            moneyScript.SetFuelAmount(fuelAmount);
        }
    }

    private IEnumerator RefuelStationCoroutine()
    {
        //Debug.Log("RefuelStationCoroutine started");
        refuelingStationClicked = true;

        // Перевірка наявності коштів для поповнення палива
        int litersToRefill = (int)(maxFuel - currentFuel);
        int costToRefill = litersToRefill * 5; // Вартість поповнення: 5 монет за літр
        if (costToRefill > MoneyManager.instance.GetMoney())
        {
            //Debug.LogWarning("Недостатньо грошей для поповнення палива на станції.");
            isRefuelingInProgress = false;
            yield break;
        }

        isRefuelingInProgress = true;

        // Зменшення грошей гравця на вартість поповнення палива
        MoneyManager.instance.RemoveMoney(costToRefill);

        // Поповнення палива на станції до максимальної ємності
        while (currentFuel < maxFuel)
        {
            float fuelToAdd = fuelingRateStantion * Time.deltaTime; // Обчислюємо кількість палива для додавання
            currentFuel = Mathf.Min(currentFuel + fuelToAdd, maxFuel); // Додаємо паливо, не перевищуючи максимальну кількість
            UpdateFuelUI(); // Оновлюємо інтерфейс
            yield return null; // Чекаємо наступного кадру
        }

        //Debug.Log($"Station refilled successfully. Current fuel: {currentFuel}");

        isRefuelingInProgress = false;
        refuelingStationClicked = false;
    }

    public void UpdateFuelUI()
    {
        // Оновлення тексту або анімація, що показує поточну кількість палива на станції
        // Наприклад, можна відобразити це на інтерфейсі гравця або деінде
        //Debug.Log($"Current fuel updated in UI. Current fuel: {currentFuel}");
    }
    public void FuelingRateStantion (int fuelingRate )
    {
        fuelingRateStantion += fuelingRate ;
    }


}
