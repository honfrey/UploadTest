using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : ReleaseObject {

    public float lifeTime = 1f;

    void Start () {
        Invoke("Shrink", lifeTime);
	}
	
	void Update () {
		
	}

    void StartShrink()
    {
        Shrink();
    }
}
