# Architecture

## Overall Architecture Flow

```text
ANDROID PHONE
     |
     v
UNITY + AR FOUNDATION + ARCORE
     |
     v
SCENARIO ENGINE
     |
     v
ASSESSMENT ENGINE
     |
     +----------------------+
     |                      |
     v                      v
LOCAL STORAGE           INTERNET AVAILABLE
     |                      |
     |                      v
     |                  FASTAPI
     |                      |
     |                      v
     |                  POSTGRESQL
     |                      |
     |                      v
     |              REACT ADMIN DASHBOARD
     |
     +---- Sync when online
```

### Components and Data Flow
- **Android Phone / Unity / ARCore:** Trainees interact with hazards using AR on mobile devices.
- **Scenario Engine:** Feeds events and state to the user interface.
- **Assessment Engine:** Evaluates actions. Passes the completed payload to local storage.
- **Local Storage / Internet Logic:** Keeps data safe if offline. Syncs upward when online.
- **FastAPI / PostgreSQL:** Validates and stores results.
- **Dashboard:** Visualizes training metrics to the admin.

## Certificate System

```text
Training Result
      |
      v
Pass
      |
      v
Certificate ID
      |
      v
QR Code
      |
      v
Verification API
      |
      v
Valid / Invalid
```

## Scenario Engine
Design relies on a modular scenario system so we DO NOT hard-code each scenario.

```text
Scenario
   |
   +-- Events
   |
   +-- Required Actions
   |
   +-- Wrong Actions
   |
   +-- Scoring Rules
   |
   +-- Completion Condition
```

This engine supports Gas Leak and Fire/Explosion initially, and is extensible for Electrical, Machinery, etc., without rewriting core logic.

## Assessment Engine
- **Deterministic System:** Correct actions = positive points; Wrong actions = negative points. Critical unsafe actions result in a larger penalty. Response time provides a bonus or penalty.
- **AI constraints:** Core safety assessment MUST NOT depend on an LLM. AI may later be used for personalized feedback, but not for safety-critical evaluations.

## Offline Architecture
```text
Training
   |
   v
Assessment
   |
   v
Save locally
   |
   v
Sync Queue
   |
   +---- No Internet ----> Wait
   |
   +---- Internet --------> POST API
                              |
                              v
                           Database
```

**Prevention of duplicates:** Every training session generated locally is assigned a Unique ID (UUID) prior to synchronization, preventing duplicate rows during retry operations.
