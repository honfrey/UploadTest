using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepinessSideEffect : BaseSideEffect
{
    [Tooltip("Blink duration from open to close, then to open again.")]
    public float m_BlinkDuration = 3.0f;
    [Tooltip("Side effect post process component on the camera")]
    public SideEffectUberPostprocess m_PPEffectComp;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        if (m_PPEffectComp == null)
        {
            Debug.LogWarning("Side Effect Uber Postprocess component is not set on Sleepiness Component");
        }

        SideEffectsManager.Instance.RegisterSideEffect(this, SideEffectsManager.ESideEffectType.SET_SLEEPINESS);

    }

    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();
        if(m_EffectStarted && m_PPEffectComp != null)
        {
            m_EffectTimeAccum += Time.deltaTime;
            
            if(m_EffectTimeAccum <= m_BlinkDuration * 0.5)
            {
                float t = m_EffectTimeAccum / m_BlinkDuration * 2.0f;
                m_PPEffectComp.UpdateBlinkValue(t);
            }
            else if(m_EffectTimeAccum <= m_BlinkDuration)
            {
                float t = 1.0f - (m_EffectTimeAccum - m_BlinkDuration * 0.5f) / m_BlinkDuration * 2.0f;
                m_PPEffectComp.UpdateBlinkValue(t);
            }
            else
            {
                if (m_EffectPlayOnce)
                    m_EffectStarted = false;
                m_EffectTimeAccum = 0.0f;
            }

        }
    }

    public override void StartEffect(bool bPlayOnce = false)
    {
        base.StartEffect(bPlayOnce);
      
    }

    public override void EndEffect()
    {
        base.EndEffect();
        m_PPEffectComp.ResetBlinkValue();
        
    }

}
