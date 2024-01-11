using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    [System.Serializable]
    protected struct ShrinkAndDisappearSettings
    {
        public Vector3 initialScaleSize;
        public Vector3 finalScaleSize;
        public float scaleDuration;
        [Space]
        public float fadeTransparency;
        public float fadeDuration;
    }

    public List<Sprite> centerSprites;
    public List<Sprite> edgeSprites;
    public int edgeCount;
    public float animationLength;
    
    [Header("Shrink And Disappear Settings")]
    [SerializeField] protected HitMarker.ShrinkAndDisappearSettings centerSettings;
    [SerializeField] protected HitMarker.ShrinkAndDisappearSettings edgeSettings;
    [HideInInspector] public List<ScaleAndFade> hitMarkerScaleScripts;

    void Start()
    {
        hitMarkerScaleScripts = new List<ScaleAndFade>();

        playEffect();

        Invoke("Destruct", animationLength);
    }

    public void playEffect()
    {
        if (hitMarkerScaleScripts == null) hitMarkerScaleScripts = new List<ScaleAndFade>();
        else hitMarkerScaleScripts.Clear();

        foreach(Sprite sprite in centerSprites)
        {
            GameObject centerSpriteObject = new GameObject(sprite.name);
            centerSpriteObject.transform.SetParent(transform);
            centerSpriteObject.transform.localScale = centerSettings.initialScaleSize;

            centerSpriteObject.AddComponent<SpriteRenderer>().sprite = sprite;

            ScaleAndFade shrinkScript = centerSpriteObject.AddComponent<ScaleAndFade>();
            shrinkScript.finalScaleSize = centerSettings.finalScaleSize;
            shrinkScript.scaleDuration = centerSettings.scaleDuration;
            shrinkScript.fadeTransparency = centerSettings.fadeTransparency;
            shrinkScript.fadeDuration = centerSettings.fadeDuration;
            hitMarkerScaleScripts.Add(shrinkScript);

            centerSpriteObject.transform.position = transform.position;
        }

        for(int i = 0; i < edgeCount; i++)
        {
            Sprite spikeSprite = edgeSprites[Random.Range(0, edgeSprites.Count)];
            
            GameObject spikeSpriteObject = new GameObject(spikeSprite.name);
            spikeSpriteObject.transform.SetParent(transform);
            spikeSpriteObject.transform.localScale = edgeSettings.initialScaleSize;

            spikeSpriteObject.AddComponent<SpriteRenderer>().sprite = spikeSprite;
            
            ScaleAndFade shrinkScript = spikeSpriteObject.AddComponent<ScaleAndFade>();
            shrinkScript.finalScaleSize = edgeSettings.finalScaleSize;
            shrinkScript.scaleDuration = edgeSettings.scaleDuration;
            shrinkScript.fadeTransparency = edgeSettings.fadeTransparency;
            shrinkScript.fadeDuration = edgeSettings.fadeDuration;
            hitMarkerScaleScripts.Add(shrinkScript);

            spikeSpriteObject.transform.position = transform.position;

            float randomRotationZ = Random.Range(0, 360);
            Quaternion randomRotation = Quaternion.Euler(new Vector3(0f, 0f, randomRotationZ));
            spikeSpriteObject.transform.rotation = randomRotation;
        }

        foreach(ScaleAndFade hitMarkerChild in hitMarkerScaleScripts)
        {
            hitMarkerChild.Initiate();
        }
    }

    public virtual void Destruct()
    {
        Destroy(gameObject);
    }
}
