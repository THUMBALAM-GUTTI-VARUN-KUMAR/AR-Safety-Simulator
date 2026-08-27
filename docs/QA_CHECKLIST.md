# QA Checklist & Test Plan

This document defines the comprehensive Quality Assurance test plan covering all layers of the AR Safety Simulator for the SIM_GAS_LEAK_01 scenario.

## 1. AR Integrity & Mobile Performance Tests
- [ ] **Camera & Plane Detection**: Verify that the AR camera initializes without delay and detects horizontal/vertical planes within 3 seconds.
- [ ] **Object Placement**: Verify that 3D prefabs anchor stably to planes without jittering or clipping through the floor.
- [ ] **Lighting & Shaders**: Verify that Unlit/Simple Lit URP shaders render correctly under various physical lighting conditions.
- [ ] **Performance Limits**:
  - [ ] Total Scene Triangles $\le 15,000$ (Max $25,000$).
  - [ ] Draw Calls $\le 35$ (Max $50$).
  - [ ] Texture maps do not exceed $1024 \times 1024$.
  - [ ] Particle systems (gas cloud) do not exceed 30 active particles.
- [ ] **FPS**: Maintain stable 30+ FPS on target mid-range Android devices.

## 2. Scenario 01 (Gas Leak) Logic Tests
Execute the following interactions to verify state transitions and expected outputs:

### Step 1: Detection
- [ ] **Correct Action**: Trainee taps Gas Detector within 15 seconds.
- [ ] **Wrong Action**: Trainee waits $> 15$ seconds ($-10\text{ pts}$).
- [ ] **Feedback**: HUD zoom, red flashing numbers, beep code.
- [ ] **Telemetry**: `STEP_01_DETECTION` recorded.

### Step 2: Hazard ID
- [ ] **Correct Action**: Identifies "Toxic & Explosive Gas Leak".
- [ ] **Wrong Action**: Misclassifies as dust/steam ($-10\text{ pts}$).
- [ ] **Telemetry**: `STEP_02_HAZARD_ID` recorded.

### Step 3: PPE Selection
- [ ] **Correct Action**: Selects SCBA full-face mask.
- [ ] **Critical Hazard**: Selects Dust Mask (Immediate Failure).
- [ ] **Feedback**: Success (SCBA flow audio) vs Failure (Red cross, error buzz).
- [ ] **Telemetry**: `STEP_03_PPE_SELECTION` recorded.

### Step 4: Alarm Activation
- [ ] **Correct Action**: Swipes down on alarm lever on first/second try.
- [ ] **Wrong Action**: Fails to activate alarm after 2 attempts ($-10\text{ pts}$).
- [ ] **Feedback**: Siren audio loop and red strobe effect.
- [ ] **Telemetry**: `STEP_04_ALARM_ACTIVATION` recorded.

### Step 5: Airflow Procedure
- [ ] **Correct Action**: Evaluates windsock and chooses UPWIND path.
- [ ] **Critical Hazard**: Chooses DOWNWIND path (Immediate Failure).
- [ ] **Wrong Action**: Hesitates $> 20$ seconds ($-10\text{ pts}$).
- [ ] **Telemetry**: `STEP_05_AIRFLOW_PROCEDURE` recorded.

### Step 6: Evacuation Movement
- [ ] **Correct Action**: Moves towards highlighted green AR waypoint.
- [ ] **Critical Hazard**: Walks into the gas plume (Immediate Failure).
- [ ] **Feedback**: Footstep audio and waypoint pulse.
- [ ] **Telemetry**: `STEP_06_EVACUATION_MOVEMENT` recorded.

### Step 7: Completed
- [ ] **Correct Action**: Reaches safe zone, taps "FINISH SCENARIO".
- [ ] **Feedback**: HUD turns green, success chime, steel door closes.
- [ ] **Telemetry**: `STEP_07_COMPLETED` recorded, total time & final score generated.

## 3. Assessment Engine Tests
- [ ] **Score Calculation**: Starts at 100, correctly subtracts 10 points per non-critical mistake.
- [ ] **Critical Hazards**: Verify that triggering any Critical Hazard sets `passed = false` and terminates scenario immediately.
- [ ] **Completion Time**: Verify elapsed scenario time is accurately logged in seconds.

## 4. Offline Functionality Tests
- [ ] **Airplane Mode Save**: Complete a scenario with Wi-Fi/Data disabled. Verify result is cached to `Application.persistentDataPath` in local JSON queue.
- [ ] **Reconnect Sync**: Re-enable connection and launch app. Verify offline payload is dispatched to backend via `/api/sync`.
- [ ] **Duplicate Prevention**: Trigger `/api/sync` multiple times with the same `sessionId`. Verify backend rejects duplicate insertions.

## 5. Backend API Tests
- [ ] **Submit Result**: POST to `/api/training/result` with correct `X-API-Key` returns `200 OK`.
- [ ] **Retrieve Results**: GET `/api/training/results` returns accurate lists.
- [ ] **Auth Enforcement**: POST without valid `X-API-Key` returns `401 Unauthorized`.

## 6. Dashboard UI Tests
- [ ] **Render**: Verify dashboard loads cleanly on `localhost:5173`.
- [ ] **Aggregate Metrics**: Total Trainees, Passed, Failed reflect database accurately.
- [ ] **Trainee History**: Selecting a Trainee ID populates their history table with Score, Duration, and Pass/Fail status.
