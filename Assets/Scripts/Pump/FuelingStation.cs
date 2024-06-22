using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FuelingStation : MonoBehaviour
{
    public int bagUpgradeCost; // Add this field to track the cost of Bag upgrades
    public int fastUpgradeCost; // Add this field to track the cost of Fast upgrades
    public float fuelingRate = 10f; // Liters per second
    public float fuelingRateStantion = 1000f; // Liters per second
    public Queue<GameObject> carQueue = new Queue<GameObject>(); // Queue of cars waiting for refueling
    public bool isAutomatic = true; // Whether the refueling is automatic

    public float maxFuel = 1000f; // Maximum amount of fuel
    public float currentFuel = 1000f; // Current amount of fuel
    public int numberOfPumps; // Number of pumps

    private List<WorkerController> workers = new List<WorkerController>(); // List of workers at the station
    public Button moveToCarButton; // Button to move worker to car
    public Button startRefuelButton; // Button to start refueling
    public Button refuelingStationButton; // Button to refuel the station

    private GameObject currentCar;
    private bool moveToCarClicked;
    private bool startRefuelClicked;
    private bool refuelingStationClicked;
    private bool isRefuelingInProgress;

    public GameObject workerPrefab; // Worker prefab
    public GameObject moneyPrefab; // Money prefab
    // Array of points for worker positions
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
        // Add button event listeners
        moveToCarButton.onClick.AddListener(OnMoveToCarButtonClicked);
        startRefuelButton.onClick.AddListener(OnStartRefuelButtonClicked);
        refuelingStationButton.onClick.AddListener(OnRefuelingStationButtonClicked);

        // Initialize workers if the station is not automatic and the worker prefab is provided
        if (!isAutomatic && workerPrefab != null)
        {
            for (int i = 0; i < numberOfPumps; i++)
            {
                if (i < workerSpawnPoints.Length) // Ensure the spawn point exists
                {
                    // Instantiate worker prefab
                    GameObject workerInstance = Instantiate(workerPrefab, workerSpawnPoints[i].position, Quaternion.identity);
                    WorkerController worker = workerInstance.GetComponent<WorkerController>();
                    worker.transform.parent = transform;
                    worker.SetWorkerSpawnPoints(workerSpawnPoints); // Set worker spawn points on each worker
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
            StartCoroutine(FuelCar(car)); // Start the refueling process
        }
    }

    IEnumerator FuelCar(GameObject car)
    {
        if (currentFuel <= 0 && currentFuel == 0)
        {
            currentFuel = 0 ;
            yield return null; // Wait for the next frame, the station has no fuel
        }

        CarController carController = car.GetComponent<CarController>();

        if (!isAutomatic && workers.Count > 0)
        {
            currentCar = car;
            moveToCarButton.gameObject.SetActive(true); // Activate the move to car button

            // Wait for the move to car button click
            yield return new WaitUntil(() => moveToCarClicked);

            // Move the worker to the car
            WorkerController worker = workers[0]; // Choose the first available worker
            worker.MoveToCar(car);
            yield return new WaitUntil(() => Vector3.Distance(worker.transform.position, car.transform.position) <= 3.5f); // Wait until the worker reaches the car

            moveToCarButton.gameObject.SetActive(false); // Deactivate the move to car button

            startRefuelButton.gameObject.SetActive(true); // Activate the start refuel button
            // Wait for the start refuel button click
            yield return new WaitUntil(() => startRefuelClicked);

            startRefuelButton.gameObject.SetActive(false); // Deactivate the start refuel button

            carController.StartRefueling(); // Start refueling the car

            moveToCarClicked = false; // Reset the flag
            startRefuelClicked = false; // Reset the flag
        }
        else if (isAutomatic && workers.Count < 0)
        {
            carController.StartRefueling(); // Start automatic refueling
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
            float fuelToAdd = fuelingRate * Time.deltaTime; // Calculate the amount of fuel to add
            fuelAmount += fuelToAdd; // Accumulate the amount of refueled fuel
            currentFuel -= fuelToAdd; // Decrease the amount of fuel at the station
            carController.AddFuel(fuelToAdd); // Add fuel to the car
            yield return null; // Wait for the next frame
        }

        carController.FinishRefueling(); // Finish refueling the car
        carQueue.Dequeue(); // Remove the car from the queue

        // Create a money prefab after refueling
        CreateMoneyPrefab(fuelAmount);

        if (carQueue.Count > 0)
        {
            StartCoroutine(FuelCar(carQueue.Peek())); // Start refueling the next car in the queue
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
        //Debug.Log("Refueling Station button clicked");
        if (!isRefuelingInProgress)
        {
            //Debug.Log("Starting RefuelStationCoroutine");
            StartCoroutine(RefuelStationCoroutine());
        }
    }

    public int GetCarQueueLength()
    {
        return carQueue.Count; // Return the length of the queue
    }

    public void UpgradeFuelingRate(float rateIncrease)
    {
        fuelingRate += rateIncrease; // Increase the refueling rate
    }

    public void IncreaseMaxFuel(int fuelIncrease)
    {
        maxFuel += fuelIncrease; // Increase the maximum amount of fuel
        currentFuel += fuelIncrease; // Increase the current amount of fuel
    }

    public void AddWorker()
    {
        if (!isAutomatic && workerPrefab != null)
        {
            // Instantiate worker prefab
            GameObject workerInstance = Instantiate(workerPrefab, workerSpawnPoints[0].position, Quaternion.identity);
            WorkerController worker = workerInstance.GetComponent<WorkerController>();
            worker.transform.parent = transform;
            worker.SetWorkerSpawnPoints(workerSpawnPoints); // Set worker spawn points on each worker
            workers.Add(worker);
        }
    }

    public void WorkerReachedCar()
    {
        if (workers.Count > 0)
        {
            WorkerController worker = workers[0]; // Choose the worker who reached the car
            StartCoroutine(WaitAndReturnToStation(worker));
        }
        else
        {
            //Debug.LogWarning("No available workers to refuel the car.");
        }
    }

    private IEnumerator WaitAndReturnToStation(WorkerController worker)
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
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

        // Check if there are enough funds to refill fuel
        int litersToRefill = (int)(maxFuel - currentFuel);
        int costToRefill = litersToRefill * 5; // Refuel cost: 5 coins per liter
        if (costToRefill > MoneyManager.instance.GetMoney())
        {
            //Debug.LogWarning("Not enough money to refill fuel at the station.");
            isRefuelingInProgress = false;
            yield break;
        }

        isRefuelingInProgress = true;

        // Deduct player's money by the refuel cost
        MoneyManager.instance.RemoveMoney(costToRefill);

        // Refill fuel at the station to maximum capacity
        while (currentFuel < maxFuel)
        {
            float fuelToAdd = fuelingRateStantion * Time.deltaTime; // Calculate the amount of fuel to add
            currentFuel = Mathf.Min(currentFuel + fuelToAdd, maxFuel); // Add fuel without exceeding the maximum amount
            UpdateFuelUI(); // Update the UI
            yield return null; // Wait for the next frame
        }

        //Debug.Log($"Station refilled successfully. Current fuel: {currentFuel}");

        isRefuelingInProgress = false;
        refuelingStationClicked = false;
    }

    public void UpdateFuelUI()
    {
        // Update the text or animation showing the current amount of fuel at the station
        // For example, this can be displayed in the player's UI or elsewhere
        //Debug.Log($"Current fuel updated in UI. Current fuel: {currentFuel}");
    }

    public void FuelingRateStantion(int fuelingRate)
    {
        fuelingRateStantion += fuelingRate;
    }
}
