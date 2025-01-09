using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using challenge.DTO;
using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
         private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
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
        public void CreateCompensation_Returns_Created()
        {
            var expected = new Dictionary<string, object>
            {
                { "FirstName", "John" },
                { "LastName", "Lennon" },
                { "Position", "Development Manager" },
                { "Department", "Engineering" },
                { "EffectiveDate", new DateTime() },
                { "Salary", 135000.00m }
            };

            var compensation = new CompensationDto()
            {
                EmployeeID = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                EffectiveDate = (DateTime)expected["EffectiveDate"],
                Salary = (decimal)expected["Salary"]
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
            new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.Employee);
            Assert.AreEqual(expected["FirstName"], newCompensation.Employee.FirstName);
            Assert.AreEqual(expected["LastName"], newCompensation.Employee.LastName);
            Assert.AreEqual(expected["Department"], newCompensation.Employee.Department);
            Assert.AreEqual(expected["Position"], newCompensation.Employee.Position);
            Assert.AreEqual(expected["EffectiveDate"], newCompensation.EffectiveDate);
            Assert.AreEqual(expected["Salary"], newCompensation.Salary);
        }
        
        [TestMethod]
        public void CreateCompensation_Returns_NotFound()
        {
            // Arrange
            var compensation = new CompensationDto()
            {
                EmployeeID = "Invalid_ID",
                EffectiveDate = new DateTime(),
                Salary = 35000.00m
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        
        [TestMethod]
        public void GetCompensationById_Returns_Ok()
        {
            var expected = new Dictionary<string, object>
            {
                { "EmployeeID", "16a596ae-edd3-4847-99fe-c4518e82c86f" },
                { "FirstName", "John" },
                { "LastName", "Lennon" },
                { "Position", "Development Manager" },
                { "Department", "Engineering" },
                { "EffectiveDate", new DateTime() },
                { "Salary", 135000.00m }
            };

            var compensation = new CompensationDto()
            {
                EmployeeID = (string)expected["EmployeeID"],
                EffectiveDate = (DateTime)expected["EffectiveDate"],
                Salary = (decimal)expected["Salary"]
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            _httpClient.PostAsync("api/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json")).Wait();

            var getRequestTask = _httpClient.GetAsync($"api/compensation/{expected["EmployeeID"]}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();

            Assert.IsNotNull(newCompensation);
            Assert.IsNotNull(newCompensation.Employee);
            Assert.IsNotNull(newCompensation.Salary);
            Assert.IsNotNull(newCompensation.EffectiveDate);

            // Employee Relation
            Assert.AreEqual(expected["FirstName"], newCompensation.Employee.FirstName);
            Assert.AreEqual(expected["LastName"], newCompensation.Employee.LastName);
            Assert.AreEqual(expected["Department"], newCompensation.Employee.Department);
            Assert.AreEqual(expected["Position"], newCompensation.Employee.Position);

            // Compensation Details
            Assert.AreEqual(expected["EffectiveDate"], newCompensation.EffectiveDate);
            Assert.AreEqual(expected["Salary"], newCompensation.Salary);

        }
        
        [TestMethod]
        public void CompensationById_Returns_NotFound()
        {
            // Arrange
            var invalidID = "test";

            var getRequestTask = _httpClient.GetAsync($"api/compensation/{invalidID}");
            var response = getRequestTask.Result;
        
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
