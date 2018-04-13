using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class InteractableObject : GazeListener {

    //public Transform jiggleTransform;
    //public Transform rewardReleaseTrans;
    public InteractableManager.InteractableType interactableType;
    public bool isMainObject = false;
    public Renderer objectRenderer;
    public ParticleSystem ps;
    //public MeshFilter highlightMeshFilter;
    //public bool doRandomize = true;
    //public bool isOpen = false;
    //public float punchScale = -0.05f;
    //public RewardManager.Reward reward;
    //string jiggleVisTweenId, shakePosId;
    //public AudioClip inspectClip;   //TODO Switch this to use AudioManager
    //MeshCollider meshCollider;

    private Transform defaultParent;
    private Rigidbody rb;
    private bool isHeld = false;
    private float curFresnel;
    private bool isFocused = false;
    private bool isActiveObject = true;
    private bool isDetected = false;
    //private bool hasBeenInteracted = false;
    //private bool isHighlighted = false;

    private float detectedFresnelIntensity, focusedFresnelIntensity/*, highlightedFresnelIntensity /*, openedFresnelIntensity*/;
    
    void Start () {
        defaultParent = transform.parent;
        rb = GetComponent<Rigidbody>();
        SetColors();
        if (ps != null)
            ps.gameObject.SetActive(false);
        //meshCollider = GetComponent<MeshCollider>();
        //if (meshCollider) highlightMeshFilter.mesh = meshCollider.sharedMesh;
    }

    //private void GetRoot ()
    //{
    //    Transform temp = transform;
    //    while (temp.parent.gameObject.layer == gameObject.layer)
    //    {
    //        temp = temp.parent;
    //    }
    //    root = temp;
    //}
    //void SetOpen(bool set)
    //{
    //    isOpen = true;
    //    ChangeColorAndHighlights(openedFresnelIntensity);
    //    SetActive(false);
    //}

    public void SetActive (bool shouldBeActive)
    {
        isActiveObject = shouldBeActive;
    }

    public bool IsAnActiveReward ()
    {
        return isActiveObject;
    }

    public void HoldObject ()
    {
        isHeld = true;
        InteractableManager.instance.curHeldObject = this;
        rb.isKinematic = true;
        //Move to wand
        transform.parent = InteractableManager.instance.heldObjectContainer;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.up * -objectRenderer.bounds.extents.y;
        // Change colour
        ChangeColorAndHighlights(0);
    }

    public void DropObject ()
    {
        isHeld = false;
        InteractableManager.instance.curHeldObject = null;
        transform.parent = defaultParent;
        rb.isKinematic = false;
        rb.AddForce(InteractableManager.instance.heldObjectContainer.forward * 200f);
        OnLook();
    }

    //public void OnOpen()
    //{
    //    //if (doRandomize) RewardManager.instance.OpenRewardObject(this);
    //    //else RewardManager.instance.OpenRewardObjectFromList(this);
    //    SetOpen(true);
    //    GameManager.instance.objectsOpened++;
    //}

    //public void JiggleVis()
    //{
    //    float duration = 0.5f;
    //    Vector3 _punchScale = Vector3.one * punchScale;
    //    jiggleTransform.localScale = new Vector3(1, 1, 1);
    //    DOTween.Kill(jiggleVisTweenId);
    //    jiggleTransform.DOPunchScale(_punchScale, duration).SetId(jiggleVisTweenId);
    //}

    public override void OnLook(bool _isFocused)
    {
        if (isActiveObject)
        {
            isFocused = _isFocused;
            OnLook();
        }
    }

    public override void OnLook()
    {
        if (!isHeld)
        {
            //if (isOpen)
            //{
            //    // has been opened
            //    ChangeColorAndHighlights(openedFresnelIntensity);
            //}
            if (isFocused)
            {
                // selected at this moment
                ChangeColorAndHighlights(focusedFresnelIntensity);// used to be 4.35 //used to be 5.89
                if (ps != null)
                    ps.gameObject.SetActive(true);
            }
            //else if (isHighlighted)
            //{ // in range of flashlight or detection AoE
            //    ChangeColorAndHighlights(highlightedFresnelIntensity);// used to be 4.99 // used to be 3.2 // used to be 3.81f
            //}
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
        if (ps != null)
            ps.gameObject.SetActive(false);
        OnLook();
    }

    public override void OnLookAway(bool _isFocused)
    {
        if (isActiveObject)
        {
            isFocused = _isFocused;
            OnLookAway();
        }
    }

    public override void OnClick()
    {
        //hasBeenInteracted = true;
        switch(interactableType)
        {
            case InteractableManager.InteractableType.pickup:
                if (isHeld)
                {
                    if (InteractableManager.instance.curHeldObject == this)
                        DropObject();
                    else
                        Debug.LogError("Can't drop " + name + " because it is not registered to InteractableManager");
                }
                else
                {
                    // Pickup object
                    if (InteractableManager.instance.curHeldObject == null)
                        HoldObject();
                }
                break;
            case InteractableManager.InteractableType.open:
                break;
            case InteractableManager.InteractableType.trigger:
                break;
            default:
                Debug.LogError("Interaction type not set up!");
                break;
        }
        // Jiggle
        //JiggleVis();
        //if (isActiveObject)
        //{
            // TODO Pick up object

            // Play audio
            //AudioManager.instance.PlayInspectClip(inspectClip);
            //if (!isOpen)
            //{
                //OnOpen();
            //}
        //}
    }

    public void OnTriggerEnter(Collider other)
    {
        // if (isActiveObject && other.tag == "Flashlight")
        //{
        //    isHighlighted = true;
        //    OnLook();
        //} else 
        if (other.tag == "Detector")
        {
            isDetected = true;
            OnLook();
        } 
    }
    
    public void OnTriggerExit(Collider other)
    {
        // if (isActiveObject && other.tag == "Flashlight")
        //{
        //    isHighlighted = false;
        //    OnLookAway();
        //} else 
        if (other.tag == "Detector")
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
            if (isMainObject)
                objectRenderer.sharedMaterial = InteractableManager.instance.mainDetectedMaterial;
            else
                objectRenderer.sharedMaterial = InteractableManager.instance.detectedMaterial;
        }
        else if (Mathf.Approximately(_fresnelIntensity, focusedFresnelIntensity))
        {
            if (isMainObject)
                objectRenderer.sharedMaterial = InteractableManager.instance.mainFocusedMaterial;
            else
                objectRenderer.sharedMaterial = InteractableManager.instance.focusedMaterial;
        }
        //else if (Mathf.Approximately(_fresnelIntensity, highlightedFresnelIntensity))
        //{
        //    objectRenderer.sharedMaterial = InteractableManager.instance.highlightedMaterial;
        //}
        //else if (Mathf.Approximately(_fresnelIntensity, openedFresnelIntensity))
        //{
        //    objectRenderer.sharedMaterial = InteractableManager.instance.openedMaterial;
        //}
        else
        {
            if (isMainObject)
                objectRenderer.sharedMaterial = InteractableManager.instance.mainRegularMaterial;
            else
                objectRenderer.sharedMaterial = InteractableManager.instance.regularMaterial;
        }
    }

    void SetColors()
    {
        //Get Intensity for comparison
        detectedFresnelIntensity = InteractableManager.instance.detectedFresnelIntensity;
        focusedFresnelIntensity = InteractableManager.instance.focusedFresnelIntensity;
        //highlightedFresnelIntensity = InteractableManager.instance.highlightedFresnelIntensity;
        //openedFresnelIntensity = InteractableManager.instance.openedFresnelIntensity;
    }
}
