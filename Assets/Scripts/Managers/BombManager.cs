using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BombManager : MonoBehaviour {

    public static BombManager instance;
    public bool shouldBombsDecreaseTime = false;
    public int timePenaltyPerKey = 10;
    public GameObject[] bombPrefabs;
    public SpriteRenderer explosionScreen, fullRed;
    public ParticleSystem explosionParticles;
    public Projector alarmProjector;
    public GameObject timeLostUIBillboard;

    [Header("Bombs")]
    public List<BombType> bombTypes;

    [System.Serializable]
    public struct BombType
    {
        public int time;
        public int defuseCost;
    }

    [HideInInspector]
    public List<Bomb> bombs = new List<Bomb>();

    [HideInInspector]
    public int bombsFound = 0, bombsDefused = 0;
    
    private Sequence alarmSequence;

    void Awake()
    {
        instance = this;

        explosionScreen.enabled = true;
        explosionScreen.material.DOFade(0, 0);
        fullRed.enabled = true;
        fullRed.material.DOFade(0, 0);
    }

	void Start () {
        TurnOffAlarm();
    }
    
    public void BombExplode(int cost = 0)
    {
#if false
        if (GameManager.instance.state != GameManager.State.bomb)
        {
            bool stillAlive = true;
            
            if (shouldBombsDecreaseTime && cost > 0)
            {
                int timeCost = cost * timePenaltyPerKey;   //Time lost is equivalent to (key cost x timePenaltyPerKey)

                // create a billboard to show time lost
                if (TutorialManager.instance.GetIsTutorialEnabled())
                {
                    GameObject _billboard = (GameObject)Instantiate(timeLostUIBillboard, Vector3.zero, Quaternion.identity);
                    _billboard.GetComponent<FloatyBillboard>().Activate("Careful", "Use silver keys to defuse bombs", Color.black);
                }
                else
                    StartCoroutine(BillboardManager.instance.SendNewUIMessage("You Lost ", "-" + timeCost.ToString(), Color.red, 1.8f));

                GameManager.instance.LoseTime(timeCost);
                //AudioManager.instance.PlayExplosion();
            }
            else
            {
                stillAlive = GameManager.instance.LoseLife();
            }
           
            if (stillAlive)
            {
                if (alarmProjector != null)
                    Alarm(false);
            }
            else if (GameManager.instance.state == GameManager.State.game)
            {
                SetPause(true);
                if(alarmProjector != null)
                     Alarm(true);
                //StartCoroutine(GameManager.instance.LoseGame()); 
            }
        }
#endif
    }

    public void SetPause(bool set)
    {
        foreach (Bomb b in bombs)
        {
            b.SetPause(set);
        }
    }

    public BombType CreateBomb (int keyCost)
    {
        BombType newBomb = new BombType();
        newBomb.defuseCost = keyCost;
        newBomb.time = keyCost * timePenaltyPerKey;
        bombTypes.Add(newBomb);
        return newBomb;
    }

    public void ReleaseBomb (BombType theBomb, Transform releaseTransform)
    {
        bombsFound++;
        int time = 20;
        // spawn bomb
        Vector3 spawnPosition = releaseTransform.position + (releaseTransform.forward * 1f);
        Vector3 spawnRotation = releaseTransform.rotation.eulerAngles;
        spawnRotation.x = 0f;
        spawnRotation.z = 0f;
        Bomb newBomb = Instantiate(bombPrefabs[Mathf.Max(0, theBomb.defuseCost-1)], spawnPosition, Quaternion.Euler(spawnRotation)).GetComponent<Bomb>();
        bombs.Add(newBomb);
        //bomb setup
        newBomb.SetupBomb(time, theBomb.defuseCost, bombsFound);

        // jump onto the ground
        /* float distanceAway = 1f;
         Vector3 targetPosition = spawnPosition + (releaseTransform.forward * distanceAway);
         targetPosition.y = 0;
         newBomb.transform.DOJump(targetPosition, 1f, 1, 1f); */
         
        Rigidbody rb = newBomb.GetComponent<Rigidbody>();
        rb.AddForce(releaseTransform.forward * 250);
        rb.AddExplosionForce(150, releaseTransform.position, 100, 1, ForceMode.Force);
        newBomb.audioObject.PlayRelease();
    }
    
    public void DefuseBomb(Bomb bomb)
    {
        bombsDefused++;
        bombs.Remove(bomb);
    }

    public void RemoveBomb(Bomb bomb)
    {
        if (bombs.Contains(bomb))
            bombs.Remove(bomb);
    }

    public void HideBombs ()
    {
        foreach (var b in bombs)
        {
            if (b != null)
                b.gameObject.SetActive(false);
        }
    }

    public void Alarm(bool _isLoseScreen)
    {
        alarmProjector.gameObject.SetActive(true);
        int _loops = 3;
        if (_isLoseScreen) _loops = -1;
        Color _alarmOnColor = Color.red;
        Color _alarmOffColor = Color.black;
        float _alarmTweenDuration = 0.4f;
        alarmSequence = DOTween.Sequence();
        
        alarmSequence.SetLoops(_loops).Append(alarmProjector.material.DOColor(_alarmOffColor, 0.1f).OnComplete(AlarmAudio)).Append(alarmProjector.material.DOColor(_alarmOnColor, _alarmTweenDuration)).AppendInterval(0.2f).
            Append(alarmProjector.material.DOColor(_alarmOffColor, _alarmTweenDuration)).OnComplete(TurnOffAlarm);
    }

    void TurnOffAlarm()
    {
        if(alarmProjector != null)
            alarmProjector.gameObject.SetActive(false);
    }

    void AlarmAudio()
    {
      //AudioManager.instance.PlayAlarmClip();
    }

    private void OnDisable()
    {
        alarmSequence.Kill();
    }
}
