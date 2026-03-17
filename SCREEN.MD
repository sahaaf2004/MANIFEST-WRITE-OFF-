# Screen Flow
## ASYCUDA Write-Off Manifest Portal
**Version:** 1.0.0

---

## Screen Index

| ID | Screen Name | Type | Trigger |
|---|---|---|---|
| S1 | Entry | Input | App launch |
| S2 | Validating | Loading | S1 submit |
| E1 | ASYCUDA System Down | Error | ASYCUDA unreachable |
| E2 | R-Number Not Found | Error | ASYCUDA returns null |
| E3 | Form Not Assessed | Wait | Form status = unprocessed |
| E4 | Form Locked | Wait | Officer has form open |
| E5 | Insufficient Funds | Block | Account balance = 0 / low |
| E6 | Account Mismatch | Block | FF code ≠ consignee code |
| S3 | BL Not Found | Error + Action | Tradian: BL not found |
| S4 | Field Mismatch | Error + Action | E-Valuator: 1+ fields fail |
| S5 | Write-Off Success | Terminal | All 7 fields pass |
| S6 | Technical Error (E-Customs) | Error | BL not in E-Customs |
| S7 | Amendment Form | Form | From S4 or Late Manifest |
| S8 | Pending Review | Status | S7 submitted |
| S9 | Rejected — Re-submit | Notification | Admin rejects |
| S10 | Payment Gateway | Action | Admin approves |
| S11 | Write-Off Finalized | Terminal | Payment confirmed |
| S12 | BL Cancel Request | Form | From S3 (BL not found) |

---

## S1 — Entry Screen

**Purpose:** The only user input screen. User provides R-Number and selects port.

**Components:**
- App header: "ASYCUDA Write-Off Portal" + Maldives Customs Service branding
- Gold accent stripe (4px) at card top
- R-Number text input — label: "R-Number", placeholder: "e.g. R-2024-0001"
- Port dropdown — options:
  - Malé Commercial Harbour
  - Hulhumalé Port
  - Kulhudhuffushi Port
  - Addu City Port
  - Gaanifaru Port
- "Search & Validate" primary button (full width)
- Info note: "R-Number must match the port and year of the original manifest application"
- Inline validation if fields empty on submit

**Transitions:**
- Submit → S2 (Validating)

---

## S2 — Validating / Loading Screen

**Purpose:** System performs API calls to ASYCUDA, then chains to Tradian, E-Customs, E-Valuator. User sees live step progress.

**Components:**
- Spinner icon (gold) + heading: "Validating Manifest..."
- Step list with live status icons (spinner → check/cross):
  1. Connecting to ASYCUDA system
  2. Fetching R-Number application
  3. Extracting BL Number
  4. Checking BL with Tradian
  5. Verifying BL in E-Customs
  6. Cross-checking fields with E-Valuator
- Animated gold progress bar
- Sub-text: "This usually takes 5–15 seconds"

**Transitions (in order of check):**
- ASYCUDA unreachable → E1
- R-Number not found → E2
- Form not assessed → E3
- Form locked → E4
- Insufficient funds → E5
- Account mismatch → E6
- BL not found in Tradian → S3
- BL not in E-Customs → S6
- All fields match → S5
- 1+ fields mismatch → S4

---

## E1 — ASYCUDA System Down

**Purpose:** Hard stop. ASYCUDA API is unreachable.

**Components:**
- Full-width red banner: "ASYCUDA System Unavailable"
- Large alert icon
- Heading: "Cannot connect to the ASYCUDA system"
- Body: "The customs system is currently unavailable. This is a temporary issue. Please try again in a few minutes."
- System status timestamp: "Last checked: [time]"
- "Try Again" primary button → retries S2
- "Back to Search" ghost button → S1

**User action:** Retry. No application action possible.

---

## E2 — R-Number Not Found

**Purpose:** Hard stop. The R-Number does not exist in ASYCUDA for the selected port and year.

**Components:**
- Red banner: "R-Number Not Found"
- Details box (monospace):
  ```
  R-Number:  [entered value]
  Port:      [selected port]
  Result:    Not found in ASYCUDA
  ```
- Body: "Please verify the R-Number and ensure it matches the correct port and year."
- "Try a Different R-Number" button → S1
- "Contact Customs Support" link → contact modal

**User action:** Check and re-enter correct R-Number.

---

## E3 — Form Not Assessed

**Purpose:** Wait state. The form exists in ASYCUDA but has not yet been processed by a customs officer.

**Components:**
- Amber banner: "Form Pending Assessment"
- Clock icon (pulsing)
- Heading: "Your manifest has not yet been assessed"
- Body: "A customs officer has not yet processed this form. Once assessed, you will be able to proceed with the write-off."
- "Check Again" button (refreshes S2 check) 
- Estimated wait note: "Forms are typically assessed within 1–2 working days"
- "Back to Search" ghost button → S1

**User action:** Wait and retry. No application action.

---

## E4 — Form Locked

**Purpose:** Wait state. A customs officer currently has the form open in ASYCUDA. Concurrent access is blocked by the system.

**Components:**
- Amber banner: "Form Currently Locked"
- Lock icon
- Heading: "This form is currently open by a customs officer"
- Body: "The ASYCUDA system prevents access while an officer has this form open. Please wait a few minutes and try again."
- "Check Again" button → retries S2
- "Back to Search" ghost button → S1

**User action:** Wait and retry. No application action.

---

## E5 — Insufficient Funds

**Purpose:** Block. The freight forwarder's payment account does not have sufficient balance to cover the write-off charges.

**Components:**
- Red banner: "Insufficient Account Balance"
- Wallet icon
- Heading: "Your payment account has insufficient funds"
- Account details box:
  ```
  Account:     FF-[XXXXX]
  Status:      Insufficient funds
  ```
- Body: "Please top up your customs payment account before proceeding. Contact the Maldives Customs Service finance department."
- Contact info block
- "Try Again" button → retries check after top-up
- "Back to Search" ghost button → S1

**User action:** Top up account, then retry.

---

## E6 — Account Incompatible

**Purpose:** Block. The payment account (FF code) is not compatible with the consignee code on the manifest.

**Components:**
- Red banner: "Account Not Compatible with Consignee"
- Warning icon
- Heading: "Payment account mismatch"
- Details box:
  ```
  FF Account:      FF-[XXXXX]
  Consignee Code:  [code from manifest]
  Status:          Incompatible
  ```
- Body: "The registered payment account does not match the consignee code on this manifest. Please contact the Maldives Customs Service to resolve this."
- Contact info block
- "Back to Search" ghost button → S1

**User action:** Contact customs finance to resolve account mapping.

---

## S3 — BL Not Found (Tradian)

**Purpose:** Error + action. The BL Number extracted from the ASYCUDA form was not found in the Tradian registry. User must contact Tradian. Option to submit BL Cancel Request.

**Components:**
- Red banner: "Bill of Lading Not Found"
- SearchX icon (large, pulsing)
- Heading: "We could not locate your Bill of Lading"
- Details box:
  ```
  R-Number:    [value]
  Port:        [value]
  BL Searched: [BL Number from ASYCUDA]
  Registry:    Tradian
  Result:      NOT FOUND
  ```
- Body: "The BL Number has not been received by customs. This means the BL has not been registered by the shipping line. Contact Tradian to resolve."
- Tradian contact block
- Divider: "Alternatively, submit a BL Cancel Request"
- "Submit BL Cancel Request" button → S12
- "Back to Search" ghost button → S1

**User action:** Contact Tradian OR submit BL cancel request.

---

## S4 — Field Mismatch Screen

**Purpose:** Core error screen. BL was found and validated but 1 or more of the 7 fields do not match the E-Valuator record. Shows exact mismatches. User can apply for amendment.

**Components:**
- Amber banner: "Write-Off Unsuccessful — Data Mismatch Detected"
- Sub-text: "X field(s) do not match the E-Valuator record. Review the comparison below."
- 7-row comparison table:
  | Field | Your Manifest Value | E-Valuator Value | Status |
  |---|---|---|---|
  - Passing rows: white bg, green left border, green check icon
  - Failing rows: red bg (#FEF2F2), red left border, red X icon
  - Failing rows show values in red text
- Summary block: "X field(s) require correction before this manifest can be written off"
- Fine indicator: auto-calculated fine amount shown (Tier I: MVR 500 / Tier II: MVR 1,000)
- "Apply for Amendment" primary button → S7
- "Back to Search" ghost button → S1
- "Apply for Late Manifest" secondary link → S7 (late manifest mode)

**Transitions:**
- Amendment button → S7
- Late manifest → S7 (late manifest service type pre-selected)

---

## S5 — Write-Off Success

**Purpose:** Terminal happy path. All 7 fields matched. Manifest is automatically written off.

**Components:**
- Gold banner: "Manifest Successfully Written Off"
- Animated green CheckCircle2 icon (72px)
- Heading: "Write-Off Complete"
- Sub-heading: "All 7 validation fields matched the E-Valuator record"
- Summary panel (gold tint):
  ```
  R-Number:      [value]
  Port:          [value]
  BL Number:     [value]
  Written Off:   [DD MMM YYYY, HH:MM]
  Reference:     WO-[YEAR]-[XXXXXX]
  ```
- Expandable: "View Field Comparison" → shows all 7 as green pass
- "Download Write-Off Certificate" gold button
- "Write Off Another Manifest" ghost button → S1

---

## S6 — Technical Error (E-Customs)

**Purpose:** BL exists in Tradian but is not registered in the E-Customs system. System error requiring IT intervention.

**Components:**
- Red banner: "E-Customs Registration Error"
- AlertTriangle icon
- Heading: "BL found but not registered in E-Customs"
- Technical details box (collapsed by default, expandable):
  ```
  BL Number:    [value]
  Tradian:      FOUND
  E-Customs:    NOT REGISTERED
  Error Code:   EC-[XXX]
  ```
- Body: "This is a system-level issue. The Bill of Lading exists in Tradian but has not been registered in the E-Customs system. Please contact the IT department."
- IT contact block
- "Back to Search" ghost button → S1

**User action:** Contact IT. No self-service option.

---

## S7 — Amendment Form (Digital MCS-117)

**Purpose:** Full digital version of the MCS-117 form. Triggered from S4 (field mismatch) or late manifest path.

**Sections:**

### Section 1 — Manifest Details (pre-filled, read-only)
- Name of Vessel/Aircraft
- Voyage Number
- Date of Arrival
- Bill of Lading Number

### Section 2 — Service Type (checkbox, required)
- Amendment of House Bill of Lading
- Amendment of Master Bill of Lading
- Late Submission of Manifest (auto-checked if E4 path, shows MVR 3,000 fine warning)

### Section 3 — Amendment Type (checkbox, shows fine tier)
- Tier I (MVR 500): Exporter/Shipper, Notify Party, Place of Discharge, Final Destination
- Tier II (MVR 1,000): Consignee, Gross Weight, Container Number, Goods Description, BL/AWB Split, BL/AWB Number, Place of Loading, No. of Packages, Seal Number, Volume
- Auto-checks failing fields from S4

### Section 4 — Required Amendment
- Content Before Change (pre-filled from ASYCUDA data)
- Content After Change (user input)

### Section 5 — Reason (free text, required)

### Section 6 — Declaration
- Declaration checkbox (required before submit)
- Signature section: Shipping Agent | Consignee (Original) | Consignee (New)
- Agent/Consignee name, Consignee number, Responsible person, Designation, Contact number

### Section 7 — Supporting Documents Upload
- Original amended manifest + company stamp (required)
- Original BL/AWB after change (required)
- No-objection letter from Exporter (required if consignee change)
- No-objection letter from Shipper (required if "to order")
- Each: drag-drop zone, shows filename + size after upload, max 10MB, PDF/image

**Fine Summary Bar (sticky at bottom):**
- Shows calculated fine: MVR [amount]
- Charged to: FF-[XXXXX]
- Updates live as user selects amendment types

**Buttons:**
- "Submit Amendment Application" (primary, disabled until declaration checked + required docs uploaded)
- "Save as Draft" (ghost)
- "Cancel" (ghost) → back to S4

**Transition on submit:** → S8

---

## S8 — Pending Review

**Purpose:** Application submitted. Waiting for customs officer to review. User tracks status.

**Components:**
- Header: "Amendment Application Status"
- Reference number (large, gold, monospace): AMD-[YEAR]-[XXXXXX]
- Submitted: [DD MMM YYYY, HH:MM]
- 4-step tracker:
  1. Submitted (green, complete)
  2. Pending Review (gold pulse when active)
  3. Admin Decision (gray / green when done)
  4. Payment & Finalization (gray / green when done)
- Status message:
  - Pending: "Your application is awaiting review by a Customs Officer. You will be notified by email when a decision is made."
  - Approved: "Your application has been approved. Please proceed to payment."
- Collapsible: "View Application Summary"
- Buttons:
  - Pending: "Check Status" (ghost, re-fetches) + "Back to Dashboard"
  - Approved: "Proceed to Payment" (primary gold) → S10

**Transitions:**
- Admin approves → S10
- Admin rejects → S9

---

## S9 — Rejected — Re-submit

**Purpose:** Admin has rejected the amendment. User notified with reason and can re-submit.

**Components:**
- Red banner: "Application Rejected"
- Reference: AMD-[YEAR]-[XXXXXX]
- Rejection reason box (filled by officer):
  ```
  Reviewed by:  [Officer name]
  Date:         [DD MMM YYYY]
  Reason:       [Officer's rejection reason]
  ```
- Body: "You may correct the issues noted above and re-submit your application."
- "Re-submit Application" primary button → S7 (pre-filled with previous data)
- "Contact Customs Support" ghost button
- "Back to Dashboard" ghost button → S1

---

## S10 — Payment Gateway

**Purpose:** Admin approved. User must pay the amendment fine to finalize the write-off.

**Components:**
- Gold stripe + CreditCard icon
- Heading: "Payment Required to Finalize Write-Off"
- Amendment summary panel:
  ```
  Amendment Ref:    AMD-[YEAR]-[XXXXXX]
  Original R-No:    [value]
  Port:             [value]
  Fields Amended:   [list of fields]
  Approved By:      [Officer name]
  Approval Date:    [DD MMM YYYY]
  ```
- Payment breakdown panel:
  ```
  Amendment Fine:            MVR [amount]
  [Late Manifest Fine:]      MVR 3,000 (if applicable)
  ──────────────────────────────────────
  Total Amount Due:          MVR [total]
  Charged to Account:        FF-[XXXXX]
  ```
- Payment method selector: Bank Transfer / BML Internet Banking / Cash at Counter
- "Pay to Finalize Write-Off" gold button (full width)
- Fine print: "Payment must be made within 7 working days of approval."

**Transition:**
- Payment confirmed → S11

---

## S11 — Write-Off Finalized

**Purpose:** Terminal state after amendment payment. Manifest written off using corrected data.

**Components:**
- Gold banner: "Manifest Written Off — Amendment Applied"
- Animated CheckCircle2 icon
- "Amendment Applied" badge
- Summary (shows both original and corrected values for amended fields)
- Reference panel:
  ```
  Write-Off Ref:    WO-[YEAR]-[XXXXXX]
  Amendment Ref:    AMD-[YEAR]-[XXXXXX]
  Finalized:        [DD MMM YYYY, HH:MM]
  ```
- "Download Write-Off Certificate" gold button
- "Download Amendment Record" ghost button
- "Write Off Another Manifest" ghost button → S1

---

## S12 — BL Cancel Request

**Purpose:** User submits a formal request to cancel a BL that customs has not received. Triggered from S3.

**Components:**
- Header: "BL Cancel Request"
- Pre-filled context:
  ```
  R-Number:    [value]
  Port:        [value]
  BL Number:   [value from ASYCUDA]
  ```
- Upload zone 1: "Supporting Letter" (required) — official letter requesting cancellation, PDF/image
- Upload zone 2: "Bill of Lading Document" (required) — the actual BL document, PDF
- Reason / notes (free text, required)
- Declaration checkbox
- "Submit BL Cancel Request" primary button → S8 (same review queue)
- "Cancel" ghost → S3

**Transition on submit:** → S8 (admin review, same flow as amendment)

---

## Screen Transition Map (Summary)

```
S1 → S2
S2 → E1 | E2 | E3 | E4 | E5 | E6 | S3 | S4 | S5 | S6
S3 → S12 | S1
S4 → S7
S6 → S1
S7 → S8
S8 → S9 | S10
S9 → S7
S10 → S11
S11 → S1
S12 → S8
E1 → S2 (retry) | S1
E2 → S1
E3 → S2 (retry) | S1
E4 → S2 (retry) | S1
E5 → S2 (retry after top-up) | S1
E6 → S1
```