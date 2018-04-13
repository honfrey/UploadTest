using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_objectInteraction_room3 : MonoBehaviour {

    /*************************************************
     * Controls the object interaction part of the tutorial
     * ***********************************************/

    private int objectInteractionCounter;
    [SerializeField]
    private Tutorial_Billboard[] billboards;

    void OnEnable () {
        RewardManager.OnTutorialObjectOpened += RewardManager_OnTutorialObjectOpened;
        objectInteractionCounter = 0;
        billboards[0].StartIntro();
    }

    private void OnDisable()
    {
        RewardManager.OnTutorialObjectOpened -= RewardManager_OnTutorialObjectOpened;
    }

    private void RewardManager_OnTutorialObjectOpened()
    {
        objectInteractionCounter++;
        ChangeBillboard(objectInteractionCounter);
    }
    
    void ChangeBillboard (int _bill) {
        Debug.Log("Moving the billboard to " + _bill);

        switch (_bill)
        {
            case 0:
                billboards[0].StartIntro();
                break;
            case 3:
                billboards[0].StartOutro();
                billboards[1].StartIntro();
                break;
            case 4:
                var _newPos = billboards[1].transform.position;
                billboards[1].transform.SetParent(null);
                billboards[1].transform.position = _newPos;
                Invoke("MoveTutNextStep", 1f);

                break;
        }

        }


    void MoveTutNextStep()
    {
        TutorialManager.instance.MoveTutorialOneStepForward();

    }
}

