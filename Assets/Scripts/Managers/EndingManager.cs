using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using DG.Tweening;
using TMPro;

public class EndingManager : MonoBehaviour {

    public static EndingManager instance;
    
    public int pointsPerBombsDefused = 70;
    public int pointsPerChip = 1;
    public int pointsPerToolUsed = 50;
    public int pointsPerObjectOpened = 70;
    public int pointsPerSecondsLeft = 50;

    public TextMeshPro tierNumTMP, timeLeftTMP, timeScoreTMP, runningScoreTMP, bombsTMP, BombsScoreTMP, chipsTMP, chipsScoreTMP, objectsTMP, objectsScoreTMP, cashTMP, cashScoreTMP;
    public TextMeshPro tierNum2TMP, ticketsTMP, score3TMP, cash3TMP, tickets3TMP, loseScoreTMP, loseCashTMP;
    public Image tierProgressBar;
    public ParticleSystem bonusChestPS;
    public ParticleSystem ticketsPS;
    public GameObject[] bonusChests;
    public GameObject winBG, loseBG, endScreens, endScreen1, winText1, loseText1, timeScreen, bombsScreen, chipsScreen, objectsScreen, cashScreen;
    public GameObject endScreen2, tierGO, slotsGO, endScreen3, winScreen3, loseScreen3;
    
    private int curChestIndex = 0;
    private int ticketsWon = 0;
    private int runningScore = 0;

    void Awake()
    {
        if (!TutorialManager.instance.GetIsTutorialEnabled())
        {
            instance = this;
            //DisplayScoreSetup();
            endScreens.SetActive(false);
            for (int i = 0; i < bonusChests.Length; i++)
            {
                if (i == curChestIndex) { bonusChests[i].SetActive(true); }
                else { bonusChests[i].SetActive(false); }
            }
        }
    }
    
    void Update()
    {
        if (!TutorialManager.instance.GetIsTutorialEnabled())
        {
            //UpdateScoreText();
            if (tierProgressBar.gameObject.activeInHierarchy)
            {
                UpdateProgressDisplay();
            }
            else if (slotsGO.activeInHierarchy) { ticketsTMP.text = ticketDisplay.ToString(); }    
        }
    }

    int scoreDisplay = 0;
    int ticketDisplay = 0;

    public void DisplayScore()
    {
        //Get all inputs
     //   StartCoroutine(DisplayWinScore(CashManager.instance.cash, BombManager.instance.bombsDefused, ChipManager.instance.chips, GameManager.instance.objectsOpened, GameManager.instance.GetSecondsLeft()));
    }

    public IEnumerator DisplayWinScore(float _cashFound, int _bombsDefused, int _chipsFound, int _objectsOpened, int _timeLeft)
    {
        //print("Displaying score!!");
        //Hiding chests and bombs
        ChestManager.instance.HideChests();
        BombManager.instance.HideBombs();
        //Setup
        endScreen2.SetActive(false);
        endScreen3.SetActive(false);
        bombsScreen.SetActive(false);
        chipsScreen.SetActive(false);
        //toolsScreen.SetActive(false);
        objectsScreen.SetActive(false);
        cashScreen.SetActive(false);
        endScreens.SetActive(true);
        
        //*FIRST END SCREEN*//
        //Time
        runningScoreTMP.text = runningScore.ToString();
        tierNumTMP.text = "1";
#if false
        if (GameManager.instance.state == GameManager.State.escape)
        {
            winText1.SetActive(true);
            winBG.SetActive(true);
            loseText1.SetActive(false);
            loseBG.SetActive(false);
        }
        else
        {
            winText1.SetActive(false);
            winBG.SetActive(false);
            loseText1.SetActive(true);
            loseBG.SetActive(true);
            _timeLeft = 0;
        }
#endif
        timeLeftTMP.text = string.Format("{0}:{1:00}", _timeLeft / 60, _timeLeft % 60);
        int timeScore = _timeLeft * pointsPerSecondsLeft;
        timeScoreTMP.text = "+" + timeScore.ToString();
        endScreen1.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        runningScore += timeScore;
        yield return DOTween.To(() => scoreDisplay, x => scoreDisplay = x, runningScore, Mathf.Max(1f, _timeLeft/60f)).SetEase(Ease.OutSine).WaitForCompletion(); //Time score lerp is at least 1s or 1s per minute left
        //Bombs
        bombsTMP.text = _bombsDefused.ToString();
        int bombsDefusedScore = _bombsDefused * pointsPerBombsDefused;
        BombsScoreTMP.text = "+" + bombsDefusedScore.ToString();
        bombsScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        runningScore += bombsDefusedScore;
        yield return DOTween.To(() => scoreDisplay, x => scoreDisplay = x, runningScore, Mathf.Max(1f, _bombsDefused/2f)).SetEase(Ease.OutSine).WaitForCompletion(); //Bomb score lerp is at least 1s or 1s per 2 bombs defused
        //Objects
        objectsTMP.text = _objectsOpened.ToString();
        int objectsOpenedScore = _objectsOpened * pointsPerObjectOpened;
        objectsScoreTMP.text = "+" + objectsOpenedScore.ToString();
        objectsScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        runningScore += objectsOpenedScore;
        yield return DOTween.To(() => scoreDisplay, x => scoreDisplay = x, runningScore, Mathf.Max(1f, _objectsOpened/20f)).SetEase(Ease.OutSine).WaitForCompletion(); //Objects score lerp is at least 1s or 1s per 20 objects opened
        //Chips
        chipsTMP.text = _chipsFound.ToString();
        int chipScore = _chipsFound * pointsPerChip;
        chipsScoreTMP.text = "+" + chipScore.ToString();
        chipsScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        runningScore += chipScore;
        yield return DOTween.To(() => scoreDisplay, x => scoreDisplay = x, runningScore, Mathf.Max(1f, _chipsFound / 2000f)).SetEase(Ease.OutSine).WaitForCompletion();   //Chip score lerp time is chip num / 2000
        //Cash
        cashTMP.text = _cashFound.ToString("C");
        //cashScoreTMP.text = cashScore.ToString();
        cashScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        //Pause before transition to next screen
        yield return new WaitForSeconds(2f);

#if false
        if (GameManager.instance.state == GameManager.State.escape)
        {
            //*SECOND END SCREEN*//
            endScreen1.SetActive(false);
            tierGO.SetActive(false);
            slotsGO.SetActive(false);
            endScreen2.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            tierNum2TMP.text = tierNumTMP.text;
            tierGO.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            ticketsTMP.text = "0";
            slotsGO.SetActive(true);
            ticketsWon = GameManager.instance.GetTicketsFromTier(tierNumTMP.text);
            yield return DOTween.To(() => ticketDisplay, x => ticketDisplay = x, ticketsWon, 3f).SetEase(Ease.InOutQuart).WaitForCompletion();
            //Play chest animation and particles
            foreach (var chest in bonusChests)
            {
                if (chest.activeSelf) { chest.GetComponent<Animator>().SetTrigger("open"); break; }
            }
            yield return new WaitForSeconds(1.4f);
            //Pause before transition
            yield return new WaitForSeconds(2f);
        }
#endif

        //*THIRD END SCREEN*//
        endScreen1.SetActive(false);
        endScreen2.SetActive(false);
#if false
        if (GameManager.instance.state == GameManager.State.escape)
        {
            score3TMP.text = runningScore.ToString("N0");
            cash3TMP.text = cashTMP.text;
            tickets3TMP.text = ticketsTMP.text;
            winScreen3.SetActive(true);
            loseScreen3.SetActive(false);
            SendAnalytics(true);
        }
        else
        {
            loseScoreTMP.text = runningScore.ToString();
            loseCashTMP.text = cashTMP.text;
            winScreen3.SetActive(false);
            loseScreen3.SetActive(true);
            SendAnalytics(false);
        }
#endif
        endScreen3.SetActive(true);
    }

    private void UpdateProgressDisplay ()
    {
#if false
        runningScoreTMP.text = scoreDisplay.ToString("N0");
        if (scoreDisplay < GameManager.tier2Threshold)
        {
            tierProgressBar.fillAmount = (scoreDisplay) / (GameManager.tier2Threshold);
            tierNumTMP.text = "1";
        } else if (scoreDisplay < GameManager.tier3Threshold)
        {
            tierProgressBar.fillAmount = (scoreDisplay - GameManager.tier2Threshold) / (GameManager.tier3Threshold - GameManager.tier2Threshold);
            tierNumTMP.text = "2";
            if (curChestIndex < 1) { UpgradeBonusChest(); }
        } else if (scoreDisplay < GameManager.tier4Threshold)
        {
            tierProgressBar.fillAmount = (scoreDisplay - GameManager.tier3Threshold) / (GameManager.tier4Threshold - GameManager.tier3Threshold);
            tierNumTMP.text = "3";
            if (curChestIndex < 2) { UpgradeBonusChest(); }
        } else if (scoreDisplay < GameManager.tier5Threshold)
        {
            tierProgressBar.fillAmount = (scoreDisplay - GameManager.tier4Threshold) / (GameManager.tier5Threshold - GameManager.tier4Threshold);
            tierNumTMP.text = "4";
            if (curChestIndex < 3) { UpgradeBonusChest(); }
        } else
        {
            tierProgressBar.fillAmount = 1f;
            tierNumTMP.text = "5";
            if (curChestIndex < 4) { UpgradeBonusChest(); }
        }
#endif
    }

    private void UpgradeBonusChest ()
    {
        if (curChestIndex < bonusChests.Length - 1)
        {
            curChestIndex++;
            for (int i = 0; i < bonusChests.Length; i++)
            {
                if (i == curChestIndex)
                {
                    //Show new chest
                    bonusChests[i].SetActive(true);
                    //Remove old chest
                    if (i > 0)
                        bonusChests[i - 1].SetActive(false);
                    //Flashy upgrade FX
                    if (bonusChestPS.IsAlive())
                        bonusChestPS.Stop();
                    bonusChestPS.Play();
                }
                else { bonusChests[i].SetActive(false); }
            }
        } else
        {
            Debug.LogError("Error! Trying to upgrade chest beyond available tier levels!");
        }
    }

    public void PlayTicketAnim()
    {
        ticketsPS.Play();
    }

    // Collects info and sends it to analytics
    private void SendAnalytics(bool _playerWon)
    {
        //Collect info
        LocalAnalytics newLocalAnalytics = new LocalAnalytics()
        {
            startTime = GameManager.instance.startGameTime.ToString(),
            playerWon = _playerWon,
            hasGoldenKey = KeyManager.instance.bHasExitKey,
            keysFound = KeyManager.instance.keysFound,
            keysLeft = KeyManager.instance.keyCount,
            timeTaken = (GameManager.instance.endGameTime - GameManager.instance.startGameTime).TotalMinutes,
            timeLeft = Mathf.Max(0, (int)(GameManager.instance.GetSecondsLeft())),
            chestsFound = ChestManager.instance.chestsFound,
            chestsOpened = ChestManager.instance.chestsUnlocked,
            bombsFound = BombManager.instance.bombsFound,
            bombsDefused = BombManager.instance.bombsDefused,
            gemsFound = ChipManager.instance.chips,
            //objectsOpened = GameManager.instance.objectsOpened,
            score = runningScore,
            ticketsWon = ticketsWon,
            cashWon = CashManager.instance.cash
        };
        //Save analytics to device
        string jsonToFile = JsonUtility.ToJson(newLocalAnalytics, true) + "\n";
        string filePath = Path.Combine(Application.persistentDataPath, "Analytics.json");
        File.AppendAllText(filePath, jsonToFile);
        //Setup analytics for Unity Analytics
        var _dataDictionary1 = new Dictionary<string, object>
        {
            {"startTime", newLocalAnalytics.startTime },
            {"playerWon", newLocalAnalytics.playerWon },
            {"hasGoldenKey", newLocalAnalytics.hasGoldenKey },
            {"keysFound",  newLocalAnalytics.keysFound },
            {"keysLeft",  newLocalAnalytics.keysLeft },
            {"timeTaken",  newLocalAnalytics.timeTaken },
            {"timeLeft",  newLocalAnalytics.timeLeft },
            {"chestsFound",  newLocalAnalytics.chestsFound },
            {"chestsOpened",  newLocalAnalytics.chestsOpened },
        };
        var _dataDictionary2 = new Dictionary<string, object>
        {
            {"startTime", newLocalAnalytics.startTime },
            {"bombsFound",  newLocalAnalytics.bombsFound },
            {"bombsDefused", newLocalAnalytics.bombsDefused },
            {"gemsFound", newLocalAnalytics.gemsFound },
            {"objectsOpened", newLocalAnalytics.objectsOpened },
            {"score", newLocalAnalytics.score },
            {"ticketsWon", newLocalAnalytics.ticketsWon },
            {"cashWon", newLocalAnalytics.cashWon }
        };
        //Send info package 1 to Unity Analytics
        var _result = Analytics.CustomEvent("gameOver", _dataDictionary1);
        if (_result == AnalyticsResult.Ok)
        {
            Debug.Log("Sending analytics package 1 from Ending Manager startTime:" + GameManager.instance.startGameTime);
            //Send info package 2 to Unity Analytics
            _result = Analytics.CustomEvent("gameOver", _dataDictionary2);
            if (_result == AnalyticsResult.Ok)
                Debug.Log("Sending analytics package 2 from Ending Manager startTime:" + GameManager.instance.startGameTime);
            else
                Debug.LogError("Analytics package 2 was not sent from ending Manager because " + _result.ToString());
        }
        else
            Debug.LogError("Analytics package 1 was not sent from ending Manager because " + _result.ToString());
    }

    //private void SaveAnalytics(bool didPlayerWin)
    //{
        //LocalAnalytics newLocalAnalytics = new LocalAnalytics()
        //{
        //    startTime = GameManager.instance.startGameTime,
        //    playerWon = didPlayerWin,
        //    hasGoldenKey = KeyManager.instance.bHasExitKey,
        //    keysFound = KeyManager.instance.keysFound,
        //    keysLeft = KeyManager.instance.keyCount,
        //    timeLeft = Mathf.Max(0, GameManager.instance.GetSecondsLeft()),
        //    chestsFound = ChestManager.instance.chestsFound,
        //    chestsOpened = ChestManager.instance.chestsUnlocked,
        //    bombsFound = BombManager.instance.bombsFound,
        //    bombsDefused = BombManager.instance.bombsDefused,
        //    gemsFound = ChipManager.instance.chips,
        //    objectsOpened = GameManager.instance.objectsOpened,
        //    score = runningScore,
        //    ticketsWon = ticketsWon,
        //    cashWon = CashManager.instance.cash
        //};
        ////Serialize object to JSON
        //string jsonToFile = JsonUtility.ToJson(newLocalAnalytics, true) + "\n";
        //Debug.Log(jsonToFile);
        ////Save JSON to file in default path
        //string filePath = Path.Combine(Application.persistentDataPath, "Analytics.json");
        //File.AppendAllText(filePath, jsonToFile);
    //}

    /// <summary>
    /// Class for saving analytics to local device
    /// </summary>
    [System.Serializable]
    public class LocalAnalytics
    {
        public string startTime;
        public bool playerWon;
        public bool hasGoldenKey;
        public int keysFound;
        public int keysLeft;
        public double timeTaken;
        public int timeLeft;
        public int chestsFound;
        public int chestsOpened;
        public int bombsFound;
        public int bombsDefused;
        public int gemsFound;
        public int objectsOpened;
        public int score;
        public int ticketsWon;
        public float cashWon;
    }
}
