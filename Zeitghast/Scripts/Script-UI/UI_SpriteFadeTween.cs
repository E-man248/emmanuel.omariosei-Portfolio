using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpriteFadeTween : MonoBehaviour
{
    public Image TargetSprite;
    [SerializeField] private float baseAlpha = 0.8f; 

    [Header("Animation variables")]
    [SerializeField] private Ease fadeEaseIn = Ease.Linear;
    [SerializeField] private float fadeEaseInSpeed = 0.7f;
    [Space]

    [SerializeField] private Ease fadeEaseOut = Ease.Linear;
    [SerializeField] private float fadeEaseOutSpeed = 0.7f;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }


    public void fadeInUIGroup()
    {
        fadeInUIGroupStart();

        if (TargetSprite == null) return;
        Tweener fadeInTween = TargetSprite.DOFade(baseAlpha, fadeEaseInSpeed).SetEase(fadeEaseIn).OnComplete(fadeInUIGroupOncomplete);

        fadeInTween.SetUpdate(true);
    }

    public void fadeOutUIGroup()
    {
        fadeOutUIGroupStart();

        if (TargetSprite == null) return;
        Tweener fadeOutTween = TargetSprite.DOFade(0f, fadeEaseOutSpeed).SetEase(fadeEaseOut).OnComplete(fadeOutUIGroupOncomplete);

        fadeOutTween.SetUpdate(true);
    }

    protected virtual void fadeInUIGroupOncomplete()
    {

    }

    protected virtual void fadeOutUIGroupOncomplete()
    {

    }


    protected virtual void fadeInUIGroupStart()
    {

    }

    protected virtual void fadeOutUIGroupStart()
    {

    }
}
