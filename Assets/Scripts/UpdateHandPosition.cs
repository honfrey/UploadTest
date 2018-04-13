using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHandPosition : MonoBehaviour 
{
    public GameObject handAnchor;

    private GvrArmModel armModel;

    void Awake()
    {
        armModel = GetComponent<GvrArmModel>();
    }

	void Update() 
	{
        handAnchor.transform.localPosition = armModel.wristPosition;
        handAnchor.transform.localRotation = armModel.wristRotation;
	}
	
}
