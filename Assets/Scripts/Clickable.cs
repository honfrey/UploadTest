using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public abstract class Clickable : MonoBehaviour {

    public Renderer rend;
    bool isMouseOver = false;
    bool hasClicked = false;

    void Start () {
	
	}
	
	void Update () {
	
	}

    public abstract void ClickFunction();

    void OnMouseEnter()
    {
        ChangeAlpha(0.5f);
        isMouseOver = true;
    }

    void OnMouseDown()
    {
        ChangeAlpha(0.35f);
        hasClicked = true;
    }

    void OnMouseUp()
    {
        if (isMouseOver && hasClicked)
        {
            ClickFunction();
            ChangeAlpha(0.5f);
        } 
    }

    void OnMouseExit()
    {
        ChangeAlpha(1);
        isMouseOver = false;
    }

    void ChangeAlpha(float a)
    {
        Color c = rend.material.color;
        c.a = a;
        rend.material.color = c;
    }

    public void ChangeColour(Color c)
    {
        rend.material.color = c;
    }
}
