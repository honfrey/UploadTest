using UnityEngine;
using UnityEngine.Analytics;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class KeyManager : MonoBehaviour {

    public GameObject keyPrefab;
    public static KeyManager instance;
    public TextMesh keyCountText;
    public Transform keyUITransform;
    public GameObject exitKeySprite;
    public GameObject exitKeyPrefab;
    public Transform exitKeyTargetTransform;
    //[HideInInspector]
    public int keysFound = 0;
    //[HideInInspector]
    public int keyCount = 0;
    public bool bHasExitKey = false;

    private Vector3 keyTextScale;

    void Awake()
    {
        instance = this;
        keyTextScale = keyUITransform.localScale;
        ShowExitKey(false);
    }

    public void SetKeys(int set)
    {
        keyCount = set;
        UpdateKeyCountText();
    }

    public bool HasKeys(int num)
    {
        if (keyCount >= num) return true; else return false;
    }

    public bool HasExitKey ()
    {
        return bHasExitKey;
    }

    public void AddKeys(int amount)
    {
        if (amount > 0)
        {
            keysFound += amount;

            keyCount += amount;
            UpdateKeyCountText();
        }
        else    //Handle collecting exit key
        {
            ShowExitKey(true);
            //WandManager.instance.UpdateGoldKey("1");
        }

        float scale = keyTextScale.x;
        scale *= 2f;
        DOTween.Kill("ScaleKeysDown");
        keyUITransform.DOScale(scale, 0.5f).SetEase(Ease.OutBack).SetId("ScaleKeysUp").OnComplete(ShrinkKeys);
    }

    void ShrinkKeys()
    {
        DOTween.Kill("ScaleKeysUp");
        keyUITransform.DOScale(keyTextScale, 0.5f).SetEase(Ease.InBack).SetId("ScaleKeysDown");
    }

    private void ShowExitKey(bool shouldShow)
    {
        exitKeySprite.SetActive(shouldShow);
    }

    public void SpendKeys(int num)
    {
        keyCount -= num;
        UpdateKeyCountText();
    }

    void UpdateKeyCountText()
    {
        keyCountText.text = keyCount.ToString();
        //WandManager.instance.UpdateSilverKeys(keyCount.ToString());
    }

    public void ReleaseExitKey (Transform trans)
    {
        StartCoroutine(ReleaseKeyObjects(exitKeyPrefab, 1, trans, 0.25f));
        bHasExitKey = true;
        SendAnalytics();
    }

    public void ReleaseKeys(int numKeys, Transform trans)
    {
        StartCoroutine(ReleaseKeyObjects(keyPrefab, numKeys, trans, 0.25f));
    }

    IEnumerator ReleaseKeyObjects(GameObject keyPrefabToSpawn, int keysToSpawn, Transform trans, float timeBetweenSpawn)
    {
        for (int i = 0; i < keysToSpawn; i++)
        {
            GameObject keyObject = Instantiate(keyPrefabToSpawn, trans.position + trans.forward * 1, trans.rotation) as GameObject;
            Rigidbody rb = keyObject.GetComponent<Rigidbody>();
            rb.AddForce(trans.forward * 200);
            rb.AddExplosionForce(200, trans.position, 100, 1, ForceMode.Force);

            Key key = keyObject.GetComponent<Key>();
            key.audioObject.PlayRelease();

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }

    void SendAnalytics()
    {
        var _dataDictionary = new Dictionary<string, object>
        {
            {"GoldKey_gameStartTime", GameManager.instance.startGameTime },
            {"GoldKey_timeLeft", GameManager.instance.GetSecondsLeft() }
        };
        // send info to analytics
        var _result = Analytics.CustomEvent("goldKeyCollected", _dataDictionary);
        if (_result == AnalyticsResult.Ok)
            Debug.Log("Sending analytics from key Manager startTime:" + GameManager.instance.startGameTime + " seconds left " + GameManager.instance.GetSecondsLeft());
        else
            Debug.LogError("Analytics were not sent from bomb because " + _result.ToString());
    }
}
