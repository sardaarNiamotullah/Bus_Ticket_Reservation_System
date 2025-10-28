# Bus Ticket Reservation System - Backend (.NET 9)

This is the **backend** implementation of the **Bus Ticket Reservation System** as part of the **Internship Batch 3 - Full-stack (.NET) Developer Assignment**.

The project follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles using **.NET 9**, **Entity Framework Core** with **PostgreSQL**, and includes **unit tests** (xUnit).

---

## Project Structure (Clean Architecture Layers)

---

## Technologies Used

- **.NET 9 (C#)**
- **Entity Framework Core** (PostgreSQL)
- **xUnit** for Unit Testing
- **Repository Pattern**
- **Dependency Injection**
- **Async/Await & Transactions**
- **DTOs & Mapping**

---

## Features Implemented

- **Search Available Buses**  
  `SearchService.SearchAvailableBusesAsync(from, to, journeyDate)`
- **View Seat Layout**  
  `BookingService.GetSeatPlanAsync(busScheduleId)`
- **Book Seat(s)**  
  `BookingService.BookSeatAsync(input)` with transaction support
- **Domain Service** for seat state transitions
- **Dynamic Seats Left Calculation**
- **Unit Tests** (except final submission – pending)

---

## Setup Instructions

### 1. Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- PostgreSQL (v14 or later)
- Git

### 2. Clone the Repository

```bash
git clone https://github.com/sardaarNiamotullah/Bus_Ticket_Reservation_System.git
cd Bus_Ticket_Reservation_System/src
```

### 3. Environment Configuration

Create a .env file in the project root:

```bash
DB_HOST=localhost
DB_NAME=BusTicketReservationDb
DB_USER=yourusername
DB_PASSWORD=your_secure_password_here
```

### 4. Apply Migrations

From the Infrastructure directory:

```bash
# Navigate to Infrastructure
cd src/Infrastructure
# Remove old migrations if any
rm -rf Migrations

# Create new migration
dotnet ef migrations add InitialCreate --startup-project ../WebApi

# Apply migration to create tables
dotnet ef database update --startup-project ../WebApi
```

### Populate Seed Data (Required After Migration)

After successful migration, you must seed initial data to populate buses, routes, schedules, and seats.
Run Seed SQL Script
Connect to PostgreSQL and execute the following script:

-- Insert Buses
```bash
INSERT INTO "Buses" ("Id", "Name", "BusNumber", "IsAC", "TotalSeats", "FareAmount", "Currency", "CreatedAt", "UpdatedAt")
VALUES
(gen_random_uuid(), 'Green Line Paribahan', 'GL-001', true, 40, 800.00, 'BDT', NOW(), NOW()),
(gen_random_uuid(), 'Shyamoli Paribahan', 'SP-002', true, 36, 750.00, 'BDT', NOW(), NOW()),
(gen_random_uuid(), 'Hanif Enterprise', 'HE-003', false, 40, 600.00, 'BDT', NOW(), NOW()),
(gen_random_uuid(), 'Ena Transport', 'ET-004', true, 40, 850.00, 'BDT', NOW(), NOW()),
(gen_random_uuid(), 'Shohagh Paribahan', 'SH-005', false, 36, 650.00, 'BDT', NOW(), NOW());
```

-- Insert Routes

```bash
INSERT INTO "Routes" ("Id", "FromCity", "ToCity", "DepartureTime", "ArrivalTime", "DurationMinutes", "CreatedAt", "UpdatedAt")
VALUES
(gen_random_uuid(), 'Dhaka', 'Chittagong', '22:00:00', '06:00:00', 480, NOW(), NOW()),
(gen_random_uuid(), 'Dhaka', 'Sylhet', '23:00:00', '06:30:00', 450, NOW(), NOW()),
(gen_random_uuid(), 'Dhaka', 'Rajshahi', '21:30:00', '05:30:00', 480, NOW(), NOW()),
(gen_random_uuid(), 'Chittagong', 'Dhaka', '22:30:00', '06:30:00', 480, NOW(), NOW()),
(gen_random_uuid(), 'Dhaka', 'Coxs Bazar', '20:00:00', '08:00:00', 720, NOW(), NOW());
```

-- Generate Schedules & Seats for next 7 days

```bash
DO $$
DECLARE
    bus_record RECORD;
    route_record RECORD;
    schedule_id UUID;
    journey_date DATE;
    seat_num INT;
    total_seats INT;
BEGIN
    FOR day_offset IN 0..6 LOOP
        journey_date := CURRENT_DATE + day_offset;
        FOR bus_record IN SELECT * FROM "Buses" LOOP
            FOR route_record IN SELECT * FROM "Routes" WHERE "FromCity" = 'Dhaka' LOOP
                schedule_id := gen_random_uuid();
                total_seats := bus_record."TotalSeats";
                INSERT INTO "BusSchedules" ("Id", "BusId", "RouteId", "JourneyDate", "AvailableSeats", "Status", "CreatedAt", "UpdatedAt")
                VALUES (schedule_id, bus_record."Id", route_record."Id", journey_date, total_seats, 'Active', NOW(), NOW());
                FOR seat_num IN 1..total_seats LOOP
                    INSERT INTO "Seats" ("Id", "ScheduleId", "SeatNumber", "Status", "CreatedAt", "UpdatedAt")
                    VALUES (gen_random_uuid(), schedule_id, seat_num, 'Available', NOW(), NOW());
                END LOOP;
            END LOOP;
        END LOOP;
    END LOOP;
END   $$;
```

### Run the Backend API

```bash
cd WebApi
dotnet run
```

## API Endpoints

The backend exposes RESTful APIs under two main controllers: **Search** and **Booking**.

| Method | Endpoint                              | Description                                                 |
| ------ | ------------------------------------- | ----------------------------------------------------------- |
| `GET`  | `/api/Booking/seat-plan/{scheduleId}` | Get seat layout for a specific bus schedule                 |
| `POST` | `/api/Booking/book`                   | Book one or more seats with passenger details               |
| `POST` | `/api/Booking/cancel/{bookingId}`     | Cancel an existing booking by ID                            |
| `POST` | `/api/Search/buses`                   | Search available buses (body-based request)                 |
| `GET`  | `/api/Search/buses`                   | Search available buses using query parameters (alternative) |

---

### 1. Search Buses

#### `POST /api/Search/buses` _(Recommended)_

**Request Body:**

```json
{
  "from": "Dhaka",
  "to": "Chittagong",
  "journeyDate": "2025-10-29"
}
```

**Response:**

```json
[
  {
    "busScheduleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "companyName": "Green Line Paribahan",
    "busName": "GL-001",
    "startTime": "22:00:00",
    "arrivalTime": "06:00:00",
    "seatsLeft": 38,
    "price": 800.0
  }
]
```

### 2. Get Seat Plan

#### `GET /api/Booking/seat-plan/{scheduleId}`

**Response:**

```json
{
  "busScheduleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "seats": [
    {
      "seatNumber": 1,
      "row": 1,
      "status": "Available"
    },
    {
      "seatNumber": 5,
      "row": 2,
      "status": "Booked"
    }
  ]
}
```

### 3. Book or Buy Seat

#### `POST /api/Booking/book` 

**Request Body:**

```json
{
  "busScheduleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "seatNumbers": [1, 2],
  "passengerName": "John Doe",
  "mobileNumber": "01700000000",
  "boardingPoint": "Kallyanpur",
  "droppingPoint": "Chittagong Station"
}
```

**Response:**

```json
{
  "bookingId": "a1b2c3d4-...",
  "message": "Booking confirmed successfully",
  "totalAmount": 1600.00,
  "bookedSeats": [1, 2]
}
```

# Bus Ticket Reservation System - Frontend (Angular 19)

This is the **frontend** implementation of the **Bus Ticket Reservation System** built with **Angular 19** and **TypeScript**, located in `src/ClientApp`.

It consumes the **.NET 9 Web API** and provides a clean, responsive UI for searching buses, viewing seat layouts, and booking tickets.

---

## Features Implemented

- **Search Buses**  
  Form with `From`, `To`, `Journey Date` → Table of available buses
- **View Seat Layout**  
  Interactive grid with color-coded seat status:
  - **Green**: Available  
  - **Orange**: Booked  
  - **Red**: Sold  
- **Select & Book Seats**  
  Multi-seat selection, passenger info input, boarding/dropping points
- **Booking Confirmation**  
  Real-time API feedback with success/error messages
- **Responsive Design**  
  Mobile-friendly layout using **Bootstrap 5**

---


---

## Setup Instructions

### 1. Prerequisites

- **Node.js** (v18 or later)
- **Angular CLI** (v19)
- **Backend API Running** at `http://localhost:5123`

### 2. Navigate to ClientApp

```bash
cd src/ClientApp
```

### 3. Install Dependencies
```bash
npm install
```

### 4. Run the Frontend
```bash
ng serve
```

# TL;DR

### Run with Backend

1. Start .NET API (dotnet run in WebApi)
2. Populate Postgres Database
3. Start Angular (ng serve)
4. Open http://localhost:4200