# Changelog

All notable frontend changes to this project will be documented in this file.  
Do not document backend changes; this file is for playtesters, players, and stakeholders to track patch notes.  
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0)  
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).  
Maintainers must add a new entry for each release.

---

🛠️ **Codex Instructions:**

1. **Append** each new version’s changelog at the top of this file.  
   - Use the format: `## [x.y.z] - YYYY-MM-DD`  
   - Start each version with a short `### Summary` section (1–2 sentences max) describing the *main focus* of the update.  
   - Follow with categorized entries: `### Added`, `### Changed`, `### Fixed`, etc.

2. **Overwrite** the file at `Assets/Resources/changelog.txt` with only the current version’s entry.  
   - Include the `[x.y.z]` version header and the entire changelog block (including `### Summary`)  
   - Do **not** include older versions.

3. When appropriate, append this to the end of `changelog.txt`:  

   ---
   📝 Submit feedback: https://forms.gle/6nVJeP1MwBQdxoxg8  
   📅 Book a playtest: https://calendar.app.google/vqCvZXTd6tQFMGTLA  
   💬 Or post in the #feedback Discord forum

> `.txt` is used for Discord and in-game display.  
> `.md` stores the full changelog history and helps Codex infer what changed and why.

---

**Use in-game lingo:**
- Pawn == Familiar

---

## [Unreleased]

## [0.1.15] - 2025-06-22
### Summary
Early build focusing on small improvements and bug fixes.

### Added
- New build for early testing.

### Changed
- Polished various UI elements.

### Fixed
- Minor bugs reported by testers.

---

## [0.1.14] - 2025-06-17  
### Summary  
Tooltips, combat polish, and Familiar QoL improvements.  

### Added  
- Tooltips now appear when hovering tiles and Familiars.  
- Keywords display nested tooltips for extra details.  
- New damage types: Blunt, Slash, Piercing, and Sundering.  
- New combat cards: *Ace In The Hole*, *Barrage*, *Fireball*, and *Heavy Metal*.  
- Capture and Charge abilities for Familiars.  

### Changed  
- Card descriptions are now generated dynamically from keywords.  
- Summon cards now return when their Familiar dies.  

### Fixed  
- Duplicate card instances and Familiar recall issues.  

---

## [0.1.13] - 2025-06-11  
### Summary  
HUD overhaul and improved Familiar responsiveness.  

### Added  
- Familiars now properly trigger abilities on state transitions.  

### Changed  
- The HUD has been reworked for better clarity and responsiveness.  

### Fixed  
- Card arrangement race conditions.  

### Known issues (ignore)  
- Art is temporary.  
- Some UI icons are placeholders.  

---

## [0.1.0] - 2025-06-09  
### Summary  
Initial alpha release.  

### Added  
- Initial changelog with version 0.1.0.  
