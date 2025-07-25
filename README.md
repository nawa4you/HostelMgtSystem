# HostelMgtSystem


Project Description
A secure and streamlined ASP.NET Core MVC web application designed to simplify hostel room registration and management. This system provides distinct functionalities for both students and administrators, ensuring efficient room allocation and user oversight. It features robust user authentication, a student portal for submitting and tracking hostel room requests, and a comprehensive admin dashboard for approving/rejecting registrations, managing user accounts, and overseeing room assignments. Key enhancements include a multi-step password reset flow with verification codes and a consolidated admin action for managing registrations.

Features
User Features
User Registration & Login: Secure account creation and authentication.

Hostel Room Registration: Students can register for available hostel rooms.

Room Request Tracking: Users can view the status of their submitted room registration requests (pending or approved).

Approved Room Status: Once a room is approved, the user's dashboard prominently displays their assigned room, and they cannot submit new requests.

Cancel Pending Requests: Users can cancel their own pending room registration requests.

Password Change: Authenticated users can change their password.

Forgot Password: A multi-step process for unauthenticated users to request a password reset, involving a simulated verification code sent to their email (displayed in a popup for demonstration).

Admin Features
Admin Dashboard: Centralized access to administrative functionalities.

Manage All Registrations: View all hostel registration requests (both pending and approved).

Approve Registrations: Admins can approve pending room requests. Upon approval, the system automatically assigns an available room (if RoomId was null) and marks it as unavailable. Crucially, all other pending requests for that specific user are automatically deleted to ensure one approved room per user.

Delete Registrations (Reject/Remove): A unified delete action for admins:

For pending requests, it acts as a "Reject" function, deleting the request.

For approved registrations, it deletes the record and frees up the assigned room (marks it IsAvailable = true).

Edit Registrations: Admins can modify existing registration details, including assigning a room, with dynamic room selection based on the chosen hostel.

Manage Users (DataTables): View all registered users in an interactive, searchable, and sortable table powered by jQuery DataTables, including a persistent serial number column.

Reset User Passwords: Admins can reset user passwords to a default value for users who have submitted a password reset request.

Technologies Used
Backend: ASP.NET Core MVC (C#)

Database: Entity Framework Core (ORM) with SQL Server (or SQLite for local development)

Authentication: ASP.NET Core Cookie Authentication

Password Hashing: BCrypt.Net

Frontend:

HTML5

CSS3 (Bootstrap 5 for styling)

JavaScript

jQuery

jQuery DataTables (for user management table)

Icons: Bootstrap Icons (via CDN)

Setup and Installation
Follow these steps to get the Hostel Management System running on your local machine.

Prerequisites
.NET SDK (6.0 or higher, based on your project's target framework)

SQL Server (or another database supported by Entity Framework Core, e.g., SQLite for simplicity)

Visual Studio 2022 (recommended IDE for ASP.NET Core development) or VS Code with C# extensions.

Git

Steps
Clone the repository:



git clone https://github.com/nawa4you/HostelMgtSystem
cd HostelMgtSystem



Configure Database Connection:

Open the appsettings.json file in the project root.

Update the ConnectionStrings section to point to your SQL Server instance or configure for SQLite.

Example for SQL Server:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HostelMgtSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  // ... other settings
}


Apply Database Migrations:

Open the Package Manager Console in Visual Studio (Tools > NuGet Package Manager > Package Manager Console).

Run the following commands to apply the database schema and seed initial data:

Add-Migration InitialCreate # (If this is the very first migration, otherwise use the latest one)
Add-Migration AddVerificationCodeToPasswordResetRequest # (For password reset flow)
Add-Migration AddIsApprovedToRegistration # (For room approval status)
Update-Database

Important: If you encounter issues with migrations, you might need to delete your existing database and migration history before running Update-Database again.

Run the Application:

In Visual Studio, press F5 or click the "Run" button.

From the command line in the project root:

dotnet run

The application should launch in your default web browser.

Usage


Initial Setup
Register a User: Navigate to the /Home/RegisterUser page to create a new user account. You can register an "Admin" user if you want to test admin functionalities.

Note: The application currently assigns the "User" role by default during registration. To create an "Admin" user for testing, you might initially register as a regular user, then manually update their Role in the database to "Admin" (e.g., using SQL Server Management Studio or SQLite Browser).

Default Admin Password for Resets: When an admin resets a user's password, it defaults to 123456. Inform the user to change this immediately after logging in.

User Flow
Login: Use your registered credentials.

Register Hostel: If you don't have an approved room, you can submit a request.

My Registrations: View your pending and approved registrations. You can cancel pending requests.

Change Password: Update your password from the "My Info" page.

Forgot Password: If logged out, use the "Forgot Password?" link on the login page to initiate a password reset. You'll get a simulated verification code to proceed.

Admin Flow
Login as Admin: Use an admin account.

Admin Dashboard: Access various admin tools.

View All Registrations: See all submitted requests.

Approve: Approve pending requests. This will assign a room and delete other pending requests for that user.

Delete (Reject): Delete pending requests (acting as a rejection).

Delete (Approved): Delete approved registrations, which also frees up the assigned room.

Edit: Modify existing registration details.

Manage Users: View all system users in an interactive table.

Reset User Password: Reset passwords for users who have requested it.

Contributing


Contributions are welcome! If you have suggestions for improvements, bug fixes, or new features, please:

Fork the repository.

Create a new branch (git checkout -b feature/your-feature-name).

Make your changes.

Commit your changes (git commit -m 'Add new feature').

Push to the branch (git push origin feature/your-feature-name).

Open a Pull Request.

License
This project is licensed under the MIT License - see the LICENSE file for details.