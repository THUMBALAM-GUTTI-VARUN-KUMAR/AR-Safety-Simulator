# AR-Based Vocational Training Simulator for Industrial Safety
## Jharkhand Mining & Manufacturing Sector — Project Specification

### 1. Vision & Executive Summary
This project delivers an Augmented Reality (AR) mobile application tailored for industrial safety training in Jharkhand’s mining (coal, iron ore, mica, copper) and heavy manufacturing sectors (steel plants, chemical processing). Target users are industrial trainees, underground mine workers, and plant operators who require high-retention, practical hazard response training without real-world exposure to extreme hazards.

### 2. Primary Use Case & Domain Focus (Jharkhand Region)
- **Sector Focus**: Underground coal/metal mines (e.g., Jharia, Bokaro, West Singhbhum) and heavy industries (Jamshedpur, Ranchi).
- **Core Hazard Types**:
  1. Toxic & Flammable Gas Ingress (Methane $CH_4$, Carbon Monoxide $CO$, Hydrogen Sulfide $H_2S$, Oxygen Deficiency $O_2$).
  2. Slope Instability & Roof Falls.
  3. Electrical & Machinery Conveyor Safety.

### 3. Key Target Technical Constraints (Day 1 Baseline)
- **Target Device Class**: Mid-range Android smartphone (e.g., Snapdragon 680 / Helio G99 / Exynos 1280, 4–6 GB RAM, ARCore support).
- **AR Framework**: Unity AR Foundation (ARCore backend).
- **Asset Poly Budget**: Max 25,000 triangles total per active scene (< 5,000 tris per major asset).
- **Texture Budget**: $512 \times 512$ to $1024 \times 1024$ uncompressed or compressed ASTC/ETC2, shared texture atlases.
- **Frame Rate Target**: Stable $30\text{ FPS}$ to $60\text{ FPS}$ in handheld AR mode to prevent simulator sickness.

### 4. Team Division of Responsibilities
- **Person 1 (AR Lead / Unity Integration)**: Responsible for Unity scene hierarchy, AR Foundation tracking, user UI setup, event dispatch, and client logic.
- **Person 2 (3D Assets & Safety Scenario Lead - CURRENT ROLE)**: Responsible for hazard scenario specifications, industrial safety workflow logic, lightweight 3D asset sourcing, asset optimization specifications, licensing compliance, and assessment telemetry payload definitions.

---
> [!IMPORTANT]
> **Safety Verification Note**: All safety protocols specified herein must be verified against authoritative industrial standards (DGMS - Directorate General of Mines Safety, Coal Mines Regulations CMR 2017, and Factories Act 1948) prior to deployment in final certified training modules.
