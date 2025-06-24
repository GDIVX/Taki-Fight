# Changelog

All notable **frontend** changes to this project are documented here.  
This file is intended for **players, playtesters, and stakeholders**.  
**Do not document backend-only changes** here.

Follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0)  
Adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html)

---

🛠️ Codex Instructions:

1. **Append** new entries to the top of this file:  
   - Format: \`## [x.y.z] - YYYY-MM-DD\`  
   - Start with a short \`### Summary\` (1–2 sentences).  
   - Then list changes under: \`### Added\`, \`### Changed\`, \`### Fixed\`, etc.

2. **Overwrite** \`Assets/Resources/changelog.txt\` with the current version only:  
   - Include the full block starting from the \`[x.y.z]\` header.  
   - Do **not** include older entries.

3. Append this at the end of \`changelog.txt\` if appropriate:  

   ---
   Download, play, and tell us what you think!
   Simply use one of these commands:
   /bug  
   /idea  
   /feedback  

\> \`.txt\` is used for in-game display and Discord.  
\> \`.md\` contains the full changelog history for Codex and internal use.

---

**Terminology**  
- Use in-game lingo: “Pawn” = Familiar


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
