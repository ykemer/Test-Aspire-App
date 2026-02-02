# Platform Service

The Platform service is the central web API service in the microservices architecture. It provides RESTful endpoints using FastEndpoints, handles authentication and authorization, and includes real-time communication via SignalR hubs for enrollments, courses, and classes. It integrates with PostgreSQL database, Redis for caching, and uses MassTransit for messaging.
