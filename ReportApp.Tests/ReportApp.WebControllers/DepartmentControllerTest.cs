using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Moq;
using ReportApp.Web.Controllers;
using NUnit.Framework;
using ReportApp.Core.Abstract;
using ReportApp.Core.Entities;

namespace ReportApp.Tests.ReportApp.WebControllers
{
    [TestFixture]
    public class DepartmentControllerTest
    {
        [Test]
        public void DepartmentIndexViewContainListOfDepartment()
        {
            //Arrange
            Mock<IDepartment> mock = new Mock<IDepartment>();

            mock.Setup(m => m.GetDepartments()).Returns(new Department[]
            {
                new Department { DepartmentId = 1, DepartmentName = "Account"},
                new Department { DepartmentId = 2, DepartmentName = "Sales"} 
            }.AsEnumerable());

            DepartmentsController departments = new DepartmentsController(mock.Object);

            //Act
            var actual = (List<Department>) departments.Index().Model;

            //Assert
            Assert.IsInstanceOf<List<Department>>(actual);
        }
    }
}
