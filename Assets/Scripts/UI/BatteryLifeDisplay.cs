using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BatteryLifeDisplay : MonoBehaviour {

	/*********************************
     * Displays the battery life of the device
     * *******************************/

	void OnEnable () {
        var _textMesh = this.GetComponent<TextMeshPro>();

        _textMesh.text = (SystemInfo.batteryLevel * 100).ToString() + "% battery";
	}
	
}
