using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBasedMovementManager : MonoBehaviour
{
    [field: SerializeField] public float globalMovementAmount { get; private set; } = 1f;
    public static TileBasedMovementManager Instance = null;

    internal Vector3 OneTileUp = new Vector3(0f,1f,0f);
    internal Vector3 OneTileDown = new Vector3(0f,-1f,0f);
    internal Vector3 OneTileLeft = new Vector3(-1f,0f,0f);
    internal Vector3 OneTileRight = new Vector3(1f,0f,0f);

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

}
