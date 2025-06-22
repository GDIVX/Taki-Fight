# Changelog

All notable frontend changes to this project will be documented in this file.
Do not document backend changes, this file is for playtesters, players and stackholders to keep changes in patches.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
Maintainers should add a new entry for each release.

🛠️ Codex Instructions:
1. **Append** each new version's changelog at the top of this file.
   - Use the format: `## [x.y.z] - YYYY-MM-DD`
   - At the **start of each version**, include a short `### Summary` section (1–2 sentences max) explaining the *main focus* of the update (e.g., "Focus on UI polish and tooltips").
   - Follow with the usual `### Added`, `### Changed`, `### Fixed`, etc.
2. **Overwrite** the file at `Assets/Resources/changelog.txt` with only the current version’s entry.
   - Include the `[x.y.z]` version header and the entire changelog block (including `### Summary`).
   - Do **not** include older versions.

> The `.txt` file is used for in-game display and Discord announcements.  
> The `.md` file is used to track full changelog history and help Codex identify what changed.

Use in-game lingo:
- Pawn == Familiar

## [Unreleased]

## [0.1.14] - 2025-06-17
### Added
- Tooltips now appear when hovering tiles and Familiars.
- Keywords display nested tooltips for extra details.
- New damage types: Blunt, Slash, Piercing and Sundering.
- New combat cards: Ace In The Hole, Barrage, Fireball and Heavy Metal.
- Capture and Charge abilities for Familiars.
### Changed
- Card descriptions are built dynamically from keywords.
- Summon cards return when their Familiar dies.
### Fixed
- Duplicate card instances and Familiar recall issues.

## [0.1.13] - 2025-06-11
### Added
- Familiars would now 
### Changed
- The HUD had been reworked
### Fixed
- Card arrangement race conditions.

### Known issues (ignore)
- Art is temporary
- Some of the UI icons are temporary

## [0.1.0] - 2025-06-09
### Added
- Initial changelog with version 0.1.0.

