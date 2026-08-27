# Development Rules & Optimization Standards

## 1. Asset & Performance Budget Standards
To ensure smooth performance on mid-range Android mobile AR devices (e.g. Snapdragon 680 / Helio G99):

| Parameter | Target Limit | Hard Maximum |
| :--- | :--- | :--- |
| **Total Scene Triangles** | $\le 15,000$ tris | $25,000$ tris |
| **Single Prop Triangles** | $\le 1,500$ tris | $3,500$ tris |
| **Texture Map Resolution** | $512 \times 512$ | $1024 \times 1024$ |
| **Material Count per Object**| $1$ material | $2$ materials |
| **Draw Calls / Batches** | $\le 35$ draw calls | $50$ draw calls |
| **Particle System Active Count**| $\le 30$ particles | $50$ particles |
| **File Format Standard** | `.glb` / `.gltf` (PBR) or `.fbx` | N/A |
| **Audio Format** | `.ogg` / `.wav` compressed | $\le 1\text{ MB}$ per file |

## 2. Licensing Compliance Rules
1. **Permitted Licenses**: CC0 1.0 Universal (Public Domain), MIT License, Creative Commons Attribution (CC-BY 4.0), Unity Asset Store Free License.
2. **Prohibited Assets**: Copyrighted game rips, non-commercial restricted assets (CC-NC), untraced web assets.
3. **Attribute Tracking**: All non-CC0 assets must have their exact license, author name, and source link recorded in `ASSET_CHECKLIST.md`.

## 3. Safety Accuracy Enforcement
- Do NOT make up unverified procedures for hazardous chemicals or gas evacuation.
- Any procedure classified as "Safety Critical" must cite standard industrial or DGMS / Factories Act regulations or carry a `[VERIFICATION REQUIRED]` warning tag.
