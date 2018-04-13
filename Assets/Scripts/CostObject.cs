using UnityEngine;
using System.Collections;
using System;

public abstract class CostObject : ReleaseObject {

    public int cost = 0;
    public GameObject lockUI;
    public TextMesh keyCostText;
    bool hasPaid = false;

	void Start () {
     
	}
	
	void Update () {
	
	}

    public virtual void SetCost(int _cost)
    {
        cost = _cost;
        if (keyCostText != null) keyCostText.text = cost.ToString();
    }

    public override void OnClick()
    {
        if (hasPaid)
            return;
        if (KeyManager.instance.HasKeys(cost))
        {
            hasPaid = true;
            AudioManager.instance.PlayUnlockSound();
            KeyManager.instance.SpendKeys(cost);
            lockUI.SetActive(false);
            OnPay();
        } else
        {
            InsufficientFunds();
        }
    }

    public virtual void OnPay()
    {

    }

    public virtual void InsufficientFunds()
    {

    }
}
