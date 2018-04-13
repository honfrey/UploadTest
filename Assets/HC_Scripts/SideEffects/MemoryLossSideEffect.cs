using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryLossSideEffect : BaseSideEffect
{
    [SerializeField]
    private Transform[] memoryLossItems;

    protected override void Start()
    {
        base.Start();
        SideEffectsManager.Instance.RegisterSideEffect(this, SideEffectsManager.ESideEffectType.SET_MEMORY_LOSS);
    }

    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();
        if(m_EffectStarted)
        {

        }
    }

    public override void StartEffect(bool bPlayOnce = false)
    {
        base.StartEffect(bPlayOnce);
    }

    public override void EndEffect()
    {
        base.EndEffect();
    }

}
