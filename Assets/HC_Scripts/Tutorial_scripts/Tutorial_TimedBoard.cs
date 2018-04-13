using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial_TimedBoard : Tutorial_Billboard
{

    /**********************************************************
     * This script controls the tweening of the billbaord in
     * the tutorial. It ends based on time
     * ********************************************************/
    [SerializeField] private float timeUpTop = 5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        // if initialized start intro
        if(base.initialized)
            StartIntro();
    }

    public override void StartIntro()
    {
        Sequence introSequene = DOTween.Sequence();
        introSequene.AppendInterval(0.5f).Append(transform.DOMoveY(targetPos, 0.4f, false)).AppendCallback(IntroCompleted).AppendInterval(timeUpTop).
            Append(transform.DOMoveY(startPos, 0.4f, false)).OnComplete(OutroCompleted);
        introSequene.Play();

    }
    protected override void OutroCompleted()
    {
        Debug.Log("moving tutorial forward");
      TutorialManager.instance.MoveTutorialOneStepForward();
    }

    protected override void IntroCompleted()
    {
        base.IntroCompleted();
    }
}
