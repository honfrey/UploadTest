using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial_Interaction : Tutorial_Billboard
{

    /**********************************************************************
     * This script controls the tweening of the billboard for the tutorial*
     * It ends when players interact with the correct object              * 
     * *******************************************************************/

    [SerializeField] private bool ignoreIntroComplete;
    [SerializeField] private GameObject targetObject;

    private RaycastHit rayHit;
    protected override void OnEnable()
    {
        base.OnEnable();
        // if initialized start intro
        if (base.initialized)
            StartIntro();
    }


    protected override void Update()
    {
        // move on to the next tutorial step if players point at the floor and clicks
        if (InputManager.instance.GetInteractButtonDown())
        {
            if (Physics.Raycast(WandManager.instance.wandTip.transform.position, WandManager.instance.wandTip.transform.forward, 
                out rayHit, GazeTapControl.instance.maxInteractDistance))
            {
                if (rayHit.transform.gameObject == targetObject)
                {
                    if (ignoreIntroComplete && !introCompleted)
                        TutorialManager.instance.MoveTutorialOneStepForward();
                    else if (introCompleted)
                        StartOutro();
                }
            }
        }
    }
    

    protected override void OutroCompleted()
    {
        TutorialManager.instance.MoveTutorialOneStepForward();
    }
}
