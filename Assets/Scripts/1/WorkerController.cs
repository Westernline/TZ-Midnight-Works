using UnityEngine;
using System.Collections;

public class WorkerController : MonoBehaviour
{
    public GameObject currentCar;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    public float moveSpeed = 1f;
    public float stopDistance = 2f;
    public float rotationSpeed = 3f;

    public Transform[] workerSpawnPoints; // Масив точок для позицій працівників

    void Start()
    {
        initialRotation = transform.rotation;
        targetRotation = initialRotation * Quaternion.Euler(0f, -60f, 0f);
    }

    public void SetWorkerSpawnPoints(Transform[] spawnPoints)
    {
        workerSpawnPoints = spawnPoints;
    }

    public void MoveToCar(GameObject car)
    {
        if (car == null)
        {
            Debug.LogWarning("Car object is null in MoveToCar.");
            return;
        }

        currentCar = car;
        StartCoroutine(MoveTowardsCar());
    }

    IEnumerator MoveTowardsCar()
    {
        while (currentCar != null && Vector3.Distance(transform.position, currentCar.transform.position) > stopDistance)
        {
            Vector3 moveDirection = currentCar.transform.position - transform.position;
            moveDirection.y = 0f;

            transform.position = Vector3.MoveTowards(transform.position, currentCar.transform.position, moveSpeed * Time.deltaTime);

            if (moveDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }

            yield return null;
        }

        if (currentCar != null)
        {
            FuelingStation fuelingStation = GetComponentInParent<FuelingStation>();
            if (fuelingStation != null)
            {
                fuelingStation.WorkerReachedCar();
            }
            else
            {
                Debug.LogWarning("Fueling station not found in parent objects.");
            }
        }
    }

    public void ReturnToStation()
    {
        StartCoroutine(MoveTowardsStation());
    }

    IEnumerator MoveTowardsStation()
    {
        // Ensure there is at least one worker spawn point defined
        if (workerSpawnPoints.Length == 0)
        {
            Debug.LogWarning("Worker spawn points are not defined.");
            yield break;
        }

        // Move towards the first worker spawn point
        Vector3 targetPosition = workerSpawnPoints[0].position; // Визначаємо цільову позицію для повернення

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 moveDirection = targetPosition - transform.position;
            moveDirection.y = 0f;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (moveDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }

            yield return null;
        }

        // Rotate towards initial rotation
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset current car reference
        currentCar = null;
    }
}
