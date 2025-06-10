# Taki Fight

This repository contains the source code for the **Taki Fight** project.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/) installed and available as `dotnet` on your PATH
- [Git LFS](https://git-lfs.com/) installed and initialized
- [Unity](https://unity.com/) version **6000.0.28f1**

## Restoring Packages

After cloning the repository, run the following command to restore NuGet packages:

```bash
dotnet restore
```

## Running Tests

To execute the unit tests, run:

```bash
dotnet test
```

## Playtesting

Refer to the [Playtesting Guide](docs/PlaytestingGuide.md) for details on opening the project in Unity and running the `Bootstrap` scene.

## GitHub Secrets

The CI workflows rely on several secrets configured in the repository or
organization settings:

- `PLAYTEST_DISCORD_BUILD` – webhook URL used to post new builds to Discord.
- `PLAYTEST_DISCORD_UPDATE` – webhook URL for sending playtest updates.
- `DRIVE_SERVICE_ACCOUNT_KEY` – JSON credentials for the service account used
  to upload builds to Google Drive.
- `DRIVE_BUILD_FOLDER` – the Google Drive folder ID that receives uploaded
  builds.
- `UNITY_LICENSE` – the encoded Unity license used to activate the editor during CI.


## Coding Standards

See [CodingStandards.md](docs/CodingStandards.md) for the project's coding conventions.

## Changelog

All notable changes are documented in [CHANGELOG.md](CHANGELOG.md).
Please add an entry for each release to keep the record current.
