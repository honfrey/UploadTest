using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class GazeListener : MonoBehaviour {
    
    public virtual void OnLook()
    {

    }

    public virtual void OnLook(bool isFocused)
    {

    }

    public virtual void OnClick()
    {

    }

    public virtual void OnLookAway()
    {

    }

    public virtual void OnLookAway(bool isFocused)
    {

    }
}
