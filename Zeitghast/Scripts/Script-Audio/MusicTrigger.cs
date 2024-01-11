using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private bool stopMusicOnExit = true;
    private bool isPlaying;

    [EventRef]
    public string MusicSound = "";
    private FMOD.Studio.EventInstance musicInstance;

    protected void OnEnable()
    {
        Timer.gamePausedEvent += onGamePaused;
        Timer.gameUnpausedEvent += onGameUnPaused;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    protected void OnDisable()
    {
        Timer.gamePausedEvent -= onGamePaused;
        Timer.gameUnpausedEvent -= onGameUnPaused;
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    protected void SceneLoaded()
    {
        if(isPlaying)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }
    }

    protected void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isPlaying)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(MusicSound  != "")
        {
            musicInstance = RuntimeManager.CreateInstance(MusicSound);
        }
        Timer.gamePausedEvent += onGamePaused;
        Timer.gameUnpausedEvent += onGameUnPaused;

        SceneManager.sceneLoaded += SceneLoaded;
        SceneLoaded();
    }

    // Update is called once per frame
    void Update()
    {
        musicInstance.setPaused(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            musicInstance.start();
            isPlaying = true;
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && stopMusicOnExit)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }
    }

    private void onGamePaused()
    {

    }

    private void onGameUnPaused()
    {

    }
}
