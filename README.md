ExpenditureTrackerWeb
ExpenditureTrackerWeb is a .NET 8 web application designed to help users track, categorize, and analyze their expenses efficiently. The application supports bill image uploads, automatic data extraction using OCR, and robust category management for personalized financial tracking.

Features
•	User Authentication: Secure login and registration for individual users.
•	Expense Tracking: Add, edit, and delete expenses with detailed information.
•	Bill Image Import: Upload receipts or bills as images; extract expense data using OCR.
•	Automatic Category Detection: Uses AI/ML agents to suggest or auto-assign categories based on bill content.
•	Custom Categories: Users can create, edit, and manage their own expense categories.
•	Reporting & Analytics: Visualize spending patterns with charts and summaries.
•	Multi-user Support: Each user’s data is isolated and secure and credentials are store secure using SHA-256 hashing.

Technical Details
•	Framework: ASP.NET Core 8.0
•	Frontend: Angular pages with typescript and D3.js for data visualization
•	Backend: Entity Framework Core for data access
•	Database: SQL Server (configurable)
•	OCR Integration: Bill information is extracted using an OCR service using he Tesseract library for .NET
•	AI/ML Agent: 
	-BillInformationAnalyserAgent for intelligent data extraction and category suggestion
	- ImportDataFromCSVagent for importing expenses from CSV files
•	Dependency Injection: Used throughout for services and repositories
•	Error Handling: Centralized exception handling and user-friendly error messages
•   Swagger Documentation: API endpoints documented with Swagger for easy testing and integration
•   Mistral:7B model for natural language processing tasks

Project Structure
•	Shared/Dto: Data Transfer Objects for communication between layers
•	Shared/Entities: Entity models for EF Core
•	Shared/Mappers: Mapping logic between entities and DTOs
•	Shared/Services: Business logic and data access services
•	AutoGen: AI/ML agents for bill analysis and data import
•	Predictor: Ml.NET regression model for prediction of future expenses
•	Controllers (if API): REST endpoints for frontend-backend communication

How It Works
1.	User Registration/Login: Users create an account and log in.
2.	Expense Entry: Users can manually add expenses or upload a bill image.
3.	Bill Processing: Uploaded images are processed via OCR; extracted data is analyzed and mapped to categories.
4.	Category Management: Users can view, add, or edit categories to suit their needs.
5.	Expense Review: Users can review, filter, and analyze their expenses with built-in reports.

	
Getting Started

Prerequisites
•	.NET 8 SDK
•	SQL Server (or compatible database)
•	Visual Studio 2022 or later
•   Node.js and npm (for Angular development)
•   D3.js (for data visualization)
•   Tessearact library for .NET (for OCR functionality)
•   Ollama to run the Mistral:7b model locally.

Setup
1.	Clone the repository
2.  Open the solution in Visual Studio
	1. Restore NuGet packages- Tesseract library for .NET, ML.NET, AutoGen, and other dependencies
	2. Update the connection string in appsettings.json to point to your SQL Server instance
	3. Run database migrations to create the necessary tables 
3.  Setup the  Angular frontend
	1. Navigate to the ClientApp directory
	2. Run npm install to install dependencies
	3. Build the Angular application using ng build
4.  Setup the Ollama Mistral:7b model
	1. Install Ollama from https://ollama.com/
	2. Pull the Mistral:7b model using the command ollama pull mistral:7b

5.  Run the application
	1. Start the backend server in Visual Studio
	2. Open a browser and navigate to http://localhost:5000 (or the configured port)
	3. Start the Angular frontend using ng serve in the ClientApp directory
	4. Access the Angular frontend at http://localhost:4200
