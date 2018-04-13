using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public static InputManager instance;
    public PlatformManager platformManager;

    //private float swipeThreshold = 0.5f; // minimum distance to count as a swipe
    //private float swipeCooldownSecs = 0.4f; // the length of the cooldown

    // Swipe tracking variables
    //private Vector2 touchStartPos;
    //private bool isSwiping;

    // add a time delay so the turn can finish before we start detecting the next swipe
    //private float swipeCooldown; // if this value is > 0, we are in the cooldown period


    void Awake()
    {
        instance = this;
        if (!platformManager)
            platformManager = GetComponent<PlatformManager>();
    }

	//void Start () {
 //       swipeCooldown = 0.0f;
	//}
	
	void Update ()
    {
        //if (swipeCooldown > 0)
        //{
        //    swipeCooldown -= Time.deltaTime;
        //}

        //if (platformManager.platform == PlatformManager.Platform.Daydream)
        //{
        //    if (swipeCooldown <= 0.0f && GvrController.IsTouching)
        //    {
        //        if (isSwiping == false)
        //        {
        //            isSwiping = true;
        //            touchStartPos = GvrController.TouchPos;
        //        }
        //    }
        //    else
        //    {
        //        isSwiping = false;
        //    }
        //}
        
        if(platformManager.platform == PlatformManager.Platform.GearVRController)
        {
            if (OVRInput.GetDown(OVRInput.Button.Back))
            {
                // DO NOTHING. If a player presses back, do nothing
            }
        }
    }

    public bool GetInteractButtonDown()
    {
        switch (platformManager.platform)
        {
            case PlatformManager.Platform.PC:
                return Input.GetMouseButtonDown(1);
            //case PlatformManager.Platform.GearVR:
            //    return Input.GetMouseButtonDown(0);
            //case PlatformManager.Platform.Cardboard:
            //    return Input.GetMouseButtonDown(0);
            case PlatformManager.Platform.GearVRController:
                var _buttonPress = false;
                if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
                    _buttonPress = true;
                return _buttonPress;
            //case PlatformManager.Platform.Daydream:
            //    return GvrController.ClickButtonUp;
            default:
                return false;
        }
    }

    public bool GetInteractButtonUp()
    {
        switch (platformManager.platform)
        {
            case PlatformManager.Platform.PC:
                return Input.GetMouseButtonUp(0);

            case PlatformManager.Platform.GearVRController:
                var _buttonPress = false;

                if (OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad) || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
                    _buttonPress = true;
                return _buttonPress;

            default: return false;
        }
}

    public bool GetTeleportButtonDown ()
    {
        switch (platformManager.platform)
        {
            case PlatformManager.Platform.PC:
                return Input.GetMouseButtonDown(0);
            case PlatformManager.Platform.GearVRController:
                var _buttonPress = false;
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                    _buttonPress = true;
                return _buttonPress;
            default:
                return false;
        }
    }

    public bool GetTurnRightButtonDown()
    {
        bool returnVal = false;

        switch (platformManager.platform)
        {
            case PlatformManager.Platform.GearVRController:
                {
                    float mouseX = Input.GetAxis("Mouse X");
                    if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) && mouseX > 0.5f)
                        returnVal = true;
                    else
                        returnVal = false;
                }
                break;
            //case PlatformManager.Platform.Daydream:
            //    {
            //        if (isSwiping)
            //        {
            //            Vector2 touchPos = GvrController.TouchPos;
                        
            //            if ((touchPos.x - touchStartPos.x) > swipeThreshold)
            //            {
            //                returnVal = true;
            //                isSwiping = false; // end the swipe
            //                swipeCooldown = swipeCooldownSecs;
            //            }
            //        }
            //    }
            //    break;
            default:
                returnVal = false;
                break;
        }

        return returnVal;
    }

    public bool GetTurnLeftButtonDown()
    {
        bool returnVal = false;
        
        switch (platformManager.platform)
        {
            case PlatformManager.Platform.GearVRController:
                {
                    float mouseX = Input.GetAxis("Mouse X");
                    if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) && mouseX < 0.5f)
                        returnVal = true;
                    else
                        returnVal = false;
                }
                break;

            //case PlatformManager.Platform.Daydream:
            //    {
            //        if (isSwiping)
            //        {
            //            Vector2 touchPos = GvrController.TouchPos;

            //            if ((touchStartPos.x - touchPos.x) > swipeThreshold)
            //            {
            //                returnVal = true;
            //                isSwiping = false; // end the swipe
            //                swipeCooldown = swipeCooldownSecs;
            //            }
            //        }
            //    }
            //    break;
            default:
                {
                    returnVal = false;
                }

                break;
        }

        return returnVal;
    }
}
