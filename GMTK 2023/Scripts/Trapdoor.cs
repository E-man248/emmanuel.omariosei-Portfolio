using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tymski;
using UnityEngine.SceneManagement;

public class Trapdoor : MonoBehaviour
{
    public SceneReference targetScene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        Princess princess = collision.GetComponent<Princess>();

        if (princess == null) return;

        goTotargetScene();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision == null) return;

        Princess princess = collision.GetComponent<Princess>();

        if (princess == null) return;

        goTotargetScene();
    }

    private void  goTotargetScene()
    {
        SceneManager.LoadSceneAsync(targetScene);
    }
}
