using UnityEngine;
using System;
using System.Collections.Generic;

// public enum TimerType
// {
//     Training = 5,
//     Fast = 2,
//     Normal = 3,
//     Long = 4,
//     Master = 6
// }
public enum TimerType
{
    Training = 5,
    Fast = 1800,
    Normal = 7200,
    Long = 14400,
    Master = 28800
}

public class Demo : MonoBehaviour
{
    [SerializeField] Timer timer1;
    [SerializeField] Timer timer2;
    [SerializeField] Timer timer3;
    [SerializeField] Timer timer4;
    private Timer[] allTimers;

    private const string exitTimeKey = "ExitTime";
    private const string timerTicksListKey = "TimerTicksList";

    public List<int> timerTicksList;
    private void Awake()
    {
        // Ініціалізуємо масив у методі Awake
        allTimers = new Timer[] { timer1, timer2, timer3, timer4 };
        
    }

    private void Start()
{
    // Отримання поточного часу
    DateTime currentTime = WorldTimeAPI.Instance.GetCurrentDateTime();
    
    // Відображення поточного часу у форматі строки
    Debug.Log("Current time: " + currentTime);
    
    // Підписка на подію для кожного таймера
    foreach (Timer timer in allTimers)
    {
        timer.OnTimerEnd += HandleTimerEnd;
    }

    // Відновлення стану паузи після перезапуску гри
    timer1.SetPaused(PlayerPrefs.GetInt("Timer1_Paused", 0) == 1);
    timer2.SetPaused(PlayerPrefs.GetInt("Timer2_Paused", 0) == 1);
    timer3.SetPaused(PlayerPrefs.GetInt("Timer3_Paused", 0) == 1);
    timer4.SetPaused(PlayerPrefs.GetInt("Timer4_Paused", 0) == 1);
    
    // Відновлення збережених даних для кожного таймера
    timer1.SetDuration(PlayerPrefs.GetInt("Timer1_Duration"));
    timer2.SetDuration(PlayerPrefs.GetInt("Timer2_Duration"));
    timer3.SetDuration(PlayerPrefs.GetInt("Timer3_Duration"));
    timer4.SetDuration(PlayerPrefs.GetInt("Timer4_Duration"));

    // Отримання часу виходу з гри та розрахунок пройденого часу
    if (PlayerPrefs.HasKey(exitTimeKey))
    {
        DateTime savedExitTime;
        string exitTimeStr = PlayerPrefs.GetString(exitTimeKey);
        Debug.Log("Exit time string: " + exitTimeStr);
        if (long.TryParse(exitTimeStr, out long exitTicks))
        {
            savedExitTime = DateTime.FromBinary(exitTicks); // Конвертуємо збережений тік у DateTime
            Debug.Log("Saved exit time: " + savedExitTime);
        }
        else
        {
            // Обробка помилки
            Debug.LogError("Failed to parse exit time from PlayerPrefs.");
            return;
        }
        TimeSpan elapsedTime = WorldTimeAPI.Instance.GetCurrentDateTime() - savedExitTime;
        Debug.Log("Elapsed time: " + elapsedTime);
        

        // Враховування пройденого часу тільки для таймерів, які не на паузі
        if (!timer1.IsPaused)
            timer1.SetDuration(timer1.RemainingDuration - (int)elapsedTime.TotalSeconds);
        if (!timer2.IsPaused)
            timer2.SetDuration(timer2.RemainingDuration - (int)elapsedTime.TotalSeconds);
        if (!timer3.IsPaused)
            timer3.SetDuration(timer3.RemainingDuration - (int)elapsedTime.TotalSeconds);
        if (!timer4.IsPaused)
            timer4.SetDuration(timer4.RemainingDuration - (int)elapsedTime.TotalSeconds);
    }

    // Початок таймерів
    timer1.Begin();
    timer2.Begin();
    timer3.Begin();
    timer4.Begin();
    UpdateTimerUI();
    
}


    private void HandleTimerEnd(Timer timer)
    {
        // Перевірка, чи завершився таймер
        if (timer.RemainingDuration <= 0)
        {
            // Надання нового часу для таймера
            StartTimerWithRandomTypeAndBegin(timer);
            // Поставити таймер на паузу
            timer.SetPaused(true);
        }
    }


    private void OnApplicationQuit()
    {
        // Враховуємо стан паузи при збереженні даних перед вимкненням програми
        PlayerPrefs.SetInt("Timer1_Paused", timer1.IsPaused ? 1 : 0);
        PlayerPrefs.SetInt("Timer2_Paused", timer2.IsPaused ? 1 : 0);
        PlayerPrefs.SetInt("Timer3_Paused", timer3.IsPaused ? 1 : 0);
        PlayerPrefs.SetInt("Timer4_Paused", timer4.IsPaused ? 1 : 0);

        // Збереження часу виходу з гри
        DateTime exitTime = WorldTimeAPI.Instance.GetCurrentDateTime();
        Debug.Log("Saving exit time: " + exitTime);
        PlayerPrefs.SetString(exitTimeKey, exitTime.Ticks.ToString());

        // Збереження даних про час для кожного таймера перед вимкненням програми
        PlayerPrefs.SetInt("Timer1_Duration", timer1.RemainingDuration);
        PlayerPrefs.SetInt("Timer2_Duration", timer2.RemainingDuration);
        PlayerPrefs.SetInt("Timer3_Duration", timer3.RemainingDuration);
        PlayerPrefs.SetInt("Timer4_Duration", timer4.RemainingDuration);

        PlayerPrefs.Save();
    }


    private void UpdateTimerUI()
    {
        UpdateTimerUIText(timer1);
        UpdateTimerUIText(timer2);
        UpdateTimerUIText(timer3);
        UpdateTimerUIText(timer4);
    }

    private void UpdateTimerUIText(Timer timer)
    {
        int seconds = timer.RemainingDuration;
        timer.UpdateUI(seconds);
    }

    public void IsPausedOnTimerButtonClicked(Timer timer)
    {
        // Встановлення паузи на всіх таймерах, крім вибраного
        foreach (Timer t in allTimers)
        {
            if (t != timer)
            {
                t.SetPaused(true);
            }
        }

        // Встановлення паузи на вибраному таймері
        timer.SetPaused(!timer.IsPaused);
    }

    
    public void StartTimerWithRandomTypeAndBegin(Timer timer)
    {
        // Встановлення паузи на всіх таймерах, крім вибраного
        foreach (Timer t in allTimers)
        {
            if (t != timer)
            {
                t.SetPaused(true);
            }
        }
        if (timer.RemainingDuration <= 0)
        {
            StartTime(timer);
        }
    
        if (timer.IsPaused)
        {
            timer.SetPaused(!timer.IsPaused);
        }
        else if (timer.RemainingDuration <= 0)
        {
            StartTime(timer);
        }
    }

    private void StartTime(Timer timer)
    {
        // Перевірка чи цей таймер вже запускався
        if (!PlayerPrefs.HasKey(timer.name + "_FirstRun"))
        {
            // Якщо таймер ще не запускався, встановити його на Training = 5 і вказати, що він вже був запущений
            timer.SetDuration((int)TimerType.Training);
            timer.timerType = TimerType.Training; // Встановлення TimerType
            timer.Begin();
            timer.SaveTimerType(); // Зберегти тип таймера
            PlayerPrefs.SetInt(timer.name + "_FirstRun", 1); // Запам'ятати, що таймер вже був запущений
            return;
        }

        // Перевірка наявності та завантаження списку таймерів
        if (!PlayerPrefs.HasKey(timerTicksListKey))
        {
            CreateTimerTicksList();
            //Debug.Log("TimerTicksList created and saved");
            SaveTimerTicksList();
        }
        else
        {
            LoadTimerTicksList();
            //Debug.Log("TimerTicksList loaded");
        }

        // Вибір випадкового значення зі списку і запуск таймера
        int randomIndex = UnityEngine.Random.Range(0, timerTicksList.Count);
        int randomTick = timerTicksList[randomIndex];
        timer.SetDuration(randomTick);
        timer.timerType = (TimerType)randomTick; // Встановлення TimerType
        timer.SaveTimerType(); // Зберегти тип таймера
        timerTicksList.RemoveAt(randomIndex);
        SaveTimerTicksList(); // Зберегти оновлений список після вибору випадкового значення
        timer.Begin();

        // Перевірка на порожній список та його перестворення, якщо потрібно
        if (timerTicksList == null || timerTicksList.Count == 0)
        {
            Debug.Log("TimerTicksList is empty, recreating...");
            CreateTimerTicksList();
            SaveTimerTicksList(); // Ви можете опціонально зберегти новий список, якщо він порожній
        }
    }




    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }


    private void CreateTimerTicksList()
    {
        timerTicksList = new List<int>();

        int fastCount = 4;
        int normalCount = 8;
        int longCount = 5;
        int masterCount = 3;

        for (int i = 0; i < fastCount; i++)
        {
            timerTicksList.Add((int)TimerType.Fast);
        }

        for (int i = 0; i < normalCount; i++)
        {
            timerTicksList.Add((int)TimerType.Normal);
        }

        for (int i = 0; i < longCount; i++)
        {
            timerTicksList.Add((int)TimerType.Long);
        }

        for (int i = 0; i < masterCount; i++)
        {
            timerTicksList.Add((int)TimerType.Master);
        }

        ShuffleTimerTicksList();
        SaveTimerTicksList();
    }

    private void ShuffleTimerTicksList()
    {
        for (int i = 0; i < timerTicksList.Count; i++)
        {
            int temp = timerTicksList[i];
            int randomIndex = UnityEngine.Random.Range(i, timerTicksList.Count);
            timerTicksList[i] = timerTicksList[randomIndex];
            timerTicksList[randomIndex] = temp;
        }
    }

    private void SaveTimerTicksList()
    {
        // Convert List<int> to string for saving
        string timerTicksListString = string.Join(",", timerTicksList);
        PlayerPrefs.SetString(timerTicksListKey, timerTicksListString);
    }

    private void LoadTimerTicksList()
    {
        // Load string and convert it back to List<int>
        string timerTicksListString = PlayerPrefs.GetString(timerTicksListKey);
        timerTicksList = new List<int>();

        string[] stringArray = timerTicksListString.Split(',');
        foreach (string str in stringArray)
        {
            int value;
            if (int.TryParse(str, out value))
            {
                timerTicksList.Add(value);
            }
            else
            {
                Debug.LogError("Failed to parse string to int: " + str);
            }
        }
    }

    public void Subtract30SecondsFromTimer(Timer timer)
    {
        timer.SetDuration(timer.RemainingDuration - 30);
    }

    public void OnTimerStopButtonClicked(Timer timer)
    {
        timer.End(timer.timerType);
    }

        
}
