# Mindex Coding Challenge

## Running the Project
To run the project, execute `dotnet run` on the command line or through Visual Studio Community Edition. This starts the web application using an in-memory database that is bootstrapped with data at startup.

## Architectural Decisions

### Reporting Structure in Employee Controller
I placed the `ReportingStructure` within the `EmployeeController` because it's closely tied to employee details. This integration streamlines our API by grouping related functionalities, making it easier to manage and maintain the system as it grows.

### Separate Repository and Controller for Compensation
I set up a separate repository and controller for `Compensation` to handle its specific needs, especially since it deals with sensitive financial information. This separation helps secure and manage compensation data more effectively, aligning with the Single Responsibility Principle for better scalability and easier updates.

## Observations
- **Performance**: The in-memory database provides fast access but is limited by its non-persistent nature.
- **Usability**: Clear and straightforward REST API endpoints make it easy to test and integrate with various front-end applications.

## Challenges
- **Asynchronous Operations and Lazy Loading**: The default lazy loading posed challenges, particularly with recursive operations needed to compute total reports for an employee. Asynchronous operations delayed data availability, impacting accuracy. I transitioned to explicit loading to ensure timely access to necessary data, improving the precision of reporting structures.

## Future Scope
- **Database Migration**: Transition to a persistent storage solution like SQL Server or MongoDB.
- **Feature Expansion**: Additional features such as authentication, advanced querying capabilities, and analytics.
- **UI Development**: A dynamic front-end interface to interact with the backend more effectively.

## Making it Production-Ready
To prepare for production:
- **Persistent Storage**: Implement a robust database system.
- **Security Enhancements**: Add HTTPS, authentication, and authorization.
- **Performance Optimization**: Identify and optimize inefficient code paths.

## REST API Overview
### Employee Management
- **POST `/api/employee`**: Create a new employee record.
- **GET `/api/employee/{id}`**: Retrieve an employee by ID.
- **PUT `/api/employee/{id}`**: Update an employee record.
- **GET `/api/[controller]/reporting-structure/{id}`**: Fetches the reporting structure for a specific employee by ID. It calculates the total number of direct and indirect reports and returns the structured data. If the employee ID does not exist, it returns a `NotFound` error with a message.

### Compensation Management
- **POST `/api/compensation`**: Creates a compensation record for an employee. Validates the request, checks for existing records on the same date to avoid duplicates, and creates a new record if none exist.
- **GET `/api/compensation/{employeeId}`**: Retrieves compensation details for a specific employee by ID. If no record exists, it returns a `NotFound` response.

Each `Employee` object follows the specified JSON schema, and compensation details include fields like salary and effective date, structured in a similar JSON format.

## Testing Structure

### Reporting Structure Tests
- **Hierarchical Accuracy**: Tests ensure the API accurately computes and returns the total number of direct reports for specified employees, confirming correct responses (`Returns_Ok`).
- **Error Handling**: Assesses the system's response to invalid employee requests, verifying proper error management with a `NotFound` status.

### Compensation Controller Tests
- **Creation Validation**: Tests confirm the successful creation of compensation entries, ensuring all details like salary and effective date are correctly recorded (`Returns_Created`).
- **Retrieval Accuracy**: Ensures accurate retrieval of compensation records by employee ID, with complete detail verification (`Returns_Ok`).
- **Error Handling**: Verifies the system's response to invalid employee IDs with a `NotFound` status, demonstrating robust error management.


# About Me
I am Kalpalathika Ramanujam, a dedicated Software Engineer with over three years of industry experience specializing in developing scalable full-stack applications using C#, .NET, and Angular. Currently, I am pursuing my Master’s in Computer Science at Rochester Institue of Technology. As a skilled problem solver, I am eager to apply my technical expertise and innovative approach in a dynamic new role.










<!-- # Mindex Coding Challenge
## What's Provided
A simple [.Net 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) web application has been created and bootstrapped 
with data. The application contains information about all employees at a company. On application start-up, an in-memory 
database is bootstrapped with a serialized snapshot of the database. While the application runs, the data may be
accessed and mutated in the database without impacting the snapshot.

### How to Run
You can run this by executing `dotnet run` on the command line or in [Visual Studio Community Edition](https://www.visualstudio.com/downloads/).


### How to Use
The following endpoints are available to use:
```
* CREATE
    * HTTP Method: POST 
    * URL: localhost:8080/api/employee
    * PAYLOAD: Employee
    * RESPONSE: Employee
* READ
    * HTTP Method: GET 
    * URL: localhost:8080/api/employee/{id}
    * RESPONSE: Employee
* UPDATE
    * HTTP Method: PUT 
    * URL: localhost:8080/api/employee/{id}
    * PAYLOAD: Employee
    * RESPONSE: Employee
```
The Employee has a JSON schema of:
```json
{
  "type":"Employee",
  "properties": {
    "employeeId": {
      "type": "string"
    },
    "firstName": {
      "type": "string"
    },
    "lastName": {
          "type": "string"
    },
    "position": {
          "type": "string"
    },
    "department": {
          "type": "string"
    },
    "directReports": {
      "type": "array",
      "items" : "string"
    }
  }
}
```
For all endpoints that require an "id" in the URL, this is the "employeeId" field.

## What to Implement
Clone or download the repository, do not fork it.

### Task 1
Create a new type, ReportingStructure, that has two properties: employee and numberOfReports.

For the field "numberOfReports", this should equal the total number of reports under a given employee. The number of 
reports is determined to be the number of directReports for an employee and all of their direct reports. For example, 
given the following employee structure:
```
                    John Lennon
                /               \
         Paul McCartney         Ringo Starr
                               /        \
                          Pete Best     George Harrison
```
The numberOfReports for employee John Lennon (employeeId: 16a596ae-edd3-4847-99fe-c4518e82c86f) would be equal to 4. 

This new type should have a new REST endpoint created for it. This new endpoint should accept an employeeId and return 
the fully filled out ReportingStructure for the specified employeeId. The values should be computed on the fly and will 
not be persisted.

### Task 2
Create a new type, Compensation. A Compensation has the following fields: employee, salary, and effectiveDate. Create 
two new Compensation REST endpoints. One to create and one to read by employeeId. These should persist and query the 
Compensation from the persistence layer.

## Delivery
Please upload your results to a publicly accessible Git repo. Free ones are provided by Github and Bitbucket. -->