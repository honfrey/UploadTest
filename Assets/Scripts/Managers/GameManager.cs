using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        Tutorial,
        PreGame,
        Game,
        Ending,
        Credits,
        None,
    }
    public State state = State.Tutorial;

    public enum FinishType
    {
        Win,
        Lose,
    }

    [HideInInspector]
    public static GameManager instance;
    public float gameSpeed = 2.0f;
    public float gameTimeSeconds = 300.0f;
    private float timeLeft = 300.0f;
    public TextMesh textmeshTimer;

    public float timeClockTicInterval = 3.0f;
    public int countClockTic = 11;

    public float timeEuphoriaStart = 0.0f;
    public float timeParanoiaStart = 150.0f;

    public ParticleSystem particleSmoke;

    public SpriteRenderer fullBlack;


    [HideInInspector]
    public System.DateTime startGameTime, endGameTime;


    void Awake()
    {
        instance = this;

        // set a random value as the user ID
        Analytics.SetUserId((Random.value * 1000).ToString());
    }

    void Start() {
        FadeOutBlack(1);
        SetState(State.Tutorial);
        Application.targetFrameRate = 60;
        startGameTime = System.DateTime.Now;
    }

    void Update()
    {
        Time.timeScale = gameSpeed;
        switch(state)
        {
            case State.Tutorial:
                if(Input.GetKeyDown(KeyCode.A))
                {
                    SetState(State.Game);
                }
                break;
            case State.Game:
                if(Input.GetKeyDown(KeyCode.B))
                {
                    OnClickSmoke();
                }

                if (timeLeft < 0)
                {
                    SetState(State.Ending);
                }
                else
                {
                    timeLeft -= Time.deltaTime;
                    UpdateTimerText();
                }
                break;
            case State.Ending: break;
            default:  break;
        }
    }

    public void SetState(State _state)
    {
        switch (_state)
        {
            case State.Tutorial: 
                StartTutorial();
                break;
            case State.PreGame: break;
            case State.Game:
                StartCoroutine(BeginParanoiaCo());
                StartCoroutine(BeginEuphoriaCo());
                break;
            case State.Ending:
                StartEnding(CheckToDoObjectives());
                break;

            case State.Credits:break;
            default: break;
        }
        state = _state;
    }

    void UpdateTimerText()
    {
        if (timeLeft < 0) timeLeft = 0;
        if(textmeshTimer)
        {
            textmeshTimer.text = SecondsToString(timeLeft);
        }
    }

    string SecondsToString(float sec)
    {
        int second = Mathf.FloorToInt(sec);
        int minutes = second / 60;
        int seconds = second % 60;
        return string.Format("{0}:{1:00}", minutes, seconds);
    }

    public float GetSecondsLeft()
    {
        return timeLeft;
    }

    public void FadeInBlack(float duration)
    {
        Color c = fullBlack.material.color;
        c.a = 0;
        fullBlack.material.color = c;

        fullBlack.enabled = true;
        fullBlack.material.DOFade(1f, duration);
    }

    public void FadeOutBlack(float duration)
    {
        fullBlack.enabled = true;
        fullBlack.material.DOFade(0, duration);
    }

    public void OnClickSmoke()
    {
        Debug.Log("Smoke!");
        if(state == State.PreGame)
        {

        }
        else if(state == State.Game)
        {

        }
        TriggerTimeSlowEffect();
    }

    public void SetEnableMove(bool enable)
    {

    }

    public bool GetEnableMove()
    {
        return true;
    }

    public bool CheckToDoObjectives()
    {
        return false;
    }

    public void RoomSetup()
    {
        
    }

    public void StartTimer()
    {
        timeLeft = gameTimeSeconds;
    }

    public void StartTutorial()
    {
        Debug.Log("Tutorial Start!");
    }


    public void StartEnding(bool bWin)
    {
        if(bWin)
        {
            Debug.Log("Ending! Win!");
        }
        else
        {
            Debug.Log("Ending! Lose!");
        }
    }

    public IEnumerator StartNewGame()
    {
        float fadeDuration = 1;
        FadeInBlack(fadeDuration);

        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(0);
    }

    public IEnumerator BeginParanoiaCo()
    {
        yield return new WaitForSeconds(timeParanoiaStart);
        TriggerParanoiaEffect();
    }

    public IEnumerator BeginEuphoriaCo()
    {
        yield return new WaitForSeconds(timeEuphoriaStart);
        TriggerEuphoriaEffect();
    }

    public void TriggerParanoiaEffect()
    {
        Debug.Log("Paranoia Start!");
    }

    public void TriggerEuphoriaEffect()
    {
        SideEffectsManager.Instance.StartSideEffect(SideEffectsManager.ESideEffectType.SET_EUPHORIA);
        Debug.Log("Euphoria Start!");
    }

    public void TriggerTimeSlowEffect()
    {
        AudioManager.instance.PlayClockTicToc(timeClockTicInterval, countClockTic);
    }
}
