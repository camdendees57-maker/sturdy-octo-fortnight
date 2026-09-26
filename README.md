# VR Package Fetcher

Android app for downloading APK files and optional OBB expansion files from direct HTTPS URLs.

## Use

1. Enter the direct HTTPS URL for an APK you are authorized to download.
2. Optionally enter its matching OBB URL.
3. Tap Fetch package.
4. Android Download Manager places the files in Downloads.

## OBB note

Modern Android versions restrict applications from freely writing into another application's
Android/obb/package directories. This project downloads the OBB as a normal file instead of
attempting to bypass Android storage protections.

The app does not search for or acquire copyrighted games from unauthorized sources.

## Build

Open the repository in Android Studio and run the Gradle build. The project targets SDK 35.
