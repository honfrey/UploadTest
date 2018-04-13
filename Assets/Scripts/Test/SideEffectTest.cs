using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideEffectTest : MonoBehaviour {

    public bool TestEuphoria = true;
    public bool TestSleepiness = true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
       
	    if(TestEuphoria)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                SideEffectsManager.Instance.StartSideEffect(SideEffectsManager.ESideEffectType.SET_EUPHORIA);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                SideEffectsManager.Instance.StopSideEffect(SideEffectsManager.ESideEffectType.SET_EUPHORIA);
            }

        }

        if(TestSleepiness)
        {
            if (Input.GetKeyDown(KeyCode.D))
                SideEffectsManager.Instance.StartSideEffect(SideEffectsManager.ESideEffectType.SET_SLEEPINESS, true);
            if (Input.GetKeyDown(KeyCode.F))
                SideEffectsManager.Instance.StopSideEffect(SideEffectsManager.ESideEffectType.SET_SLEEPINESS);

        }
    }
}
