# AR-Based Vocational Training Simulator for Industrial Safety

## Overview
A smartphone-first Android AR industrial safety training platform designed for Jharkhand's Mining & Manufacturing Sector.

## Problem
Industrial workers often face hazardous environments, but traditional training lacks realism, scale, and safe hands-on practice.

## Solution
An AR-based training simulator that allows workers to practice safety-critical procedures (like handling gas leaks or fires) directly on their smartphones without real-world danger.

## Features
- **Immersive AR Scenarios:** Practice in real space using ARCore.
- **Deterministic Assessment:** Get scored based on correct procedures, reaction time, and safety violations.
- **Offline Mode:** Train without internet and sync data automatically when back online.
- **Multi-language:** Designed with localization in mind (e.g., Hindi, Santali).
- **Verifiable Certificates:** Passing trainees receive a QR-verifiable certificate.
- **Admin Dashboard:** Monitor trainee performance across the organization.

## Architecture
See [ARCHITECTURE.md](docs/ARCHITECTURE.md) for full details. The system consists of:
- Unity AR Android App (AR Foundation, ARCore)
- Local SQLite Storage (Offline first)
- Python FastAPI Backend
- PostgreSQL / Supabase Database
- React + Vite Admin Dashboard

## Technology Stack
- **Mobile AR:** Unity, C#, Unity AR Foundation, ARCore, Android
- **Backend:** Python, FastAPI
- **Database:** PostgreSQL (Supabase)
- **Dashboard:** React, Vite, Tailwind CSS
- **Offline Sync:** Local storage queue pattern
- **Version Control:** Git, GitHub

## Team Structure
See [TEAM_WORKFLOW.md](docs/TEAM_WORKFLOW.md) for role assignments.

## How to Contribute
1. Follow the [Development Rules](docs/DEVELOPMENT_RULES.md).
2. Adhere to the [Git Workflow](docs/TEAM_WORKFLOW.md).
3. Do not alter the core [API Contract](docs/API_CONTRACT.md) without team consensus.

## Development Status
Current Status: **Phase 2 (Architecture and repository setup)**
See [PROJECT_SPECIFICATION.md](docs/PROJECT_SPECIFICATION.md) for full roadmap.
