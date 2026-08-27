# System Architecture & Event Telemetry Schema

## 1. High-Level Architecture Overview

```
 ┌─────────────────────────────────────────────────────────┐
 │               Unity AR Mobile Client (Android)           │
 │                                                         │
 │  ┌──────────────────┐       ┌────────────────────────┐  │
 │  │ AR Foundation    │       │ Gas Leak Scenario      │  │
 │  │ Tracking / Plane │ ────> │ State Machine Controller│  │
 │  └──────────────────┘       └────────────────────────┘  │
 │                                         │               │
 │                                         ▼               │
 │                             ┌───────────────────────┐   │
 │                             │ Event Dispatcher      │   │
 │                             └───────────────────────┘   │
 └─────────────────────────────────────────┼───────────────┘
                                           │ (JSON Payload)
                                           ▼
 ┌─────────────────────────────────────────────────────────┐
 │               Assessment & Telemetry Engine             │
 │        (Evaluates actions, scores, safety compliance)    │
 └─────────────────────────────────────────────────────────┘
```

## 2. Event Telemetry Data Contract

The Unity AR application dispatches standardized event JSON objects during training scenarios to log trainee decisions and timing metrics.

### Event Payload Schema (`GasLeakEvent`)
```json
{
  "sessionId": "string (UUID)",
  "traineeId": "string",
  "scenarioId": "SIM_GAS_LEAK_01",
  "timestampIso": "2026-08-26T19:45:00Z",
  "elapsedSeconds": 45.2,
  "stepId": "STEP_03_PPE_SELECTION",
  "actionType": "SELECT_ITEM",
  "objectId": "ppe_scba_mask_01",
  "isCorrect": true,
  "isCriticalHazard": false,
  "penaltyPoints": 0,
  "hazardContext": {
    "gasType": "CH4_CO_MIX",
    "ppmLevel": 450,
    "alarmState": "ALERT_ACTIVE"
  },
  "metadata": {
    "traineeDistanceToExitMeters": 12.4,
    "selectedPpeType": "SCBA_FULL_FACE"
  }
}
```

## 3. Communication Contract (Person 2 -> Person 1 Interface)
- **Asset Naming Convention**: `pref_<category>_<asset_name>_01` (e.g., `pref_ppe_scba_mask_01`, `pref_env_gas_valve_01`).
- **Transform Anchor Rules**: Origin $(0,0,0)$ must be centered at the base/pivot point of the asset for clean AR ground/wall placement.
- **Scale Standard**: $1.0\text{ unit} = 1.0\text{ meter}$ in real world dimensions.
