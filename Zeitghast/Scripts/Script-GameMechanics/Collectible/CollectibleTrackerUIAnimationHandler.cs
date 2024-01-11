using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleTrackerUIAnimationHandler : EntityAnimationHandler
{

    #region Animation Strings

    public const string AppearNotificationAnimationString = "CollectibleTrackerUIAppearNotification";
    public const string AppearAnimationString = "CollectibleTrackerUIAppear";
    public const string DisappearAnimationString = "CollectibleTrackerUIDisappear";
    public const string ShownAnimationString = "CollectibleTrackerUIShown";
    public const string HiddenAnimationString = "CollectibleTrackerUIHidden";

    #endregion

    private Coroutine notifyAnimationHideCoroutine;

    private CollectibleTrackerUI collectibleTrackerUI;

    protected override void Awake()
    {
        base.Awake();

        if (collectibleTrackerUI == null)
        {
            collectibleTrackerUI = GetComponentInParent<CollectibleTrackerUI>();
            if (collectibleTrackerUI == null)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void PlayAppearAnimation()
    {
        if (getCurrentAnimation() == ShownAnimationString) return;

        if (getCurrentAnimation() == AppearNotificationAnimationString)
        {
            if (notifyAnimationHideCoroutine != null) StopCoroutine(notifyAnimationHideCoroutine);
        }
        else
        {
            playAnimationOnceFull(AppearAnimationString);
        }
    }

    public void PlayDisappearAnimation()
    {
        if (getCurrentAnimation() == HiddenAnimationString) return;

        if (notifyAnimationHideCoroutine != null) StopCoroutine(notifyAnimationHideCoroutine);

        playAnimationOnceFull(DisappearAnimationString);
    }

    public void PlayShownAnimation()
    {
        playAnimationOnceFull(ShownAnimationString);
    }

    public void PlayHiddenAnimation()
    {
        playAnimationOnceFull(HiddenAnimationString);
    }

    #region Notify Animation

    public void PlayNotifyAnimation()
    {
        NotifyAnimationShow();

        notifyAnimationHideCoroutine = StartCoroutine(NotifyAnimationHideCoroutine());
    }

    private void NotifyAnimationShow()
    {
        if (getCurrentAnimation() == ShownAnimationString) return;

        playAnimationOnceFull(AppearNotificationAnimationString);
    }

    private IEnumerator NotifyAnimationHideCoroutine()
    {
        yield return new WaitForSecondsRealtime(getAnimationLength(AppearNotificationAnimationString));

        NotifyAnimationHide();
    }

    private void NotifyAnimationHide()
    {
        playAnimationOnceFull(DisappearAnimationString);

        if (notifyAnimationHideCoroutine != null) StopCoroutine(notifyAnimationHideCoroutine);
    }

    #endregion

    #region Animation Cycle Functions

    protected override void animate()
    {
        AnimateDisplayActivity();

        base.animate();
    }

    private void AnimateDisplayActivity()
    {
        if (IsDisplayActive())
        {
            nextAnimation = ShownAnimationString;
        }
        else
        {
            nextAnimation = HiddenAnimationString;
        }
    }

    #endregion

    #region Animation Booleans

    private bool IsDisplayActive()
    {
        return collectibleTrackerUI.displayActive;
    }

    #endregion
}
