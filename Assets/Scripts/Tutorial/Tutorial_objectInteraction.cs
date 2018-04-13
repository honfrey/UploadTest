using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_objectInteraction : MonoBehaviour {

    /*************************************************
     * Controls the object interaction part of the tutorial
     * ***********************************************/

    private int objectInteractionCounter;
    [SerializeField]
    private Tutorial_Billboard[] billboards;

    void OnEnable () {
        RewardManager.OnTutorialObjectOpened += RewardManager_OnTutorialObjectOpened;
        objectInteractionCounter = 0;
        ChangeBillboard(objectInteractionCounter);
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

        if (objectInteractionCounter < billboards.Length)
        {
            for (int i = 0; i < billboards.Length; i++)
            {
                if (billboards[i].GetIsIntroComplete())
                    billboards[i].StartOutro();
                else
                    billboards[i].gameObject.SetActive(false);
            }
            billboards[_bill].gameObject.SetActive(true);

            Debug.Log("start billboard intro " + _bill);
            billboards[_bill].StartIntro();
        } else
        {
            int _lastBillboard = billboards.Length - 1;
            billboards[_lastBillboard].transform.SetParent(transform.root);

            if (billboards[_lastBillboard].GetIsIntroComplete())
            {
               // billboards[_lastBillboard].StartOutro();
            }
            else
            {
                billboards[_lastBillboard].gameObject.SetActive(false);
            }

            TutorialManager.instance.MoveTutorialOneStepForward();
        }
    }
}
