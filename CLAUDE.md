# CLAUDE.md — ASYCUDA Manifest Write-Off Portal

## What We Are Building

A web-based portal for the **Maldives Customs Service** that allows shipping agents and consignees to electronically submit manifest write-off requests. The system automates cross-validation of manifest data against four external government systems, eliminating manual processing and paper-based office visits.

**Key Business Value:**
- Replaces physical MCS-117 amendment forms with a digital workflow
- Automates cross-checking of 7 critical manifest fields across 4 government systems
- Reduces write-off processing time from 1–5 working days to under 10 minutes (automated path)
- Provides 100% audit trail for regulatory compliance

---

## Technology Stack

### Frontend
- **React 19** with TypeScript
- **Vite 6** (dev server + build tool) — dev server proxies `/api` to `localhost:5000`
- **Tailwind CSS 4** (utility-first styling)
- **Lucide-React** (icons)
- **Motion** (animations)
- State managed via React Context API (`AppContext`)

### Backend
- **.NET 9.0** — ASP.NET Core
- C# controllers under `Controllers/`
- Serves the React build from `client-app/dist/`

### Database & Auth
- **Supabase** — managed PostgreSQL + Auth (magic link / username+password) + Storage (document uploads) + Edge Functions (email notifications)

### External Integrations (accessed via backend only, never from frontend)
| System | Purpose |
|--------|---------|
| **ASYCUDA World** | Fetch manifest data by R-Number |
| **Tradian** | Verify Bill of Lading exists (source of truth) |
| **E-Customs** | Verify BL is registered and active |
| **E-Valuator** | Cross-validate 7 manifest fields against official BL |

---

## Project Structure

```
/home/user/manifest-write-off/
├── Program.cs                         # .NET entry point
├── myapp.csproj                       # .NET project (net9.0)
├── appsettings.json / appsettings.Development.json
├── Controllers/
│   └── WeatherForecastController.cs   # Example only — real controllers needed
├── Properties/
│   └── launchSettings.json
│
├── client-app/                        # React frontend
│   ├── package.json
│   ├── vite.config.ts                 # Proxy: /api → localhost:5000
│   ├── tsconfig.json
│   └── src/
│       ├── main.tsx
│       ├── App.tsx
│       ├── index.css                  # Tailwind + CSS variables
│       ├── context/
│       │   └── AppContext.tsx         # Central state machine (AppState, AppStatus)
│       └── components/
│           ├── ScreenContainer.tsx    # Maps AppStatus → Screen component
│           ├── DevControls.tsx        # Dev-only state switcher
│           └── screens/              # 18 screen components (see below)
│
├── PRD                                # Product Requirements Document
├── PRODUCT SPEC                       # Full technical specification
├── SCREEN.MD                          # Screen-by-screen UI spec
├── APP_FLOW.MD                        # Application flow diagram / logic
├── blueprint.md                       # Project blueprint & change log
└── GEMINI.md / gemini.md              # AI coding rules & memory
```

---

## Application Flow & Screens

### Status Enum → Screen Mapping

The entire app is a state machine. `AppContext` holds a `status` field that maps to exactly one screen.

```
AppStatus        Screen Component        Description
─────────────────────────────────────────────────────────────────────
entry            S1Entry                 R-Number + port selection
loading          S2Validating            Live progress: ASYCUDA → BL → E-Valuator
asycuda_down     E1AyscudaDown           Hard stop: ASYCUDA system unavailable
r_not_found      E2RNotFound             Hard stop: R-Number not in ASYCUDA
not_assessed     E3NotAssessed           Wait state: form not yet assessed by officer
form_locked      E4FormLocked            Wait state: officer has form locked
insufficient_funds E5InsufficientFunds   Block: FF account balance too low
account_mismatch E6AccountMismatch       Block: FF account ≠ consignee code
bl_not_found     S3BLNotFound            BL not in Tradian → option to submit cancel request
field_mismatch   S4FieldMismatch         1+ of 7 fields don't match → triggers amendment
write_off_success S5Success              All 7 fields matched → auto write-off certificate
technical_error  S6TechnicalError        BL in Tradian but not registered in E-Customs
amendment_form   S7AmendmentForm         Digital MCS-117 amendment form
pending_review   S8PendingReview         Waiting for officer approval (4-step tracker)
rejected         S9Rejected              Admin rejected → can re-submit with corrections
payment_ready    S10Payment              Payment gateway (charged to FF account)
finalized        S11Finalized            Amendment approved + paid → write-off certificate
bl_cancel        S12BLCancel             BL cancel request submission form
```

### Happy Path (Auto Write-Off)
```
S1 → S2 → S5
```

### Amendment Path
```
S1 → S2 → S4 → S7 → S8 → (S9 → S7) | S10 → S11
```

### BL Cancel Path
```
S1 → S2 → S3 → S12
```

---

## Core Data Models (TypeScript — AppContext)

```typescript
// Central application state
interface AppState {
  status: AppStatus;
  rNumber: string;
  port: string;
  year: number;
  manifestData: ManifestData | null;
  fieldResults: FieldResult[];
  failedFields: FieldResult[];
  amendmentRef: string | null;
  amendmentData: AmendmentData | null;
  calculatedFine: number;
  paymentRef: string | null;
  writeOffRef: string | null;
  certificateUrl: string | null;
}

// From ASYCUDA
interface ManifestData {
  vessel: string;
  voyage: string;
  dateArrival: string;
  blNumber: string;
  ffAccount: string;
  consigneeCode: string;
  grossWeight: string;
  packageCode: string;
  packsNumber: string;
  exporter: string;
  consignee: string;
  carrier: string;
}

// E-Valuator field comparison result
interface FieldResult {
  field: string;
  manifestValue: string;
  evaluatorValue: string;
  match: boolean;
  fineTier: 1 | 2;
}

// Amendment form data (MCS-117)
interface AmendmentData {
  serviceTypes: string[];     // "House BL amendment" | "Master BL amendment" | "Late manifest"
  amendmentTypes: string[];   // which of the 7 fields are being amended
  contentBefore: string;
  contentAfter: string;
  reason: string;
  documents: Document[];
  declaredBy: string;
}
```

---

## Backend API Endpoints (To Be Implemented)

The only existing controller is the example `WeatherForecastController`. All of the following need to be built:

```
POST   /api/validate
       Body: { r_number, port, year }
       Orchestrates: ASYCUDA → Tradian → E-Customs → E-Valuator
       Response: { status, manifestData, fieldResults, errors }

POST   /api/amendment/submit
       Body: { r_number, port, bl_number, service_types[], amendment_types[],
               content_before, content_after, reason, ff_account,
               document_urls[], declaration_accepted, signatory_details }
       Response: { amendment_ref, submitted_at, fine_amount, status }

GET    /api/amendment/status/{amendment_ref}
       Response: { status: "pending_review" | "approved" | "rejected", rejection_reason? }

POST   /api/bl-cancel/submit
       Body: BL cancel request details

POST   /api/payment/initiate
       Body: { amendment_ref, payment_method }
       Response: { payment_ref, redirect_url }

POST   /api/payment/confirm
       Body: { payment_ref, transaction_ref }
       Response: { write_off_ref, certificate_url }

GET    /api/certificate/{write_off_ref}
       Response: PDF certificate (binary)
```

### External API Calls Made by Backend

```
ASYCUDA:    GET  /manifest/{port_code}/{year}/{r_number}
            GET  /account/validate/{ff_code}/{consignee_code}

Tradian:    GET  /bl/{bl_number}
            POST /bl/cancel-request

E-Customs:  GET  /bl/verify/{bl_number}

E-Valuator: POST /validate  { grossWeight, packageCode, packsNumber,
                              exporter, consignee, carrier, blNumber }
```

---

## Database Schema (Supabase / PostgreSQL)

```sql
applications
  id UUID PK, r_number, port, year, bl_number,
  ff_account_code, consignee_code,
  status ENUM(validated|written_off|amendment_pending|finalized),
  write_off_ref, created_at, updated_at, created_by

validation_results
  id, application_id FK, field_name, manifest_value,
  evaluator_value, match BOOL, fine_tier INT

amendments
  id, amendment_ref UNIQUE, application_id FK,
  service_types[], amendment_types[],
  content_before, content_after, reason, fine_amount,
  status ENUM(pending_review|approved|rejected),
  reviewed_by, reviewed_at, rejection_reason, created_by

documents
  id, amendment_id FK,
  document_type ENUM(amended_manifest|bl_copy|noc_exporter|noc_shipper|bl_cancel_letter|bl_document),
  file_url, file_name, file_size

payments
  id, payment_ref UNIQUE, amendment_id FK,
  ff_account_code, amount, method,
  status ENUM(initiated|confirmed|failed),
  transaction_ref, initiated_at, confirmed_at

audit_log
  id, application_id FK, event, from_state, to_state,
  user_id, ip_address, metadata JSONB, created_at
```

---

## Business Rules

### Fine Calculation
| Amendment Type | Fine Amount |
|----------------|-------------|
| Tier I field mismatch | MVR 500 |
| Tier II field mismatch | MVR 1,000 |
| Late manifest | MVR 3,000 |
| Multiple mismatches | Highest applicable tier |

### The 7 Validated Fields (E-Valuator cross-check)
1. Gross Weight
2. Package Code
3. Packs Number
4. Exporter
5. Consignee
6. Carrier
7. BL Number

### Validation Sequence (S2 loading steps)
1. Fetch manifest from ASYCUDA (timeout: 10s)
2. Validate FF account vs consignee code (E5/E6 checks)
3. Check BL in Tradian (timeout: 8s)
4. Verify BL in E-Customs (timeout: 8s)
5. Cross-validate 7 fields via E-Valuator (timeout: 12s)

---

## Environment Variables Required

```env
# External Government APIs
ASYCUDA_BASE_URL=
ASYCUDA_API_KEY=
TRADIAN_BASE_URL=
TRADIAN_API_KEY=
ECUSTOMS_BASE_URL=
ECUSTOMS_API_KEY=
EVALUATOR_BASE_URL=
EVALUATOR_API_KEY=

# Supabase
SUPABASE_URL=
SUPABASE_ANON_KEY=
SUPABASE_SERVICE_ROLE_KEY=

# App
APP_ENV=development|production
FRONTEND_URL=
```

---

## Design System

```
Colors:
  Primary:    #3E7CB1  (blue)
  Secondary:  #254E70  (dark blue)
  Dark:       #1A364E
  Danger:     #C0392B  (red)
  Success:    #27AE60  (green)

Typography:
  Font:       DM Sans
  Body:       16px
  Headings:   600 weight

Components:
  - Cards with top border stripe (color varies by state)
  - Primary / Ghost / Gold button variants
  - Status badges (color-coded by state)
  - 7-field comparison table (green rows = match, red = mismatch)
  - 4-step progress tracker (S8 pending review)

Responsive:
  - Desktop & tablet: fully interactive (1024px+)
  - Mobile: read-only view
```

---

## Development Notes

### Current Implementation Status
- **Frontend:** All 18 screen components scaffolded in `client-app/src/components/screens/`
- **AppContext:** State machine with all statuses and data types defined
- **Backend:** Only example `WeatherForecastController` exists — **all real controllers need to be built**
- **Database:** Schema defined in spec but not yet created in Supabase
- **External APIs:** Integration architecture defined but not yet implemented
- **Auth:** Supabase Auth planned but not yet wired up
- **Admin Portal:** Separate app for officer review — not yet started

### Running Locally
```bash
# Backend (.NET)
dotnet run

# Frontend (separate terminal)
cd client-app
npm install
npm run dev
# Vite dev server at :5173, proxies /api to :5000
```

### Key Files to Read Before Making Changes
- `PRD` — product requirements and user stories
- `PRODUCT SPEC` — full technical specification
- `SCREEN.MD` — screen-by-screen UI details
- `APP_FLOW.MD` — state transition logic
- `client-app/src/context/AppContext.tsx` — central state management
- `client-app/src/components/ScreenContainer.tsx` — screen routing

### Security Rules
- External API keys are **never** exposed to the frontend
- All external API calls go through the .NET backend only
- File uploads: PDF/image only, max 10MB per file
- Rate limiting: 10 requests/min per user (to be implemented)
- CORS: whitelist frontend origin only
- Every state transition must be logged to `audit_log`

### Performance Targets
- End-to-end validation: < 15 seconds
- System uptime: 99.5% during business hours (07:30–16:00 MVT, Sun–Thu)
