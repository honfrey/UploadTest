using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    /***********************************************************
     * This class controls the tutorial steps,
     * making sure that all of the steps are completed in order
     * *********************************************************/
    [SerializeField]
    private bool enableTutorial;

    [SerializeField] private GameObject[] tutorialSteps;

    [Tooltip("This is the index of the scene that will be loaded when this tutorial is complete.")]
    public int nextSceneBuildIndex;

    public static TutorialManager instance;

    [SerializeField] private int skipSecondThreshold = 5;

    private bool triggerPressed;
    private Coroutine triggerPressedCoroutine;
    private int stepCounter;
    // are we in the end of the tutorial
    private bool isLastStep;
    void Awake () {
        instance = this;
        stepCounter = 0;
    }

    private void Start()
    {
        if (enableTutorial)
        {
            EnableCorrectStep(stepCounter);
        }
    }

    // Tells tut to go to the next step
    public void MoveTutorialOneStepForward () {

        stepCounter++;
        
        if (stepCounter < tutorialSteps.Length)
        {
            EnableCorrectStep(stepCounter);
        }
        else
        {
            isLastStep = true;
        }
    }

    // Deactivate all tutorial
    void DeactivateAllSteps()
    {
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            tutorialSteps[i].SetActive(false);
        }
    }

    // enable correct tutorial step
    void EnableCorrectStep(int _step)
    {
        DeactivateAllSteps();
        tutorialSteps[_step].SetActive(true);
    }

    public bool GetIsLastStep() { return isLastStep; }

    public bool GetIsTutorialEnabled() { return enableTutorial; }

    // we use the update to give players the ability to skip the tutorial
    private void Update()
    {
        if (enableTutorial)
        {
            if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryTouchpad)) || (Input.GetMouseButton(0) && Input.GetMouseButton(1)))
            {
                if (!triggerPressed)
                {
                    triggerPressed = true;
                    triggerPressedCoroutine = StartCoroutine("TriggerPressed");
                }
            }
            else if (triggerPressedCoroutine != null)
            {
                StopCoroutine(triggerPressedCoroutine);
                triggerPressed = false;
            }
        }
    }

    // this is a countdown to make sure players have the button held down
    IEnumerator TriggerPressed()
    {
        yield return new WaitForSeconds(skipSecondThreshold);
        nextSceneBuildIndex = 3;
        //GameManager.instance.StartWinSequence();
    }

}
