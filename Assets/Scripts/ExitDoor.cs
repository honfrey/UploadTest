using UnityEngine;
using System.Collections;
using System;

public class ExitDoor : CostObject {

    public Animator doorAnim;

    void Start () {
        SetCost(cost);
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
        }
    }

    public override void OnPay()
    {
        StartCoroutine(EscapeTheRoom());
    }

    IEnumerator EscapeTheRoom()
    {
        doorAnim.SetTrigger("openDoorTrigger");
        yield return new WaitForSeconds(doorAnim.GetCurrentAnimatorClipInfo(0).Length);
        //GameManager.instance.EscapeTheRoom();
    }
}
