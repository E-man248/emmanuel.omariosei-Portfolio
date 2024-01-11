using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEventCache : MonoBehaviour
{
    private static float MaxTextEventLifeTime;
    private TMPro.TextMeshProUGUI textMesh;
    private List<TextEvent> eventCache;
    
    //[SerializeField]
    //private int maxCacheSize = 1;

    [SerializeField]
    private float maxTextLifeTime = 1;

    [System.Serializable]
    private class TextEvent
    {
        public string text;
        public float lifeTime;

        public TextEvent(string text, float lifeTime)
        {
            this.text = text;
            this.lifeTime = lifeTime;
        }
    }

    public static TextEventCache instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    void Start()
    {
        textMesh = GetComponent<TMPro.TextMeshProUGUI>();
        eventCache = new List<TextEvent>();
        MaxTextEventLifeTime = maxTextLifeTime;
    }

    void Update()
    {
        textMesh.text = "";
        MaxTextEventLifeTime = maxTextLifeTime;
        for (int i = eventCache.Count-1; i >= 0; i--)
        {
            TextEvent element = eventCache[i];

            textMesh.text += "\n" + eventCache[i].text;
            eventCache[i].lifeTime -= Time.deltaTime;
            if (eventCache[i].lifeTime <= 0)
            {
                removeTextEvent(i);
            }
        }
    }

    public static void annouceTextEvent(string textEvent)
    {
        if (instance == null) return;

        if (MaxTextEventLifeTime != 0 && instance.eventCache.Count == MaxTextEventLifeTime)
        {
            instance.eventCache.RemoveAt(0);
        }
        instance.eventCache.Add( new TextEvent(textEvent, MaxTextEventLifeTime) );
    }

    private void removeTextEvent(int index)
    {
        if (index < eventCache.Count && index >= 0)
        eventCache.RemoveAt(index);
    }

}
