using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using UnityEngine;
using Leap.Unity;
using Leap;

namespace HandPhysicsToolkit.Input
{
    public class LeapMotionInputDataProvider : InputDataProvider
    {
        [Header("Leap Motion")]
        public Transform origin;

        [Header("Scale estimation")]
        public bool autoScale = true;
        [Range(-1.5f, 1.5f)]
        public float scaleOffset = 0.0f;
        public BoneScales defaultBoneScales;

        private Quaternion thumbTrapeziumRotation, leapToHPTKRotation;

        public override void InitData()
        {
            base.InitData();

            thumbTrapeziumRotation = Quaternion.AngleAxis(side == Helpers.Side.Left ? 75 : -75, Vector3.up) * Quaternion.AngleAxis(side == Helpers.Side.Left ? 180 : 0, Vector3.right);
            leapToHPTKRotation = Quaternion.AngleAxis(side == Helpers.Side.Left ? 90 : -90, Vector3.up) * Quaternion.AngleAxis(side == Helpers.Side.Left ? 180 : 0, Vector3.right);
        }

        public override void UpdateData()
        {
            base.UpdateData();

            var hand = side == Helpers.Side.Left ? Hands.Left : Hands.Right;
            if (hand == null)
            {
                log = "No hand/bone data!";
                confidence = 0f;
                return;
            }

            log = "Updating from hand tracking...";

            /*
            * 0 - wrist
            * 1 - forearm
            * 
            * 2 - thumb0
            * 3 - thumb1
            * 4 - thumb2
            * 5 - thumb3
            * 
            * 6 - index1
            * 7 - index2
            * 8 - index3
            * 
            * 9 - middle1
            * 10 - middle2
            * 11 - middle3
            * 
            * 12 - ring1
            * 13 - ring2
            * 14 - ring3
            * 
            * 15 - pinky0
            * 16 - pinky1
            * 17 - pinky2
            * 18 - pinky3
            */

            // Wrist and forearm
            SetBonePosAndRot(0, hand.WristPosition, hand.Rotation);
            SetBonePosAndRot(1, hand.Arm.ElbowPosition, hand.Arm.Rotation);

            // Thumb
            // Handle additional thumb joint by creating a "fake" trapezium located at same point as metacarpal root with a "realistic" orientation
            bones[2].space = Space.Self;
            bones[2].position = (hand.WristPosition - hand.Fingers[0].bones[1].PrevJoint).ToVector3();
            bones[2].rotation = Quaternion.Inverse(LeapToUnityQuaternion(hand.Rotation)) * Quaternion.LookRotation(hand.Direction.ToVector3(), hand.PalmNormal.ToVector3()) * thumbTrapeziumRotation;
            SetBonePosAndRot(3, hand.Fingers[0].bones[1], hand.Fingers[0].bones[1].PrevJoint, Quaternion.LookRotation(hand.Direction.ToVector3(), hand.PalmNormal.ToVector3()).ToLeapQuaternion());
            SetBonePosAndRot(4, hand.Fingers[0].bones[2], hand.Fingers[0].bones[1].PrevJoint, hand.Fingers[0].bones[1].Rotation);
            SetBonePosAndRot(5, hand.Fingers[0].bones[3], hand.Fingers[0].bones[2].PrevJoint, hand.Fingers[0].bones[2].Rotation);

            // Index
            SetBonePosAndRot(6, hand.Fingers[1].bones[1], hand.WristPosition, hand.Rotation);
            SetBonePosAndRot(7, hand.Fingers[1].bones[2], hand.Fingers[1].bones[1].PrevJoint, hand.Fingers[1].bones[1].Rotation);
            SetBonePosAndRot(8, hand.Fingers[1].bones[3], hand.Fingers[1].bones[2].PrevJoint, hand.Fingers[1].bones[2].Rotation);

            // Middle
            SetBonePosAndRot(9, hand.Fingers[2].bones[1], hand.WristPosition, hand.Rotation);
            SetBonePosAndRot(10, hand.Fingers[2].bones[2], hand.Fingers[2].bones[1].PrevJoint, hand.Fingers[2].bones[1].Rotation);
            SetBonePosAndRot(11, hand.Fingers[2].bones[3], hand.Fingers[2].bones[2].PrevJoint, hand.Fingers[2].bones[2].Rotation);

            // Ring
            SetBonePosAndRot(12, hand.Fingers[3].bones[1], hand.WristPosition, hand.Rotation);
            SetBonePosAndRot(13, hand.Fingers[3].bones[2], hand.Fingers[3].bones[1].PrevJoint, hand.Fingers[3].bones[1].Rotation);
            SetBonePosAndRot(14, hand.Fingers[3].bones[3], hand.Fingers[3].bones[2].PrevJoint, hand.Fingers[3].bones[2].Rotation);

            // Pinky
            SetBonePosAndRot(15, hand.Fingers[4].bones[0], hand.WristPosition, hand.Rotation);
            SetBonePosAndRot(16, hand.Fingers[4].bones[1], hand.Fingers[4].bones[0].PrevJoint, hand.Fingers[4].bones[0].Rotation);
            SetBonePosAndRot(17, hand.Fingers[4].bones[2], hand.Fingers[4].bones[1].PrevJoint, hand.Fingers[4].bones[1].Rotation);
            SetBonePosAndRot(18, hand.Fingers[4].bones[3], hand.Fingers[4].bones[2].PrevJoint, hand.Fingers[4].bones[2].Rotation);

            UpdateFingerPosesFromBones();

            // Set confidence to 1 if values are unreliable
            confidence = CanUseConfidence() ? hand.Confidence : 1f;

            // Hand scale estimation
            if (autoScale)
            {
                var handScale = GetMeanHandScale(hand);
                scale = handScale + scaleOffset;
            }

            log = "Updated from hand tracking!";
        }

        void SetBonePosAndRot(int idx, Bone leapBone, Vector parentPos, LeapQuaternion parentRot)
        {
            bones[idx].space = Space.Self;
            bones[idx].position = (parentPos - leapBone.PrevJoint).ToVector3();
            bones[idx].rotation = Quaternion.Inverse(LeapToUnityQuaternion(parentRot)) * LeapToUnityQuaternion(leapBone.Rotation);
        }

        void SetBonePosAndRot(int idx, Vector pos, LeapQuaternion rot)
        {
            // Wrist and forearm in world space
            bones[idx].space = Space.World;
            bones[idx].position = pos.ToVector3();
            bones[idx].rotation = LeapToUnityQuaternion(rot);

            if (origin)
            {
                bones[idx].position = origin.TransformPoint(bones[idx].position);
                bones[idx].rotation = origin.rotation * bones[idx].rotation;
            }
        }

        Quaternion LeapToUnityQuaternion(LeapQuaternion rot)
        {
            return rot.ToQuaternion() * leapToHPTKRotation;
        }

        bool CanUseConfidence()
        {
            // Find the LeapSvc.exe as it has the current version
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Leap Motion", "Core Services", "LeapSvc.exe");

            if (File.Exists(path))
            {
                // Get the version info from the service
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(path);
                string versionString = new string(myFileVersionInfo.FileVersion.Where(c => c == '.' || char.IsDigit(c)).ToArray());

                // Parse the version or use default (1.0.0)
                Version version = new Version();
                Version.TryParse(versionString, out version);

                // Gemini (V5) currently reports incorrect confidence values
                if(version.Major == 5)
                {
                    return false;
                }
            }

            return true;
        }

        float GetMeanHandScale(Hand hand)
        {
            var acc = 0f;
            acc += hand.Fingers[0].bones[2].Length / defaultBoneScales.thumbProx;
            acc += hand.Fingers[0].bones[3].Length / defaultBoneScales.thumbDist;
            acc += hand.Fingers[1].bones[1].Length / defaultBoneScales.indexProx;
            acc += hand.Fingers[1].bones[2].Length / defaultBoneScales.indexInter;
            acc += hand.Fingers[1].bones[3].Length / defaultBoneScales.indexDist;
            acc += hand.Fingers[2].bones[1].Length / defaultBoneScales.middleProx;
            acc += hand.Fingers[2].bones[2].Length / defaultBoneScales.middleInter;
            acc += hand.Fingers[2].bones[3].Length / defaultBoneScales.middleDist;
            acc += hand.Fingers[3].bones[1].Length / defaultBoneScales.ringProx;
            acc += hand.Fingers[3].bones[2].Length / defaultBoneScales.ringInter;
            acc += hand.Fingers[3].bones[3].Length / defaultBoneScales.ringDist;
            acc += hand.Fingers[4].bones[1].Length / defaultBoneScales.pinkyProx;
            acc += hand.Fingers[4].bones[2].Length / defaultBoneScales.pinkyInter;
            acc += hand.Fingers[4].bones[3].Length / defaultBoneScales.pinkyDist;

            return acc / 14f;
        }

        [System.Serializable]
        public class BoneScales
        {
            // Ignore thumb meta as HPTK has an extra bone
            // Vales set at default hand model scale
            public float thumbProx  = 0.03379309f,
                        thumbDist   = 0.0247f,
                        indexProx   = 0.0379273f,
                        indexInter  = 0.0243017f,
                        indexDist   = 0.0216f,
                        middleProx  = 0.04292699f,
                        middleInter = 0.02754715f,
                        middleDist  = 0.0241f,
                        ringProx    = 0.0389961f,
                        ringInter   = 0.0265734f,
                        ringDist    = 0.0222f,
                        pinkyProx   = 0.03072041f,
                        pinkyInter  = 0.02031138f,
                        pinkyDist   = 0.0212f;
        }
    }
}
