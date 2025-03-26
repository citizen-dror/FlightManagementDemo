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
- **Notification Service** – Sends push notifications (runs as a Windows Service or on Kubernetes as a Background Job). (future)
