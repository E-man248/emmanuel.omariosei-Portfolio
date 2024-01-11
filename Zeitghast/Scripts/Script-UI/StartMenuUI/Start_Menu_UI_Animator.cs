using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_Menu_UI_Animator : MonoBehaviour
{
    public bool debug;
    public GameObject Target_UI_Group;

    [Header("Animation variables")]
    [SerializeField] private Ease scaleEaseIn = Ease.Linear;
    [SerializeField] private float scaleEaseInSpeed = 0.7f;
    [Space]

    [SerializeField] private Ease scaleEaseOut = Ease.Linear;
    [SerializeField] private float scaleEaseOutSpeed = 0.7f;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        if (Target_UI_Group == null)
        {
            Debug.LogError(name + "'s Target_UI_Group is not set");
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }


    public void ScaleInUIGroup()
    {
        if (Target_UI_Group == null) return;

        if (debug) print(name + " tries scale in");

        Target_UI_Group.transform.localScale = Vector3.zero;

        Target_UI_Group.SetActive(true);
        Tweener ScaleInTween =  Target_UI_Group.transform.DOScale(Vector3.one, scaleEaseInSpeed).SetEase(scaleEaseIn).OnComplete(() =>
        {
            if (debug) print(name + "'s scale in done");
        });

        ScaleInTween.SetUpdate(true);
    }

    public void ScaleOutUIGroup()
    {
        if (Target_UI_Group == null) return;

        if (debug) print(name + " tries scale out");

        Target_UI_Group.transform.localScale = Vector3.one;

        Tweener ScaleOutTween  = Target_UI_Group.transform.DOScale(Vector3.zero, scaleEaseOutSpeed).SetEase(scaleEaseOut).OnComplete(() =>
        {
            if (debug) print(name + "'s scale out done");
            Target_UI_Group.SetActive(false);
        });

        ScaleOutTween.SetUpdate(true);
    }
}
