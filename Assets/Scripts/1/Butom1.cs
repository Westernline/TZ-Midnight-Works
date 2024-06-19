using UnityEngine;

public class Butom1 : MonoBehaviour
{
    [SerializeField] private FuelingStation fuelingStation; // Посилання на об'єкт FuelingStation

    void OnMouseUpAsButton()
    {
        //Debug.Log("This is a button, I must have a collider to detect");

        if (fuelingStation != null)
        {
            // Запускаємо корутину заправки станції
            fuelingStation.OnRefuelingStationButtonClicked();
        }
        else
        {
            Debug.LogWarning("FuelingStation reference not set!");
        }
    }
}
