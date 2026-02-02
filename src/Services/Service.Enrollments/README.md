# Service.Enrollments

The Service.Enrollments is a microservice that handles enrollment operations. It provides gRPC services for managing enrollments and communicates with other services via MassTransit messaging (e.g., dispatching enrollment events and receiving student or course deletions). It utilizes PostgreSQL for database storage and Redis for caching.
