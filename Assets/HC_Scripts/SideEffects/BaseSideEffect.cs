using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSideEffect : MonoBehaviour
{

    protected bool m_EffectStarted = false;
    protected bool m_EffectPlayOnce = false;

    protected float m_EffectTimeAccum = 0.0f;
    
    public virtual void StartEffect(bool bPlayOnce = false)
    {
        m_EffectStarted = true;
        m_EffectPlayOnce = bPlayOnce;
    }

    public virtual void EndEffect()
    {
        m_EffectStarted = false;
        m_EffectTimeAccum = 0.0f;
    }

	// Use this for initialization
	protected virtual void Start ()
    {
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		
	}
}
