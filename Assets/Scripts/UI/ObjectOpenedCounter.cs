using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ObjectOpenedCounter : MonoBehaviour {

    private TextMeshPro myText;

	// Use this for initialization
	void Start () {
        myText = this.GetComponentInChildren<TextMeshPro>();
	}
	
	// Update is called once per frame
	void Update () {
      //  myText.text = GameManager.instance.objectsOpened.ToString() + " out of 60 objects found.";
    }
}
