🔁 Art Integration Flow (Fire Jelly Art Pipeline)
🧑‍🎨 1. Create the Art
Make the sprite (character, card, etc.)

Optionally, check the Art Status Window to see:

What's missing

What's outdated

⬇️ 2. Sync Git
Pull latest main

🌿 3. Create Branch
art_import_# (e.g. art_import_5)

🖼 4. Import the Art
Option A: Use the Art Status Window

Click Import on each asset needing update or creation

Option B: Drag sprites into Assets/Import/

Use menu: Tools → Art Pipeline → Validate Import Folder

(Naming convention won't be enforced unless processed by the main pipeline)

✅ 5. Verify
Confirm that all assets now appear correctly in the pipeline

Missing or incorrect references should be highlighted

🧹 6. Fix or Report
Fix local issues or ping team for unique edge cases

💾 7. Commit
GitHub will typically auto-generate a commit message (e.g. “Updated 4 assets”)

📤 8. Push Branch
🔁 9. Open Pull Request
Ready for review, nothing else needed

👌 Key Features:
✅ Automatic renaming

✅ Versioning handled

✅ Linking handled

✅ No manual editing of ScriptableObjects

✅ Designed for non-technical use (by artists or producers)