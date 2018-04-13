using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using DG.Tweening;

public class Bomb : CostObject {

    public Image timeBar;
    public Text numOfKeysForBomb;
    public TextMeshPro timePenalty1,timePenalty2;
    public RectTransform sparkImage;

    public int time = 30;
    public int index = 0;
    public int beepWarnsAt = 5;
    public bool autoStart = true;

    int currentTime;
    public float bombTicker = 0;
    bool isTicking = true;

    public bool doSetup = false;
    public AudioClip tickClip, tickClipWarning;
    public AudioClip defuseClip;
    public AudioSource tickSource;
    public GameObject explosionParticles;
    public GameObject[] keys;

    // the order number of this bomb as it appeared in the game
    private int bombCount;
    // the time the bomb appeared
    private float bombAppearedTime;
    private Animator anim;
    public override void Awake()
    {
        base.Awake();
        anim = this.GetComponent<Animator>();
    }

    void Start () {

        if (doSetup)
        {
            SetTimer(time);
            SetCost(cost);
        }

        if (!autoStart) isTicking = false;

        timePenalty1.text = "-" + (BombManager.instance.timePenaltyPerKey * cost).ToString() + "s";
        timePenalty2.text = timePenalty1.text;


    }

    public void SetupBomb(int _timerSeconds, int _bombCost, int _bombCount)
    {
        SetTimer(_timerSeconds);
        SetCost(_bombCost);
        bombCount = _bombCount;
        bombAppearedTime = GameManager.instance.GetSecondsLeft();
    }

    void SetTimer(int seconds)
    {
        time = seconds;
        currentTime = time;
        UpdateTimeBar();
    }

    void Explode()
    {
        SendAnalytics(false);
        BombManager.instance.BombExplode(cost);
        SetPause(true);
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        OnShrinkComplete();
    }

    public override void OnShrinkComplete()
    {
        BombManager.instance.RemoveBomb(this);
        base.OnShrinkComplete();
    }

    void UpdateTimeBar()
    {
        timeBar.fillAmount = (float)currentTime / time;
        sparkImage.anchorMin = new Vector2(1- timeBar.fillAmount, sparkImage.anchorMin.y);
        sparkImage.anchorMax = new Vector2(1- timeBar.fillAmount, sparkImage.anchorMax.y);
        sparkImage.anchoredPosition = Vector3.zero;
    }

    public override void SetCost(int _cost)
    {
        cost = _cost;
        if (numOfKeysForBomb != null) numOfKeysForBomb.text = cost.ToString();
        
    }

    public void SetPause(bool set)
    {
        isTicking = !set;
    }

    void Tick()
    {
        currentTime--;
        UpdateTimeBar();
        if (currentTime == 0) Explode();
        else TickSound();
    }

    void TickSound()
    {
        tickSource.PlayOneShot(tickClip);
    }

    void Update ()
    {
        if (isTicking)
        {
            if (bombTicker >= 1)
            {
                Tick();
                bombTicker = 0;
            }
            else
            {
                bombTicker = Mathf.MoveTowards(bombTicker, 1f, Time.deltaTime);
            }
        }
	}

    public override void OnPay()
    {
        SetPause(true);
        tickSource.PlayOneShot(defuseClip);
        BombManager.instance.DefuseBomb(this);
        StartCoroutine(DefuseSequence());
    }

    public override void InsufficientFunds()
    {
        anim.SetTrigger("CancellationTrigger");
    }

    // sequence of animations as bomb is defused
    IEnumerator DefuseSequence()
    {
        SendAnalytics(true);
        // calls keys from the wand to the bomb
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].SetActive(true);
            var _finalPos = keys[i].transform.position;
            keys[i].transform.rotation.SetLookRotation((WandManager.instance.wandTip.position - _finalPos));
            keys[i].transform.position = WandManager.instance.wandTip.position;
            keys[i].transform.DOMove(_finalPos, 0.3f).OnComplete( ()=> {
                AnimateKey(keys[i]); });
            yield return new WaitForSeconds(0.3f);
        }
        // animates the locks falling
        anim.SetTrigger("DefuseTrigger");
    }

    // tells the key to animate
    void AnimateKey(GameObject _key)
    {
        var _keyAnim = _key.GetComponent<Animator>();
        _keyAnim.enabled = true;
        _keyAnim.SetTrigger("RotateKey");
    }


    // talk to the analytics
    void SendAnalytics(bool _defused)
    {
        var _dataDictionary = new Dictionary<string, object>
        {
            {"bomb_gameStartTime", GameManager.instance.startGameTime },
            {"bomb_ExplosionCounter", bombCount },
            {"bomb_keyCost", cost  },
            {"bomb_silverKeysLeft", KeyManager.instance.keyCount},
            {"bomb_defused", _defused },
            {"bomb_appearedTime", bombAppearedTime },
            {"bomb_endTime", GameManager.instance.GetSecondsLeft() },
            {"bomb_hasGoldenKey", KeyManager.instance.bHasExitKey}
        };
        // send info to analytics
       var _result = Analytics.CustomEvent("bombExploded", _dataDictionary);
        if (_result == AnalyticsResult.Ok)
            Debug.Log("Sending analytics from bomb startTime:" + GameManager.instance.startGameTime + " bomb count " + bombCount);
        else
            Debug.LogError("Analytics were not sent from bomb because " + _result.ToString());
    }
}
