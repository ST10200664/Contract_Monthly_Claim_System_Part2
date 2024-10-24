# Contract_Monthly_Claim_System_Part2

# Overview
The **Contract Monthly Claim System (CMCS)** is a web application designed to manage and process monthly claims. Users such as lecturers, academic managers, and program coordinators can submit, review, and approve claims. This project is built using **ASP.NET Core MVC**.

---

# Prerequisites

Before running the application, make sure you have the following installed:

1. **.NET SDK 7.0 or later**  
   Download and install it from [here](https://dotnet.microsoft.com/download).

2. **Visual Studio 2022 or later**  
   Make sure the following workloads are installed:
   - ASP.NET and web development
   - .NET Core cross-platform development

3. **SQL Server**  
   Either install SQL Server locally or use **SQL Server Express**.

4. **Git**  
   If you don't have Git installed, download it from [here](https://git-scm.com/).

---

# Cloning the Repository

1. Open a terminal or command prompt.
2. Navigate to the directory where you want to clone the repository.
3. Run the following command to clone the repository:

    ```bash
    git clone https://github.com/YourUsername/CMCS-System.git
    ```

4. After cloning, navigate into the project directory:

    ```bash
    cd CMCS-System
    ```

---

# Setting Up the Application

# 1. Database Setup

The application uses **SQL Server** as the database. You can either configure a local SQL Server or use **Azure SQL**.

1. Update the Connection String:
   - Open the `appsettings.json` file located in the root directory of the project.
   - Update the `ConnectionStrings` section with your SQL Server credentials:

    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=your_server_name;Database=CMCS_DB;User Id=your_user;Password=your_password;"
    }
    ```

2. Run Migrations to Set Up the Database:
   - Open a terminal in the project folder or use the integrated terminal in Visual Studio.
   - Run the following command to apply the migrations and create the database:

    ```bash
    dotnet ef database update
    ```

# 2. Installing Dependencies

Before running the application, install the required dependencies by using the following command in the project folder:

```bash
dotnet restore
