using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectPrefab;
    private static int poolGrowthSize = 10;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    protected abstract void Awake();
    protected void Start()
    {
        GrowPool();
    }

    protected void GrowPool()
    {
        for (int i = 0; i < poolGrowthSize; i++)
        {
            var instanceToAdd = Instantiate(gameObjectPrefab, transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
