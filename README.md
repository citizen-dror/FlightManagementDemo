# Flight Management System in .NET Core 8

This is a microservice-based backend demo built in **.NET Core 8**, utilizing **SQL Server** and **RabbitMQ** for communication. The system is designed to manage flight-related events, particularly handling price alerts and sending push notifications to users' mobile devices.

## Components Implemented

- **Alert Service**: Manages the logic for processing and handling price alerts.
- **Infrastructure**: Provides the necessary DB access and repositories.
- **Domain**: Defines the entities and interfaces that the system interacts with.
- **Common**: Contains shared DTOs (Data Transfer Objects) and utility functions.
- **Unit Tests**: Ensures the correctness of the system with some basic input validation.

## Technologies Used

- **.NET Core 8**
- **SQL Server**
- **RabbitMQ**

The system integrates **RabbitMQ** for message-driven communication, ensuring that alerts and notifications are efficiently processed and sent in real-time.


# Solution & Project Structure

## Single Solution with Multiple Projects (Monorepo Approach)

A single .NET Core 8 solution containing multiple microservices as separate projects:
FlightManagementSystem.sln  
│── FlightManagement.Api (API Gateway)  
│── FlightManagement.AlertService (Manages Alerts)  
│── FlightManagement.PriceAggregator (Fetches Prices)  
│── FlightManagement.NotificationService (Sends Notifications)  
│── FlightManagement.Domain (Entities, Interfaces)  
│── FlightManagement.Infrastructure (Repositories, DB Access)  
│── FlightManagement.Common (Shared DTOs, Utilities)  
│── FlightManagement.Tests (Unit & Integration Tests)

### Each Service Runs as a Separate API:

- **API Gateway** – Routes requests (runs on IIS or Kestrel with Load Balancer). (future)
- **Alert Service** – Manages CRUD operations for alerts (runs on IIS). (done)
- **Price Aggregator** – Calls airline APIs & processes prices (runs as a Windows Service or Background Worker). (future)
- **Notification Service** – Sends push notifications (runs as a Windows Service or on Kubernetes as a Background Job). (future)|

# Service Deployment (IIS vs. Windows Service)

| Service               | Runs on                          | Why?                                      |
|-----------------------|--------------------------------|-------------------------------------------|
| **API Gateway**       | IIS / Load Balancer            | Handles external traffic & security      |
| **Alert Service**     | IIS                            | CRUD operations, scalable as API         |
| **Price Aggregator**  | Windows Service / Kubernetes Job | Polls external APIs periodically        |
| **Notification Service** | Windows Service / Kubernetes Job | Needs event-driven processing          |
| **Database**         | SQL Server (on RDS/Azure SQL)  | Centralized storage                      |

## Architecture Diagram, Data Flow 
see: 

flight_price_demo-architecture_future.mermaid
this is the future architecture

flight_price_demo-data_flow.mermaid
this is the currenct data flow 


## How to Install and Run the Project

1. **Create the Database and Login**
   - Run the script: `init-create_db_and_login.sql`

2. **Create the Schema**
   - Run the migration script see : `init-run_migration_db_sql.txt`

3. **Insert Initial Values into the Database (e.g., Users)**
   - Run the script: `init-insert_db_values.sql`

4. **Initialize RabbitMQ using Docker**
   - Follow the instructions in `init-rabbit.txt` to set up RabbitMQ.

5. **Run the Server**
   - Start the backend server to launch the application.

6. **Use Curl to Add Price Alerts**
   - Execute the appropriate `curl` commands to add price alerts.
   - See curl_requets.txt

7. **Verify RabbitMQ Info**
   1. Run the server if it is not running.
   2. Use `curl` to send a POST request to check alerts.
   3. If a new price is lower than the existing price alert for the same origin and destination, you should see a new events in the `FlightsNotificationQueue` in RabbitMQ


