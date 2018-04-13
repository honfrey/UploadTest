using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DetectorScript : MonoBehaviour {

    public Transform detectorMesh;
    public GameObject linePrefab;
    public TextMeshPro detectorTMP;
    public Collider influenceCol;
    public Transform[] propellers;
    public float propellerSpeed = 10f;
    public Transform antenna;
    public float antennaSpeed = -10f;
    public GameObject placementDrone;
    public GameObject actualDrone;
    public RewardManager.RewardType rewardTypeLookingFor;
    public List<RewardObject> detectedRewardObjects = new List<RewardObject>();
    
    private bool isDeployed = false;

    // Update is called once per frame
    void Update()
    {
        if (isDeployed)
        {
            ////Movement
            //float x = Input.GetAxis("Horizontal");
            //float z = Input.GetAxis("Vertical");
            //Vector3 newPosition = transform.position;
            //newPosition.x += (x * Time.deltaTime);
            //newPosition.z += (z * Time.deltaTime);
            //transform.position = newPosition;
            //Propeller rotation
            foreach (var propeller in propellers)
            {
                propeller.Rotate(Vector3.up * Time.deltaTime * propellerSpeed);
            }
            //Antenna rotation
            antenna.Rotate(Vector3.up * Time.deltaTime * antennaSpeed);
            //Get number of detected items
            int numDetectedItems = 0;
            for (int i = 0; i < detectedRewardObjects.Count; i++)
            {
                ////Update line position
                //lines[i].SetPositions(new Vector3[] { detectorMesh.position, detectedRewardObjects[i].transform.position });
                //Update colours
                if (detectedRewardObjects[i].IsAnActiveReward())
                {
                    if (detectedRewardObjects[i].reward.rewardType.Equals(rewardTypeLookingFor))
                    {
                        //lines[i].startColor = Color.green;
                        //lines[i].endColor = Color.green;
                        //Add to detected count
                        numDetectedItems++;
                    }
                    //else
                    //{
                    //    lines[i].startColor = Color.yellow;
                    //    lines[i].endColor = Color.yellow;
                    //}
                }
                //else
                //{
                //    lines[i].startColor = Color.red;
                //    lines[i].endColor = Color.red;
                //}
            }
            //Update detection count text
            detectorTMP.text = numDetectedItems.ToString();
            if (numDetectedItems < 1)
            {
                isDeployed = false;
                StartCoroutine(Retire());
            }
        }
    }

    private IEnumerator Retire ()
    {
        yield return transform.DOMoveY(-10f, 2f).SetEase(Ease.InCubic).WaitForCompletion();
        Destroy(gameObject);
    }

    public IEnumerator Deploy ()
    {
        placementDrone.SetActive(false);
        actualDrone.SetActive(true);
        influenceCol.enabled = true;
        yield return new WaitForSeconds(0.5f);
        isDeployed = true;
    }

    //Collisions!
    void OnTriggerEnter(Collider col)
    {
        RewardObject possibleRO = col.gameObject.GetComponent<RewardObject>();
        if (possibleRO != null)
        {
            detectedRewardObjects.Add(possibleRO);
            //LineRenderer newLineRenderer = Instantiate(linePrefab).GetComponent<LineRenderer>();
            //newLineRenderer.positionCount = 2;
            //lines.Add(newLineRenderer);
        }
    }

    void OnTriggerExit(Collider col)
    {
        RewardObject possibleRO = col.gameObject.GetComponent<RewardObject>();
        if (possibleRO != null)
        {
            if (detectedRewardObjects.Contains(possibleRO))
            {
                ////Remove line
                //int roIndex = detectedRewardObjects.IndexOf(possibleRO);
                //Destroy(lines[roIndex].gameObject);
                //lines.RemoveAt(roIndex);
                //Remove RO
                detectedRewardObjects.Remove(possibleRO);
            }
        }
    }
}
