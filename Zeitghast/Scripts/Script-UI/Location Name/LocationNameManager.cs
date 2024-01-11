using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationNameManager : MonoBehaviour
{
    private const float REFERENCE_SCREEN_WIDTH = 1920f;
    private const float REFERENCE_SCREEN_HEIGHT = 1080f;

    [Header("Main text settings")]
    public Color mainTextColor;
    public TMPro.TextMeshProUGUI mainTextUI;

    [Header("Sub text settings")]
    public Color subTextColor;
    public TMPro.TextMeshProUGUI subTextUI;

    public static LocationNameManager Instance = null;

    public float Duration;
    public float FadeInDuration;
    private float FadeInTimer;
    public float FadeOutDuration;
    private float FadeOutTimer;

    private bool fadeIn;
    private bool fadeOut;

    [Header("Icon settings")]
    public Color iconColor;
    public Image locationIconUI;
    public Vector2 iconOffest;

    //private Vector2 defaultMainTextPosition;
    private bool hasIcon;

    private Coroutine keepTextOnScreenCoroutine;
    private Coroutine startFadeOutCoroutine;

    [Header("Pause fade settings")]
    public float pauseFadeDuration;
    private float pauseFadeTimer;
    [Range(0f,1f)]public float pauseFadevalue;
    private Coroutine pauseFadeCoroutine;
    private float originalMainTextAlphaValue;
    private float originalSubTextAlphaValue;


    private void Awake()
    {
        //Singelton Checking
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        //Setting default values
        //locationTextUI = GetComponent<TMPro.TextMeshProUGUI>();
        mainTextUI.color = transparentColor(mainTextColor);
        subTextUI.color = transparentColor(subTextColor);
        locationIconUI.color = transparentColor(iconColor);

        //Null checks
        if(mainTextUI == null)
        {
            Debug.LogError("location Main Text UI is empty");
        }
        if (subTextUI == null)
        {
            Debug.LogError("location Sub Text UI is empty");
        }

        if (locationIconUI == null)
        {
            Debug.LogError("location Icon UI is empty");
        }

        Timer.gamePausedEvent += onGamePaused;
        Timer.gameUnpausedEvent += onGameUnPaused;
    }

    protected void OnEnable()
    {
        Timer.gamePausedEvent += onGamePaused;
        Timer.gameUnpausedEvent += onGameUnPaused;
    }
    protected void OnDisable()
    {
        Timer.gamePausedEvent -= onGamePaused;
        Timer.gameUnpausedEvent -= onGameUnPaused;
    }
    protected void OnDestroy()
    {
        Timer.gamePausedEvent -= onGamePaused;
        Timer.gameUnpausedEvent -= onGameUnPaused;
    }


    void Update()
    {
        fadingText();
    }

    //Location UI with just icon and no text
    public void displayOnlyIconUI(Sprite icon, float newDuration, float newFadeInDuration, float newFadeOutDuration)
    {
        displayLocationUI(icon,iconOffest,"","",newDuration,newFadeInDuration,newFadeOutDuration);
    }

    //Location UI with just Main text and no icon
    public void displayMainandSubTextUI(string mainText, string subText, float newDuration, float newFadeInDuration, float newFadeOutDuration)
    {
        displayLocationUI(null,iconOffest,mainText, subText, newDuration, newFadeInDuration, newFadeOutDuration);
    }

    //Location UI with just Main text and Sub text and no icon
    public void displayMainTextUI(string mainText, float newDuration, float newFadeInDuration, float newFadeOutDuration)
    {
        displayLocationUI(null, iconOffest, mainText, "", newDuration, newFadeInDuration, newFadeOutDuration);
    }

    //Shows the icon and text 
    public void displayLocationUI(Sprite icon, Vector2 newtextOffset, string mainText, string subText,  float newDuration, float newFadeInDuration, float newFadeOutDuration)
    {
        //Reseting the Location text and getting it ready
        mainTextUI.color = transparentColor(mainTextUI.color);
        subTextUI.color = transparentColor(subTextUI.color);

        StopAllCoroutines();
        fadeIn = false;
        fadeOut = false;

        //Checking if there is an icon and reseting
        locationIconUI.color = transparentColor(locationIconUI.color);
        if (icon != null)
        {
            hasIcon = true;

            locationIconUI.sprite = icon;
            iconOffest = newtextOffset;

            Vector3 mainTextPosition = mainTextUI.rectTransform.anchoredPosition;
            mainTextUI.rectTransform.anchoredPosition = new Vector3(mainTextPosition.x + iconOffest.x, mainTextPosition.y + iconOffest.y, mainTextPosition.z);

            Vector3 subTextPosition = subTextUI.rectTransform.anchoredPosition;
            subTextUI.rectTransform.anchoredPosition = new Vector3(subTextPosition.x + iconOffest.x, subTextPosition.y + iconOffest.y, subTextPosition.z);
        }
        else
        {
            hasIcon = false;
        }

        //Setting Text
        mainTextUI.text = mainText;
        subTextUI.text = subText;

        //Setting the animation values
        Duration = newDuration;
        FadeInDuration = newFadeInDuration;
        FadeOutDuration = newFadeOutDuration;

        //Starting the fade-in
        FadeInTimer = 0f;
        fadeIn = true;

        //Waiting for the fade-in to finish and then moving to the text staying on screen
        if (keepTextOnScreenCoroutine != null)
        {
            StopCoroutine(keepTextOnScreenCoroutine);
        }
        keepTextOnScreenCoroutine = StartCoroutine(keepTextOnScreen(FadeInDuration));
    }

    //Changes only the texts color and not alpha
    public void changeMainTextUIColor(Color newColor)
    {
        mainTextUI.color = new Color(newColor.r, newColor.g, newColor.b, mainTextUI.color.a);
    }

    public void changeSubTextUIColor(Color newColor)
    {
        subTextUI.color = new Color(newColor.r, newColor.g, newColor.b, mainTextUI.color.a);
    }

    //Changes only the Icons color and not alpha
    public void changeIconUIColor(Color newColor)
    {
        locationIconUI.color = new Color(newColor.r, newColor.g, newColor.b, mainTextUI.color.a);
    }

    private IEnumerator keepTextOnScreen(float duration)
    {
        //Waiting for the fade-in on screen to finish
        yield return new WaitForSeconds(duration);

        //Waiting for staying on screen to finish and then moving to fading out animation
        if (startFadeOutCoroutine != null)
        {
            StopCoroutine(startFadeOutCoroutine);
        }
        startFadeOutCoroutine = StartCoroutine(startFadeOut(Duration));
    }
    private IEnumerator startFadeOut(float duration)
    {
        //Waiting for staying on screen to finish
        yield return new WaitForSeconds(duration);

        //Starting fade Out
        FadeOutTimer = FadeOutDuration;
        fadeOut = true;
    }

    private void fadingText()
    {
        //checking if the game is not paused
        if(Timer.gamePaused)
        {
            return;
        }

        //Handling fading out the text. alpha
        if(fadeOut)
        {
            //fading out text
            mainTextUI.color = new Color(mainTextUI.color.r, mainTextUI.color.g, mainTextUI.color.b,(FadeOutTimer / FadeOutDuration));
            subTextUI.color = new Color(subTextUI.color.r, subTextUI.color.g, subTextUI.color.b,(FadeOutTimer / FadeOutDuration));

            //fading out Icon
            if(hasIcon)
            {
                locationIconUI.color = new Color(locationIconUI.color.r, locationIconUI.color.g, locationIconUI.color.b, (FadeOutTimer / FadeOutDuration));
            }

            //Checks if the fading is complete
            if (mainTextUI.color.a <= 0f)
            {
                fadeOut = false;
            }
        }

        //Handling fading out the text. alpha
        if (fadeIn)
        {
            //fading in text
            mainTextUI.color = new Color(mainTextUI.color.r, mainTextUI.color.g, mainTextUI.color.b, (FadeInTimer / FadeInDuration));
            subTextUI.color = new Color(subTextUI.color.r, subTextUI.color.g, subTextUI.color.b, (FadeInTimer / FadeInDuration));

            //fading in Icon
            if(hasIcon)
            {
                locationIconUI.color = new Color(locationIconUI.color.r, locationIconUI.color.g, locationIconUI.color.b, (FadeInTimer / FadeInDuration));
            }

            //Checks if the fading is complete
            if (mainTextUI.color.a >= 1)
            {
                fadeIn = false;
            }
        }

        //The ticking of the timers
        FadeOutTimer -= Time.deltaTime;
        FadeInTimer += Time.deltaTime;
    }
    public void onGamePaused()
    {
        //Checking if the alpha already at or  below our target alpha
        if (mainTextUI.color.a <= pauseFadevalue)
        {
            return;
        }

        //Stoping any previous coroutine that was happening
        if(pauseFadeCoroutine != null)
        {
            StopCoroutine(pauseFadeCoroutine);
        }

        //Setting up and starting the pause fade 
        pauseFadeTimer = 0f;
        originalMainTextAlphaValue = mainTextUI.color.a;
        originalSubTextAlphaValue = subTextUI.color.a;
        pauseFadeCoroutine = StartCoroutine(startPauseFade());
    }

    public void onGameUnPaused()
    {
        //Checking if the alpha already at or  below our target alpha
        if (mainTextUI.color.a <= pauseFadevalue)
        {
            return;
        }

        //Stopping the fade coroutine
        if (pauseFadeCoroutine != null)
        {
            StopCoroutine(pauseFadeCoroutine);
        }

        //Reverting the texts and icons alpha to the original
        mainTextUI.color = new Color(mainTextUI.color.r, mainTextUI.color.g, mainTextUI.color.b, originalMainTextAlphaValue);
        subTextUI.color = new Color(subTextUI.color.r, subTextUI.color.g, subTextUI.color.b, originalSubTextAlphaValue);
        
        //Reverting if an icon is being used
        if(hasIcon)
        {
            locationIconUI.color = new Color(locationIconUI.color.r, locationIconUI.color.g, locationIconUI.color.b, originalMainTextAlphaValue);
        }
    }

    //Starts the fading whne paused
    private IEnumerator startPauseFade()
    {
        while ((pauseFadeTimer / pauseFadeDuration) < 1)
        {
            float lerpedAlphaValue = Mathf.Lerp(mainTextUI.color.a, pauseFadevalue, pauseFadeTimer / pauseFadeDuration);

            mainTextUI.color = new Color(mainTextUI.color.r, mainTextUI.color.g, mainTextUI.color.b, lerpedAlphaValue);
            subTextUI.color = new Color(subTextUI.color.r, subTextUI.color.g, subTextUI.color.b, lerpedAlphaValue);
            
            if (hasIcon)
            {
                locationIconUI.color = new Color(locationIconUI.color.r, locationIconUI.color.g, locationIconUI.color.b, lerpedAlphaValue);
            }
            
            pauseFadeTimer += Time.unscaledDeltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
    //Returns a color with zero  alpha
    private Color transparentColor(Color originalColor)
    {
        return new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    //Returns a color with one/full alpha
    private Color opaqueColor(Color originalColor)
    {
        return new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }
}
