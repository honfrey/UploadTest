using UnityEngine;
using System.Collections;

public class DebugPrintMe : MonoBehaviour
{

    void OnEnable()
    {
        Debug.Log("Enabling " + gameObject);
    }

    void OnDisable()
    {
        Debug.Log("Disabling " + gameObject);
    }
}
