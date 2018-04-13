using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance;

    public int pointsPerDollar = 5;
    public int pointsPerKey = 100;
    public int pointsPerBombsFound = 10;
    public int pointsPerBombsDefused = 50;
    public int pointsPerLockBoxesUnlocked = 70;
    public int pointsPerChip = 50;
    public int pointsPerToolUsed = 50;
    public int pointsPerObjectOpened = 70;
    public int pointsPerLife = 50;

    public TextMeshPro titleTMP, numTMP, pointsTMP;
    public TextMesh cashNumText, keysNumText, bombsDefusedNumText, lockBoxesUnlockedNumText, chipNumText, toolNumText, ObjectsOpenedNumText, lifeNumText;
    public TextMesh cashPointsText, keysPointsText, bombsDefusedPointsText, lockBoxesUnlockedPointsText, chipPointsText, toolsPointsText, objectsOpenedPointsText, lifePointsText;
    public TextMesh finalScoreText;

    public Transform scoreTextHolder;

    private ScoreState curScoreState = ScoreState.None;
    private enum ScoreState
    {
        None, Cash, Chips, Bombs, LockBoxes, Tools, Objects, Life, Total
    }

    void Awake()
    {
        instance = this;
        DisplayScoreSetup();
    }

    void DisplayScoreSetup()
    {
        cashPointsText.text = string.Format(cashPointsText.text, pointsPerDollar);
        //keysPointsText.text = string.Format(keysPointsText.text, pointsPerKey);
        bombsDefusedPointsText.text = string.Format(bombsDefusedPointsText.text, pointsPerBombsDefused);
    }

    void Update()
    {
        UpdateScoreText();

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    DisplayScore();
        //}
    }

    public float CalculateScore()
    {
        //TODO Update
        float score = CalculateScore(CashManager.instance.cash, KeyManager.instance.keysFound, BombManager.instance.bombsFound, BombManager.instance.bombsDefused, LockBoxManager.instance.lockBoxesUnlocked, ChipManager.instance.chips);
        return score;
    }

    public int CalculateScore(float _cashFound, int _keysFound, int _bombsFound, int _bombsDefused, int _lockBoxesUnlocked, int _chipsFound)
    {
        int score = 0;
        //score += Mathf.FloorToInt(_cashFound) * pointsPerDollar;
        //score += _keysFound * pointsPerKey;
        //score += _bombsFound * pointsPerBombsFound;
        //score += _bombsDefused * pointsPerBombsDefused;
        //score += _lockBoxesUnlocked * pointsPerLockBoxesUnlocked;
        //score += _chipsFound * pointsPerChip;
        return score;
    }

    float cashNumDisplay = 0;
    //int keysFoundDisplay = 0;
    int bombsDefusedDisplay = 0;
    int lockBoxesUnlockedDisplay = 0;
    int chipsFoundDisplay = 0;
    int toolsUsedDisplay = 0;
    int objectsOpenedDisplay = 0;
    int lifeDisplay = 0;

    int scoreDisplay = 0;

    public void DisplayScore()
    {
        //TODO Get all inputs
        //DisplayScore(CashManager.instance.cash, KeyManager.instance.keysFound, BombManager.instance.bombsFound, BombManager.instance.bombsDefused, ChestManager.instance.chestsUnlocked, ChipManager.instance.chips, 0, GameManager.instance.objectsOpened, GameManager.instance.lives);
    }

    public void DisplayScore(float _cashFound, int _keysFound, int _bombsFound, int _bombsDefused, int _lockBoxesUnlocked, int _chipsFound, int _toolsUsed, int _objectsOpened, int _lifeLeft)
    {
        scoreTextHolder.gameObject.SetActive(true);

        int cashScore = Mathf.FloorToInt(_cashFound) * pointsPerDollar;
        //int keyScore = _keysFound * pointsPerKey;
        //int bombsFoundScore = _bombsFound * pointsPerBombsFound;
        int bombsDefusedScore = _bombsDefused * pointsPerBombsDefused;
        int lockBoxesUnlockedScore = _lockBoxesUnlocked * pointsPerLockBoxesUnlocked;
        int chipScore = _chipsFound * pointsPerChip;
        int toolsUsedScore = _toolsUsed * pointsPerToolUsed;
        int objectsOpenedScore = _objectsOpened * pointsPerObjectOpened;
        int lifeScore = _lifeLeft * pointsPerLife;
        int score = cashScore + bombsDefusedScore + lockBoxesUnlockedScore + chipScore + toolsUsedScore + objectsOpenedScore + lifeScore;

        cashPointsText.text = "+ " + cashScore.ToString();
        //keysPointsText.text = "+ " + keyScore.ToString();
        bombsDefusedPointsText.text = "+ " + bombsDefusedScore.ToString();
        lockBoxesUnlockedPointsText.text = "+ " + lockBoxesUnlockedScore.ToString();
        chipPointsText.text = "+ " + chipScore.ToString();
        toolsPointsText.text = "+ " + toolsUsedScore.ToString();
        objectsOpenedPointsText.text = "+ " + objectsOpenedScore.ToString();
        lifePointsText.text = "+ " + lifeScore.ToString();

        float duration = 2f;
        float punchDuration = 0.5f;
        float punch = 0.2f;
        Vector3 punchScale = new Vector3(punch, punch, punch);

        Sequence sequence = DOTween.Sequence();
        curScoreState = ScoreState.Cash;
        sequence.Append(DOTween.To(() => cashNumDisplay, x => cashNumDisplay = x, _cashFound, duration).SetEase(Ease.InSine).OnComplete(OnFinishCashTally));
        sequence.Append(cashNumText.transform.DOPunchScale(punchScale, punchDuration).OnComplete(OnFinish));

        sequence.Append(DOTween.To(() => chipsFoundDisplay, x => chipsFoundDisplay = x, _chipsFound, duration).SetEase(Ease.InSine).OnComplete(OnFinishChipTally));
        sequence.Append(chipNumText.transform.DOPunchScale(punchScale, punchDuration).OnComplete(OnFinish));

        sequence.Append(DOTween.To(() => bombsDefusedDisplay, x => bombsDefusedDisplay = x, _bombsDefused, duration).SetEase(Ease.InSine).OnComplete(OnFinishBombDefuseTally));
        sequence.Append(bombsDefusedNumText.transform.DOPunchScale(punchScale, punchDuration).OnComplete(OnFinish));

        sequence.Append(DOTween.To(() => lockBoxesUnlockedDisplay, x => lockBoxesUnlockedDisplay = x, _lockBoxesUnlocked, duration).SetEase(Ease.InSine).OnComplete(OnFinishlockBoxesUnlockedTally));
        sequence.Append(lockBoxesUnlockedNumText.transform.DOPunchScale(punchScale, punchDuration).OnComplete(OnFinish));
        
        sequence.Append(DOTween.To(() => toolsUsedDisplay, x => toolsUsedDisplay = x, _toolsUsed, duration).SetEase(Ease.InSine).OnComplete(OnFinishToolsTally));
        sequence.Append(toolNumText.transform.DOPunchScale(punchScale, punchDuration).OnComplete(OnFinish));

        sequence.Append(DOTween.To(() => objectsOpenedDisplay, x => objectsOpenedDisplay = x, _objectsOpened, duration).SetEase(Ease.InSine).OnComplete(OnFinishObjectsOpenedTally));
        sequence.Append(ObjectsOpenedNumText.transform.DOPunchScale(punchScale, punchDuration).OnComplete(OnFinish));

        sequence.Append(DOTween.To(() => lifeDisplay, x => lifeDisplay = x, _lifeLeft, duration).SetEase(Ease.InSine).OnComplete(OnFinishLifeTally));
        sequence.Append(lifeNumText.transform.DOPunchScale(punchScale, punchDuration).OnComplete(OnFinish));

        sequence.Append(DOTween.To(() => scoreDisplay, x => scoreDisplay = x, score, 0.5f).SetEase(Ease.InSine).OnComplete(OnFinishScoreTally));
        sequence.Append(finalScoreText.transform.DOPunchScale(punchScale, punchDuration));
    }

    void OnFinishCashTally()
    {
        cashPointsText.gameObject.SetActive(true);
    }

    void OnFinish ()
    {
        StartCoroutine(NextState());
    }

    IEnumerator NextState()
    {
        yield return new WaitForSeconds(1f);
        curScoreState += 1;
    }

    void OnFinishChipTally ()
    {
        chipPointsText.gameObject.SetActive(true);
    }

    void OnFinishToolsTally()
    {
        toolsPointsText.gameObject.SetActive(true);
    }

    void OnFinishBombDefuseTally()
    {
        bombsDefusedPointsText.gameObject.SetActive(true);
    }

    void OnFinishlockBoxesUnlockedTally()
    {
        lockBoxesUnlockedPointsText.gameObject.SetActive(true);
    }

    void OnFinishObjectsOpenedTally()
    {
        objectsOpenedPointsText.gameObject.SetActive(true);
    }

    void OnFinishLifeTally ()
    {
        lifePointsText.gameObject.SetActive(true);
    }

    void OnFinishScoreTally()
    {

    }

    void UpdateScoreText()
    {
        cashNumText.text = "$" + cashNumDisplay.ToString("F2");
        chipNumText.text = chipsFoundDisplay.ToString();
        //keysNumText.text = keysFoundDisplay.ToString();
        bombsDefusedNumText.text = bombsDefusedDisplay.ToString();
        lockBoxesUnlockedNumText.text = lockBoxesUnlockedDisplay.ToString();
        toolNumText.text = toolsUsedDisplay.ToString();
        ObjectsOpenedNumText.text = objectsOpenedDisplay.ToString();
        lifeNumText.text = lifeDisplay.ToString();

        finalScoreText.text = scoreDisplay.ToString();

        switch (curScoreState)
        {
            case ScoreState.None:
                UpdateBonusRoomText("", "", "");
                break;
            case ScoreState.Cash:
                UpdateBonusRoomText("CASH FOUND:", "$" + cashNumDisplay.ToString("F2"), cashPointsText.text);
                break;
            case ScoreState.Chips:
                UpdateBonusRoomText("CHIPS FOUND:", chipsFoundDisplay.ToString(), chipPointsText.text);
                break;
            case ScoreState.Bombs:
                UpdateBonusRoomText("BOMBS DEFUSED:", bombsDefusedDisplay.ToString(), bombsDefusedPointsText.text);
                break;
            case ScoreState.LockBoxes:
                UpdateBonusRoomText("CHESTS OPENED:", lockBoxesUnlockedDisplay.ToString(), lockBoxesUnlockedPointsText.text);
                break;
            case ScoreState.Tools:
                UpdateBonusRoomText("TOOLS USED:", toolsUsedDisplay.ToString(), toolsPointsText.text);
                break;
            case ScoreState.Objects:
                UpdateBonusRoomText("OBJECTS OPENED:", objectsOpenedDisplay.ToString(), objectsOpenedPointsText.text);
                break;
            case ScoreState.Life:
                UpdateBonusRoomText("LIVES LEFT:", lifeDisplay.ToString(), lifePointsText.text);
                break;
            case ScoreState.Total:
                UpdateBonusRoomText("SCORE:", scoreDisplay.ToString(), "");
                break;
        }
    }

    void UpdateBonusRoomText (string title, string num, string points)
    {
        if (titleTMP != null) titleTMP.text = title;
        if (numTMP != null) numTMP.text = num;
        if (pointsTMP != null) pointsTMP.text = points;
    }

}
