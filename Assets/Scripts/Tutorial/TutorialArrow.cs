using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour {

    // This class controls the arrows that appear in the tutorial

    public GameObject[] arrows;
    public float timeToAppear;

	void Start () {
		for(int i =0; i < arrows.Length; i++)
        {
            arrows[i].SetActive(false);
        }

        StartCoroutine(Appear());
	}
	
	// Update is called once per frame
	IEnumerator Appear () {

        yield return new WaitForSeconds(timeToAppear);
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].SetActive(true);
        }

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
