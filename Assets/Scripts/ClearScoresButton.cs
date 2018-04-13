using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearScoresButton : GazeListener {

    public TextMesh clearScoresText;
    public MeshRenderer clearScoresTextRenderer;
    public Color hoverColor = Color.red, idleColor = Color.grey;

    enum State { clearScores, areYouSure }
    State state = State.clearScores;

    void Start () {
		
	}
	
	void Update () {
		
	}

    void ChangeTextColor(Color c)
    {
        clearScoresTextRenderer.material.color = c;
    }

    public override void OnLook()
    {
        base.OnLook();
        ChangeTextColor(hoverColor);
    }

    public override void OnLookAway()
    {
        base.OnLookAway();
        ChangeTextColor(idleColor);
        ChangeState(State.clearScores);
    }

    
    public override void OnClick()
    {
        if (state == State.clearScores)
        {
            ChangeState(State.areYouSure);
        } 
        else if (state == State.areYouSure)
        {
            Leaderboard.instance.ClearScores();
            ChangeState(State.clearScores);
        }      
    }

    void ChangeState(State s)
    {
        state = s;
        switch (s)
        {
            case State.areYouSure:
                clearScoresText.text = "TAP TO CLEAR";
                break;

            case State.clearScores:
                clearScoresText.text = "CLEAR SCORES";
                break;
        }
    }
}
