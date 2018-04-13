using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMarker : MonoBehaviour {

    public Renderer[] renderers;
    public Transform footprints;
    public bool isVisible = false;
    public Transform arrowHolder;

	void Start () {
		
	}
	
	void Update () {
        if (isVisible)
        {
            Vector3 footprintRotation = WandManager.instance.wandTip.rotation.eulerAngles;
            footprintRotation.x = 90f; // transform.rotation.eulerAngles.x;
            footprintRotation.z = 0f;
            footprints.rotation = Quaternion.Euler(footprintRotation);
        }
    }

    public void SetVisible(bool set)
    {
        // Always false if game is not running
        //if (GameManager.instance.state != GameManager.State.game) { set = false; }
        //Set visibility to 'set'
        foreach (Renderer r in renderers)
            r.enabled = set;
        //Update flag
        isVisible = set;
    }
}
