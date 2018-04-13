// Tools have been cut before fully implemented
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour {

    public static ToolManager instance;

    public Transform radarPrefab;
    public Transform currentTool;

    private float doubleClickSpeed = 0.4f;
    private bool isPlacingTool = false;
    private bool alreadyClicked = false;

    void Awake()
    {
        instance = this;
    }
    
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (OVRInput.GetDown(OVRInput.Button.DpadRight) || OVRInput.GetDown(OVRInput.Button.DpadLeft))
        {
            if (isPlacingTool)
            {
                //TODO Move through available tools
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
        {
            if (isPlacingTool)
            {
                if (alreadyClicked) //Double Click
                {
                    //TODO Cancel
                }
                else    //Wait for double click detection
                    DoubleClickDetection();
            }
            else
            {
                isPlacingTool = true;
                currentTool = Instantiate(radarPrefab, transform.position, radarPrefab.rotation);
            }
            //GazeTapControl.instance.SetPlacingToolMode(isPlacingTool);
        }
    }

    private IEnumerator DoubleClickDetection ()
    {
        yield return new WaitForSeconds(doubleClickSpeed);
        //Timeout done - handle single click
        alreadyClicked = false;
        isPlacingTool = false;
        StartCoroutine(currentTool.GetComponent<DetectorScript>().Deploy());
    }

    private IEnumerator DoubleClickCooldown ()
    {
        yield return new WaitForSeconds(1f);
    }
}
