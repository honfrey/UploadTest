using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class LockBoxManager : MonoBehaviour {

    public GameObject lockBoxPrefab;

    [Header("Lock Box Types")]
    public LockBoxType[] lockBoxTypes;

    [System.Serializable]
    public struct LockBoxType
    {
        public int boxOpenCost;
        public int boxMinRewardDrop;
        public int boxMaxRewardDrop;
        //public RewardManager.TypeDistribution[] chanceRewardDistribution;
    }

    public static LockBoxManager instance;

    //List<LockBox> lockBoxes = new List<LockBox>();

    //[HideInInspector]
    public int lockBoxesFound = 0, lockBoxesUnlocked = 0;

    void Awake()
    {
        instance = this;
    }

    void Start () {

    }
	
	void Update () {
	
	}

    public void ReleaseLockBoxType(int typeIndex, Transform releaseTransform)
    {
        //LockBoxType t = lockBoxTypes[typeIndex];

        //Vector3 spawnPosition = releaseTransform.position + (releaseTransform.forward * 1f);
        //Vector3 spawnRotation = releaseTransform.rotation.eulerAngles;
        //spawnRotation.x = 0f;
        //spawnRotation.z = 0f;

        //LockBox lockBox = Instantiate(lockBoxPrefab, spawnPosition, Quaternion.Euler(spawnRotation)).GetComponent<LockBox>();
        //lockBox.SetCost(t.boxOpenCost);

        //int rewardsNum = Random.Range(t.boxMinRewardDrop, t.boxMaxRewardDrop);
        //RewardManager.RewardType[] rewards = new RewardManager.RewardType[rewardsNum];

        //// fill the box with rewards
        //RewardManager.RewardType[] randomRewardsPool = RewardManager.instance.RandomRewardArrayFromDistribution(t.chanceRewardDistribution, 100);
        //string message = "LOCK BOX REWARDS:\n";
        //for (int i = 0; i < rewardsNum; i++)
        //{
        //    rewards[i] = randomRewardsPool[Random.Range(0, randomRewardsPool.Length - 1)];
        //    message += rewards[i] + "\n";
        //}
        //lockBox.rewards = rewards;
        //print(message);

        //// jump onto the ground
        //float distanceAway = 1f;
        //Vector3 targetPosition = spawnPosition + (releaseTransform.forward * distanceAway);
        //targetPosition.y = 0;
        //lockBox.transform.DOJump(targetPosition, 1f, 1, 1f);

        //lockBoxesFound++;
    }
}
