using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class RewardObject : GazeListener {

    public GameObject staticGO;
    public Transform jiggleTransform;
    public Transform rewardReleaseTrans;
    public Renderer outlineRenderer;
    public MeshFilter highlightMeshFilter;
    public bool doRandomize = true;
    public bool isOpen = false;
    public float punchScale = -0.05f;
    public RewardManager.Reward reward;
    string jiggleVisTweenId, shakePosId;
    public AudioClip inspectClip;
    MeshCollider meshCollider;

    public float curFresnel;
    private bool isFocused = false;
    public bool isActiveReward = false;
    private bool isDetected = false;
    private bool hasBeenInteracted = false;
    private bool isHighlighted = false;

    public float detectedFresnelIntensity, focusedFresnelIntensity, highlightedFresnelIntensity, openedFresnelIntensity;
    
    void Start () {
        if (jiggleTransform)
        {
            jiggleVisTweenId = "JiggleVis" + jiggleTransform.gameObject.GetInstanceID();
        }
        SetColors();
        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider) highlightMeshFilter.mesh = meshCollider.sharedMesh;
    }

    void SetOpen(bool set)
    {
        isOpen = true;
        ChangeColorAndHighlights(openedFresnelIntensity);
        SetActive(false);
    }

    public void SetActive (bool shouldBeActive)
    {
        isActiveReward = shouldBeActive;
    }

    public bool IsAnActiveReward ()
    {
        return isActiveReward;
    }

    public void OnOpen()
    {
        if (doRandomize) RewardManager.instance.OpenRewardObject(this);
        else RewardManager.instance.OpenRewardObjectFromList(this);
        SetOpen(true);
        //GameManager.instance.objectsOpened++;
    }

    public void JiggleVis()
    {
        float duration = 0.5f;
        Vector3 _punchScale = Vector3.one * punchScale;
        jiggleTransform.localScale = new Vector3(1, 1, 1);
        DOTween.Kill(jiggleVisTweenId);
        jiggleTransform.DOPunchScale(_punchScale, duration).SetId(jiggleVisTweenId);
    }

    public override void OnLook(bool _isFocused)
    {
        if (isActiveReward)
        {
            isFocused = _isFocused;
            OnLook();
        }
    }

    public override void OnLook()
    {
        if (!hasBeenInteracted)
        {
            if (isOpen)
            {
                // has been opened
                ChangeColorAndHighlights(openedFresnelIntensity);
            }
            else if (isFocused)
            {
                // selected at this moment
                ChangeColorAndHighlights(focusedFresnelIntensity);// used to be 4.35 //used to be 5.89
            }
            else if (isHighlighted)
            { // in range of flashlight or detection AoE
                ChangeColorAndHighlights(highlightedFresnelIntensity);// used to be 4.99 // used to be 3.2 // used to be 3.81f
            }
            else if (isDetected)
            {
                // selected at this moment
                ChangeColorAndHighlights(detectedFresnelIntensity);
            }
            else
            {
                ChangeColorAndHighlights(0f);
            }
        }
    }
    
    public override void OnLookAway()
    {
        OnLook();
    }

    public override void OnLookAway(bool _isFocused)
    {
        if (isActiveReward)
        {
            isFocused = _isFocused;
            OnLookAway();
        }
    }

    public override void OnClick()
    {
        //Debug.Log("Clicking on " + jiggleTransform.name + " to release " + reward.rewardType);
        JiggleVis();
        hasBeenInteracted = true;
        ChangeColorAndHighlights(openedFresnelIntensity);
        if (isActiveReward)
        {
            //AudioManager.instance.PlayInspectClip(inspectClip);
            if (!isOpen)
            {
                OnOpen();
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
         if (isActiveReward && other.tag == "Flashlight")
        {
            isHighlighted = true;
            OnLook();
        } else if (other.tag == "Detector")
        {
            isDetected = true;
            OnLook();
        } 
    }
    
    public void OnTriggerExit(Collider other)
    {
         if (isActiveReward && other.tag == "Flashlight")
        {
            isHighlighted = false;
            OnLookAway();
        } else if (other.tag == "Detector")
        {
            isDetected = false;
            OnLookAway();
        }
    }

    void ChangeColorAndHighlights(float _fresnelIntensity)
    {
        if (Mathf.Approximately(_fresnelIntensity, curFresnel)) { return; }
        else { curFresnel = _fresnelIntensity; }
        //Get appropriate material
        if (Mathf.Approximately(_fresnelIntensity, detectedFresnelIntensity))
        {
            outlineRenderer.sharedMaterial = RewardManager.instance.detectedMaterial;
        }
        else if (Mathf.Approximately(_fresnelIntensity, focusedFresnelIntensity))
        {
            outlineRenderer.sharedMaterial = RewardManager.instance.focusedMaterial;
        }
        else if (Mathf.Approximately(_fresnelIntensity, highlightedFresnelIntensity))
        {
            outlineRenderer.sharedMaterial = RewardManager.instance.highlightedMaterial;
        }
        else if (Mathf.Approximately(_fresnelIntensity, openedFresnelIntensity))
        {
            outlineRenderer.sharedMaterial = RewardManager.instance.openedMaterial;
        }
        else
        {
            outlineRenderer.sharedMaterial = RewardManager.instance.regularMaterial;
        }
    }

    void SetColors()
    {
        //Get Intensity for comparison
        detectedFresnelIntensity = RewardManager.instance.detectedFresnelIntensity;
        focusedFresnelIntensity = RewardManager.instance.focusedFresnelIntensity;
        highlightedFresnelIntensity = RewardManager.instance.highlightedFresnelIntensity;
        openedFresnelIntensity = RewardManager.instance.openedFresnelIntensity;
    }
}
