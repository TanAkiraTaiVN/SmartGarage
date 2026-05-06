---
name: testing-smartgarage
description: Test the SmartGarage parking management platform end-to-end. Use when verifying Admin UI, check-in/check-out, payment, or dashboard changes.
---

# Testing SmartGarage

## Setup

1. Restore and build:
   ```bash
   cd /home/ubuntu/repos/SmartGarage
   dotnet restore && dotnet build
   ```
2. Delete old database for fresh seed data (optional):
   ```bash
   rm -f SmartGarage.API/SmartGarage.db
   ```
3. Start the server:
   ```bash
   cd SmartGarage.API && dotnet run --urls "http://0.0.0.0:5000"
   ```
4. Server runs at `http://localhost:5000`, Swagger at `/swagger`

## Devin Secrets Needed

No external secrets required. The app uses local SQLite and a hardcoded JWT dev key.

## Test Credentials

- **Admin**: `admin@smartgarage.com` / `Admin@123`
- Login form at `http://localhost:5000` has credentials pre-filled

## Seed Data

- 1 Admin user
- 90 parking spots: 3 floors x (10 Car spots in Zone A + 20 Motorcycle spots in Zone B)
  - Car spots: A101-A110 (T1), A201-A210 (T2), A301-A310 (T3)
  - Motorcycle spots: B101-B120 (T1), B201-B220 (T2), B301-B320 (T3)
- 4 parking rates: Motorcycle 5,000/hr, Car 20,000/hr, Bicycle 2,000/hr, Truck 30,000/hr

## UI Navigation (Admin Panel)

Sidebar nav items after login:
- **Tổng quan** (Dashboard): Stats grid + active tickets table
- **Vé xe** (Tickets): All tickets list
- **Phương tiện** (Vehicles): Vehicle CRUD, "Thêm phương tiện" button opens modal
- **Vị trí đỗ** (Spots): Spot stats + spot table
- **Thanh toán** (Payments): Payment history table
- **Check-in / Check-out**: Two-panel layout — left for check-in (vehicle + spot dropdowns), right for check-out (ticket code + payment method)

## Primary Test Flow (Full Lifecycle)

1. Login → verify dashboard shows 90/90/0/0 VND
2. Go to Phương tiện → add vehicle (e.g. motorcycle 51A-12345)
3. Go to Check-in/Check-out → select vehicle + motorcycle spot (Zone B) → Check-in
4. Verify QR code image and ticket code (TK-YYYYMMDD-XXXXXXXX) appear
5. Go to Tổng quan → verify available=89, in-use=1, active ticket shown
6. Go to Check-in/Check-out → enter ticket code + select payment method → Check-out
7. Verify fee calculation (motorcycle: ceiling(hours) x 5,000 VND)
8. Go to Thanh toán → verify payment record (PAY-YYYYMMDD-XXXXXXXX)
9. Go to Tổng quan → verify spots back to 90/0, revenue updated

## Known Issues / Gotchas

- **SQLite decimal aggregation**: EF Core's SQLite provider might not support `SumAsync()` on decimal columns. If dashboard API returns 500 errors, check for `System.NotSupportedException: SQLite cannot apply aggregate operator 'Sum'`. The fix is to use client-side aggregation: `.Select(p => p.Amount).ToListAsync()` then `.Sum()` in memory.
- **Vehicle type matching**: Check-in validates that vehicle type matches spot type. Motorcycle vehicles can only park in Zone B motorcycle spots, cars in Zone A car spots.
- **Long spot dropdowns**: The spot dropdown has 90 options. Using JS console to set `.value` directly is faster than scrolling: `document.getElementById('checkinSpot').value = '11'` (B101).
- **Database reset**: Delete `SmartGarage.API/SmartGarage.db` and restart to get fresh seed data.
