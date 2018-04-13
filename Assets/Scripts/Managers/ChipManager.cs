using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ChipManager : MonoBehaviour {

    public static ChipManager instance;
    public int startingChips = 0;
    public int chips = 0;
    int visChips = 0;
    
    [Header("Chip Stacks")]
    public ChipStack[] chipStacks;
    [System.Serializable]
    public struct ChipStack
    {
        public GameObject chipPrefab;
        public int value;
        public int num;
    }
    
    void Awake()
    {
        instance = this;
    }

    void Start () {
        SetChips(startingChips);
	}

    void Update () {
        UpdateChipText();
    }

    public bool HasChips(float amount)
    {
        if (chips >= amount) return true; else return false;
    }
    
    public void AddChips(int amount)
    {
        chips += amount;
        float minSpeed = 0.25f, maxSpeed = 1.75f;
        float maxAmount = 20f;
        float duration = Mathf.Lerp(minSpeed, maxSpeed, amount / maxAmount);
        
        if (!DOTween.IsTweening("Chips"))
        {
            DOTween.Kill("ScaleChipsDown");
        }

        DOTween.Kill("Chips");
        DOTween.To(x => visChips = (int)x, visChips, chips, duration).SetEase(Ease.OutSine).SetId("Chips").OnComplete(ShrinkChips);
    }

    void ShrinkChips()
    {
        DOTween.Kill("ScaleChipsUp");
        UpdateChipText();
    }

    public void SetChips(int set)
    {
        chips = set;
        UpdateChipText();
    }

    public void SpendChips(int amount)
    {
        chips -= amount;
        UpdateChipText();
    }

    void UpdateChipText()
    {
        //WandManager.instance.UpdatePoints(visChips);
    }
    
    public float ReleaseChips(List<int> chipList, Transform trans)
    {
        float timeBetweenSpawns = 0.1f;
        StartCoroutine(ReleaseChipObjects(chipList, trans, timeBetweenSpawns));
        //Return expected length of release
        return timeBetweenSpawns * chipList.Count;
    }

    IEnumerator ReleaseChipObjects(List<int> chipList, Transform trans, float timeBetweenSpawn)
    {
        for (int i = 0; i < chipList.Count; i++)
        {
            GameObject chipToSpawn = null;
            foreach (var item in chipStacks)
            {
                if (item.value == chipList[i])
                {
                    chipToSpawn = item.chipPrefab;
                    break;
                }
            }
            GameObject chipObject = Instantiate(chipToSpawn, trans.position + (trans.forward * 1), trans.rotation) as GameObject;
            Chip chip = chipObject.GetComponent<Chip>();
            chip.chipAmount = chipList[i];
            Rigidbody rb = chipObject.GetComponent<Rigidbody>();
            rb.AddForce(trans.forward * 150);
            rb.AddExplosionForce(150, trans.position, 100, 1, ForceMode.Force);
            chip.audioObject.PlayRelease();
            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }
}
