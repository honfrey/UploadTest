using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial_ReadyGoBillbaord : Tutorial_Billboard
{

    /**********************************************************
     * This script controls the tweening og the billbaord that
     * delivers the ready message for the tutorial
     * ********************************************************/

    private bool holding = false;

    protected override void OnEnable()
    {
     //   transform.position = position;
        introCompleted = true;
    }

    protected void Start()
    {
          GazeTapControl.instance.enabled = false;
    }


    protected override void  Update()
    {
        if ( introCompleted)
        {
           if(InputManager.instance.GetInteractButtonDown() && !holding)
            {
                holding = true;
                StartCoroutine(HoldButton());
            }

            if (InputManager.instance.GetInteractButtonUp() && holding )
            {
                holding = false;
                StopCoroutine("HoldButton");
            }
        }
    }
    
    IEnumerator HoldButton()
    {
        yield return new WaitForSeconds(1);

        if (holding)
        {
            GazeTapControl.instance.enabled = true;
            StartOutro();
        }
    }

    protected override void OutroCompleted()
    {
        TutorialManager.instance.MoveTutorialOneStepForward();
    }
}
