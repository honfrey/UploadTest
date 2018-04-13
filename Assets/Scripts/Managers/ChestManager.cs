using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ChestManager : MonoBehaviour {

    public GameObject chestPrefab;
    public GameObject clockPrefab;
    public List<ChestContents> chests = new List<ChestContents>();

    [System.Serializable]
    public struct ChestContents
    {
        public int keyCost;
        public RewardManager.RewardType rewardType;
        public float numReward;
        public List<int> chipList;
    }

    public static ChestManager instance;
    [HideInInspector]
    public int chestsFound = 0, chestsUnlocked = 0;

    private List<GameObject> spawnedChests = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    public ChestContents CreateChest (int _keyCost, RewardManager.RewardType _rewardType, float _NumReward)
    {
        ChestContents newChest = new ChestContents();
        newChest.keyCost = _keyCost;
        newChest.rewardType = _rewardType;
        newChest.numReward = _NumReward;
        chests.Add(newChest);
        return newChest;
    }

    public void ReleaseChest (ChestContents theChest, Transform releaseTransform)
    {
        Vector3 spawnPosition = releaseTransform.position + (releaseTransform.forward * 1f);
        Vector3 spawnRotation = releaseTransform.rotation.eulerAngles;
        spawnRotation.x = 0f;
        spawnRotation.z = 0f;
        Chest newChest = Instantiate(chestPrefab, spawnPosition, Quaternion.Euler(spawnRotation)).GetComponent<Chest>();
        newChest.Setup(theChest);
        spawnedChests.Add(newChest.gameObject);
        Rigidbody rb = newChest.GetComponent<Rigidbody>();
        rb.AddForce(releaseTransform.forward * 320);
        rb.AddExplosionForce(100, releaseTransform.position, 100, 1, ForceMode.Force);
        chestsFound++;
    }

    public void ReleaseHeart (Transform trans)
    {
        GameObject clock = Instantiate(clockPrefab, trans.position + trans.forward * 1, trans.rotation) as GameObject;
        Rigidbody rb = clock.GetComponent<Rigidbody>();
        rb.AddForce(trans.forward * 200);
        rb.AddExplosionForce(200, trans.position, 100, 1, ForceMode.Force);

        Heart heart = clock.GetComponent<Heart>();
        heart.audioObject.PlayRelease();
    }

    public void RemoveChestContents (ChestContents chestToRemove)
    {
        chests.Remove(chestToRemove);
    }

    public void RemoveChest(GameObject chest)
    {
        if (spawnedChests.Contains(chest))
            spawnedChests.Remove(chest);
    }

    public void HideChests ()
    {
        foreach (var c in spawnedChests)
        {
            if (c != null)
                c.SetActive(false);
        }
    }
}
