# Gas Leak / Confined Space Training Flow

**Disclaimer:** *As per our Development Rules, this training flow is designed based on standard industrial safety guidelines (e.g., OSHA confined space/hazardous gas protocols). It must be reviewed by an authorized industrial safety officer before deployment in the field.*

---

## 1. Scenario Introduction
**Context:** You are inspecting a pipeline section in a confined industrial area. Your objective is to ensure the area is safe and secure.
**Goal:** Respond correctly to a sudden toxic/flammable gas leak without risking your life or causing secondary hazards.

## 2. Initial Environment
- **Setting:** Industrial plant walkway with pipes, valves, and a control panel.
- **AR Objects (Interactable):** 
  - Main pipe with a clearly marked shut-off valve.
  - Secondary pipe with a dummy valve.
  - A toolbox containing a portable gas monitor.
  - A safety locker containing PPE (Self-Contained Breathing Apparatus / SCBA).
  - The user's virtual mobile phone.
- **Ambience:** Normal factory hum.

## 3. Emergency Trigger
- **Event:** A sudden loud hissing sound from the main pipe accompanied by a faint visual cue (vapor/gas cloud).
- **System Action:** The assessment timer begins immediately when the hissing starts.

## 4. Expected Trainee Actions (SOP)
1. **Detect**: Retrieve and use the portable gas monitor to check the air quality.
2. **Protect**: Open the safety locker, retrieve, and equip the SCBA / Respirator mask.
3. **Isolate**: Navigate into the hazard zone to the main shut-off valve and turn it to stop the leak.
4. **Evacuate**: Proceed immediately to the designated muster point (Exit).

## 5. Correct Actions
- Checking air quality with monitor (+20 points)
- Equipping SCBA *before* entering the gas cloud zone (+30 points)
- Closing the correct main shut-off valve (+30 points)
- Evacuating the area promptly (+20 points)

## 6. Incorrect Actions (Mistakes)
- **Turning the wrong valve:** (-10 points)
- **Taking too long (>30s) to equip PPE after leak detection:** (-10 points)
- **Running instead of walking briskly (simulated via rapid device movement):** (-5 points, as running can cause panic/tripping in confined spaces).

## 7. Critical Unsafe Actions (Immediate Failure)
- **Entering the gas cloud without SCBA:** Inhaling toxic/asphyxiating gas is lethal. Triggers immediate scenario failure.
- **Using a cell phone or causing a spark:** Flammable gases can ignite. Using electronic devices not rated for explosive atmospheres triggers an immediate explosion simulation and failure.

## 8. Scoring Recommendations
- **Max Score:** 100
- **Passing Score:** 80
- **Time Limit:** 120 seconds
- **Formula:** Starts at 0. Correct actions add points. Mistakes subtract points. Time bonus if completed under 60 seconds (+10 points, up to max 100). Critical unsafe actions immediately terminate and fail the scenario.

## 9. Completion Condition
- **Success:** The trainee successfully closes the valve and reaches the evacuation zone with a score >= 80 and no critical failures.
- **Failure:** The trainee commits a critical unsafe action, drops below the point threshold due to mistakes, or runs out of time (120 seconds).

## 10. Feedback After Completion
- **Pass:** "Excellent. You correctly identified the hazard, protected yourself, and isolated the leak according to SOP."
- **Fail (No PPE):** "Critical Failure: You entered a toxic environment without proper respiratory protection. Always equip your SCBA first."
- **Fail (Ignition Source):** "Critical Failure: You introduced an ignition source into a potentially explosive atmosphere."
- **Fail (Timeout):** "Critical Failure: You failed to isolate the leak and evacuate within the safe time window."

## 11. Hindi Content (Translations for UI/AR Overlays)
*Note: Subject to QA localization review.*
- **Scenario Title:** गैस रिसाव (Gas Leak)
- **Objective:** गैस रिसाव को रोकें और सुरक्षित बाहर निकलें (Stop the gas leak and evacuate safely)
- **Gas Monitor:** गैस मॉनिटर (Gas Monitor)
- **Mask/PPE:** सुरक्षा मास्क / एससीबीए (Safety Mask / SCBA)
- **Valve:** वाल्व (Valve)
- **Exit:** बाहर निकलने का रास्ता (Exit)
- **Warning Overlay:** बिना मास्क के आगे न बढ़ें! (Do not proceed without a mask!)

## 12. Santali Content (Translations for UI/AR Overlays)
*Note: Phonetic Devnagari/Roman representation. Must be verified by native Santali speaker from the region.*
- **Scenario Title:** गेस उडुक (Gas Leak)
- **Objective:** गेस उडुक बंद मे आर निरापद ते बाहिर मे (Stop the leak and exit safely)
- **Gas Monitor:** गेस यंत्र (Gas Machine/Monitor)
- **Mask/PPE:** मोचा रेयाक् मास्क / बचाव एमान (Face mask / Protection)
- **Valve:** वाल्व (Valve)
- **Exit:** बाहिर (Exit)
- **Warning Overlay:** मास्क बांग ते बाहिर आलोम चालाक् आ! (Do not go without a mask!)

## 13. Test Cases (For QA & Assessment Engine)

| Test Case | Steps Executed | Expected Result |
| :--- | :--- | :--- |
| **TC-01: Perfect Execution** | Grab monitor -> Grab mask -> Turn main valve -> Go to exit. | `Score: 100`, `Mistakes: 0`, `Passed: True` |
| **TC-02: Wrong Valve** | Grab mask -> Turn secondary valve -> Turn main valve -> Go to exit. | `Score: 90`, `Mistakes: 1`, `Passed: True` |
| **TC-03: No Mask (Critical)** | Hissing starts -> Go straight to main valve. | `Score: 0`, `Mistakes: 1`, `Passed: False` (Asphyxiation trigger) |
| **TC-04: Ignition (Critical)** | Hissing starts -> Open virtual mobile phone near leak. | `Score: 0`, `Mistakes: 1`, `Passed: False` (Explosion trigger) |
| **TC-05: Timeout** | Hissing starts -> Stand still for 120 seconds. | `Score: 0`, `Passed: False` (Timeout trigger) |
