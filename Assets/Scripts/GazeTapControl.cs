using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class GazeTapControl : MonoBehaviour {

    //public PostProcessLayer ppl;

    public static GazeTapControl instance;
    public Transform raycastFromTrans, raycastEdgeTrans;
    public LayerMask rewardObjMask;
    public LayerMask gazeListenerMask;
    public LayerMask groundLayerMask;
    public InteractableObject selectedObject;
    public float maxWalkDistance;
    public SpriteRenderer blackout;
    public Transform cameraHolder;
    public AudioSource stepAudioSource;
    public AudioClip stepClip;
    public AudioClip startMoveClip;
    public MoveMarker moveMarker;
    public bool showMoveMarker = true;
    public LineRenderer line;
    public bool doShowLine = false;
    public Color activeLaserColour;
    public Color inactiveLaserColour;
    //public Color exitLaserColour;
    public float activeLaserWidth = 3f;
    //public float exitLaserWidth = 4f;
    public float maxInteractDistance = 2;
    public bool turnLaserOn = false;
    //public bool laserAlreadyOn = false;
    //public bool turnExitLaserOn = false;

    private float defaultLaserWidth;
    private Tween laserWidthTween;
    private Coroutine warpCoroutine;

    void Awake()
    {
        instance = this;
    }

    void Start () {
        blackout.enabled = true;
        blackout.material.DOFade(0, 0);
        
        if (!showMoveMarker)
            moveMarker.gameObject.SetActive(false);

        defaultLaserWidth = line.widthMultiplier;
        raycastFromTrans = WandManager.instance.wandTip;
    }
	
	void Update ()
    {
        turnLaserOn = false;
        //turnExitLaserOn = false;
        moveMarker.SetVisible(false);
        //Laser
        if (doShowLine)
        {
            line.SetPosition(0, raycastFromTrans.position);
            // default to edge position
            ChangeLaserEdge(raycastFromTrans.position + raycastFromTrans.forward * 100);
            line.gameObject.SetActive(true);
        }
        else { line.gameObject.SetActive(false); }

        if (HeldInteractableObject()) { }
        else if (InteractableObjectSelection()) { }
        //else if (CollectWithGaze()) { }
        else ClickGroundToMove();
        //Turn on laser
        if (turnLaserOn)
        {
            //if (laserAlreadyOn)
            //{
                //if (turnExitLaserOn && ColoursAreSimilar(line.startColor, activeLaserColour))
                //{
                //    laserWidthTween.Kill();
                //    SetLaserColour(exitLaserColour);
                //    line.widthMultiplier = defaultLaserWidth * exitLaserWidth;
                //}
                //else if (!turnExitLaserOn && ColoursAreSimilar(line.startColor, exitLaserColour))
                //{
                //    laserWidthTween.Kill();
                //    SetLaserColour(activeLaserColour);
                //    line.widthMultiplier = defaultLaserWidth * activeLaserWidth;
                //}
            //}
            //else
            {
                //laserAlreadyOn = true;
                //if (turnExitLaserOn)
                //{
                //    SetLaserColour(exitLaserColour);
                //    laserWidthTween = DOTween.To(() => line.widthMultiplier, x => line.widthMultiplier = x, defaultLaserWidth * exitLaserWidth, 0.5f).SetEase(Ease.OutBounce);
                //}
                //else
                {
                    SetLaserColour(activeLaserColour);
                    laserWidthTween = DOTween.To(() => line.widthMultiplier, x => line.widthMultiplier = x, defaultLaserWidth * activeLaserWidth, 0.5f).SetEase(Ease.OutBounce);
                }
            }
        }
        else
        {
            if (laserWidthTween != null) { DOTween.Kill(laserWidthTween); }
            SetLaserColour(inactiveLaserColour);
            line.widthMultiplier = defaultLaserWidth;
            //laserAlreadyOn = false;
        }
    }

    private bool ColoursAreSimilar(Color c1, Color c2)
    {
        c1 *= 10f;
        c2 *= 10f;
        if (Mathf.RoundToInt(c1.r) != Mathf.RoundToInt(c2.r))
            return false;
        if (Mathf.RoundToInt(c1.g) != Mathf.RoundToInt(c2.g))
            return false;
        if (Mathf.RoundToInt(c1.b) != Mathf.RoundToInt(c2.b))
            return false;
        if (Mathf.RoundToInt(c1.a) != Mathf.RoundToInt(c2.a))
            return false;
        return true;
    }
    
    private void SetLaserColour (Color newColour)
    {
        line.startColor = newColour;
        line.endColor = newColour;
    }
    
    public IEnumerator WarpToPosition(Vector3 position, float fadeOutDuration = 0.2f, float fadeInDuration = 0.2f)
    {
        //Audio from walk coroutine
        float pitchVariance = 0.025f;
        stepAudioSource.pitch = Random.Range(1 - pitchVariance, 1 + pitchVariance);
        stepAudioSource.PlayOneShot(stepClip);
        //Camera fade from MoveToPosition
        DOTween.Kill("Fade");
        blackout.material.DOFade(1, fadeOutDuration).SetId("Fade");
        yield return new WaitForSeconds(fadeOutDuration);
        cameraHolder.transform.position = position;
        blackout.material.DOFade(0, fadeInDuration);
    }
    
    //GazeListener gazeListener;
    //bool CollectWithGaze()
    //{
    //    Ray ray = new Ray(raycastFromTrans.position, raycastFromTrans.forward);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit, float.MaxValue, gazeListenerMask))
    //    {
    //        GazeListener gl = hit.transform.GetComponent<GazeListener>();

    //        if (gl != gazeListener)
    //        {
    //            if (gazeListener) gazeListener.OnLookAway(); 
    //            gazeListener = gl;
    //            gl.OnLook();
    //        }

    //        if (InputManager.instance.GetInteractButtonDown())
    //        {
    //            if (gazeListener) gazeListener.OnClick();
    //        }
    //        //Active laser
    //        if (hit.transform.name == "ExitDoor")
    //        {
    //            if (KeyManager.instance.HasExitKey())
    //            {
    //                turnExitLaserOn = true;
    //                turnLaserOn = true;

    //                ChangeLaserEdge(hit.point);
    //            }
    //        }
    //        else { turnLaserOn = true; }
    //        return true;
    //    }
    //    else if (gazeListener)
    //    {
    //        gazeListener.OnLookAway();
    //        gazeListener = null;
    //    }

    //    return false;
    //}

    private bool HeldInteractableObject ()
    {
        if (InputManager.instance.GetInteractButtonDown())
        {
            if (InteractableManager.instance.curHeldObject != null)
            {
                InteractableManager.instance.curHeldObject.OnClick();
                return true;
            }
        }
        return false;
    }

    bool InteractableObjectSelection()
    {
        Ray ray = new Ray(raycastFromTrans.position, raycastFromTrans.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, float.MaxValue, rewardObjMask))
        {
            InteractableObject io = hit.transform.GetComponent<InteractableObject>();

            if (hit.distance <= maxInteractDistance)
            {
                if (io != selectedObject)
                {
                    if (selectedObject) selectedObject.OnLookAway(false);
                    selectedObject = io;
                    selectedObject.OnLook(true);
                }

                if (InputManager.instance.GetInteractButtonDown())
                {
                    //Debug.Log("Clicking on " + selectedObject.jiggleTransform.name + " to release " + selectedObject.reward.rewardType);
                    if (selectedObject) selectedObject.OnClick();
                }
                //Enable line for ROs
                if (io.IsAnActiveReward())
                {
                    turnLaserOn = true;
                }

                ChangeLaserEdge(hit.point);

                return true;
            }
            else
            {
                if (selectedObject)
                {
                    selectedObject.OnLookAway(false);
                    selectedObject = null;
                }
            }
            
        }
        else if (selectedObject)
        {
            selectedObject.OnLookAway(false);
            selectedObject = null;
        }

        return false;
    }

    void ClickGroundToMove()
    {
        Ray ray = new Ray(raycastFromTrans.position, raycastFromTrans.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxWalkDistance, groundLayerMask))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                //Enable line for ROs
                turnLaserOn = true;
                //Enable move marker
                moveMarker.SetVisible(true);
                moveMarker.transform.position = hit.point;
                if (InputManager.instance.GetTeleportButtonDown())
                {
                    if (warpCoroutine != null)
                        StopCoroutine(warpCoroutine);
                    warpCoroutine = StartCoroutine(WarpToPosition(new Vector3(hit.point.x, cameraHolder.position.y, hit.point.z)));
                }
                ChangeLaserEdge(hit.point);
            }
        }
    }

    private void ChangeLaserEdge(Vector3 _pos)
    {
        if (_pos == raycastEdgeTrans.position)
            raycastEdgeTrans.gameObject.SetActive(true);
        else
            raycastEdgeTrans.gameObject.SetActive(false);
        line.SetPosition(1, _pos);
    }

    private void OnDisable()
    {
        line.gameObject.SetActive(false);
        moveMarker.gameObject.SetActive(false);
    }
}
