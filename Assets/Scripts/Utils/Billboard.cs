using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    [SerializeField] private Transform objectToLookAt;
    [SerializeField] private bool rotateInX = false, rotateInY = false, rotateInZ = false;

    // Update is called once per frame
    void LateUpdate()
    {
        if (objectToLookAt == null) objectToLookAt = Camera.main.transform;

        var lookPos = transform.position - objectToLookAt.position;

        if (!rotateInX)
            lookPos.x = 0;
        if (!rotateInY)
            lookPos.y = 0;
        if (!rotateInZ)
            lookPos.z = 0;

        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
