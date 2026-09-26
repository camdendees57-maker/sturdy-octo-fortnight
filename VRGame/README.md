# VR Locomotion Test

Standalone Unity 6 prototype for Meta Quest/OpenXR.

## Included

- Stereoscopic XR camera driven by OpenXR.
- Tracked left/right controller positions.
- Gorilla-style arm-push locomotion.
- Reach clamping.
- Gravity and body collision.
- Procedural test arena with walls, platforms, and climb poles.
- No Gorilla Tag assets or proprietary code.

## Setup

Use Unity 6000.0.66f2 or newer. Meta currently recommends Unity 6.1+ and Unity OpenXR for new Quest projects.

Open the project folder VRGame and load Assets/Scenes/Main.unity.

For Quest, install Android Build Support, enable OpenXR for Android/Meta Quest, select the Meta Quest/Android build profile, and build to a developer-enabled headset.

If this prototype becomes the main app, use the Android application identifier TagtusVR.IsCool.

The original Android downloader remains outside VRGame.
