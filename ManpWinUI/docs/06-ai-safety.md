# Working Safely with AI - Corruption Prevention

## Why This Matters
- **Large files** (like DESIGN_PLAN.md) can overwhelm AI context windows
- **AI engine crashes** can corrupt in-progress edits
- **Lost work** is frustrating and wastes time
- **Git history** is your safety net

---

## Golden Rules for AI-Assisted Development

### 1. **Commit Early, Commit Often** ⭐ MOST IMPORTANT

```bash
# After EVERY successful AI edit session (even small ones):
git add .
git commit -m "docs: update DESIGN_PLAN.md with [what changed]"
git push origin feature/add-winui-interface

# Frequency: Every 15-30 minutes of work
# Benefit: Maximum 30 minutes of work lost if crash occurs
```

**Why:** If AI crashes mid-edit, you can always `git reset --hard` to last commit.

---

### 2. Work in Small Chunks

**Bad (risky):**
- "Update all 9 phase sections at once"
- "Rewrite the entire architecture section"
- "Make 50 changes across 10 files"

**Good (safe):**
- "Add Phase 2 commit examples to Git section"
- "Update MAUI section with file I/O patterns"
- "Create 3 new service interfaces in one commit"

**Rule of Thumb:**
- **Single file edits:** Safe for files < 2000 lines
- **Multi-file edits:** Max 3-5 files at a time
- **Large refactors:** Break into multiple sessions

---

### 3. File Size Awareness

**Risk Levels:**
- ✅ **< 500 lines:** Very safe
- ⚠️ **500-1,500 lines:** Safe with precautions
- 🚨 **1,500-3,000 lines:** Higher risk, split if possible
- ❌ **> 3,000 lines:** High corruption risk

**Action Plan:**
If a file exceeds 2,000 lines, consider splitting:
```
DESIGN_PLAN.md (overview + links)
├── docs/architecture.md
├── docs/implementation-phases.md
├── docs/git-strategy.md
├── docs/maui-compatibility.md
└── docs/ai-safety.md
```

---

### 4. Session Management Strategy

**Start of Session:**
```bash
# 1. Pull latest changes
git checkout feature/add-winui-interface
git pull origin feature/add-winui-interface

# 2. Sync with development (get C++ fixes)
git checkout development
git pull origin development
git checkout feature/add-winui-interface
git merge development

# 3. Create session branch (optional but recommended)
git checkout -b session/2025-01-15-phase2-work

# 4. Verify clean state
git status
```

**During Session:**
```bash
# After each successful AI interaction:
git add -A
git commit -m "wip: [brief description]"

# Every 30 minutes:
git push origin session/2025-01-15-phase2-work
```

**End of Session:**
```bash
# 1. Review changes
git log --oneline -5

# 2. Squash WIP commits (optional)
git rebase -i HEAD~5  # Combine "wip:" commits

# 3. Merge to main branch
git checkout feature/add-winui-interface
git merge session/2025-01-15-phase2-work

# 4. Push
git push origin feature/add-winui-interface

# 5. Delete session branch
git branch -d session/2025-01-15-phase2-work
```

---

### 5. Backup Before Major Edits

**Before large refactors:**
```bash
# Create a backup branch
git branch backup/before-phase3-refactor

# Or tag current state
git tag backup-2025-01-15

# If something goes wrong:
git reset --hard backup/before-phase3-refactor
# or
git reset --hard backup-2025-01-15
```

**Before editing critical files:**
```bash
# Copy file outside git
cp ManpWinUI/DESIGN_PLAN.md ../DESIGN_PLAN.md.backup

# If AI corrupts it:
cp ../DESIGN_PLAN.md.backup ManpWinUI/DESIGN_PLAN.md
```

---

### 6. AI Crash Recovery Procedures

**If AI crashes during file edit:**

#### Option A: Revert to last commit (safest)
```bash
# Check what changed
git status
git diff ManpWinUI/DESIGN_PLAN.md

# If file is corrupted or incomplete:
git checkout HEAD -- ManpWinUI/DESIGN_PLAN.md

# Verify
git status
```

#### Option B: Manual recovery (if some work is salvageable)
```bash
# 1. Copy corrupted file
cp ManpWinUI/DESIGN_PLAN.md ~/corrupted-backup.md

# 2. Restore clean version
git checkout HEAD -- ManpWinUI/DESIGN_PLAN.md

# 3. Manually copy salvageable parts from corrupted-backup.md
# Use diff tool to compare
```

#### Option C: Use Git reflog (if you committed before crash)
```bash
# Find commit before crash
git reflog

# Example output:
# a1b2c3d HEAD@{0}: commit: wip: added phase 2 details
# e4f5g6h HEAD@{1}: commit: docs: update git strategy

# Reset to good commit
git reset --hard e4f5g6h
```

---

### 7. Preventing Context Window Overload

**Signs AI is struggling:**
- Responses getting slower
- Partial/incomplete edits
- Repeated "I apologize" messages
- Missing context from earlier in conversation

**Solutions:**
1. **Start fresh conversation** every 10-15 exchanges
2. **Reference specific line numbers** instead of large blocks
3. **Break tasks smaller:** "Add section X" vs "Rewrite entire file"
4. **Use multiple tools:**
   - AI for generation
   - Manual editing for simple changes
   - Git for version control

---

### 8. Multi-File Edit Safety

**When making changes across multiple files:**

```bash
# 1. List files to change
echo "Files to edit:
- ManpWinUI/App.xaml.cs
- ManpWinUI/MainWindow.xaml
- ManpWinUI/ViewModels/MainViewModel.cs"

# 2. Edit ONE file at a time with AI
# 3. Commit after EACH file
git add ManpWinUI/App.xaml.cs
git commit -m "feat(winui): configure DI container in App.xaml.cs"

git add ManpWinUI/MainWindow.xaml
git commit -m "feat(ui): add basic layout to MainWindow"

git add ManpWinUI/ViewModels/MainViewModel.cs
git commit -m "feat(mvvm): create MainViewModel with commands"

# 4. If AI crashes, only lose current file (not all 3)
```

---

### 9. AI-Assisted Editing Best Practices

**Good prompts (specific, bounded):**
- ✅ "Add a new section after line 450 titled 'C++ Interop Strategy'"
- ✅ "Update Phase 2 checklist to include 3 new tasks"
- ✅ "Replace the file I/O example at lines 200-220 with async version"

**Bad prompts (vague, unbounded):**
- ❌ "Improve the entire document"
- ❌ "Add examples to all sections"
- ❌ "Reorganize everything for better flow"

**When AI suggests large changes:**
```
User: "I need to add MAUI compatibility info"
AI: "I can add a comprehensive MAUI section with 15 subsections..."

STOP! Ask AI to:
1. Create outline first (small edit)
2. Commit outline
3. Fill in sections one-by-one (small edits)
4. Commit after each section
```

---

### 10. File Corruption Detection

**After AI edits, always verify:**

```bash
# 1. Check file is valid markdown
code ManpWinUI/DESIGN_PLAN.md  # Visual Studio Code has MD preview

# 2. Check file size didn't drastically change
git diff --stat

# Example good output:
# ManpWinUI/DESIGN_PLAN.md | 45 +++++++++++++++++++++++++++++++++++

# Example bad output (possible corruption):
# ManpWinUI/DESIGN_PLAN.md | 2000 ++++++++++++++++-----------------

# 3. Scan for common corruption signs
grep "undefined" ManpWinUI/DESIGN_PLAN.md
grep "null" ManpWinUI/DESIGN_PLAN.md
grep "```" ManpWinUI/DESIGN_PLAN.md | wc -l  # Should be even number
```

**Corruption Indicators:**
- File size suddenly doubled or halved
- Odd number of code fences (```)
- Duplicate sections
- Truncated/incomplete sentences at end
- Missing headings (broken structure)

---

### 11. Emergency Rollback Procedures

#### Nuclear option (revert all uncommitted changes)
```bash
# See what would be lost
git status
git diff

# Point of no return
git reset --hard HEAD
git clean -fd  # Remove untracked files

# Verify
git status  # Should say "nothing to commit, working tree clean"
```

#### Surgical option (revert specific file)
```bash
# Revert one file to last commit
git checkout HEAD -- ManpWinUI/DESIGN_PLAN.md

# Revert to specific commit
git checkout a1b2c3d -- ManpWinUI/DESIGN_PLAN.md
```

#### Time travel (go back to earlier state)
```bash
# View commit history
git log --oneline --graph --all

# Create new branch from earlier commit
git checkout -b recovery/phase1-state a1b2c3d

# Compare current vs old
git diff feature/winui-modernization recovery/phase1-state

# If old state is better:
git checkout feature/add-winui-interface
git reset --hard recovery/phase1-state
git push --force origin feature/add-winui-interface  # ⚠️ Use with caution
```

---

### 12. Recommended Workflow for This Project

**Daily Work Cycle:**

```bash
# MORNING: Start fresh
git checkout development
git pull origin development
git checkout feature/add-winui-interface
git pull origin feature/add-winui-interface
git merge development  # Get latest C++ fixes

git checkout -b session/2025-01-15-am

# WORK SESSION 1 (30-45 minutes)
# - Make 3-5 small commits
# - Push every 30 min

git push origin session/2025-01-15-am

# BREAK (15 minutes)

# WORK SESSION 2 (30-45 minutes)
# - Continue on session branch
# - Make 3-5 more commits

git push origin session/2025-01-15-am

# AFTERNOON: Merge to main branch
git checkout feature/add-winui-interface
git merge --squash session/2025-01-15-am
git commit -m "feat(phase2): complete C++ interop design"
git push origin feature/add-winui-interface
git branch -d session/2025-01-15-am
```

**Benefits:**
- ✅ Work is always backed up to GitHub
- ✅ Session branches isolate experimental work
- ✅ Easy to discard bad session without affecting main branch
- ✅ Clean commit history on main branch

---

### 13. AI Conversation Management

**Signs you need to start fresh conversation:**
- AI is forgetting context from earlier
- Responses reference non-existent files
- AI suggests changes to wrong file
- Token limit warnings

**How to transition:**
```
Current conversation (at limit):
User: "We've been working for 2 hours, let's checkpoint"
AI: [provides summary]

NEW conversation:
User: "Continuing from previous session. We're working on
Phase 2 of the WinUI modernization project. Last we did X.
Next I need to do Y. Here's the current state: [brief context]"
```

**Context Preservation:**
- Keep DESIGN_PLAN.md up to date (it's your context)
- Commit frequently (Git is your memory)
- Take notes in comments or TODO.md
- Reference specific line numbers or section titles

---

### 14. Testing AI Edits

**Before committing AI-generated code:**

```bash
# For C# code:
dotnet build ManpWinUI/ManpWinUI.csproj

# For C++ code:
msbuild ManpWIN64/ManpWIN64.vcxproj /t:Build

# For markdown:
# Open in VS Code markdown preview
code --preview ManpWinUI/DESIGN_PLAN.md
```

**Checklist:**
- [ ] File compiles/builds (if code)
- [ ] Markdown renders correctly (if docs)
- [ ] No obvious syntax errors
- [ ] Indentation looks correct
- [ ] Code follows project style
- [ ] No "TODO" or placeholder comments from AI

---

### 15. Recovery Contacts

**If you get stuck:**

1. **Git reflog:** `git reflog` - see every state your repo has been in
2. **GitHub web interface:** Browse commits, download specific file versions
3. **Local backups:** Check `../backup/` folder (if you created one)
4. **Visual Studio:** "Local History" feature (if enabled)

**Prevention > Recovery:**
- Commit every 15-30 minutes ⭐
- Push to GitHub hourly ⭐
- Keep backups of critical files ⭐

---

## Quick Reference Card

```
┌─────────────────────────────────────────────────┐
│  AI SAFETY CHECKLIST - Keep This Handy!        │
├─────────────────────────────────────────────────┤
│                                                 │
│  Before AI Edit Session:                       │
│  [ ] git status (clean?)                       │
│  [ ] git pull (up to date?)                    │
│  [ ] Create session branch                     │
│                                                 │
│  During AI Session:                            │
│  [ ] Work in small chunks (< 50 lines/edit)    │
│  [ ] Commit after each successful edit         │
│  [ ] Push every 30 minutes                     │
│  [ ] Verify file isn't corrupted               │
│                                                 │
│  After AI Session:                             │
│  [ ] Review git diff                           │
│  [ ] Test builds (if code)                     │
│  [ ] Squash WIP commits                        │
│  [ ] Push to main branch                       │
│  [ ] Delete session branch                     │
│                                                 │
│  If AI Crashes:                                │
│  1. git status                                 │
│  2. git diff (salvageable?)                    │
│  3. git checkout HEAD -- <file> (if corrupted) │
│  4. Start fresh conversation                   │
│                                                 │
└─────────────────────────────────────────────────┘
```

---

## File Split Strategy

**When to split a file:**
- File exceeds 2,000 lines
- AI struggles with context
- Multiple distinct topics in one file
- Frequent merge conflicts

**How to split:**

```bash
# Create docs folder if needed
mkdir docs

# Move sections to separate files
# (Do this manually or with AI, one file at a time)

# Update main file with links
echo "See [Architecture](docs/02-architecture.md)" >> DESIGN_PLAN.md

# Commit each new file separately
git add docs/01-project-analysis.md
git commit -m "docs: extract project analysis to separate file"
```

**Recommended split structure:**
```
docs/
├── README.md (overview, links to all docs)
├── 01-project-analysis.md
├── 02-architecture.md
├── 03-implementation-phases.md
├── 04-technology-stack.md
├── 05-git-strategy.md
├── 06-ai-safety.md (this file)
├── 07-maui-compatibility.md
└── 08-references.md
```

**Benefits:**
- ✅ Smaller files = safer AI edits
- ✅ Easier to navigate
- ✅ Parallel work on different sections
- ✅ Faster AI processing
- ✅ Reduced merge conflicts

---

## Summary

**Remember:** Git is your time machine. Commit often, push frequently, work in small chunks. 🚀

**Key Takeaways:**
1. Commit every 15-30 minutes
2. Keep files under 2,000 lines
3. Work in small, focused chunks
4. Use session branches for isolation
5. Always have a rollback plan
6. Start fresh conversations when AI struggles
7. Verify edits before committing
8. Prevention is easier than recovery
