using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ReleaseObject : GazeListener {

    [HideInInspector]
    public Collider myCollider;
    [HideInInspector]
    public Rigidbody myRigidbody;

    public CollectObjectAudio audioObject;

    public virtual void Awake()
    {
        myCollider = GetComponent<Collider>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    public virtual void MoveToWand ()
    {
        float moveTime = 0.3f;
        transform.DOMove(WandManager.instance.wandTip.position, moveTime);
        Shrink(moveTime);
    }

    public void Shrink(float shrinkTime = 0.3f)
    {
        transform.DOScale(0, shrinkTime).SetEase(Ease.InSine).OnComplete(OnShrinkComplete);
    }

    public virtual void Collect()
    {
        audioObject.PlayCollect();
        audioObject.gameObject.transform.parent = null;
        myCollider.enabled = false;
        MoveToWand();
        ResetRigidbody();
    }

    public virtual void OnShrinkComplete()
    {
        Destroy(gameObject);
    }

    public void ResetRigidbody()
    {
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.useGravity = false;
    }
}
