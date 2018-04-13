using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InitialBillboardScript : MonoBehaviour {

	/**********************************************************
     * This script controls the tweening og the billbaord that
     * delivers the golden key message for the UX experiments
     * ********************************************************/
	void Start () {

        var _targetYPos = transform.position.y;
        var _startYPos = _targetYPos - 4;
        transform.position = new Vector3(transform.position.x, _startYPos, transform.position.z);

        Sequence introSequene = DOTween.Sequence();
        introSequene.AppendInterval(2f).Append(transform.DOLocalMoveY(_targetYPos, 0.4f, false)).AppendInterval(3f).Append(transform.DOLocalMoveY(_startYPos, 0.4f, false));

        
	}
}
