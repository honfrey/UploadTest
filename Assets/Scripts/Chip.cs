using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Chip : ReleaseObject {

    [SerializeField]
    private GameObject billboardUI;
    //[HideInInspector]
    public int chipAmount = 0;
    Collider col;
    public ParticleSystem collectParticles;
    public ParticleSystem doubleScoreParticles;

    private bool hasCollected = false;
    private float collectDelay = 3f;
    private bool hasHitGround = false;
    void Start()
    {
        StartCoroutine(DelayedCollect());
    }

    private IEnumerator DelayedCollect()
    {
        yield return new WaitForSeconds(collectDelay);
        Collect();
    }

    public override void OnLook()
    {
        Collect();
    }

    public override void Collect()
    {
        if (!hasCollected)
        {
            var _valueAmount = chipAmount;

            if (!hasHitGround)
            {
                _valueAmount = chipAmount * 2;
                doubleScoreParticles.Emit(1);
                doubleScoreParticles.transform.parent = null;
                Destroy(doubleScoreParticles, 3f);
            }
            ChipManager.instance.AddChips(_valueAmount);
            collectParticles.Play();
            base.Collect();
        }
    }

    //public override void MoveToWand()
    //{
    //    transform.DOMove(WandManager.instance.wandTip.position, 0.3f).OnComplete(ActivateBillboardUI);

    //    Shrink();
    //}

    //void ActivateBillboardUI()
    //{
    //    WandManager.instance.CreateGemFeedback(chipAmount, hasHitGround);
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Default"))
            hasHitGround = true;
    }
}
