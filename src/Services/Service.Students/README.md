# Service.Students

The Service.Students is a microservice dedicated to student management. It offers gRPC endpoints for student-related operations. The service integrates with PostgreSQL database, Redis for caching, and MassTransit for messaging to ensure data consistency across the system.

## gRPC Endpoints

- **GetStudentById**: Retrieves a student by ID.
- **ListStudents**: Lists students with pagination.
- **CreateStudent**: Creates a new student.
- **DeleteStudent**: Deletes a student (only if no active enrollments).

## Broker Usage (RabbitMQ via MassTransit)

### Publishes
- **StudentDeletedEvent**: Published when a student is successfully deleted.

### Consumes
- **IncreaseStudentEnrollmentsCountEvent**: Increases the enrollments count for a student.
- **DecreaseStudentEnrollmentsCountEvent**: Decreases the enrollments count for a student.

Upon consuming enrollment count events, it publishes corresponding success or failure events.
