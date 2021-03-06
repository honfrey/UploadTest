﻿using UnityEngine;
using CognitiveVR;
using System.Collections;
using System.Collections.Generic;
using System;
#if CVR_STEAMVR
using Valve.VR;
#endif

#if CVR_META
using System.Runtime.InteropServices;
#endif

/// <summary>
/// initializes CognitiveVR analytics. Add components to track additional events
/// </summary>

//init components
//update ticks + events
//level change events
//get hmd + controllers
//quit and destroy events
//otherwise use core

namespace CognitiveVR
{
    public partial class CognitiveVR_Manager : MonoBehaviour
    {

#if CVR_META
        [DllImport("MetaVisionDLL", EntryPoint = "getSerialNumberAndCalibration")]
        internal static extern bool GetSerialNumberAndCalibration([MarshalAs(UnmanagedType.BStr), Out] out string serial, [MarshalAs(UnmanagedType.BStr), Out] out string xml);
#elif CVR_STEAMVR
        string GetStringProperty(CVRSystem system, uint deviceId, ETrackedDeviceProperty prop)
        {
            var error = ETrackedPropertyError.TrackedProp_Success;
            var result = new System.Text.StringBuilder();

            var capacity = system.GetStringTrackedDeviceProperty(deviceId, prop, result, 64, ref error);
            if (capacity > 0)
                return result.ToString();
            return string.Empty;
        }
#endif

        #region Events
        public delegate void CoreInitHandler(Error initError);
        /// <summary>
        /// CognitiveVR Core.Init callback
        /// </summary>
        public static event CoreInitHandler InitEvent;
        public void OnInit(Error initError)
        {
            Util.logDebug("CognitiveVR OnInit recieved response " + initError.ToString());
            if (initError == Error.AlreadyInitialized)
            {
                return;
            }
            initResponse = initError;

            OutstandingInitRequest = false;

            if (initError == Error.Success)
            {
                new CustomEvent("cvr.session").Send();
            }
            else //some failure
            {
                StopAllCoroutines();
            }

            var components = GetComponentsInChildren<CognitiveVR.Components.CognitiveVRAnalyticsComponent>();
            for (int i = 0; i < components.Length; i++)
            {
                components[i].CognitiveVR_Init(initError);
            }
            PlayerRecorderInit(initError);
            if (InitEvent != null) { InitEvent(initError); }

            //required for when restarting cognitiveVR manager
            /*foreach (var d in InitEvent.GetInvocationList())
            {
                InitEvent -= (CoreInitHandler)d;
            }*/

#if CVR_META
            string serialnumber;
            string xml;
            if (GetSerialNumberAndCalibration(out serialnumber, out xml))
            {              
                var metaProperties = new Dictionary<string,object>();
                metaProperties.Add("cvr.vr.serialnumber",serialnumber);
                UpdateDeviceState(metaProperties);
                //Instrumentation.updateDeviceState(metaProperties);
            }
#elif CVR_STEAMVR
            var serialnumber = GetStringProperty(OpenVR.System, 0, ETrackedDeviceProperty.Prop_SerialNumber_String);

            if (!string.IsNullOrEmpty(serialnumber))
            {
                var properties = new Dictionary<string, object>();
                properties.Add("cvr.vr.serialnumber", serialnumber);

                UpdateDeviceState(properties);
                //Instrumentation.updateDeviceState(properties);
            }
#endif
        }

        public delegate void UpdateHandler();
        /// <summary>
        /// Update. Called through Manager's update function for easy enabling/disabling
        /// </summary>
        public static event UpdateHandler UpdateEvent;
        public void OnUpdate() { if (UpdateEvent != null) { UpdateEvent(); } }

        public delegate void TickHandler();
        /// <summary>
        /// repeatedly called. interval is CognitiveVR_Preferences.Instance.PlayerSnapshotInterval
        /// </summary>
        public static event TickHandler TickEvent;
        public void OnTick() { if (TickEvent != null) { TickEvent(); } }

        public delegate void QuitHandler(); //quit
        /// <summary>
        /// called from Unity's built in OnApplicationQuit. Cancelling quit gets weird - do all application quit stuff in Manager
        /// </summary>
        public static event QuitHandler QuitEvent;
        public void OnQuit() { if (QuitEvent != null) { QuitEvent(); } }

        public delegate void LevelLoadedHandler(); //level
        /// <summary>
        /// called from Unity's SceneManager.SceneLoaded(scene scene)
        /// </summary>
        public static event LevelLoadedHandler LevelLoadedEvent;
        public void OnLevelLoaded() { if (LevelLoadedEvent != null) { LevelLoadedEvent(); } }

#if CVR_STEAMVR
        //1.1
        /*
        public delegate void PoseUpdateHandler(params object[] args);
        /// <summary>
        /// params are SteamVR pose args. does not check index. Currently only used for TrackedDevice valid/disconnected
        /// </summary>
        public static event PoseUpdateHandler PoseUpdateEvent;
        public void OnPoseUpdate(params object[] args) { if (PoseUpdateEvent != null) { PoseUpdateEvent(args); } }
        */

        //1.2
        public delegate void PoseUpdateHandler(params TrackedDevicePose_t[] args);
        /// <summary>
        /// params are SteamVR pose args. does not check index. Currently only used for TrackedDevice valid/disconnected
        /// </summary>
        public static event PoseUpdateHandler PoseUpdateEvent;
        public void OnPoseUpdate(params TrackedDevicePose_t[] args) { if (PoseUpdateEvent != null) { PoseUpdateEvent(args); } }

        //1.1 and 1.2
        public delegate void PoseEventHandler(Valve.VR.EVREventType eventType);
        /// <summary>
        /// polled in Update. sends all events from Valve.VR.OpenVR.System.PollNextEvent(ref vrEvent, size)
        /// </summary>
        public static event PoseEventHandler PoseEvent;
        public void OnPoseEvent(Valve.VR.EVREventType eventType) { if (PoseEvent != null) { PoseEvent(eventType); } }

#endif
#endregion

#region HMD and Controllers


#if CVR_OCULUS
        static OVRCameraRig _cameraRig;
        static OVRCameraRig CameraRig
        {
            get
            {
                if (_cameraRig == null)
                {
                    _cameraRig = FindObjectOfType<OVRCameraRig>();
                }
                return _cameraRig;
            }
        }
#endif

        private static Transform _hmd;
        /// <summary>Returns HMD based on included SDK, or Camera.Main if no SDK is used. MAY RETURN NULL!</summary>
        public static Transform HMD
        {
            get
            {
                if (_hmd == null)
                {
#if CVR_STEAMVR
                    SteamVR_Camera cam = FindObjectOfType<SteamVR_Camera>();
                    if (cam != null){ _hmd = cam.transform; }
                    if (_hmd == null)
                    {
                        if (Camera.main == null)
                            return null;
                        _hmd = Camera.main.transform;
                    }
#elif CVR_OCULUS
                    OVRCameraRig rig = FindObjectOfType<OVRCameraRig>();
                    if (rig != null)
                    {
                        Camera cam = rig.GetComponentInChildren<Camera>();
                        _hmd = cam.transform;
                    }
                    if (_hmd == null)
                    {
                        if (Camera.main == null)
                            return null;
                        _hmd = Camera.main.transform;
                    }
#elif CVR_FOVE
                    /*FoveEyeCamera eyecam = FindObjectOfType<FoveEyeCamera>();
                    if (eyecam != null)
                    {
                        Camera cam = eyecam.GetComponentInChildren<Camera>();
                        _hmd = cam.transform;
                    }*/
                    if (_hmd == null)
                    {
                        if (Camera.main == null)
                            return null;
                        _hmd = Camera.main.transform;
                    }
#else
                    if (Camera.main == null)
                        return null;
                    _hmd = Camera.main.transform;
#endif
                }
                return _hmd;
            }
        }

#if CVR_STEAMVR

        static void InitializeControllers()
        {
            //OnEnable order breaks everything. just read the controller variables from controller manager - cannot compare indexes

            SteamVR_ControllerManager cm = FindObjectOfType<SteamVR_ControllerManager>();
            if (cm == null)
            {
                Util.logError("Can't find SteamVR_ControllerManager. Unable to initialize controllers");
                return;
            }

            if (controllers[0] == null)
            {
                controllers[0] = new ControllerInfo() { transform = cm.left.transform, isRight = false };
            }

            if (controllers[0].id < 0)
            {
                if (cm.left != null)
                {
                    int controllerIndex = (int)cm.left.GetComponent<SteamVR_TrackedObject>().index;
                    if (controllerIndex > 0)
                    {
                        controllers[0].id = controllerIndex;
                    }
                }
            }

            if (controllers[1] == null)
            {
                controllers[1] = new ControllerInfo() { transform = cm.right.transform, isRight = true };
            }
            if (controllers[1].id < 0)
            {
                if (cm.right != null)
                {

                    int controllerIndex = (int)cm.right.GetComponent<SteamVR_TrackedObject>().index;
                    if (controllerIndex > 0)
                    {
                        controllers[1].id = controllerIndex;
                    }
                }
            }
        }

        public class ControllerInfo
        {
            public Transform transform;
            public bool isRight;
            public int id = -1;
        }

        static ControllerInfo[] controllers = new ControllerInfo[2];

        public static ControllerInfo GetControllerInfo(int deviceID)
        {
            InitializeControllers();
            if (controllers[0].id == deviceID) { return controllers[0]; }
            if (controllers[1].id == deviceID) { return controllers[1]; }
            return null;
        }
#endif
        /// <summary>
        /// steamvr ID is tracked device id
        /// oculus ID 0 is right, 1 is left controller
        /// </summary>
        public static Transform GetController(int deviceid)
        {
#if CVR_STEAMVR
            InitializeControllers();
            if (controllers[0].id == deviceid) { return controllers[0].transform; }
            if (controllers[1].id == deviceid) { return controllers[1].transform; }
            return null;
#elif CVR_OCULUS
            // OVR doesn't allow access to controller transforms - Position and Rotation available in OVRInput
            return null;
#else
            return null;
#endif
        }

        public static Transform GetController(bool right)
        {
#if CVR_STEAMVR
            InitializeControllers();
            if (right == controllers[0].isRight) { return controllers[0].transform; }
            if (right == controllers[1].isRight) { return controllers[1].transform; }
            return null;
#elif CVR_OCULUS
            return null;
#else
            return null;
#endif
        }

        /// <summary>Returns Tracked Controller position by index. Based on SDK</summary>
        public static Vector3 GetControllerPosition(bool rightController)
        {
#if CVR_STEAMVR
            InitializeControllers();
            if (rightController)
            {
                if (controllers[0].transform != null)
                { return controllers[0].transform.position; }
            }
            else
            {
                if (controllers[1].transform != null)
                { return controllers[1].transform.position; }
            }
            return Vector3.zero;
#elif CVR_OCULUS
            if (rightController)
            {
                if (CameraRig != null)
                    return CameraRig.rightHandAnchor.position;
            }
            else
            {
                if (CameraRig != null)
                    return CameraRig.leftHandAnchor.position;
            }
            return Vector3.zero;
#else
            return Vector3.zero;
#endif
        }

#endregion

        private static CognitiveVR_Manager instance;
        public static CognitiveVR_Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CognitiveVR_Manager>();
                }
                return instance;
            }
        }
        YieldInstruction playerSnapshotInverval;

        [Tooltip("Enable automatic initialization. If false, you must manually call Initialize()")]
        public bool InitializeOnStart = true;

        [HideInInspector] //complete this option later
        [Tooltip("Save ExitPoll questions and answers to disk if internet connection is unavailable")]
        public bool SaveExitPollOnDevice = false;

        static Error initResponse = Error.NotInitialized;
        public static Error InitResponse { get { return initResponse; } }
        bool OutstandingInitRequest = false;

        public float StartupDelayTime = 2;

        private void OnEnable()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            //TODO expose this value to initialize a pool when writing lots of dynamic objects
            for (int i = 0; i < 100; i++)
            {
                DynamicObjectSnapshot.snapshotQueue.Enqueue(new DynamicObjectSnapshot());
            }
        }

        IEnumerator Start()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            if (StartupDelayTime > 0)
            {
                yield return new WaitForSeconds(StartupDelayTime);
            }
            if (InitializeOnStart)
                Initialize("");
        }

        private void OnValidate()
        {
            if (StartupDelayTime < 0) { StartupDelayTime = 0;}
        }

        public void Initialize(string userName="", Dictionary<string,object> userProperties = null)
        {
            Util.logDebug("CognitiveVR_Manager Initialize");
            if (instance != null && instance != this)
            {
                Util.logDebug("CognitiveVR_Manager Initialize instance is not null and not this! Destroy");
                Destroy(gameObject);
                return;
            } //destroy if there's already another manager
            if (instance == this && Core.Initialized)
            {
                Util.logDebug("CognitiveVR_Manager Initialize instance is this! <color=red>Skip Initialize</color>");
                return;
            } //skip if this manage has already been initialized

            if (!CognitiveVR_Preferences.Instance.IsAPIKeyValid)
            {
                Util.logDebug("CognitiveVR_Manager Initialize does not have valid apikey");
                return;
            }
            if (OutstandingInitRequest)
            {
                Util.logDebug("CognitiveVR_Manager Initialize already called. Waiting for response");
                return;
            }
            
            OutstandingInitRequest = true;

            ExitPoll.Initialize();

            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            CognitiveVR_Preferences.SetTrackingSceneName(sceneName);

            Instrumentation.SetMaxTransactions(CognitiveVR_Preferences.S_TransactionSnapshotCount);

            playerSnapshotInverval = new WaitForSeconds(CognitiveVR.CognitiveVR_Preferences.S_SnapshotInterval);
            StartCoroutine(Tick());

#if CVR_STEAMVR
            SteamVR_Events.NewPoses.AddListener(OnPoseUpdate); //steamvr 1.2
            //SteamVR_Utils.Event.Listen("new_poses", OnPoseUpdate); //steamvr 1.1
#endif

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_SceneLoaded;
            SceneManager_SceneLoaded(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), UnityEngine.SceneManagement.LoadSceneMode.Single);

            Core.UserId = userName;

            CognitiveVR.Core.init(OnInit); //TODO return errors from init method, not callback since there isn't a delay on startup
            UpdateDeviceState(Util.GetDeviceProperties() as Dictionary<string,object>);
            UpdateUserState(userProperties);
            UpdateUserState("name", userName);
        }
        
        private void SceneManager_SceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            Debug.Log("scene manager scene loaded event");
            DynamicObject.ClearObjectIds();
            if (!CognitiveVR_Preferences.Instance.SendDataOnLevelLoad)
            {
                CognitiveVR_Preferences.SetTrackingSceneName(scene.name);
                OnLevelLoaded();
                return;
            }

            if (!string.IsNullOrEmpty(CognitiveVR_Preferences.TrackingSceneName))
            {
                CognitiveVR_Preferences.SceneSettings lastSceneSettings = CognitiveVR_Preferences.FindTrackingScene();
                if (lastSceneSettings != null)
                {
                    Debug.Log("scene load last scene was " + lastSceneSettings.SceneName);
                    if (!string.IsNullOrEmpty(lastSceneSettings.SceneId))
                    {
                        Util.logDebug("SceneManager_SceneLoaded ======================PlayerRecorder SceneLoaded send all data");
                        Core.SendDataEvent();
                        //SendPlayerGazeSnapshots();
                        CognitiveVR_Manager.TickEvent -= CognitiveVR_Manager_OnTick;
                    }
                }

                Core.CurrentSceneId = string.Empty;
                Core.CurrentSceneVersionNumber = 0;

                CognitiveVR_Preferences.SceneSettings sceneSettings = CognitiveVR_Preferences.Instance.FindScene(scene.name);
                if (sceneSettings != null)
                {
                    if (!string.IsNullOrEmpty(sceneSettings.SceneId))
                    {
                        CognitiveVR_Manager.TickEvent += CognitiveVR_Manager_OnTick;
                        Core.CurrentSceneId = sceneSettings.SceneId;
                        Core.CurrentSceneVersionNumber = sceneSettings.VersionNumber;
                    }
                    else
                    {
                        Util.logWarning("PlayerRecorder sceneLoaded SceneId is empty for scene " + sceneSettings.SceneName + ". Not recording");
                    }
                }
                else
                {
                    Util.logWarning("PlayerRecorder sceneLoaded couldn't find scene " + scene.name + ". Not recording");
                }
            }

            CognitiveVR_Preferences.SetTrackingSceneName(scene.name);
            OnLevelLoaded();
        }

        public static int frameCount;

        //start after successful init callback
        IEnumerator Tick()
        {
            while (Application.isPlaying) //cognitive manager is destroyed on end session, which will stop this
            {
                yield return playerSnapshotInverval;
                frameCount = Time.frameCount;
                OnTick();
            }
        }

        void Update()
        {
            if (initResponse != Error.Success)
            {
                return;
            }

            //doPostRender = false;

            OnUpdate();
            UpdateSendHotkeyCheck();

#if CVR_STEAMVR
            var system = Valve.VR.OpenVR.System;
            if (system != null)
            {
                var vrEvent = new Valve.VR.VREvent_t();
                var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));
                for (int i = 0; i < 64; i++)
                {
                    if (!system.PollNextEvent(ref vrEvent, size))
                        break;
                    OnPoseEvent((Valve.VR.EVREventType)vrEvent.eventType);
                }
            }
#endif
        }

        void UpdateSendHotkeyCheck()
        {
            CognitiveVR_Preferences prefs = CognitiveVR_Preferences.Instance;

            if (!prefs.SendDataOnHotkey) { return; }
            if (Input.GetKeyDown(prefs.SendDataHotkey))
            {
                if (prefs.HotkeyShift && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) { return; }
                if (prefs.HotkeyAlt && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt)) { return; }
                if (prefs.HotkeyCtrl && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)) { return; }

                Core.SendDataEvent();
            }
        }

        /// <summary>
        /// End the cognitivevr session. sends any outstanding data to dashboard and sceneexplorer
        /// requires calling Initialize to create a new session id and begin recording analytics again
        /// </summary>
        public void EndSession()
        {
            double playtime = Util.Timestamp() - Core.SessionTimeStamp;
            new CustomEvent("cvr.session").SetProperty("sessionlength", playtime).Send();

            Core.SendDataEvent();

            //clear properties from last session
            newDeviceProperties.Clear();
            knownDeviceProperties.Clear();

            newUserProperties.Clear();
            knownUserProperties.Clear();

            CleanupEvents();
            Core.reset();
            initResponse = Error.NotInitialized;
        }

        void OnDestroy()
        {
            if (instance != this) { return; }
            if (!Application.isPlaying) { return; }

            OnQuit();
            //OnSendData();

            if (Core.Initialized)
            {
                Core.reset();
            }

            CleanupEvents();
        }

        void CleanupEvents()
        {
            CleanupPlayerRecorderEvents();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_SceneLoaded;
            initResponse = Error.NotInitialized;
        }

        //writes manifest entry and object snapshot to string then send http request
        public IEnumerator Thread_StringThenSend(Queue<DynamicObjectManifestEntry> SendObjectManifest, Queue<DynamicObjectSnapshot> SendObjectSnapshots, CognitiveVR_Preferences.SceneSettings trackingSettings, string uniqueid, double sessiontimestamp, string sessionid)
        {
            //save and clear snapshots and manifest entries
            DynamicObjectManifestEntry[] tempObjectManifest = new DynamicObjectManifestEntry[SendObjectManifest.Count];
            SendObjectManifest.CopyTo(tempObjectManifest,0);
            SendObjectManifest.Clear();


            DynamicObjectSnapshot[] tempSnapshots = new DynamicObjectSnapshot[SendObjectSnapshots.Count];
            SendObjectSnapshots.CopyTo(tempSnapshots, 0);

            //for (int i = 0; i<tempSnapshots.Length; i++)
            //{
            //    var s = DynamicObject.SetSnapshot(tempSnapshots[i]);
            //    Debug.Log(">>>>>>>>>>>>>queue snapshot  " + s);
            //}


            //write manifest entries to thread
            List<string> manifestEntries = new List<string>();
            bool done = true;
            if (tempObjectManifest.Length > 0)
            {
                done = false;
                new System.Threading.Thread(() =>
                {
                    for (int i = 0; i < tempObjectManifest.Length; i++)
                    {
                        manifestEntries.Add(DynamicObject.SetManifestEntry(tempObjectManifest[i]));
                    }
                    done = true;
                }).Start();

                while (!done)
                {
                    yield return null;
                }
            }



            List<string> snapshots = new List<string>();
            if (tempSnapshots.Length > 0)
            {
                done = false;
                new System.Threading.Thread(() =>
                {
                    for (int i = 0; i < tempSnapshots.Length; i++)
                    {
                        snapshots.Add(DynamicObject.SetSnapshot(tempSnapshots[i]));
                    }
                    //System.GC.Collect();
                    done = true;
                }).Start();

                while (!done)
                {
                    yield return null;
                }
            }

            while (SendObjectSnapshots.Count > 0)
            {
                SendObjectSnapshots.Dequeue().ReturnToPool();
            }

            DynamicObject.SendSavedSnapshots(manifestEntries, snapshots,trackingSettings,uniqueid,sessiontimestamp,sessionid);
        }

#region Application Quit
        bool hasCanceled = false;
        void OnApplicationQuit()
        {
            if (hasCanceled) { return; }

            if (InitResponse != Error.Success) { return; }

            double playtime = Util.Timestamp() - Core.SessionTimeStamp;
            if (QuitEvent == null)
            {
				CognitiveVR.Util.logDebug("session length " + playtime);
                new CustomEvent("cvr.session").SetProperty("sessionlength",playtime).Send();
                return;
            }

			CognitiveVR.Util.logDebug("session length " + playtime);
            new CustomEvent("cvr.session").SetProperty("sessionlength", playtime).Send();
            Application.CancelQuit();


            Core.SendDataEvent();
            Core.reset();


            //Camera.onPostRender -= MyPostRender;
            //OnQuit();
            StartCoroutine(SlowQuit());
        }

        IEnumerator SlowQuit()
        {
            yield return new WaitForSeconds(0.5f);
            hasCanceled = true;            
            Application.Quit();
        }

        #endregion

        public static Dictionary<string, object> GetNewDeviceProperties(bool clearNewProperties)
        {
            if (clearNewProperties)
            {
                Dictionary<string, object> returndict = new Dictionary<string, object>(newDeviceProperties);
                newDeviceProperties.Clear();
                return returndict;
            }
            return newDeviceProperties;
        }
        static Dictionary<string, object> newDeviceProperties = new Dictionary<string, object>();
        static Dictionary<string, object> knownDeviceProperties = new Dictionary<string, object>();
        public static void UpdateDeviceState(Dictionary<string, object> dictionary)
        {
            if (dictionary == null) { dictionary = new Dictionary<string, object>(); }

            foreach (var kvp in dictionary)
            {
                if (knownDeviceProperties.ContainsKey(kvp.Key) && knownDeviceProperties[kvp.Key] != kvp.Value) //update value
                {
                    if (newDeviceProperties.ContainsKey(kvp.Key))
                    {
                        newDeviceProperties[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        newDeviceProperties.Add(kvp.Key, kvp.Value);
                    }
                    knownDeviceProperties[kvp.Key] = kvp.Value;
                }
                else if (!knownDeviceProperties.ContainsKey(kvp.Key)) //add value
                {
                    knownDeviceProperties.Add(kvp.Key, kvp.Value);
                    newDeviceProperties.Add(kvp.Key, kvp.Value);
                }
            }
        }
        public static void UpdateDeviceState(string key, object value)
        {
            if (knownDeviceProperties.ContainsKey(key) && knownDeviceProperties[key]!=value) //update value
            {
                if (newDeviceProperties.ContainsKey(key))
                {
                    newDeviceProperties[key] = value;
                }
                else
                {
                    newDeviceProperties.Add(key, value);
                }
                knownDeviceProperties[key] = value;
            }
            else if(!knownDeviceProperties.ContainsKey(key)) //add value
            {
                knownDeviceProperties.Add(key, value);
                newDeviceProperties.Add(key, value);
            }
        }

        public static Dictionary<string, object> GetNewUserProperties(bool clearNewProperties)
        {
            if (clearNewProperties)
            {
                Dictionary<string, object> returndict = new Dictionary<string, object>(newUserProperties);
                newUserProperties.Clear();
                return returndict;
            }
            return newUserProperties;
        }
        static Dictionary<string, object> newUserProperties = new Dictionary<string, object>();
        static Dictionary<string, object> knownUserProperties = new Dictionary<string, object>();
        public static void UpdateUserState(Dictionary<string, object> dictionary)
        {
            if (dictionary == null) { dictionary = new Dictionary<string, object>(); }
            foreach (var kvp in dictionary)
            {
                if (knownUserProperties.ContainsKey(kvp.Key) && knownUserProperties[kvp.Key] != kvp.Value) //update value
                {
                    if (newUserProperties.ContainsKey(kvp.Key))
                    {
                        newUserProperties[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        newUserProperties.Add(kvp.Key, kvp.Value);
                    }
                    knownUserProperties[kvp.Key] = kvp.Value;
                }
                else if (!knownUserProperties.ContainsKey(kvp.Key)) //add value
                {
                    knownUserProperties.Add(kvp.Key, kvp.Value);
                    newUserProperties.Add(kvp.Key, kvp.Value);
                }
            }
        }
        public static void UpdateUserState(string key, object value)
        {
            if (knownUserProperties.ContainsKey(key) && knownUserProperties[key] != value) //update value
            {
                if (newUserProperties.ContainsKey(key))
                {
                    newUserProperties[key] = value;
                }
                else
                {
                    newUserProperties.Add(key, value);
                }
                knownUserProperties[key] = value;
            }
            else if (!knownUserProperties.ContainsKey(key)) //add value
            {
                knownUserProperties.Add(key, value);
                newUserProperties.Add(key, value);
            }
        }
    }
}