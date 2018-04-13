using UnityEngine;
using DG.Tweening;
using System.Collections;
using TMPro;

public class Cash : ReleaseObject {

    [SerializeField]
    private GameObject billboardUI;
    //[HideInInspector]
    public float cashAmount = 0;
    Collider col;
    public ParticleSystem collectParticles;
    
    private bool hasCollected = false;
    private float collectDelay = 3f;

    void Start()
    {
        StartCoroutine(DelayedCollect());
    }

    private IEnumerator DelayedCollect ()
    {
        yield return new WaitForSeconds(collectDelay);
        Collect();
    }

    public override void OnLook()
    {
        Collect();
    }

    public override void MoveToWand()
    {
        transform.DOMove(WandManager.instance.wandTip.position, 0.3f).OnComplete(ActivateBillboardUI);
        Shrink();
    }

    public override void Collect()
    {
        if (!hasCollected)
        {
            hasCollected = true;
            CashManager.instance.AddCash(cashAmount);
            collectParticles.Play();
            base.Collect();
        }
    }

    void ActivateBillboardUI()
    {
        // create a billboard to show time lost
        GameObject _billboard = (GameObject)Instantiate(billboardUI, Vector3.zero, Quaternion.identity);
        _billboard.GetComponentInChildren<TextMeshPro>().text = "+ $" + cashAmount.ToString("F2");
        _billboard.GetComponent<FloatyBillboard>().Activate(WandManager.instance.wandTip.position, "", "+ $" + cashAmount.ToString(), Color.black);

    }
}
