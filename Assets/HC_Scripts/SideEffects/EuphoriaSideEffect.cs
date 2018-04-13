using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EuphoriaSideEffect : BaseSideEffect
{
    [Tooltip("Side effect post process component on the camera")]
    public SideEffectUberPostprocess m_PPEffectComp;
    //public float m_ParanoiaStartTime = 10.0f;
    //public AudioClip m_ParanoiaAudioClip;

    //[Tooltip("Saturate transition duration from one value to another")]
    //public float m_TransitionDuration = 1.0f;
    private int m_stageIndex = 0;
    private float m_lastSaturateValue = 1.0f;
    private float m_lastTriggerTime = 0.0f;

    [System.Serializable]
    public struct EuphoriaData
    {
        [Tooltip("The transition time for the saturate value change which is measured from the beginning of this effect")]
        public float SaturateTransitionTime;
        public float SaturateValue;
    }

    public List<EuphoriaData> m_EuphoriaStages = new List<EuphoriaData>();

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();
        if (m_PPEffectComp == null)
        {
            Debug.LogError("Side Effect Uber Postprocess component is not set on Euphoria Component.");
        }
        SideEffectsManager.Instance.RegisterSideEffect(this, SideEffectsManager.ESideEffectType.SET_EUPHORIA);

#if true
        StartEffect();
#endif
    }

    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();
                 
	    if(m_EffectStarted && m_PPEffectComp != null)
        {
            m_EffectTimeAccum += Time.deltaTime;

            //Update visual effect
            if (m_stageIndex < m_EuphoriaStages.Count)
            {
                EuphoriaData eData = m_EuphoriaStages[m_stageIndex];
                if(m_EffectTimeAccum <= eData.SaturateTransitionTime)
                {
                    float t = (m_EffectTimeAccum - m_lastTriggerTime) / (eData.SaturateTransitionTime - m_lastTriggerTime);
                    t = Mathf.Clamp01(t);

                    float saturateVal = Mathf.Lerp(m_lastSaturateValue, eData.SaturateValue, t);
                    m_PPEffectComp.UpdateSaturateValue(saturateVal);
                }
                else
                {
                    m_lastSaturateValue = eData.SaturateValue;
                    m_lastTriggerTime = m_EffectTimeAccum;
                    m_stageIndex++;
                }
            }
            else
            {
                if (!m_EffectPlayOnce)
                {
                    m_stageIndex = 0;
                    m_lastTriggerTime = 0.0f;
                    m_lastSaturateValue = 1.0f;
                    m_EffectTimeAccum = 0.0f;
                }
                else
                {
                    EndEffect();
                }
            }

            //Update audio
            /*if (m_EffectTimeAccum >= m_ParanoiaStartTime)
            {
                if(m_ParanoiaAudioClip != null)
                {
                    //AudioManager.instance;
                }
            }*/
        }

      
	}

    public override void EndEffect()
    {
        base.EndEffect();
        m_stageIndex = 0;
        m_lastTriggerTime = 0.0f;
        m_lastSaturateValue = 1.0f;
        m_PPEffectComp.ResetSaturateValue();
    }
}
