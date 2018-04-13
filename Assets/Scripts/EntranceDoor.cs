using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class EntranceDoor : GazeListener { 
    public Animation doorAnim;
    public Transform doorUI;
    
    public override void OnClick()
    {
        AudioManager.instance.PlayUnlockSound();
        doorUI.gameObject.SetActive(false);
        StartCoroutine(EnterTheRoom());
    }

    IEnumerator EnterTheRoom()
    {
        //GameManager.instance.SetGazeTapControl(false);

        doorAnim.Play("open");
        yield return new WaitForSeconds(doorAnim["open"].length);

        float fadeDuration = 1f;
        GameManager.instance.FadeInBlack(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        SceneManager.LoadScene(1);
    }
}
