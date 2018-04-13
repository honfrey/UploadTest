using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CashManager : MonoBehaviour {

    public static CashManager instance;
    public float startingCash = 0;
    public float cash = 0;
    public GameObject cashPrefab;

    private float visCash = 0;
    private float approximatelyThreshold = 0.001f;

    [Header("Cash Objects")]
    public CashObject[] cashObjects;
    [System.Serializable]
    public struct CashObject
    {
        public float value;
    }
    
    void Awake()
    {
        instance = this;
    }

    void Start () {
        SetCash(startingCash);
	}

    void Update () {
        UpdateCashText();
    }

    public bool HasCash(float amount)
    {
        if (cash >= amount) return true; else return false;
    }

    public void AddCash(float amount)
    {
        cash += amount;
        float minSpeed = 0.25f, maxSpeed = 1.75f;
        float maxAmount = 20f;
        float duration = Mathf.Lerp(minSpeed, maxSpeed, amount / maxAmount);
        
        DOTween.Kill("Cash");
        DOTween.To(x => visCash = x, visCash, cash, duration).SetEase(Ease.OutSine).SetId("Cash").OnComplete(ShrinkCash);
    }

    void ShrinkCash()
    {
        DOTween.Kill("ScaleCashUp");
    }

    public void SetCash(float set)
    {
        cash = set;
        UpdateCashText();
    }

    public void SpendCash(float amount)
    {
        cash -= amount;
        UpdateCashText();
    }

    void UpdateCashText()
    {
        //WandManager.instance.UpdateMoney("$" + visCash.ToString("F2"));
    }

    public void ReleaseCash(float amount, Transform trans)
    {
        Debug.Log("Found cash reward of " + amount);
        List<CashObject> cashObjectsToSpawn = new List<CashObject>();
        float remaining = amount;
        int guard = 5;
        while (cashObjectsToSpawn.Count < 5 && guard >= 0)
        {
            for (int i = cashObjects.Length - 1; i >= 0; i--)
            {
                float cashValue = cashObjects[i].value;
                if (remaining > cashValue || (FastApproximately(remaining, cashValue) && (guard == 5 || i == 0)))
                {
                    float numOfCashObject = (remaining / cashValue);
                    numOfCashObject *= 100f;
                    numOfCashObject = Mathf.Round(numOfCashObject);
                    numOfCashObject *= 0.01f;
                    numOfCashObject = Mathf.Floor(numOfCashObject);
                    remaining -= numOfCashObject * cashValue;
                    for (int c = 0; c < numOfCashObject; c++) cashObjectsToSpawn.Add(cashObjects[i]);
                }
            }
            //If too few items, break biggest denomination
            string wallet = "[";
            foreach (var item in cashObjectsToSpawn)
            {
                wallet += " " + item.value.ToString();
            }
            Debug.Log(wallet + " ] - remaining: " + remaining);
            if (cashObjectsToSpawn.Count < 5 && guard >= 0)
            {
                CashObject coToBreak = cashObjectsToSpawn[0];
                //print("Breaking " + coToBreak.value);
                remaining += coToBreak.value;
                cashObjectsToSpawn.Remove(coToBreak);
                guard--;
            }
        }
        //Watch out for tiny floats
        if (cashObjectsToSpawn.Count < 1 || remaining > 0f)
        {
            if (!FastApproximately(remaining, 0f))
                Debug.LogError("Error in distributing " + amount + " cash between release objects! " + remaining + " remaining undistributed");
        }
        StartCoroutine(ReleaseCashObjects(cashObjectsToSpawn, trans, 0.1f));
    }

    IEnumerator ReleaseCashObjects(List<CashObject> cashobjects, Transform trans, float timebetweenspawn)
    {
        for (int i = cashobjects.Count-1; i >= 0; i--)
        {
            GameObject cashobject = Instantiate(cashPrefab, trans.position + (trans.forward * 1), trans.rotation) as GameObject;
            Cash c = cashobject.GetComponent<Cash>();
            c.cashAmount = cashobjects[i].value;
            //Debug.Log("Releasing " + c.cashAmount);
            Rigidbody rb = cashobject.GetComponent<Rigidbody>();
            rb.AddForce(trans.forward * 150);
            rb.AddExplosionForce(150, trans.position, 100, 1, ForceMode.Force);
            c.audioObject.PlayRelease();
            yield return new WaitForSeconds(timebetweenspawn);
        }
    }

    private bool FastApproximately (float a, float b)
    {
        return ((a < b) ? (b - a) : (a - b)) <= approximatelyThreshold;
    }
}
