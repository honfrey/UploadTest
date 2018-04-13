using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial_TimeAndMovementBillbaord : Tutorial_Billboard
{

    /**********************************************************
     * This script controls the tweening of the billbaord based
     * on movement but with a time cap as well
     * ********************************************************/
    [SerializeField] private float timeUpTop = 5f;

    private bool outroStarted;
    private Sequence introSequene;
    protected override void OnEnable()
    {
        base.OnEnable();
        outroStarted = false;
        StartIntro();
    }

    public override void StartIntro()
    {
        introSequene = DOTween.Sequence();
        introSequene.AppendInterval(0.5f).Append(transform.DOMoveY(targetPos, 0.4f, false)).AppendCallback(IntroCompleted).AppendInterval(timeUpTop).
            AppendCallback(StartOutro);
        introSequene.Play();

    }

    protected override void  Update()
    {
        if (InputManager.instance.GetInteractButtonDown() && introCompleted && !outroStarted)
        {
            StartOutro();
        }
    }

    protected override void IntroCompleted()
    {
        base.IntroCompleted();
    }


    protected override void OutroCompleted()
    {
        TutorialManager.instance.MoveTutorialOneStepForward();
    }

    public override void StartOutro()
    {
        introSequene.Kill();
        outroStarted = true;
        base.StartOutro();
    }
}
