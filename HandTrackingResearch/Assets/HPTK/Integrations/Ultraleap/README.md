# Setup instructions

- Download the Leap Motion Unity Modules (https://developer.leapmotion.com/unity)
- Unpack Core.unitypackage into your Unity project

## If using UnityXR
- Follow the [UnityXR setup instructions](https://jorge-jgnz94.gitbook.io/hptk/setup/basics) but use the **DefaultSetup.LeapMotion** prefab instead of DefaultSetup.UnityXR

## Else
- Follow the [relevant setup instructions](https://jorge-jgnz94.gitbook.io/hptk/setup) for your headset
- Add a `LeapXRServiceProvider` to your MainCamera
- Add the IDPs.Ultraleap prefab as a child of your HPTK GameObject
- Set the Input Data Providers in your HPTK component to the respective LeapMotionTrackers

If using the DefaultAvatar it should now be driven by Leap Motion data