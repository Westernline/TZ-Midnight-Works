using System.Collections;
using UnityEngine;

public class PositionRotationChecker : MonoBehaviour
{
    public Transform targetObject; // Об'єкт, позицію і ротацію якого ми будемо перевіряти
    public Transform destinationObject; // Об'єкт, на який ми будемо присвоювати позицію і ротацію

    void Start()
    {
        StartCoroutine(CheckPositionRotationRepeatedly(1f)); // Почати перевірку кожну секунду
    }

    IEnumerator CheckPositionRotationRepeatedly(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            if (targetObject != null && destinationObject != null)
            {
                // Присвоюємо позицію destinationObject з targetObject з інверсією по осі y
                Vector3 invertedPosition = targetObject.position;
                invertedPosition.y = -invertedPosition.y + 22.16f; // Інверсія по осі y

                destinationObject.position = invertedPosition;
                destinationObject.rotation = targetObject.rotation; // Копіюємо ротацію без змін
            }
            else
            {
                Debug.LogWarning("Target or Destination object is not assigned.");
            }
        }
    }
}
