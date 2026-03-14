
CoreInventory – Inventory Management System (IMS)
README

1. Project Introduction
CoreInventory is a modular Inventory Management System developed using ASP.NET Core MVC,
Entity Framework Core, and SQLite database. The system replaces manual registers and Excel
sheets with a centralized application to manage stock receipts, deliveries, warehouse transfers,
and stock adjustments in real time.

The project also includes a secure authentication system with Login, Logout, and Forgot Password
functionality using Email OTP verification. The OTP verification allows only 3 attempts for security.

2. Technology Stack
- ASP.NET Core MVC
- C# Programming Language
- Entity Framework Core
- SQLite Database (stored inside project files)
- Bootstrap for UI
- SMTP Email Service for OTP verification

3. Main Features
- User Login and Logout system
- Forgot Password with Email OTP verification
- OTP allowed only 3 attempts
- Dashboard showing inventory summary
- Product Management with SKU, Category, and Units
- Receipt module for incoming stock
- Delivery Orders for outgoing stock
- Internal Stock Transfer between warehouses
- Stock Adjustment to correct inventory mismatches
- Move History showing all inventory movement logs
- Multi-warehouse inventory management

4. Dashboard Overview
The dashboard provides a quick overview of inventory operations.

Dashboard KPIs:
- Total Products in Stock
- Low Stock / Out of Stock Items
- Pending Receipts
- Pending Deliveries
- Internal Transfers

5. System Modules

Products
- Create and manage products
- Manage product categories
- View stock availability per warehouse

Receipts (Incoming Goods)
- Used when items arrive from vendors
- Add supplier and products
- Enter received quantities
- Validate to increase stock automatically

Delivery Orders (Outgoing Goods)
- Used when items leave the warehouse
- Add customer and products
- Validate to reduce stock automatically

Internal Transfers
- Move stock from one warehouse to another warehouse

Stock Adjustments
- Fix mismatches between recorded stock and physical stock

Move History
- Shows complete inventory movement logs

Dashboard
- Shows summary of inventory operations

Settings
- Manage warehouses

Profile Menu
- My Profile
- Logout

6. Inventory Workflow

Step 1: Receive Goods from Vendor
Example:
Receive 100 units of a product → Stock increases by 100

Step 2: Internal Transfer
Move stock from one warehouse to another location

Step 3: Deliver Goods
Deliver products to customers → Stock decreases

Step 4: Stock Adjustment
Adjust stock for damaged or missing items

7. How to Run the Project (Step-by-Step)

Step 1: Download and install Visual Studio 2022.

Step 2: During installation, select:
ASP.NET and Web Development workload.

Step 3: Clone or download the project to your computer.

Step 4: Open the project folder and locate the solution file:
HackathonMvcSqlite.sln

Step 5: Double-click HackathonMvcSqlite.sln to open the project in Visual Studio.

Step 6: Install the required NuGet packages:

- Microsoft.EntityFrameworkCore 6.0.25
- Microsoft.EntityFrameworkCore.Sqlite 6.0.25
- Microsoft.EntityFrameworkCore.Design 6.0.25
- Microsoft.EntityFrameworkCore.Tools 6.0.25

Step 7: Restore NuGet packages if needed.

Step 8: Build the project:
Build → Build Solution

Step 9: Run the project using IIS Express or press the Run button in Visual Studio.

Step 10: The application will open in your browser and the CoreInventory system will start running.

8. Project Purpose

This project was developed as a solution for the CoreInventory problem statement.
The system digitizes inventory operations including stock receipts, deliveries,
internal transfers, and stock adjustments while providing real-time tracking of stock movements.
