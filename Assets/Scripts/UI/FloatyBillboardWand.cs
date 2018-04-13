using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class FloatyBillboardWand : FloatyBillboard {

    /**************************************************
     * This class controls the UI billboard which shows
     * when players gain money on the wand
     * ************************************************/

    public override void MoveSequence()
    {
        Sequence introSequene = DOTween.Sequence();
        //Debug.Log("the final y value is "+finalYValue + " the start y value is "+ startYValue);
        introSequene.Append( transform.DOLocalMoveY(finalYValue, moveUpTime, false)).AppendInterval(holdTime).OnComplete(SequenceCompleted);
        introSequene.Play();
    }

    public override void SequenceCompleted()
    {
        Destroy(gameObject);
    }

    public void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }
}
