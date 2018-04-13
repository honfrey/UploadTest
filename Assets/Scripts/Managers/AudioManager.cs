using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    //Env Sound
    public AudioClip clipDayEnv;
    public AudioClip clipNightEnv;
    //Dialogue
    public AudioClip clipDiag_Start1;
    public AudioClip clipDiag_Start2;
    public AudioClip clipDiag_Normal_Countdown;
    //Effect Sound
    public AudioClip clipClickJoint;
    public AudioClip clipSmoke;
    public AudioClip clipCoughing;
    //
    public AudioClip clipClockTic;
    public AudioSource sourceClock;

    Coroutine CoroutineClockTic;
    int ClockTicCount = 5;
    float ClockTicInterval = 3.0f;

    void Awake()
    {
        instance = this;
    }

    void PlayClip(AudioSource source, AudioClip clip)
    {
        source.Play();
    }

    public void PlayUnlockSound()
    {

    }

    public void ChangeToBackground()
    {

    }

    public void LoseGame(float fadeDuration)
    {

    }

    public void WinGame()
    {

    }

    public void PlaySlideShowClickClip()
    {

    }

    public void PlayClockTicToc(float intervalTime, int totalCount)
    {
        ClockTicInterval = intervalTime;
        ClockTicCount = totalCount;
        CoroutineClockTic = StartCoroutine(ClockTic());
    }

    public void  StopClockTic()
    {
        StopCoroutine(CoroutineClockTic);
    }

    private IEnumerator ClockTic()
    {
        do
        {
            if(sourceClock && clipClockTic)
            {
                sourceClock.clip = clipClockTic;
                sourceClock.Play();
            }
            ClockTicCount--;
            yield return new WaitForSeconds(ClockTicInterval);
        }
        while (ClockTicCount > 0);
    }




}
