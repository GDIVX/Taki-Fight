# Technical Debt Log

This document tracks known technical debt and TODO items in the project. When adding new TODOs or resolving existing ones, please update this list.

| File | Line | Description | Severity |
| ---- | ---- | ----------- | -------- |
| Assets/Scripts/Runtime/GameManager.cs | 82 | Implement progression after combat victory | Medium |
| Assets/Scripts/Runtime/RunManagement/GameRunState.cs | 13 | Replace temporary field `Combats` with final implementation | Low |
| Assets/Scripts/Runtime/CardGameplay/Card/CardController.cs | 155 | Card should play before animation sequence | Medium |
| Assets/Scripts/Runtime/CardGameplay/Card/CardBehaviour/AttackTargetCardPlay.cs | 47 | Use magic power stat in attack calculations | High |
| Assets/Scenes/MainMenu.unity | 585 | Implement options menu | Low |

Please keep this log up to date and create a matching GitHub issue labeled `tech-debt` when possible.
