using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReportApp.Web.Controllers;
using NUnit.Framework;
using ReportApp.Core.Abstract;
using ReportApp.Core.Entities;
using Assert = NUnit.Framework.Assert;

namespace ReportApp.Tests.ReportApp.WebControllers
{
    [TestClass]
    public class DepartmentControllerTest
    {
        [TestMethod]
        public void DepartmentIndexViewContainListOfDepartment()
        {
            //Arrange
            Mock<IDepartment> mock = new Mock<IDepartment>();

            mock.Setup(m => m.GetDepartments()).Returns(new List<Department>
            {
                new Department { DepartmentId = 1, DepartmentName = "Account"},
                new Department { DepartmentId = 2, DepartmentName = "Sales"} 
            }.AsEnumerable());

            DepartmentsController departments = new DepartmentsController(mock.Object);

            //Act
            var model = departments.Index() as ViewResult;
            var actual = (List<Department>) model.Model;

            //Assert
            Assert.IsInstanceOf<List<Department>>(actual);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count()); //Failed test here
        }

        [TestMethod]
        public void DepartmentDetailsViewById()
        {
            //Arrange
            Mock<IDepartment> mock = new Mock<IDepartment>();

            mock.Setup(m => m.GetDepartments()).Returns(new List<Department>
            {
                new Department { DepartmentId = 1, DepartmentName = "Account"},
                new Department { DepartmentId = 2, DepartmentName = "Sales"} 
            }.AsEnumerable());

            DepartmentsController departments = new DepartmentsController(mock.Object);

            //Act
            var model = departments.Details(2);
            var result = model.ToString();

            //Assert
            Assert.IsNotNull(result);
        }
    }
}
