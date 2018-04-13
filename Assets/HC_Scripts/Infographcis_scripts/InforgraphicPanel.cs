using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class InfographicInfo
{
    public Texture info_image;
}

public class InforgraphicPanel : MonoBehaviour {

	/*******************************************
     * This class controls the infographic panels'
     * appearance and disappearance; and making sure
     * the correct information appears
     * *****************************************/
    
        // list of all the sideffects
    public enum SideEffect
    {
        memoryLoss = 0,
        paranoia = 1,
        sleepiness = 2,
        timePerception = 3
    }
    // list of all the info in the infographics
    [SerializeField] private InfographicInfo[] allInfographics;
    // time it is visible after intro animation
    [SerializeField] private float timeVisible = 1.0f;
    // the offset in the x-axis from the player
    [SerializeField] float horizontalOffset = 1.0f;
    // what the maximum distance it will move forward
    [SerializeField] float maxForwardDistance = 4.0f;
    private Vector3 startPos, startScale;
    private Renderer myRenderer;
    private TextMeshPro[] texts;
    private Camera mainCam;
    private RaycastHit rayHit;
    
    private int counter;
    // initialize all variables
    void Start () {
        myRenderer = this.GetComponentInChildren<Renderer>();

        texts = this.GetComponentsInChildren<TextMeshPro>();
        

        startScale = transform.localScale;

        mainCam = Camera.main;
        counter = 0;
    }
	
	
	private void ShowPanel (int _effect)
    {
        // travel to behind the camera.
        transform.position = mainCam.transform.position - mainCam.transform.forward * 2 + mainCam.transform.right * horizontalOffset;
        transform.position = new Vector3(transform.position.x, mainCam.transform.position.y, transform.position.z);

        // rotate in y the same as the camera
        // enable all renderers and sprite
        transform.LookAt(mainCam.transform.position);
        

        // cancel out the x and z rotations
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y + 160, 0)); //180
        
        // attach image
        myRenderer.material.mainTexture = allInfographics[_effect].info_image;
        

        // move the panel
        MovePanel();
    }

    public void ShowPanel(SideEffect _effect)
    {
        ShowPanel((int)_effect);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))//InputManager.instance.GetInteractButtonDown())
        {
            if (counter >= allInfographics.Length)
                counter = 0;

            ShowPanel(counter);

            counter++;

        } 
    }

    private void MovePanel()
    {
        StopAllCoroutines();
        // scale and move zero
        transform.localScale = Vector3.zero;
        transform.DOScale(startScale, 1f);
        // save start pos        
        startPos = transform.position;
        // move towards the calculated distance
        transform.DOMove(mainCam.transform.position - transform.forward.normalized * FindDistance(), 1f).OnComplete(IntroComplete);
    }

    void IntroComplete()
    {
        // start hide
        StartCoroutine(HidePanel(timeVisible));

    }

    // waits until timer is up and then makes the panel disappear
    IEnumerator HidePanel(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);

        transform.DOMove(transform.position - transform.up.normalized * 4, 1f);
    }

    // fires a ray in front, if it hits something return a bit under the distance
    float FindDistance()
    {
        // we do a raycast to a bit over the maxdistance to account for the panel's rotation
        if (Physics.Raycast(mainCam.transform.position, -transform.forward.normalized, out rayHit, maxForwardDistance * 1.45f))
        {
            transform.rotation = Quaternion.LookRotation(rayHit.normal, Vector3.up);
            return rayHit.distance - 0.1f;
        }
        else
            return maxForwardDistance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(startPos, Vector3.one * 0.5f);
        Gizmos.DrawLine(startPos, startPos - transform.forward * maxForwardDistance);
    }
}
