
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

         [TestMethod]
        public async Task ReportingStructure_Returns_Ok()
        {
            var expectedResponse = new ReportingStructure(
                new Employee()
                {
                    EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                    FirstName = "Pete",
                    LastName = "Best",
                    Position = "Developer VI",
                    Department = "Engineering",
                    DirectReports = new List<Employee>()
                    {
                        new Employee()
                        {
                            EmployeeId = "62c1084e-6e34-4630-93fd-9153afb65309", 
                            FirstName = "Pete",
                            LastName = "Best",
                            Position = "Developer II",
                            Department = "Engineering",
                            DirectReports = new List<Employee>() 
                        },
                        new Employee()
                        {
                            EmployeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c", 
                            FirstName = "George",
                            LastName = "Harrison",
                            Position = "Developer III",
                            Department = "Engineering",
                            DirectReports = new List<Employee>() 
                        }
                    }
                },
                2 // Number of reports
            );

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/employee/reporting-structure/{expectedResponse.Employee.EmployeeId}");
            request.Headers.Add("Cache-Control", "no-cache");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Parse the response content into a JObject
            var jsonResponse = JObject.Parse(responseContent);

            // Extract the 'result' field and deserialize it into the ReportingStructure class
            var report = jsonResponse["result"].ToObject<ReportingStructure>();
            
            Assert.IsNotNull(report);
            Assert.IsNotNull(report.NumberOfReports);
            Assert.IsNotNull(report.Employee);
            
            // Assert that employee details match the expected response
            Assert.AreEqual(expectedResponse.Employee.FirstName, report.Employee.FirstName);
            Assert.AreEqual(expectedResponse.Employee.LastName, report.Employee.LastName);
            Assert.AreEqual(expectedResponse.Employee.Position, report.Employee.Position);
            Assert.AreEqual(expectedResponse.Employee.Department, report.Employee.Department);

            // Assert that direct reports match the expected ones
            Assert.AreEqual(expectedResponse.Employee.DirectReports[0].EmployeeId, report.Employee.DirectReports[0].EmployeeId);
            Assert.AreEqual(expectedResponse.Employee.DirectReports[1].EmployeeId, report.Employee.DirectReports[1].EmployeeId);
            
            // Assert that the number of reports is correct
            Assert.AreEqual(expectedResponse.NumberOfReports, report.NumberOfReports);
        }


        [TestMethod]
        public void ReportingStructure_Returns_NotFound()
        {
            string INVALID_ID = "0";

            var getRequestTask = _httpClient.GetAsync($"api/reporting-structure/{INVALID_ID}");
            var response = getRequestTask.Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
