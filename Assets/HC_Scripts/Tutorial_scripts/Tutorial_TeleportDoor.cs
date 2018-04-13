using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_TeleportDoor : MonoBehaviour {

    /**********************************
     * This class controls door teleportation
     * *******************************/

    [SerializeField] private Transform moveToTransform;
    private RaycastHit rayHit;
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (InputManager.instance.GetInteractButtonDown())
        {
            if (Physics.Raycast(WandManager.instance.wandTip.transform.position, WandManager.instance.wandTip.transform.forward, out rayHit, 100f))
            {
                if (rayHit.transform == transform)
                {
                    TutorialManager.instance.MoveTutorialOneStepForward();
                    StartCoroutine(GazeTapControl.instance.WarpToPosition(moveToTransform.position));
                }
            }
        }
    }
}
