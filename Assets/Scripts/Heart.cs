using UnityEngine;
using System.Collections;

public class Heart : ReleaseObject {

    [SerializeField]
    private GameObject billboardUI;
    public ParticleSystem collectParticles;

    private int additionalTime = 60;
    private bool hasCollected = false;
    private float collectDelay = 3f;

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
            // create a billboard to show time lost
            StartCoroutine(BillboardManager.instance.SendNewUIMessage("You Gained ", "+" + additionalTime.ToString(), Color.black, 0.2f));
           // GameManager.instance.AddTime(additionalTime);
            collectParticles.Play();
            base.Collect();
        }
    }
}
