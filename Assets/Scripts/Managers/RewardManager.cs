using UnityEngine;
using System.IO;
using UnityEngine.Analytics;
using System.Collections;
using System.Collections.Generic;

public class RewardManager : MonoBehaviour {

    public static RewardManager instance;

    public enum RewardType
    {
        none, chest, bomb, cash, chips, key, keyExit, heart
    }

    public enum TutorialRewardType
    {
        gems, oneKey, goldKeyChest, fourKey, oneKeyBomb, twoKeyBomb
    }

    [System.Serializable]
    public struct Reward
    {
        public RewardType rewardType;
        public int intValue;
        public float floatValue;
        public List<int> chipList;
        public ChestManager.ChestContents rewardChest;
        public BombManager.BombType rewardBomb;
    }

    [Header("Debug Settings")]
    public bool debugBuild = false;
    public RewardType debugReward = RewardType.chest;

    private bool tutorialBuild = false;
    [Header("Tutorial Settings")]
    public TutorialRewardType[] tutorialRewards;

    //[Space(10)]
    [Header("Reward Objects")]
    public int numInteractableObjects = 60;
    public List<RewardObject> rewardObjects = new List<RewardObject>();

    [Header("Cluster Settings")]
    public Transform[] clusterTransforms;
    public int maxBombsInCluster = 3;
    public int maxChestsInCluster = 3;
    public int maxKeysInCluster = 3;

    public Cluster[] clusters;
    [System.Serializable]
    public struct Cluster
    {
        public Transform trans;
        public List<RewardObject> ROs;
        public int rosUsed;
        //public float distributionPercentage;
        public int bombsNum;
        public int chestsNum;
        public int keysNum;
    }
    
    [Header("Materials")]
    public Material regularMaterial;
    public Material detectedMaterial;
    public Material focusedMaterial;
    public Material highlightedMaterial;
    public Material openedMaterial;

    [Header("Highlight colors")]
    public Color detectedColor = Color.white;
	public Color focusedColour = new Color(195 / 256f, 229 / 256f, 206 / 256f, 1f);
	public Color highlightedColour = new Color(151 / 256f, 156 / 256f, 151 / 256f, 1f);
    public Color openedColour = new Color(120 / 256f, 120 / 256f, 120 / 256f, 1f); //Grey;
    public Color regularColour = Color.white;

	public Color detectedFresnelColor = new Color(189 / 256f, 232 / 256f, 87 / 256f, 1f); 
	public Color focusedFresnelColour = new Color(0 / 256f, 255 / 256f, 12 / 256f, 1f);  //Green
	public Color highlightedFresnelColour = new Color(5 / 256f, 104 / 256f, 68 / 256f, 1f); //dark green
    public Color openedFresnelColour = new Color(96 / 256f, 96 / 256f, 96 / 256f, 1f); //Grey

    public float detectedFresnelIntensity = 1.5f;
    public float focusedFresnelIntensity = 3.77f;
    public float highlightedFresnelIntensity = 6.58f;
    public float openedFresnelIntensity = 0f;

    [HideInInspector]
    public Transform releaseTransform;

    private float[] cashDistribution = new float[] { 0f, 0f, 0f, 0f, 0f, 0f };
    private float cashPrize = 0f;
    private int numChests = 0;
    //private int chestKeys = 0;
    private int numBombs = 0;
    private int requiredKeys = 0;
    private int totalnumKeys = 0;
    private int roIndex = 0;

    // tutorial variables
    private int tutorialCounter;
    public delegate void TutorialObjectOpened();
    public static event TutorialObjectOpened OnTutorialObjectOpened;
    
    //Clustering variables
    private bool usingClusterDistribution = true;
    private List<Reward> cashRewards = new List<Reward>();
    private List<Reward> chestRewards = new List<Reward>();
    private List<Reward> bombRewards = new List<Reward>();
    private List<Reward> keyRewards = new List<Reward>();
    private List<Reward> chipRewards = new List<Reward>();
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        tutorialBuild = TutorialManager.instance.GetIsTutorialEnabled();

        if (debugBuild) { SetupDebugRewards(); }
        else if (tutorialBuild) { tutorialCounter = -1; }
        else
        {
            SearchForRewardObjects();
            SetupRewards();
            SendAnalytics();
        }
    }

    void SetupDebugRewards ()
    {
        //Find RewardObjects in scene
        RewardObject[] searchForROs = FindObjectsOfType<RewardObject>();
        //Populate Reward Objects with Debug Reward
        for (int i = 0; i < searchForROs.Length; i++) {
            Reward newReward = new Reward();
            switch (debugReward)
            {
                case RewardType.chest:
                    ChestManager.ChestContents newChest = ChestManager.instance.CreateChest(0, RewardType.keyExit, 1);
                    //Distribute chest
                    newReward.rewardType = RewardType.chest;
                    newReward.rewardChest = newChest;
                    break;
                case RewardType.bomb:
                    //Create bombs
                    BombManager.BombType newBomb = BombManager.instance.CreateBomb(0);
                    //Distribute bomb
                    newReward.rewardType = RewardType.bomb;
                    newReward.rewardBomb = newBomb;
                    break;
                case RewardType.cash:
                    cashDistribution = new float[] { 0.50f, 0.50f, 0.60f, 0.60f, 0.80f, 1.00f };
                    newReward.rewardType = RewardType.cash;
                    newReward.floatValue = cashDistribution[Random.Range(0, cashDistribution.Length)];
                    break;
                case RewardType.chips:
                    newReward.rewardType = RewardType.chips;
                    int numChips = Random.Range(1, 20);
                    List<int> chipValues = new List<int>();
                    for (int j = 0; j < numChips; j++) { chipValues.Add(ChipManager.instance.chipStacks[Random.Range(0, ChipManager.instance.chipStacks.Length)].value); }
                    newReward.chipList = chipValues;
                    break;
                case RewardType.key:
                    //Distribute key(s)
                    newReward.rewardType = RewardType.key;
                    newReward.intValue = Random.Range(1, 3+1);
                    break;
                default:
                    print("Reward type " + debugReward + " is not implemented");
                    break;
            }
            //Add reward to bomb
            searchForROs[i].reward = newReward;
            searchForROs[i].SetActive(true);
        }
    }

    public void SetupTutorialRewards()
    {
        //Find Tutorial Manager in scene
        var _tutManager = transform.parent.GetComponentInChildren<TutorialManager>();
        if(_tutManager == null)
        {
            Debug.LogError("Couldn't find tutorial manager underneath the "+transform.parent.name);
        }

       // RewardObject[] searchForROs = _tutManager.activeInteractableObjects;

        /* check it's the same as the number of rewards
        if(searchForROs.Length != tutorialRewards.Length)
        {
            Debug.LogError("Different number of Reward objects ("+searchForROs.Length + ") and tutorial rewards ("+tutorialRewards.Length+")");
        } else {

            //Populate Reward Objects with Debug Reward
            for (int i = 0; i < searchForROs.Length; i++)
            {
                //Add reward to object
                searchForROs[i].reward = GetTutorialRewards(tutorialRewards[i]);
                searchForROs[i].SetActive(true);
            }
        }
        */
    }

    Reward GetTutorialRewards( TutorialRewardType _rewardType)
    {
        Reward newReward = new Reward();
        switch (_rewardType)
        {
            case TutorialRewardType.goldKeyChest:
                ChestManager.ChestContents newChest = ChestManager.instance.CreateChest(1, RewardType.keyExit, 1);
                //Distribute chest
                newReward.rewardType = RewardType.chest;
                newReward.rewardChest = newChest;
                break;
            case TutorialRewardType.oneKeyBomb:
                //Create bombs
                BombManager.BombType newBomb = BombManager.instance.CreateBomb(1);
                //Distribute bomb
                newReward.rewardType = RewardType.bomb;
                newReward.rewardBomb = newBomb;
                break;
            case TutorialRewardType.twoKeyBomb:
                //Create bombs
                BombManager.BombType newBomb2 = BombManager.instance.CreateBomb(2);
                //Distribute bomb
                newReward.rewardType = RewardType.bomb;
                newReward.rewardBomb = newBomb2;
                break;
            case TutorialRewardType.gems:
                newReward.rewardType = RewardType.chips;
                int numChips = Random.Range(1, 20);
                List<int> chipValues = new List<int>();
                for (int j = 0; j < numChips; j++) { chipValues.Add(ChipManager.instance.chipStacks[Random.Range(0, ChipManager.instance.chipStacks.Length)].value); }
                newReward.chipList = chipValues;
                break;
            case TutorialRewardType.oneKey:
                //Distribute key(s)
                newReward.rewardType = RewardType.key;
                newReward.intValue = 1;
                break;
            case TutorialRewardType.fourKey:
                //Distribute key(s)
                newReward.rewardType = RewardType.key;
                newReward.intValue = 4;
                break;
            default:
                print("Reward type " + debugReward + " is not implemented");
                break;
        }

        return newReward;
    }

    void SearchForRewardObjects()
    {
        //Find RewardObjects in scene
        RewardObject[] searchForROs = FindObjectsOfType<RewardObject>();
        if (clusterTransforms.Length > 0)
        {
            //Set up cluster arrays
            clusters = new Cluster[clusterTransforms.Length];
            for (int i = 0; i < clusterTransforms.Length; i++)
            {
                clusters[i].trans = clusterTransforms[i];
                clusters[i].ROs = new List<RewardObject>();

            }
            for (int i = 0; i < searchForROs.Length; i++)
            {
                for (int j = 0; j < clusterTransforms.Length; j++)
                {
                    if (searchForROs[i].transform.IsChildOf(clusterTransforms[j]))
                    {
                        clusters[j].ROs.Add(searchForROs[i]);
                    }
                }
            }
            //Randomly sort Reward Objects within each cluster
            for (int c = 0; c < clusters.Length; c++)
            {
                for (int i = 0; i < clusters[c].ROs.Count; i++)
                {
                    var temp = clusters[c].ROs[i];
                    int randomIndex = Random.Range(i, clusters[c].ROs.Count);
                    clusters[c].ROs[i] = clusters[c].ROs[randomIndex];
                    clusters[c].ROs[randomIndex] = temp;
                }
            }
            //Determine cluster distribution by lowest percentage full until all 'numInteractableObjects' have been used
            float[] clusterDistributionPercentage = new float[clusterTransforms.Length];
            for (int i = 0; i < numInteractableObjects; i++)
            {
                for (int j = 0; j < clusterTransforms.Length; j++)
                {
                    clusterDistributionPercentage[j] = clusters[j].rosUsed / (float)(clusters[j].ROs.Count);
                }
                float newMin = Mathf.Min(clusterDistributionPercentage);
                for (int j = 0; j < clusterTransforms.Length; j++)
                {
                    if (clusterDistributionPercentage[j] == newMin)
                    {
                        clusters[j].rosUsed++;
                        break;
                    }
                }
            }
            //Populate 'searchForROs' with 'numUsedFromCluster' ROs from each cluster
            List<RewardObject> usedROs = new List<RewardObject>();
            for (int c = 0; c < clusters.Length; c++)
            {
                for (int ro = 0; ro < clusters[c].rosUsed; ro++)
                {
                    usedROs.Add(clusters[c].ROs[ro]);
                }
            }
            searchForROs = usedROs.ToArray();
        } else
        {
            Debug.LogError("Cluster transforms are not set up in Reward Manager - Clustering disabled");
            usingClusterDistribution = false;
        }
        //Randomly sort Reward Objects
        for (int i = 0; i < searchForROs.Length; i++)
        {
            var temp = searchForROs[i];
            int randomIndex = Random.Range(i, searchForROs.Length);
            searchForROs[i] = searchForROs[randomIndex];
            searchForROs[randomIndex] = temp;
        }
        //Only use first 60 Reward Objects
        if (searchForROs.Length < numInteractableObjects) Debug.LogError("There are less than " + numInteractableObjects + " reward objects in the scene: " + searchForROs.Length);
        int ROs = (searchForROs.Length < numInteractableObjects) ? searchForROs.Length : numInteractableObjects;
        for (int i = 0; i < ROs; i++)
        {
            rewardObjects.Add(searchForROs[i]);
            searchForROs[i].SetActive(true);
        }
    }

    void SetupRewards()
    {
        GenerateMonetaryRewards();
        GenerateChests();
        GenerateBombs();
        GenerateKeys();
        if (usingClusterDistribution)
            DistributeRewards();
        else
            GenerateChips();
    }

    /// <summary>
    /// Only place that sets cashPrize float
    /// Sets the maximum amount of money available in room
    /// </summary>
    private void GenerateMonetaryRewards()
    {
        //Percentage chance of each range
        float pc1 = 0f;
        float pc2 = 0.4f;
        float pc3 = 0.3f;
        float pc4 = 0.184f;
        float pc5 = 0.07f;
        float pc6 = 0.038f;
        float pc7 = 0.005f;
        float pc8 = 0.002f;
        float pc9 = 0.001f;
        //Set cash prize value and distribution
        float randomCashValue = Random.Range(0f, 1f);
        if (randomCashValue < pc1)
        {
            cashPrize = 0f;
        }
        else if (randomCashValue < (pc1 + pc2))
        {
            cashPrize = 4f;
            cashDistribution = new float[] { 0.50f, 0.50f, 0.60f, 0.60f, 0.80f, 1.00f };
        }
        else if (randomCashValue < (pc1 + pc2 + pc3))
        {
            cashPrize = 7f;
            cashDistribution = new float[] { 0.50f, 0.75f, 1.00f, 1.25f, 1.50f, 2.00f };
        }
        else if (randomCashValue < (pc1 + pc2 + pc3 + pc4))
        {
            cashPrize = 10f;
            cashDistribution = new float[] { 0.50f, 1.00f, 1.50f, 2.00f, 2.25f, 2.75f };
        }
        else if (randomCashValue < (pc1 + pc2 + pc3 + pc4 + pc5))
        {
            cashPrize = 15f;
            cashDistribution = new float[] { 0.75f, 1.75f, 2.00f, 2.50f, 3.00f, 5.00f };
        }
        else if (randomCashValue < (pc1 + pc2 + pc3 + pc4 + pc5 + pc6))
        {
            cashPrize = 20f;
            cashDistribution = new float[] { 1.00f, 2.00f, 3.00f, 4.00f, 4.80f, 5.20f };
        }
        else if (randomCashValue < (pc1 + pc2 + pc3 + pc4 + pc5 + pc6 + pc7))
        {
            cashPrize = 50f;
            cashDistribution = new float[] { 2.50f, 5.00f, 7.50f, 10.00f, 12.00f, 13.00f };
        }
        else if (randomCashValue < (pc1 + pc2 + pc3 + pc4 + pc5 + pc6 + pc7 + pc8))
        {
            cashPrize = 100f;
            cashDistribution = new float[] { 5.00f, 10.00f, 15.00f, 20.00f, 24.00f, 26.00f };
        }
        else if (randomCashValue <= (pc1 + pc2 + pc3 + pc4 + pc5 + pc6 + pc7 + pc8 + pc9))
        {
            cashPrize = 200f;
            cashDistribution = new float[] { 10.00f, 20.00f, 30.00f, 40.00f, 45.00f, 55.00f };
        }
        else
            Debug.LogError("Error! " + randomCashValue + " is not between 0 and 1!");
        //Distribute cash (if applicable)
        if (cashPrize > 0)
        {
            if (usingClusterDistribution) { cashRewards = new List<Reward>(); }
            for (int i = 0; i < cashDistribution.Length; i++)
            {
                Reward newReward = new Reward();
                newReward.rewardType = RewardType.cash;
                newReward.floatValue = cashDistribution[i];
                if (usingClusterDistribution)
                {
                    cashRewards.Add(newReward);
                }
                else
                {
                    rewardObjects[roIndex].reward = newReward;
                    roIndex++;
                }
            }
        }
        else print("No money in the room!");
        print(cashPrize + " cash in the room!");
    }

    /// <summary>
    /// Generate 3 - 6 chests
    /// </summary>
    private void GenerateChests ()
    {
        //Set chest number
        int minChests = 3;
        int maxChests = 6;
        numChests = Random.Range(minChests, maxChests+1);
        //Create chest with keyNum and populate with items, then distribute
        if (usingClusterDistribution) { chestRewards = new List<Reward>(); }
        for (int i = 0; i < numChests; i++)
        {
            int numKeys = Random.Range(1, 3 + 1);
            ChestManager.ChestContents newChest;
            if (i == 0)
                newChest = ChestManager.instance.CreateChest(numKeys, RewardType.keyExit, 1);
            else if (i == 1)
                newChest = ChestManager.instance.CreateChest(numKeys, RewardType.heart, 1);
            else
                newChest = ChestManager.instance.CreateChest(numKeys, RewardType.chips, -1);
            //Keep track of keys required
            requiredKeys += numKeys;
            //Distribute chest
            Reward newReward = new Reward();
            newReward.rewardType = RewardType.chest;
            newReward.intValue = numKeys;
            newReward.rewardChest = newChest;
            if (usingClusterDistribution)
            {
                chestRewards.Add(newReward);
            }
            else
            {
                rewardObjects[roIndex].reward = newReward;
                roIndex++;
            }
        }
    }

    private void GenerateBombs ()
    {
        //Set bomb number
        int minBombs = 6;
        int maxBombs = 12;
        numBombs = Random.Range(minBombs, maxBombs + 1);
        //Create bombs with keyNum
        if (usingClusterDistribution) { bombRewards = new List<Reward>(); }
        for (int i = 0; i < numBombs; i++)
        {
            int numKeys = Random.Range(1, 3 + 1);
            BombManager.BombType newBomb = BombManager.instance.CreateBomb(numKeys);
            //Keep track of keys required
            requiredKeys += numKeys;
            //Distribute bomb
            Reward newReward = new Reward();
            newReward.rewardType = RewardType.bomb;
            newReward.intValue = numKeys;
            newReward.rewardBomb = newBomb;
            if (usingClusterDistribution)
            {
                bombRewards.Add(newReward);
            }
            else
            {
                rewardObjects[roIndex].reward = newReward;
                roIndex++;
            }
        }
    }

    private void GenerateKeys ()
    {
        //Set keys
        int keysToRemove = 6;
        //Log number of keys in room
        totalnumKeys = requiredKeys - keysToRemove;
        //Debug.Log(totalnumKeys + " total keys");
        //Distribute keys across up to 18 objects
        if (usingClusterDistribution) { keyRewards = new List<Reward>(); }
        int keysLeft = totalnumKeys;
        for (int i = 18; i > 0; i--)
        {
            float fi = i;
            int numKeys;
            //Debug.Log(keysLeft / fi);
            if (keysLeft/fi > 2)
            {
                numKeys = 3;
                keysLeft -= numKeys;
            } else if (keysLeft/fi > 1)
            {
                numKeys = 2;
                keysLeft -= numKeys;
            } else if (keysLeft/fi > 0)
            {
                if (Mathf.Approximately(keysLeft / fi, 0f)) { break; }  //Watch out for tiny floats
                numKeys = 1;
                keysLeft -= numKeys;
            } else
            {
                break;
            }
            //Distribute key(s)
            Reward newReward = new Reward();
            newReward.rewardType = RewardType.key;
            newReward.intValue = numKeys;
            if (usingClusterDistribution)
            {
                keyRewards.Add(newReward);
            }
            else
            {
                rewardObjects[roIndex].reward = newReward;
                roIndex++;
            }
        }
        //Debug.Log(totalnumKeys + " keys in the room - " + keysLeft + " left after distribution");
        if (keysLeft > 0) { Debug.LogError("Error! Not all keys distributed! " + keysLeft + " left after distribution"); }
    }

    private void GenerateChips()
    {
        //Find chests that can be filled with chips
        int emptyChestSlots = 0;
        for (int i = 0; i < numChests; i++)
        {
            if (ChestManager.instance.chests[i].rewardType == RewardType.chips) { emptyChestSlots++; }
        }
        //Determine number of places left to put chips
        int chipObjects = 60 - roIndex + emptyChestSlots;
        if (usingClusterDistribution)
        {
            chipObjects = emptyChestSlots;
            for (int i = 0; i < rewardObjects.Count; i++)
            {
                if (rewardObjects[i].reward.rewardType == RewardType.none) { chipObjects++; }
            }
        }
        //Breakdown chips into available places
        int totalItems = 200;
        //Create list of chips
        List<int> chipList = new List<int>();
        for (int i = 0; i < ChipManager.instance.chipStacks.Length; i++)
        {
            ChipManager.ChipStack c = ChipManager.instance.chipStacks[i];
            for (int j = 0; j < c.num; j++)
            {
                chipList.Add(c.value);
            }
        }
        //Randomly sort chip list
        for (int i = 0; i < chipList.Count; i++)
        {
            var temp = chipList[i];
            int randomIndex = Random.Range(i, chipList.Count);
            chipList[i] = chipList[randomIndex];
            chipList[randomIndex] = temp;
        }
        //Distribute chips into chip objects
        int chipsleft = totalItems;
        int chipChests = emptyChestSlots;
        if (usingClusterDistribution) { chipRewards = new List<Reward>(); }
        for (int i = chipObjects; i > 0; i--)
        {
            float openSpots = i;
            int chipsToDistribute = Mathf.FloorToInt(chipsleft / openSpots);
            if (chipsToDistribute < 1)
                Debug.LogError("Error! There are no chips left for this spot!");
            if (chipChests > 0)        //Use triple the amount of objects for an empty chest slot
            {
                chipsToDistribute *= 3;
            }
            else if ((openSpots > 1) && (chipsleft > (chipsToDistribute + openSpots)))  //Use a random number between half and double for all other spots before the last one
            {
                chipsToDistribute = Random.Range(Mathf.CeilToInt(chipsToDistribute / 2f), (chipsToDistribute * 2));
            }
            int chipSum = 0;
            List<int> chipValues = new List<int>();
            for (int j = 0; j < chipsToDistribute; j++)
            {
                chipSum += chipList[chipList.Count - 1];
                chipValues.Add(chipList[chipList.Count - 1]);
                chipList.RemoveAt(chipList.Count - 1);
            }
            chipsleft -= chipsToDistribute;
            //Distribute chips
            if (chipChests > 0)
            {
                if (usingClusterDistribution)
                {
                    ChestManager.ChestContents chipChest = ChestManager.instance.chests[(ChestManager.instance.chests.Count) - chipChests];
                    int chestIndex;
                    for (chestIndex = 0; chestIndex < rewardObjects.Count; chestIndex++)
                    {
                        if (rewardObjects[chestIndex].reward.rewardChest.Equals(chipChest)) { break; }
                    }
                    if (chestIndex >= rewardObjects.Count)
                        Debug.LogError("Error! Chip chest could not be properly setup!");
                    chipChest.numReward = chipSum;
                    chipChest.chipList = chipValues;
                    ChestManager.instance.chests[(ChestManager.instance.chests.Count) - chipChests] = chipChest;
                    rewardObjects[chestIndex].reward.rewardChest = chipChest;
                    chipChests--;
                }
                else
                {
                    ChestManager.ChestContents chipChest = ChestManager.instance.chests[(ChestManager.instance.chests.Count) - chipChests];
                    int chestIndex;
                    for (chestIndex = 0; chestIndex < rewardObjects.Count; chestIndex++)
                    {
                        if (rewardObjects[chestIndex].reward.rewardChest.Equals(chipChest)) { break; }
                    }
                    if (chestIndex >= rewardObjects.Count)
                        Debug.LogError("Error! Chip chest could not be properly setup!");
                    chipChest.numReward = chipSum;
                    chipChest.chipList = chipValues;
                    ChestManager.instance.chests[(ChestManager.instance.chests.Count) - chipChests] = chipChest;
                    rewardObjects[chestIndex].reward.rewardChest = chipChest;
                    chipChests--;
                }
            }
            else
            {
                Reward newReward = new Reward();
                newReward.rewardType = RewardType.chips;
                newReward.chipList = chipValues;
                if (usingClusterDistribution)
                {
                    chipRewards.Add(newReward);
                }
                else
                {
                    rewardObjects[roIndex].reward = newReward;
                    roIndex++;
                }
            }
        }
        if (chipList.Count > 0)
            Debug.LogError("Error! There are still " + chipList.Count + " chips not distributed");
    }

    private void DistributeRewards ()
    {
        //Distribute bombs evenly
        while (bombRewards.Count > 0)
        {
            bool bombDistributed = false;
            for (int ro = 0; ro < rewardObjects.Count; ro++)    //Look at each RO
            {
                if (rewardObjects[ro].reward.rewardType == RewardType.none) //Make sure it's empty
                {
                    for (int c = 0; c < clusters.Length; c++)   //Find which cluster it's in
                    {
                        if (clusters[c].ROs.Contains(rewardObjects[ro]) && clusters[c].bombsNum < maxBombsInCluster)    //Check if it can accept more bombs
                        {
                            rewardObjects[ro].reward = bombRewards[0];  //Distribute if so, otherwise move to next RO
                            bombRewards.Remove(bombRewards[0]);
                            clusters[c].bombsNum++;
                            bombDistributed = true;
                            break;
                        }
                    }
                    if (bombDistributed) { break; }
                }
            }
        }
        //Distribute chests evenly
        while (chestRewards.Count > 0)
        {
            bool chestDistributed = false;
            for (int ro = 0; ro < rewardObjects.Count; ro++)    //Look at each RO
            {
                if (rewardObjects[ro].reward.rewardType == RewardType.none) //Make sure it's empty
                {
                    for (int c = 0; c < clusters.Length; c++)   //Find which cluster it's in
                    {
                        if (clusters[c].ROs.Contains(rewardObjects[ro]) && clusters[c].chestsNum < maxChestsInCluster)    //Check if it can accept more chests
                        {
                            rewardObjects[ro].reward = chestRewards[0];  //Distribute if so, otherwise move to next RO
                            chestRewards.Remove(chestRewards[0]);
                            clusters[c].chestsNum++;
                            chestDistributed = true;
                            break;
                        }
                    }
                    if (chestDistributed) { break; }
                }
            }
        }
        //Distribute keys evenly
        while (keyRewards.Count > 0)
        {
            bool keyDistributed = false;
            for (int ro = 0; ro < rewardObjects.Count; ro++)    //Look at each RO
            {
                if (rewardObjects[ro].reward.rewardType == RewardType.none) //Make sure it's empty
                {
                    for (int c = 0; c < clusters.Length; c++)   //Find which cluster it's in
                    {
                        if (clusters[c].ROs.Contains(rewardObjects[ro]) && clusters[c].keysNum < maxKeysInCluster)    //Check if it can accept more keys
                        {
                            rewardObjects[ro].reward = keyRewards[0];  //Distribute if so, otherwise move to next RO
                            keyRewards.Remove(keyRewards[0]);
                            clusters[c].keysNum++;
                            keyDistributed = true;
                            break;
                        }
                    }
                    if (keyDistributed) { break; }
                }
            }
        }
        //Distribute cash
        if (cashRewards.Count > 0)
        {
            int cashIndex = 0;
            for (int ro = 0; ro < rewardObjects.Count; ro++)    //Look at each RO
            {
                if (rewardObjects[ro].reward.rewardType == RewardType.none) //Make sure it's empty
                {
                    rewardObjects[ro].reward = cashRewards[cashIndex++];  //Distribute if so, otherwise move to next RO
                    if (cashIndex == cashRewards.Count) { break; }
                }
            }
            cashRewards.Clear();
        }
        //Generate chips
        GenerateChips();
        //Distribute chips
        int chipIndex = 0;
        for (int ro = 0; ro < rewardObjects.Count; ro++)    //Look at each RO
        {
            if (rewardObjects[ro].reward.rewardType == RewardType.none) //Make sure it's empty
            {
                rewardObjects[ro].reward = chipRewards[chipIndex++];  //Distribute if so, otherwise move to next RO
                if (chipIndex == chipRewards.Count) { break; }
            }
        }
        chipRewards.Clear();
    }

    int objectsInspected = 0;
    
    public void OpenRewardObject(RewardObject rewardObject)
    {
        objectsInspected++;
        ReleaseReward(rewardObject.reward, rewardObject.rewardReleaseTrans);
    }

    public void OpenRewardObjectFromList(RewardObject rewardObject)
    {
        tutorialCounter++;
        if(OnTutorialObjectOpened != null){ OnTutorialObjectOpened(); }
        var _objectToSpawn = GetTutorialRewards(tutorialRewards[tutorialCounter]);
        Debug.Log("getting object "+ _objectToSpawn.rewardType);
        ReleaseReward(_objectToSpawn, rewardObject.rewardReleaseTrans);
    }
    
    public void ReleaseReward(Reward reward, Transform _releaseTransform)
    {
        switch (reward.rewardType)
        {
            case RewardType.key:
                KeyManager.instance.ReleaseKeys(reward.intValue, _releaseTransform);
                break;
            case RewardType.keyExit:
                KeyManager.instance.ReleaseExitKey(_releaseTransform);
                break;
            case RewardType.chips:
                ChipManager.instance.ReleaseChips(reward.chipList, _releaseTransform);
                break;
            case RewardType.cash:
                CashManager.instance.ReleaseCash(reward.floatValue, _releaseTransform);
                break;
            case RewardType.bomb:
                BombManager.instance.ReleaseBomb(reward.rewardBomb, _releaseTransform);
                break;
            case RewardType.chest:
                ChestManager.instance.ReleaseChest(reward.rewardChest, _releaseTransform);
                break;
            default:
                Debug.LogError("Reward type " + reward.rewardType + " doesn't exist!");
                break;
        }
    }

    //ANALYTICS
    private void SendAnalytics ()
    {
        //Save analytics
        StartCoroutine(SaveAnalytics());
    }

    private IEnumerator SaveAnalytics()
    {
        //Wait a frame to ensure all scripts have started up
        yield return null;
        //Collect info
        LocalAnalytics newLocalAnalytics = new LocalAnalytics()
        {
            startTime = GameManager.instance.startGameTime.ToString(),
            cashInRoom = cashPrize.ToString(),
            chestsInRoom = numChests.ToString(),
            bombsInRoom = numBombs.ToString(),
            keysInRoom = totalnumKeys.ToString(),
        };
        //Save analytics to device
        string jsonToFile = JsonUtility.ToJson(newLocalAnalytics, true) + "\n";
        string filePath = Path.Combine(Application.persistentDataPath, "Analytics.json");
        File.AppendAllText(filePath, jsonToFile);
        //Setup analytics for Unity Analytics
        var _dataDictionary = new Dictionary<string, object>
        {
            {"startTime", newLocalAnalytics.startTime },
            {"cashInRoom", newLocalAnalytics.cashInRoom },
            {"chestsInRoom", newLocalAnalytics.chestsInRoom },
            {"bombsInRoom",  newLocalAnalytics.bombsInRoom },
            {"keysInRoom",  newLocalAnalytics.keysInRoom }
        };
        //Send info package to Unity Analytics
        var _result = Analytics.CustomEvent("gameOver", _dataDictionary);
        if (_result == AnalyticsResult.Ok)
            Debug.Log("Sending analytics from Reward Manager startTime:" + GameManager.instance.startGameTime);
        else
            Debug.LogError("Analytics was not sent from Reward Manager because " + _result.ToString());
    }

    /// <summary>
    /// Class for saving analytics to local device
    /// </summary>
    [System.Serializable]
    public class LocalAnalytics
    {
        public string startTime;
        public string cashInRoom;
        public string chestsInRoom;
        public string bombsInRoom;
        public string keysInRoom;
    }
}
