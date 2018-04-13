using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour {

    public enum Platform {PC, GearVRController}
    public Platform platform;
    public Transform transferUnderCam;
    public Transform defaultPlatformTrans;

    [System.Serializable]
    public struct PlatformInfo
    {
        public string name;
        public Platform platform;
        public Transform cameraTrans;
        public GameObject[] activateGos, deactivateGos;
    }

    public PlatformInfo[] platformInfos;
    public Dictionary<Platform, PlatformInfo> platformInfoNameDictionary = new Dictionary<Platform, PlatformInfo>();

    void Awake () {
        Setup();
	}
	
    void Setup()
    {
        // setup dictionary
        foreach (PlatformInfo pi in platformInfos)
            platformInfoNameDictionary.Add(pi.platform, pi);

        // get the specified platform info
        PlatformInfo platformInfo = platformInfoNameDictionary[platform];

        // switch platforms to the correct camera
        for (int i = transferUnderCam.childCount - 1; i >= 0; --i)
        {
            Transform child = transferUnderCam.GetChild(i);
            child.SetParent(platformInfo.cameraTrans, false);
        }

        // turn off default camera transform
        defaultPlatformTrans.gameObject.SetActive(false);

        // turn on GameObjects
        foreach (GameObject go in platformInfo.activateGos)
            go.SetActive(true);

        // turn off GameObjects
        foreach (GameObject go in platformInfo.deactivateGos)
            go.SetActive(false);
    }
}
