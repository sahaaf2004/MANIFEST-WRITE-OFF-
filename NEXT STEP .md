Next Step: Build with Mock API Layer
Since ASYCUDA, Tradian, E-Customs, and E-Valuator are unavailable, the right move is to build a mock service layer that simulates all 4 APIs with realistic data and all possible response scenarios. This lets you complete the full app without being blocked.

Recommended Build Order
Phase 1 — Mock API Layer (Backend) ← Start here
Build a MockValidationService in .NET that returns hardcoded/configurable responses for each scenario:

Happy path (all 7 fields match → auto write-off)
Each error state (E1–E6)
Field mismatches (S4) with Tier I and Tier II fines
BL not found (S3), technical error (S6)
Wire this up to real POST /api/validate endpoint so the frontend actually calls it.

Phase 2 — Remaining Backend Endpoints

POST /api/amendment/submit
GET /api/amendment/status/{ref}
POST /api/bl-cancel/submit
POST /api/payment/initiate + /confirm
GET /api/certificate/{ref} (return a dummy PDF)
Phase 3 — Supabase Setup

Create the 6 tables (schema is fully defined in CLAUDE.md)
Wire Supabase client into the .NET backend
Persist applications, amendments, payments, audit logs
Phase 4 — Connect Frontend to Real Backend
Right now the React screens likely use hardcoded/dev state. Replace with actual fetch() calls to /api/validate etc.

Phase 5 — Admin Portal
Separate view for officers to approve/reject amendments (currently not started at all).

Why Mocks First?
The entire frontend is already scaffolded — it just needs a real backend to call
Mocks let you test every screen and edge case (all 18 screens) right now
When the real APIs become available, you swap the mock service for a real HTTP client — the controllers don't change at all
You can demo a fully working app to stakeholders immediately