using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UphreidStantion : MonoBehaviour
{
    public GameObject Stantion1;
    public GameObject Stantion1botum;
    public GameObject Stantion1botumTrue;
    private MoneyManager moneyManager; // Reference to the MoneyManager
    public int cost;
    private bool bloc;
    private SaveLoadManager saveLoadManager; // Reference to the SaveLoadManager

    private void Start()
    {
        bloc = true;
        moneyManager = MoneyManager.instance; 
        saveLoadManager = FindObjectOfType<SaveLoadManager>(); // Get the SaveLoadManager instance
    }

    void OnMouseUpAsButton()
    {
        if (moneyManager.GetMoney() >= cost && bloc)
        {
            bloc = false;
            moneyManager.RemoveMoney(cost);
            Stantion1.SetActive(true);

            if (Stantion1botumTrue)
            {
                Stantion1botumTrue.SetActive(true);
            }

            // Запустити корутину для анімації
            StartCoroutine(ScaleOverTime(Stantion1, 0.5f));
        }
        else
        {
            // Debug.LogWarning("Not enough money or station already upgraded!");
        }
    }

    private IEnumerator ScaleOverTime(GameObject target, float duration)
    {
        Vector3 originalScale = target.transform.localScale;
        Vector3 reducedScale = originalScale / 100f;
        float time = 0f;

        // Зменшити об'єкт в 100 разів
        target.transform.localScale = reducedScale;

        // Повернути об'єкт до початкового розміру за 0.5 секунди
        while (time < duration)
        {
            time += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(reducedScale, originalScale, time / duration);
            yield return null;
        }

        target.transform.localScale = originalScale;

        // Деактивація Stantion1botum після завершення анімації
        Stantion1botum.SetActive(false);

        // Save the game after upgrading the station
        if (saveLoadManager != null)
        {
            saveLoadManager.SaveGame();
        }
        else
        {
            Debug.LogWarning("SaveLoadManager reference not set!");
        }
    }
}
