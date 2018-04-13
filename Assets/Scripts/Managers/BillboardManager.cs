using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BillboardManager : MonoBehaviour {

    /************************************************
     * This class controls the billboards that come in front 
     * of the camera; making sure they don't overlap
     * **********************************************/

    public static BillboardManager instance;

    [SerializeField]
    private Transform cameraBillboardOrigin;
    [SerializeField]
    private GameObject cameraBillboardGO;

    private FloatyBillboard currentBillboard, nextBillboard;
    
    void Awake()
    {
        instance = this;
    }
	
	public IEnumerator SendNewUIMessage (string _title, string _message, Color _textColor, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        GameObject _billboard = (GameObject)Instantiate(cameraBillboardGO, Vector3.zero, Quaternion.identity);
        _billboard.transform.parent = cameraBillboardOrigin;

        currentBillboard = _billboard.GetComponent<FloatyBillboard>();
        currentBillboard.GetComponentInChildren<TextMeshPro>().text = _message;
        currentBillboard.GetComponentInChildren<TextMeshPro>().color = _textColor;

        currentBillboard.Activate(cameraBillboardOrigin.position, _title, _message, _textColor);
    }
}
