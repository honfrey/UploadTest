using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRBillboardIcon : MonoBehaviour {

    Vector3 scale = new Vector3(1, 1, 1);
    public float maxDistance = 5f;
    public float minScale = 0.5f;

    public bool lookAtCamera = true;
    public bool updateScale = true;

	void Start () {
        scale = transform.localScale;
	}
	
	void Update () {
        if (lookAtCamera)
            transform.LookAt(Camera.main.transform);

        if (updateScale)
            UpdateScale();
    }

    void UpdateScale()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        float scalePerc = distance / maxDistance;
        float newScale = Mathf.Clamp(scale.x * scalePerc, minScale, float.MaxValue);
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
