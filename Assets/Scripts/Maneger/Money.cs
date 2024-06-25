using UnityEngine;

public class Money : MonoBehaviour
{
    private float fuelAmount;
    public float rotationSpeed = 30f; // Швидкість обертання
    private bool take ;
    public ParticleSystem clickParticles; 
    
     void Awake ()
    {
        take = true;
    }

    void Update()
    {
        // Обертаємо об'єкт навколо його осі Y
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    public void SetFuelAmount(float amount)
    {
        fuelAmount = amount;
        
    }

    private void ConvertFuelToMoney()
    {
        if(take)
        {
            take=false;
            if (clickParticles != null)
        {
            clickParticles.Play();
        }
            int moneyAmount = Mathf.RoundToInt(fuelAmount * 10); // Наприклад, 10 доларів за 1 літр палива
        MoneyManager.instance.AddMoney(moneyAmount);
        Destroy(gameObject, 0.1f); // Знищуємо об'єкт через 2 секунди після створення

        }
        
    }
    void OnMouseUpAsButton()
    {
        ConvertFuelToMoney();
        //Debug.Log("This is a button, I must have a collider to detect");
    }
}
