using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseManager : MonoBehaviour {

    public static ReleaseManager instance;

    public GameObject snakePrefab;

    void Awake()
    {
        instance = this;
    }
    
    public void ReleaseSnake(Transform releaseTrans)
    {
        List<GameObject> snakes = new List<GameObject>();
        snakes.Add(snakePrefab);
        StartCoroutine(ReleaseObjects(snakes, releaseTrans, 0.2f));
    }

    IEnumerator ReleaseObjects(List<GameObject> releaseObjects, Transform trans, float timeBetweenSpawn)
    {
        for (int i = 0; i < releaseObjects.Count; i++)
        {
            GameObject go = Instantiate(releaseObjects[i], trans.position + (trans.forward * 1), trans.rotation) as GameObject;
            ReleaseObject ro = go.GetComponent<ReleaseObject>();
            if (ro == null) yield return null;

            Rigidbody rb = ro.GetComponent<Rigidbody>();
            rb.AddForce(trans.forward * 150);
            rb.AddExplosionForce(150, trans.position, 100, 1, ForceMode.Force);
            ro.audioObject.PlayRelease();

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }
}
