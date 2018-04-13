using UnityEngine;
using System.Collections;

public class LockBox : CostObject {

    public RewardManager.RewardType[] rewards;
    public Transform rewardReleaseTransform;
    [HideInInspector]
    public int index = 0;
    public Animation anim;

    void Start () {
        //SetCost(cost);
	}

    public void Setup(RewardManager.RewardType[] _rewards, int openCost)
    {
        rewards = _rewards;
        SetCost(openCost);
    }

 //   void Update () {
	
	//}

    public override void OnPay()
    {
        StartCoroutine(ReleaseCashObjects());
        LockBoxManager.instance.lockBoxesUnlocked++;
        //LockBoxManager.instance.RemoveLockBox(index);
    }

    IEnumerator ReleaseCashObjects()
    {
        float timeBetweenSpawn = 1f;
        anim.Play("open");

        yield return new WaitForSeconds(anim["open"].length); 

        for (int i = 0, max = rewards.Length; i < max; ++i)
        {
            //RewardManager.RewardType rewardType = rewards[i];
            //RewardManager.instance.GetRewardType(rewardType, rewardReleaseTransform);
            yield return new WaitForSeconds(timeBetweenSpawn);
        }

        anim.Play("close");
        yield return new WaitForSeconds(anim["close"].length + timeBetweenSpawn/2);

        Shrink();
    }
}
