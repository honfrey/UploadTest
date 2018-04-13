using UnityEngine;
using System.Collections;
using DG.Tweening;


public class Chest : CostObject {

    public ChestManager.ChestContents chestReward;
    public Transform rewardReleaseTransform;
    public AudioClip openChestClip;
    public GameObject[] keys;

    private Animator anim;
    private bool hasBeenOpened;
    
    public override void Awake()
    {
        base.Awake();
        anim = this.GetComponent<Animator>();
    }

    void Start () {
        hasBeenOpened = false;
	}

    public void Setup(ChestManager.ChestContents _reward)
    {
        chestReward = _reward;
        SetCost(chestReward.keyCost);
    }

    public override void OnPay()
    {
        if (!hasBeenOpened)
        {
            hasBeenOpened = true;
            ChestManager.instance.chestsUnlocked++;
            ChestManager.instance.RemoveChestContents(chestReward);
            StartCoroutine(OpenSequence());
        }
    }

    IEnumerator ReleaseChestContents()
    {
        anim.enabled = true;
        anim.SetTrigger("openTrigger");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);
        switch (chestReward.rewardType)
        {
            case RewardManager.RewardType.heart:
                ChestManager.instance.ReleaseHeart(rewardReleaseTransform);
                break;
            case RewardManager.RewardType.keyExit:
                KeyManager.instance.ReleaseExitKey(rewardReleaseTransform);
                break;
            case RewardManager.RewardType.chips:
                float releaseTime = ChipManager.instance.ReleaseChips(chestReward.chipList, rewardReleaseTransform);
                yield return new WaitForSeconds(releaseTime);
                break;
            default:
                break;
        }
        anim.SetTrigger("closeTrigger");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);
        yield return new WaitForSeconds(0.25f);
        ChestManager.instance.RemoveChest(gameObject);
        Shrink();
    }

    public override void InsufficientFunds()
    {
        if (!hasBeenOpened)
        {
            anim.enabled = true;
            anim.SetTrigger("cancellationTrigger");
        }
    }

    // sequence of animations as bomb is defused
    IEnumerator OpenSequence()
    {
        // calls keys from the wand to the bomb
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].SetActive(true);
            var _finalPos = keys[i].transform.position;
            keys[i].transform.rotation.SetLookRotation((WandManager.instance.wandTip.position - _finalPos));
            keys[i].transform.position = WandManager.instance.wandTip.position;
            keys[i].transform.DOMove(_finalPos, 0.3f).OnComplete(() => {
                AnimateKey(keys[i]);
            });
            yield return new WaitForSeconds(0.3f);
        }
        // release contents
        StartCoroutine(ReleaseChestContents());
    }

    // tells the key to animate
    void AnimateKey(GameObject _key)
    {
        _key.GetComponent<Animator>().SetTrigger("RotateKey");
    }
}
