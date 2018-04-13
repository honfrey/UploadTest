using UnityEngine;
using System.Collections;
using System;

public class Tutorial_ExitDoor : CostObject {

    public Animator doorAnim;
    public Tutorial_Billboard goldKeyNeededBillboard;
    public Tutorial_Billboard silverKeyUseBillboard;

    // we save this here to deactivate it when we transition.
    public GameObject timerGO;

    void Start () {
        SetCost(cost);
        goldKeyNeededBillboard.gameObject.SetActive(false);
        if (silverKeyUseBillboard != null)
            silverKeyUseBillboard.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        RewardManager.OnTutorialObjectOpened += RewardManager_OnTutorialObjectOpened;
    }

    void OnDisable()
    {
        RewardManager.OnTutorialObjectOpened -= RewardManager_OnTutorialObjectOpened;

    }

    void Update () {
	if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(EscapeTheRoom());
        }
	}

    public override void OnClick()
    {
        if (KeyManager.instance.HasExitKey())
        {
            base.OnClick();
        } else
        {
            if (goldKeyNeededBillboard.gameObject.activeSelf == false)
            {
                // show need gold key message
                goldKeyNeededBillboard.gameObject.SetActive(true);
                goldKeyNeededBillboard.StartIntro();
            }

        }
    }

    public override void OnPay()
    {
        if(silverKeyUseBillboard != null)
             silverKeyUseBillboard.gameObject.SetActive(false);

        goldKeyNeededBillboard.gameObject.SetActive(false);
        

        StartCoroutine(EscapeTheRoom());
    }

    IEnumerator EscapeTheRoom()
    {
        doorAnim.SetTrigger("tutorialDoor_open_Trigger");

        yield return new WaitForSeconds(doorAnim.GetCurrentAnimatorClipInfo(0).Length);

        if (timerGO != null)
            timerGO.SetActive(false);
        //GameManager.instance.EscapeTheRoom();
    }

    void RewardManager_OnTutorialObjectOpened()
    {
        if (TutorialManager.instance.GetIsLastStep() && silverKeyUseBillboard != null)
        {
            silverKeyUseBillboard.gameObject.SetActive(true);
            silverKeyUseBillboard.StartIntro();
        }
    }
}
