using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial_MovementBillbaord : Tutorial_Billboard
{

    /**********************************************************
     * This script controls the tweening of the billboard that
     * delivers the calibration message for the tutorial
     * ********************************************************/

    [SerializeField] private bool ignoreIntroComplete;

    private RaycastHit rayHit;
    protected override void OnEnable()
    {
        base.OnEnable();
        StartIntro();
    }


    protected override void Update()
    {
        // move on to the next tutorial step if players point at the billboard and click
        if (InputManager.instance.GetInteractButtonDown())
        {
            if (Physics.Raycast(WandManager.instance.wandTip.transform.position, WandManager.instance.wandTip.transform.forward, out rayHit, 100f))
            {
                if (rayHit.transform == transform)
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
