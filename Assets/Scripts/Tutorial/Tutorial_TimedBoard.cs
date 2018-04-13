using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial_TimedBoard : Tutorial_Billboard
{

    /**********************************************************
     * This script controls the tweening og the billbaord that
     * delivers the calibration message for the tutorial
     * ********************************************************/
    [SerializeField] private float timeUpTop = 5f;
    [SerializeField] private bool jumpToNextSceneAtTheEnd;
    [SerializeField] private bool jumpToNextStepAtTheEnd = true;

    protected override void OnEnable()
    {
        base.OnEnable();
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
#if false
        if (!jumpToNextSceneAtTheEnd)
        {
            if(jumpToNextStepAtTheEnd)
                TutorialManager.instance.MoveTutorialOneStepForward();
        }
        else
            GameManager.instance.EscapeTheRoom();
#endif
    }

    protected override void IntroCompleted()
    {
        base.IntroCompleted();
    }
}
