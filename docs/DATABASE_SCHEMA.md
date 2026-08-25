# Database Schema

Design strategy: Do not over-engineer. Focus on core requirements.

## Tables

### `users`
- `id` (UUID / Primary Key)
- `name` (String)
- `employee_id` (String / Unique)
- `role` (String - Admin, Trainee)
- `language` (String)
- `created_at` (Timestamp)

### `scenarios`
- `id` (String / Primary Key)
- `name` (String)
- `category` (String)
- `version` (String)
- `active` (Boolean)

### `training_sessions`
- `id` (UUID / Primary Key) - Created locally on device to prevent duplicate syncs.
- `user_id` (UUID / Foreign Key -> `users.id`)
- `scenario_id` (String / Foreign Key -> `scenarios.id`)
- `score` (Integer)
- `duration_seconds` (Integer)
- `mistakes` (Integer)
- `passed` (Boolean)
- `completed_at` (Timestamp)

### `actions`
- `id` (UUID / Primary Key)
- `session_id` (UUID / Foreign Key -> `training_sessions.id`)
- `action_name` (String)
- `correct` (Boolean)
- `points` (Integer)
- `timestamp` (Timestamp)

### `certificates`
- `id` (UUID / Primary Key)
- `user_id` (UUID / Foreign Key -> `users.id`)
- `session_id` (UUID / Foreign Key -> `training_sessions.id`)
- `certificate_number` (String / Unique)
- `issued_at` (Timestamp)
- `status` (String - Active, Revoked)

## Relationships
- `users` 1:N `training_sessions`
- `scenarios` 1:N `training_sessions`
- `training_sessions` 1:N `actions`
- `training_sessions` 1:1 `certificates`
- `users` 1:N `certificates`
