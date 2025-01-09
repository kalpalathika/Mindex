using CodeChallenge.Models;

/// <summary>
/// Represents the reporting structure for an employee.
/// </summary>
public class ReportingStructure {
    public Employee Employee {get;set;}
    public int NumberOfReports {get;set;}

    public ReportingStructure(Employee employee, int numberOfReports){
        {
            Employee = employee;
            NumberOfReports = numberOfReports;
        }

    }
}