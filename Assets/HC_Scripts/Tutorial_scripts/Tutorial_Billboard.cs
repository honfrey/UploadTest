using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial_Billboard : MonoBehaviour
{

    /**********************************************************
     * This script controls the tweening of the billboard that
     * delivers the billboard messages for the tutorial
     * ********************************************************/

    [SerializeField] private float startPossOffset = 4;
    [SerializeField] private float targetPosOffset = 0;
    [SerializeField] private float introInitialDelay = 0.5f;
    protected float startPos, targetPos;
    protected bool introCompleted, outroCompleted;


    protected virtual void Awake()
    {
        targetPos = transform.position.y + targetPosOffset;
        startPos = transform.position.y - (startPossOffset);
        introCompleted = false;
        outroCompleted = false;
    }

    protected virtual void OnEnable()
    {
        transform.position = new Vector3(transform.position.x, startPos, transform.position.z);
        introCompleted = false;
    }

    public virtual void StartIntro()
    {
       // Debug.Log("Start Intro for " + gameObject.name + "intro complete is "+introCompleted);

        if (introCompleted == false)
        {
            Sequence introSequene = DOTween.Sequence();
            introSequene.AppendInterval(introInitialDelay).Append(transform.DOMoveY(targetPos, 0.4f, false)).AppendInterval(1f).OnComplete(IntroCompleted);
            introSequene.Play();
        }
    }


    public virtual void StartOutro()
    {
        Sequence outroSequene = DOTween.Sequence();
        outroSequene.Append(transform.DOMoveY(startPos, 0.4f, false)).OnComplete(OutroCompleted);
        outroSequene.Play();
    }
    
    protected virtual void IntroCompleted()
    {
        introCompleted = true;
    }

    protected virtual void OutroCompleted()
    {
        introCompleted = false;
        outroCompleted = true;
    }

    public virtual bool GetIsIntroComplete()
    {
        return introCompleted;
    }

    public virtual bool GetIsOutroComplete()
    {
        return outroCompleted;
    }

    protected virtual void Update() { }
}
