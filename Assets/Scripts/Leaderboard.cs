using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Leaderboard : MonoBehaviour {

    public static Leaderboard instance;
    List<float> topScores = new List<float>();
    public TextMesh topScoresText;
    public TextMesh lastScoreText;
    float lastScore;
    int displayScoresNum = 5;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        else if (instance != this)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScoresFromPlayerPrefs();
        }
    }

    public void SubmitScore(float score)
    {       
        UpdateLastScore(score);
        lastScore = score;

        //print("submit: last score " + lastScore);

        // insert the score into the list and update the highscore table
        int i = 0;
        foreach (int s in topScores)
        {
            if (score > s)
            {
                topScores.Insert(i, score);
                UpdateTopScores(topScores);
                break;
            }
            i++;
        }
        SaveScoresToPlayerPrefs();
    }

    public void ClearScores()
    {
        for (int i = 0; i < topScores.Count; i++)
        {
            topScores[i] = 0;
        }
        UpdateTopScores(topScores);
        SaveScoresToPlayerPrefs();
    }

    void LoadScoresFromPlayerPrefs()
    {
        topScores.Clear();
        for (int i = 0; i < displayScoresNum; i++)
        {
            string key = "TopScore" + i.ToString();

            if (PlayerPrefs.HasKey(key)){
                float score = PlayerPrefs.GetFloat(key);
                topScores.Add(score);
            }
            else
            {
                topScores.Add(0);
            }
        }
        UpdateTopScores(topScores);
    }

    void SaveScoresToPlayerPrefs()
    {
        for (int i = 0; i < displayScoresNum; i++)
        {
            float score = topScores[i];
            string key = "TopScore" + i.ToString();
            PlayerPrefs.SetFloat(key, score);
        }
    }

    void UpdateLastScore(float _lastScore)
    {
        lastScore = _lastScore;
        lastScoreText.text = lastScore.ToString("F0");
        //print("updated last score to " + lastScore);
    }

    void UpdateTopScores(List<float> _topScores)
    {
        topScores = _topScores;

        string text = "";

        int i = 0;
        foreach(float score in _topScores)
        {
            string displayScore = score.ToString("F0");
            text += (i + 1).ToString() + ". " + displayScore;
            if (i < topScores.Count) text += "\n";

            if (i == displayScoresNum-1) break;

            i++;
        }

        topScoresText.text = text;
    }

	void Start () {
		
	}
	
	void Update () {
        //DebugControls();
	}

    void DebugControls()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //SubmitScore(Random.Range(1000, 100000));
        }
    }
}
