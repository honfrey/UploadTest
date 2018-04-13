using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArrow : MonoBehaviour {

    public float bobSpeed = 1.5f;
    public float bobHeight = 0.25f;
    public float startY;

	void Start () {
        startY = transform.position.y;   
	}
	
	void Update () {
        transform.localPosition = new Vector3(0, startY + (Mathf.Sin(Time.time * bobSpeed) * bobHeight), 0);
    }
}
