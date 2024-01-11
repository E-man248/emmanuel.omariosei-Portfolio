using UnityEngine;

public class TimeNotificationUI : MonoBehaviour
{
    [Header("Time Notification Settings")]
    [SerializeField] private float notifyTime;
    [SerializeField] private bool notifyOnce = false;
    [SerializeField] private float coolDown = 0;
    private float coolDownTimer = 0;
    private bool hasNotified = false;
    
    
    // Utilities:
    private TimeNotificationAnimationHandler animationHandler;

    private void Awake()
    {
        animationHandler = GetComponentInChildren<TimeNotificationAnimationHandler>();
    }

    private void Update()
    {
        if (Timer.gamePaused) return;

        if (Timer.Instance == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // Must wait for cool down time:
        if (coolDownTimer > 0)
        {
            coolDownTimer -= Time.deltaTime;
            return;
        }
        
        if (isNotifyTime() && !hasNotified)
        {
            hasNotified = true;
            PlayNotification();
            coolDownTimer = coolDown;
            return;
        }

        if (isResetTime() && !notifyOnce)
        {
            ResetNotification();
        }
    }

    private bool isNotifyTime()
    {
        // Current time must be less than or equal to notify time:
        return Timer.Instance.CurrentTime <= notifyTime;
    }

    private bool isResetTime()
    {
        // Current time must be greater than notify time:
        return Timer.Instance.CurrentTime > notifyTime;
    }

    public void PlayNotification()
    {
        animationHandler.PlayTimeNotification();
    }

    public void ResetNotification()
    {
        hasNotified = false;
    }
}
