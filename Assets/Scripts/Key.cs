using UnityEngine;
using System.Collections;

public class Key : ReleaseObject {

    public ParticleSystem collectParticles;
    public bool bIsExitKey = false;

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
            KeyManager.instance.AddKeys(bIsExitKey ? 0 : 1);
            collectParticles.Play();
            base.Collect();
        }
    }
}
