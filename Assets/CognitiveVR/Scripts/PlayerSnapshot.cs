﻿using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// container for cached player tracking data points
/// </summary>

namespace CognitiveVR
{
    public class PlayerSnapshot
    {
        public enum SnapshotType
        {
            World,
            Dynamic,
            Sky
        }
        public SnapshotType snapshotType;


        public static int Resolution = 64;

        public static ColorSpace colorSpace = ColorSpace.Linear;

        public double timestamp;
        //public Dictionary<string, object> Properties = new Dictionary<string, object>();
        public string ObjectId = "";
        //used instead of GazePoint when looking at dynamic object
        public Vector3 LocalGaze;
        //position of HMD
        public Vector3 Position;
        //direction of hmd forward
        public Vector3 HMDForward;
        public float NearDepth;
        public float FarDepth;
        public RenderTexture RTex;
        public Quaternion HMDRotation;
        public Vector3 GazeDirection;
        //world gaze point. used when looking at world, not dynamic or sky
        public Vector3 GazePoint;
        //xy of screen when tracking eyes
        public Vector3 HMDGazePoint;

        public PlayerSnapshot(int framecount)
        {
            timestamp = Util.Timestamp(framecount);
        }

        public float GetAdjustedDistance(float far, Vector3 gazeDir, Vector3 camForward)
        {
            //====== edge
            //float height = 2 * far * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            //float width = height * aspect;
            //Debug.Log("height: " + height + "\nwidth: "+width);
            //this is the maximum distance - looking directly at the edge of the frustrum
            //float edgeDistance = Mathf.Sqrt(Mathf.Pow(width * 0.5f, 2) + Mathf.Pow(cam.farClipPlane, 2));

            //Vector3 position = CognitiveVR_Manager.HMD.position;

            //======= gaze (farclip distance)
            //Debug.DrawRay(position, gazeDir * far, Color.red, 0.1f);
            //Debug.DrawRay(position, camForward * far, new Color(0.5f, 0, 0), 0.1f);

            //Debug.Log("get adjusted distance. gaze direction " + gazeDir + "      camera forward  " + camForward);

            //dot product to find projection of gaze with direction
            float fwdAmount = Vector3.Dot(gazeDir.normalized * far, camForward.normalized);
            //Vector3 fwdPoint = camForward * fwdAmount;
            //Debug.DrawRay(fwdPoint, Vector3.up, Color.cyan, 0.1f);
            //Debug.DrawRay(gazeDir * far, Vector3.up, Color.yellow, 0.1f);

            //======= angle towards farPlane
            //get angle between center and gaze direction. cos(A) = b/c
            float gazeRads = Mathf.Acos(fwdAmount / far);
            if (Mathf.Approximately(fwdAmount, far))
            {
                //when fwdAmount == far, Acos returns NaN for some reason
                gazeRads = 0;
            }
            
            float dist = far * Mathf.Tan(gazeRads);

            float hypotenuseDist = Mathf.Sqrt(Mathf.Pow(dist, 2) + Mathf.Pow(far, 2));

            //float missingDist = hypotenuseDist-far;
            //Debug.DrawRay(position + gazeDir * far, gazeDir * missingDist, Color.green, 0.1f); //appended distance to hit farPlane
            return hypotenuseDist;
        }

        /// <summary>
        /// ignores width and height if using gaze tracking from fove/pupil labs
        /// returns true if it hit a valid point. false if the point is at the farplane
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool GetGazePoint(int width, int height, out Vector3 gazeWorldPoint)
        {
#if CVR_GAZETRACK
            float relativeDepth = 0;

            Vector2 snapshotPixel = HMDGazePoint;

            snapshotPixel *= Resolution;

            snapshotPixel.x = Mathf.Clamp(snapshotPixel.x, 0, Resolution-1);
            snapshotPixel.y = Mathf.Clamp(snapshotPixel.y, 0, Resolution-1);

            var color = GetRTColor(RTex, (int)snapshotPixel.x, (int)snapshotPixel.y);

            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                relativeDepth = color.linear.r; //does running the color through this linear multiplier cause NaN issues? GetAdjustedDistance passed in essentially 0?
            }
            else
            {
                relativeDepth = color.r;
            }

            if (relativeDepth > 0.99f)
            {
                gazeWorldPoint = Vector3.zero;
                return false;
            }

            //Debug.Log("relativeDepth " + relativeDepth);

            //how does this get the actual depth? missing an argument?
            float actualDepth = GetAdjustedDistance(FarDepth, GazeDirection, HMDForward);
            //Debug.Log("actualDepth " + actualDepth); //adjusted for trigonometry

            float actualDistance = Mathf.Lerp(NearDepth, actualDepth, relativeDepth);
            //Debug.Log("actualDistance " + actualDistance);

            gazeWorldPoint = Position + GazeDirection * actualDistance;

            //Debug.Log("gazeWorldPoint " + gazeWorldPoint);

            return true;
#else
            float relativeDepth = 0;
            //Vector3 gazeWorldPoint;

            //var color = GetRTColor((RenderTexture)Properties["renderDepth"], width / 2, height / 2);
            var color = GetRTColor(RTex, width / 2, height / 2);
            if (colorSpace == ColorSpace.Linear)
            {
                relativeDepth = color.linear.r;

            }
            else
            {
                relativeDepth = color.r;
            }

            if (relativeDepth > 0.99f)
            {
                gazeWorldPoint = Util.vector_zero;
                return false;
            }


            //float actualDistance = Mathf.Lerp(NearDepth, FarDepth, relativeDepth);
            float actualDistance = NearDepth + (FarDepth - NearDepth) * relativeDepth;
            gazeWorldPoint = Position + HMDForward * actualDistance;

            //float actualDistance = Mathf.Lerp((float)Properties["nearDepth"], (float)Properties["farDepth"], relativeDepth);
            //gazeWorldPoint = (Vector3)Properties["position"] + (Vector3)Properties["hmdForward"] * actualDistance;
            return true;
#endif


        }

        public static Texture2D tex;
        public Color GetRTColor(RenderTexture rt, int x, int y)
        {
            if (tex == null)
            {
#if CVR_GAZETRACK
                tex = new Texture2D(Resolution, Resolution);
#else
                tex = new Texture2D(1,1);
#endif
            }

            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = rt;

#if CVR_GAZETRACK //TODO read 1 pixel from the render texture where the request point is
            tex.ReadPixels(new Rect(0, 0, Resolution, Resolution), 0, 0, false);
            var color = tex.GetPixel(x,y);
#else
            tex.ReadPixels(new Rect(x, y, 1, 1), 0, 0, false);
            var color = tex.GetPixel(0,0);
#endif

            RenderTexture.active = currentActiveRT;
            return color;
        }
    }
}