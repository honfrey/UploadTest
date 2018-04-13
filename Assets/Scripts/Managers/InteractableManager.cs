using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour {

    public static InteractableManager instance;

    [Header("Objects")]
    [HideInInspector]
    public InteractableObject curHeldObject;
    public Transform heldObjectContainer;
    //public Transform objectsContainer;

    [Header("Main Materials")]
    public Material mainRegularMaterial;
    public Material mainDetectedMaterial;
    public Material mainFocusedMaterial;

    [Header("Prop Materials")]
    public Material regularMaterial;
    public Material detectedMaterial;
    public Material focusedMaterial;

    //[Header("Highlight colors")]
    //public Color detectedColor = Color.white;
    //public Color focusedColour = new Color(195 / 256f, 229 / 256f, 206 / 256f, 1f);
    //public Color highlightedColour = new Color(151 / 256f, 156 / 256f, 151 / 256f, 1f);
    //public Color openedColour = new Color(120 / 256f, 120 / 256f, 120 / 256f, 1f); //Grey;
    //public Color regularColour = Color.white;

    //public Color detectedFresnelColor = new Color(189 / 256f, 232 / 256f, 87 / 256f, 1f);
    //public Color focusedFresnelColour = new Color(0 / 256f, 255 / 256f, 12 / 256f, 1f);  //Green
    //public Color highlightedFresnelColour = new Color(5 / 256f, 104 / 256f, 68 / 256f, 1f); //dark green
    //public Color openedFresnelColour = new Color(96 / 256f, 96 / 256f, 96 / 256f, 1f); //Grey

    public float detectedFresnelIntensity = 1.5f;
    public float focusedFresnelIntensity = 3.77f;
    //public float highlightedFresnelIntensity = 6.58f;
    //public float openedFresnelIntensity = 0f;

    public enum InteractableType
    {
        pickup,
        open,
        trigger
    }

    private void Awake()
    {
        instance = this;
    }

}
