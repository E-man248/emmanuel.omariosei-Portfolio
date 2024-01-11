
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SuspicionMeter : MonoBehaviour
{
    [field: SerializeField] public int currnetSuspicionAmount { get; private set; }
    [field: SerializeField] public int suspicionCap { get; private set; }
    [field: SerializeField] public int suspicionStartingValue { get; private set; }

    public static SuspicionMeter instance { get; private set; }


    //will handle events for each stage of suspicion level.
    public UnityEvent<int> suspicionMeterChangeEvent;

    // Start is called before the first frame update
    void Awake()
    {
        //singleton
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        if(suspicionMeterChangeEvent == null)
        {
            suspicionMeterChangeEvent = new UnityEvent<int>();
        }
    }

    public void Start()
    {
        //Debug.Log("resting sus amount");
        GameStateManger.Instance.GameOver.AddListener(endGame);
        resetSuspicionAmount();
    }
    public void addSuspicionAmount(int addAmount)
    {
        currnetSuspicionAmount += addAmount;
        if(currnetSuspicionAmount > suspicionCap)
        {
            currnetSuspicionAmount = suspicionCap;
        }
        suspicionMeterChangeEvent.Invoke(currnetSuspicionAmount);
    }

    public void removeSuspicionAmount(int removeAmount)
    {
        currnetSuspicionAmount -= removeAmount;
        if (currnetSuspicionAmount > suspicionCap)
        {
            resetSuspicionAmount();
        }
        suspicionMeterChangeEvent.Invoke(currnetSuspicionAmount);
    }

    public void setSuspicionAmount(int setAmount)
    {
        if (currnetSuspicionAmount != setAmount)
        {
            currnetSuspicionAmount = setAmount;
            suspicionMeterChangeEvent.Invoke(currnetSuspicionAmount);
        }
    }

    public void resetSuspicionAmount()
    {
        currnetSuspicionAmount = suspicionStartingValue;
        suspicionMeterChangeEvent.Invoke(currnetSuspicionAmount);
    }

    public void endGame()
    {
        // stuff that needs to happen when gameover
        SceneManager.LoadScene("Main Menu");
    }
}
