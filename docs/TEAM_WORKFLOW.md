# Team Workflow

## Team Structure & Ownership
*(This is the initial ownership model. It can change later after the team discovers individual strengths.)*

- **Person 1:** Unity + AR Lead
  - AR Foundation, ARCore, Android build, AR interaction
- **Person 2:** Unity + 3D/Simulation
  - 3D environment, assets, scenarios, interactions
- **Person 3:** Assessment + Offline
  - Scoring, timer, mistakes, local storage, sync queue
- **Person 4:** Backend + Database
  - FastAPI, PostgreSQL/Supabase, APIs, synchronization
- **Person 5:** Dashboard
  - React, Vite, Tailwind, admin dashboard, analytics
- **Person 6:** Content + QA + Localization
  - Safety procedures, Hindi/Santali, UX, testing, documentation

## GitHub Workflow

### Branch Strategy
- `main`
- `develop`
- `feature/ar`
- `feature/scenarios`
- `feature/assessment`
- `feature/backend`
- `feature/dashboard`
- `feature/content`

### Rules
1. No direct pushes to `main`.
2. Work on `feature` branches.
3. Pull changes from `develop` before starting work.
4. Commit small working changes.
5. Open pull request to `develop`.
6. At least one teammate reviews before merging.
7. `main` contains only stable versions.
8. **CRITICAL:** Do not commit API keys, passwords, Supabase secrets or `.env` files.
