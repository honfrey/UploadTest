using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideEffectsManager : MonoBehaviour
{

    public enum ESideEffectType
    {
        SET_EUPHORIA = 0,
        SET_SLEEPINESS,
        SET_PARANOIA,
        SET_MEMORY_LOSS,
        SET_TIME_SLOWDOWN,
        SET_NUM
    }

    protected static SideEffectsManager m_Instance = null;

    private Dictionary<ESideEffectType, BaseSideEffect> m_sideEffectDict = new Dictionary<ESideEffectType, BaseSideEffect>();

    public static SideEffectsManager Instance
    {
        get
        {
            return m_Instance;
        }
    }
   

    void Awake()
    {
        m_Instance = this;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RegisterSideEffect(BaseSideEffect sideEffect, ESideEffectType type)
    {
        if (!m_sideEffectDict.ContainsKey(type))
        {
            m_sideEffectDict.Add(type, sideEffect);
        }
        else
        {
            Debug.LogWarningFormat("Side Effect {0} shouldn't be registered twice.", type.ToString());
        }
    }

    public void StartSideEffect(ESideEffectType type, bool bPlayOnce = true)
    {
        BaseSideEffect sideEffect = GetSideEffect(type);
        if(sideEffect != null)
        {
            sideEffect.StartEffect(bPlayOnce);
        }

    }

    public void StopSideEffect(ESideEffectType type)
    {
        BaseSideEffect sideEffect = GetSideEffect(type);
        if (sideEffect != null)
        {
            sideEffect.EndEffect();
        }
    }

    protected BaseSideEffect GetSideEffect(ESideEffectType type)
    {
        if (m_sideEffectDict.ContainsKey(type))
        {
            return m_sideEffectDict[type];
        }
        else
        {
            Debug.LogWarningFormat("Side Effect {0} can not be found.", type.ToString());
            return null;

        }

    }


}
