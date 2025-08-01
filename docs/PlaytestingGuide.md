# Playtesting Guide

This guide explains how to open **Taki Fight** in Unity and run the game locally.

## Required Unity Version

Use **Unity 6000.0.28f1**. Make sure it is installed via the Unity Hub.

## Opening the Project

1. Launch Unity Hub and click **Open**.
2. Select the repository root folder.
3. Once the project loads, open the `Bootstrap` scene found under `Assets/Scenes`.
4. Press **Play** to run the scene.

## CI Build Artifacts

Unity builds are generated by the GitHub Actions workflow. You can download the
latest build from the workflow run's **Artifacts**. After each build completes,
the workflow uploads the files to Google Drive and posts the folder link in the
project's Discord **#builds** channel. Check the pinned message there for a
permanent link to the Drive folder.

