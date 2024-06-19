using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public GameObject[] waypoints;
    int currentWP = 0;
    public float speed = 10.0f;
    public float fuel = 100f;
    public float fuelConsumptionRate = 1f;
    public float maxFuel = 100f;

    public enum CarState { Moving, WaitingForFuel, Refueling, Finished }
    public CarState state = CarState.Moving;

    public FuelingStation fuelingStation;
    public GameObject[] trackObjects;
    public List<int> refuelPoints;
    private bool station;
    private NavMeshAgent navAgent;
    public FuelCapacityUI fuelCapacityUI;


    void Start()
    {
        station = false;
        currentWP = FindNearestWaypointIndex();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = speed;



        MoveToWaypoint();
    }

    void Update()
    {

        if (fuel > 0 && state != CarState.WaitingForFuel && state != CarState.Refueling)
        {
            fuel -= fuelConsumptionRate * Time.deltaTime;

            if (fuel <= 0 && state == CarState.Moving)
            {
                FindNearestFuelingStation();
            }
        }
        else if (state != CarState.WaitingForFuel && state != CarState.Refueling)
        { 
            fuel = 0;
        }

        switch (state)
        {
            case CarState.Moving:
                if (navAgent.remainingDistance < 2f)
                {
                    if (waypoints[currentWP].CompareTag("FuelingPoint"))
                    {
                        state = CarState.WaitingForFuel;
                        fuelingStation.StartRefueling(this.gameObject);
                    }
                    else
                    {
                        currentWP++;
                        if (currentWP >= waypoints.Length)
                        {
                            currentWP = 0;
                        }

                        MoveToWaypoint();

                        if (fuel <= 0)
                        {
                            FindNearestFuelingStation();
                        }
                    }
                }
                break;
            case CarState.WaitingForFuel:
                break;
            case CarState.Refueling:
                break;
            case CarState.Finished:
                MoveToNextWaypointAfterFuel();
                break;
        }
    }

    void MoveToWaypoint()
    {
        if (waypoints.Length == 0)
            return;

        navAgent.SetDestination(waypoints[currentWP].transform.position);
    }

    void MoveToNextWaypointAfterFuel()
    {
        FindOriginalWaypoints();
        currentWP = FindNearestWaypointIndex();
        state = CarState.Moving;
        MoveToWaypoint();
    }

    int FindNearestWaypointIndex()
    {
        int nearestIndex = 0;
        float nearestDistance = Mathf.Infinity;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, waypoints[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    public void StartRefueling()
    {
        state = CarState.Refueling;
        navAgent.isStopped = true;
    }

    public void FinishRefueling()
    {
        station = false;
        state = CarState.Finished;
        navAgent.isStopped = false;
    }

    public float GetMaxFuel()
    {
        return maxFuel;
    }

    public float GetCurrentFuel()
    {
        return fuel;
    }

    public void AddFuel(float amount)
    {
        fuel = Mathf.Min(fuel + amount, maxFuel);
    }

    void FindNearestFuelingStation()
    {
        if (refuelPoints.Contains(currentWP))
        {
            FuelingStation nearestStation = null;
            float nearestDistance = Mathf.Infinity;
            FuelingStation nearestStationWithShortestQueue = null;
            int shortestQueue = int.MaxValue;

            foreach (FuelingStation station in FindObjectsOfType<FuelingStation>())
            {
                float distance = Vector3.Distance(transform.position, station.transform.position);

                if (distance <= 25f)
                {
                    int queueLength = station.GetCarQueueLength();

                    if (queueLength == 0 && station.IsCarNotInQueue(this.gameObject) && distance < nearestDistance)
                    {
                        nearestStation = station;
                        nearestDistance = distance;
                    }
                    else if (queueLength == 0 && station.IsCarNotInQueue(this.gameObject) && (queueLength < shortestQueue || (queueLength == shortestQueue && distance < nearestDistance)))
                    {
                        nearestStationWithShortestQueue = station;
                        shortestQueue = queueLength;
                    }
                }
            }

            if (nearestStation != null)
            {
                nearestStation.AddCarToQueue(this.gameObject);
                Transform fuelingPoint = nearestStation.transform.GetChild(0);
                waypoints = new GameObject[] { fuelingPoint.gameObject };
                currentWP = 0;
                fuelingStation = nearestStation;
                MoveToWaypoint();
            }
            else if (nearestStationWithShortestQueue != null)
            {
                nearestStationWithShortestQueue.AddCarToQueue(this.gameObject);
                Transform fuelingPoint = nearestStationWithShortestQueue.transform.GetChild(0);
                waypoints = new GameObject[] { fuelingPoint.gameObject };
                currentWP = 0;
                fuelingStation = nearestStationWithShortestQueue;
                MoveToWaypoint();
            }
        }
    }

    void FindOriginalWaypoints()
    {
        waypoints = new GameObject[trackObjects.Length];
        for (int i = 0; i < trackObjects.Length; i++)
        {
            waypoints[i] = trackObjects[i];
        }
    }
}
