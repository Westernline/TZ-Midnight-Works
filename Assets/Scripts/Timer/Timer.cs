 using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour
{
    [Header("Timer UI references:")]
    [SerializeField] private Image uiFillImage;
    [SerializeField] private Text uiText;
    public TimerType timerType; 

    public TimerType TimerType 
    { 
        get { return timerType; } 
        private set { timerType = value; } 
    }
    public int Duration { get; private set; }
    public bool IsPaused { get; private set; }
    public int RemainingDuration { get; private set; } 

    private UnityAction onTimerBeginAction;
    private UnityAction<int> onTimerChangeAction;
    private UnityAction onTimerEndAction;
    private UnityAction<bool> onTimerPauseAction;

    private bool isPaused; 

    private const string timerTypeKeyPrefix = "TimerType_";
    
    public delegate void TimerEndHandler(Timer timer);
    public event TimerEndHandler OnTimerEnd;

    

    private void Awake()
    {
        LoadTimerType();
        ResetTimer();
        
    }

    private void ResetTimer()
    {
        uiText.text = "00:00:00";
        uiFillImage.fillAmount = 0f;

        Duration = RemainingDuration = 0;

        onTimerBeginAction = null;
        onTimerChangeAction = null;
        onTimerEndAction = null;
        onTimerPauseAction = null;

        IsPaused = isPaused = false;
    }

    private void LoadTimerType()
    {
        if (PlayerPrefs.HasKey(timerTypeKeyPrefix + gameObject.name))
        {
            int typeIndex = PlayerPrefs.GetInt(timerTypeKeyPrefix + gameObject.name);
            timerType = (TimerType)typeIndex;
        }
    }

    public void SaveTimerType()
    {
        PlayerPrefs.SetInt(timerTypeKeyPrefix + gameObject.name, (int)timerType);
        PlayerPrefs.Save();
    }

    public void SetPaused(bool paused)
    {
        
        IsPaused = paused;
        isPaused = paused; 

        if (onTimerPauseAction != null)
            onTimerPauseAction.Invoke(IsPaused);
    }

    public Timer SetDuration(int seconds)
    {
        Duration = RemainingDuration = seconds;
        return this;
    }

    public Timer OnBegin(UnityAction action)
    {
        onTimerBeginAction = action;
        return this;
    }

    public Timer OnChange(UnityAction<int> action)
    {
        onTimerChangeAction = action;
        return this;
    }

    public Timer OnEnd(UnityAction action)
    {
        onTimerEndAction = action;
        return this;
    }

    public Timer OnPause(UnityAction<bool> action)
    {
        onTimerPauseAction = action;
        return this;
    }

    public void Begin()
    {
        if (onTimerBeginAction != null)
            onTimerBeginAction.Invoke();

        StopAllCoroutines();
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (RemainingDuration > 0)
        {
            if (!IsPaused)
            {
                if (onTimerChangeAction != null)
                    onTimerChangeAction.Invoke(RemainingDuration);

                UpdateUI(RemainingDuration);
                RemainingDuration--;
            }
            yield return new WaitForSeconds(1f);
        }
        End(timerType);
    }
    

    public void UpdateUI(int seconds)
    {
        uiText.text = string.Format("{0:D2}:{1:D2}", seconds / 60, seconds % 60);
        uiFillImage.fillAmount = Mathf.InverseLerp(0, Duration, seconds);
    }


    public void End(TimerType timerType)
    {
        if (onTimerEndAction != null)
            onTimerEndAction.Invoke();

        ActivateBonusBasedOnTimerType(timerType);

        // Видалення типу таймера з PlayerPrefs
        PlayerPrefs.DeleteKey(timerTypeKeyPrefix + gameObject.name);
        PlayerPrefs.Save();
        

        ResetTimer();
        Debug.Log("Timer ended");
        EndTimer();
    }
    private void EndTimer()
    {
        if (OnTimerEnd != null)
        {
            OnTimerEnd.Invoke(this);
        }
    }


    private void ActivateBonusBasedOnTimerType(TimerType timerType)
    {
        switch (timerType)
        {
            case TimerType.Training:
                Debug.Log("Training bonus activated");
                break;
            case TimerType.Fast:
                Debug.Log("Fast bonus activated");
                break;
            case TimerType.Normal:
                Debug.Log("Normal bonus activated");
                break;
            case TimerType.Long:
                Debug.Log("Long bonus activated");
                break;
            case TimerType.Master:
                Debug.Log("Master bonus activated");
                break;
            default:
                Debug.LogWarning("Unknown TimerType: " + timerType);
                break;
        }
    }


    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
