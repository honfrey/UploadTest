using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public abstract class HoldClickable : MonoBehaviour
{

    public Renderer rend;
    //bool isMouseOver = false;
    bool hasClicked = false;
    public float timeToActivate = 0.75f;
    float activateTicker = 0;
    [HideInInspector]
    public bool hasActivated = false;

    void Start()
    {

    }

    void Update()
    {

    }

    public abstract void ClickFunction();

    void OnMouseEnter()
    {
        ChangeAlpha(0.5f);
    }

    void OnMouseDown()
    {
        ChangeAlpha(0.35f);
        hasClicked = true;
    }

    void OnMouseOver()
    {
        if (hasClicked)
        {
            activateTicker = Mathf.MoveTowards(activateTicker, timeToActivate, Time.deltaTime);
            ChangeColour(Color.Lerp(Color.red, Color.gray, activateTicker / timeToActivate));

            if (activateTicker == timeToActivate)
            {
                ClickFunction();
                hasClicked = false;
                hasActivated = true;
            }
        }
    }

    void OnMouseUp()
    {
        if (!hasActivated)
        {
            ResetTicker();
            //ChangeAlpha(0.5f);
        }

        ChangeAlpha(0.5f);
    }

    void OnMouseExit()
    {
        if (!hasActivated) ResetTicker();
        ChangeAlpha(1);
    }

    void ResetTicker()
    {
        hasClicked = false;
        activateTicker = 0;
        ChangeColour(Color.Lerp(Color.red, Color.gray, activateTicker / timeToActivate));
    }

    public void ResetActivation()
    {
        hasActivated = false;
        ResetTicker();
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
