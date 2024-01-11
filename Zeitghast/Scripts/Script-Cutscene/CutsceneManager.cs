using System.Collections;
using System.Collections.Generic;
using Tymski;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene State")]
    public List<CutscenePage> pages;
    [SerializeField] protected int currentPageIndex;
    private CutscenePage currentPage;
    internal bool playing;
    internal bool finished;

    [Header("Exit Scene Settings")]
    [SerializeField] private SceneReference exitScene;
    [SerializeField] private TransitionType transitionIn;
    [SerializeField] private TransitionType transitionOut;

    protected void Start()
    {
        Timer.PauseGame();

        resetCutscene();

        Timer.UnpauseGame();
    }

    protected void Update()
    {
        // Update "currentPage" Object when it is changed:
        syncCurrentPageWithIndex();

        if (currentPage != null && currentPage.finished)
        {
            currentPage.gameObject.SetActive(false);
            startNextPage();
        }

        // Recieve and Handle Player Input:
        handlePlayerInput();
    }

    private void startNextPage()
    {
        // If index is greater than the end of list then stop
        if (currentPageIndex == pages.Count - 1 && currentPage.finished)
        {
            Invoke("skipCutscene", currentPage.getAnimationLength("Cutscene_Page_Finished"));
            currentPageIndex++;
            return;
        }

        if (currentPageIndex > pages.Count - 1)
        {
            skipCutscene();
            return;
        }

        // If index is less than the start of list set to start
        if (currentPageIndex < 0)
        {
            currentPageIndex = -1;
        }

        // If animation playing is not the first animation,
        // then skip the current page so the next can start
        if (currentPageIndex > 0)
        {
            pages[currentPageIndex].skipAllPanels();
        }

        // Increment to next page and sync
        currentPageIndex++;
        syncCurrentPageWithIndex();
        
        // Start the first panel of the new current page
        if (currentPage != null)
        {
            currentPage.gameObject.SetActive(true);
            continueCurrentPage();
        }
        
        playing = true;
        finished = false;
    }

    private void handlePlayerInput()
    {
        //Old input System: Skips the cutscene
        if (Input.GetButtonDown("Cancel"))
        {
            skipCutscene();
        }
        if (Input.GetMouseButtonDown(0))
        {
            continueCurrentPage();
        }
        else if (Input.anyKeyDown)
        {
            continueCurrentPage();
        }
    }

    public void continueCurrentPage()
    {
        if (currentPage == null) return;

        if (!currentPage.finished)
        {
            currentPage.startNextPanel();
            
            playing = true;
            finished = false;
        }
    }

    public void startNextPanel()
    {
        if (currentPage == null) return;

        if (!currentPage.finished)
        {
            currentPage.startNextPanel();
            
            playing = true;
            finished = false;
        }
    }

    public void skipCutscene()
    {
        Timer.PauseGame();
        
        AdvancedSceneManager.Instance.loadScene(exitScene.name, transitionIn, transitionOut);

        playing = false;
        finished = true;
    }

    public void resetCutscene()
    {
        currentPageIndex = -1;
        currentPage = null;

        resetPages();

        playing = false;
        finished = false;

        startNextPage();
    }

    private void resetPages()
    {
        foreach(CutscenePage page in pages)
        {
            page.resetPage();
            page.gameObject.SetActive(false);
        }
    }

    private void syncCurrentPageWithIndex()
    {
        if (currentPageIndex > pages.Count - 1 || currentPageIndex < 0)
        {
            currentPage = null;
        }
        else
        {
            currentPage = pages[currentPageIndex];
        }
    }

    private IEnumerator disablePage(CutscenePage page, float time)
    {
        yield return new WaitForSeconds(time);

        page.gameObject.SetActive(false);
    }
}
