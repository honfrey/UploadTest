using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

/**************************************************
 * This class controls the UI billboard which shows
 * when players gain money or time
 * ************************************************/

public class FloatyBillboard : MonoBehaviour {
    
    [SerializeField]
    protected TextMeshProUGUI value, title;
    [SerializeField]
    protected bool lockYRotation, lockXRotation, lockZRotation, goToCameraPos;
    [SerializeField]
    protected Image[] imageCollection; 
    [SerializeField]
    protected float moveUpTime, holdTime, moveDownTime;
    [SerializeField]
    protected float finalYValue, startYValue;
    protected void Start()
    {
        if(goToCameraPos)
            finalYValue = Camera.main.transform.position.y;
    }

    public void Activate (Vector3 _position, string _title, string _value, Color _textColor) {
        
        if(title!= null)
            title.text = _title;
        if (value != null)
        {
            value.text = _value;
            value.color = _textColor;
        }

        if (goToCameraPos)
            finalYValue = Camera.main.transform.position.y;
        
        transform.position = _position;

        var _lookDir = _position - Camera.main.transform.position;
        
        if (lockXRotation)
            _lookDir.x = 0;
        if (lockYRotation)
            _lookDir.y = 0;
        if (lockZRotation)
            _lookDir.z = 0;

            transform.rotation = Quaternion.LookRotation(_lookDir);

        MoveSequence();
 }

    // if we don't get a position it defaults to a few meters before the camera
    public virtual void Activate(string _title, string _value, Color _textColor)
    {
        var _newPos = FindCorrectPos(5f);
        _newPos.y = startYValue;
        Activate(_newPos, _title, _value, _textColor);
    }

    protected Vector3 FindCorrectPos(float _distance)
    {
        // get camera view and orientation
        var _camTrans = Camera.main.transform;
        // select a point forward
        var _tempPos = _camTrans.position + _camTrans.forward * _distance;
        RaycastHit outHitForward, outHitDown;

        // the final position we'll send our billboard; if you find nothing, just use the normal _tempPos
        Vector3 _finalPos = _tempPos;

        // fire a ray forward for _distance
        if (Physics.Raycast(_camTrans.position, _camTrans.forward, out outHitForward, _distance, (1 << LayerMask.NameToLayer("Walls"))))
        {
            // if we hit a wall, change the temp pos to a bit before that
            _tempPos = outHitForward.point - Vector3.Normalize(_camTrans.forward);
            // find the floor
            Physics.Raycast(_tempPos, -Vector3.up, out outHitDown, 400f);
            _finalPos = outHitDown.point - Vector3.up * 10;
        }
        else if (Physics.Raycast(_camTrans.position, _camTrans.forward, out outHitDown, _distance, (1 << LayerMask.NameToLayer("Ground"))))
        {
            _tempPos = outHitDown.point - Vector3.Normalize(_camTrans.forward);

            // shoot a ray forward; if it hits a wall
            if (Physics.Raycast(outHitDown.point + Vector3.up * 2, _camTrans.forward, out outHitForward, _distance, (1 << LayerMask.NameToLayer("Walls"))))
            {
                // change the final pos to just before the wall
                _finalPos = outHitForward.point - Vector3.Normalize(_camTrans.forward) - Vector3.up * 10;
            }
            else
            {
                // if not just use the camera pos
                _finalPos = _camTrans.position + _camTrans.forward * _distance - Vector3.up * 10;
            }
        } 
        return _finalPos;
    }

    public virtual void SequenceCompleted()
    {
        Destroy(gameObject);
    }

    protected void EnableCorrectImage( int _image)
    {
        for(int i = 0; i< imageCollection.Length; i++)
        {
            imageCollection[i].enabled = false;
        }
        imageCollection[_image].enabled = true;
    }

    public virtual void MoveSequence()
    {
        Sequence introSequene = DOTween.Sequence();
        introSequene.AppendInterval(1f).Append(transform.DOLocalMoveY(finalYValue, moveUpTime, false)).AppendInterval(holdTime).Append(transform.DOLocalMoveY(startYValue, moveDownTime, false)).OnComplete(SequenceCompleted);
    }
}
