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
using CollectionAssert = Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert;

namespace ReportApp.Tests.ReportApp.WebControllers
{
    [TestClass]
    public class DepartmentControllerTest
    {
        private Mock<IDepartment> _mock;
        private DepartmentsController _departments;

        [TestInitialize]
        public void Initialize()
        {
            _mock = new Mock<IDepartment>();
            _mock.Setup(m => m.GetDepartments()).Returns(new List<Department>
            {
                new Department { DepartmentId = 1, DepartmentName = "Account"},
                new Department { DepartmentId = 2, DepartmentName = "Sales"} 
            }.AsEnumerable());

           _departments = new DepartmentsController(_mock.Object);
        }
        

        [TestMethod]
        public void DepartmentIndexViewContainListOfDepartment()
        {
            //Arrange
            //mock.Setup(m => m.GetDepartments()).Returns(new List<Department>
            //{
            //    new Department { DepartmentId = 1, DepartmentName = "Account"},
            //    new Department { DepartmentId = 2, DepartmentName = "Sales"} 
            //}.AsEnumerable());

            //DepartmentsController departments = new DepartmentsController(mock.Object);

            //Act
            var model = _departments.Index() as ViewResult;
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
            //mock.Setup(m => m.GetDepartments()).Returns(new List<Department>
            //{
            //    new Department { DepartmentId = 1, DepartmentName = "Account"},
            //    new Department { DepartmentId = 2, DepartmentName = "Sales"} 
            //}.AsEnumerable());

            //DepartmentsController departments = new DepartmentsController(mock.Object);

            //Act
            var model = _departments.Details(2);
            var result = model.ToString();

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DepartmentExist()
        {
            //Arrange
            _mock.Setup(m => m.GetDepartmentById(It.IsAny<int>()));

            //Act
            var model = _mock.Object.GetDepartmentById(1);

            //Assert
            Assert.IsNotNull(model);

            // Lets call the action method now
            //ViewResult result = _departments.Details(1) as ViewResult;

            //var model = _mock.Object.GetDepartmentById(1);

            //Assert.AreEqual(result.Model, model);
        }

        [TestMethod]
        public void TestCreate()
        {
            var dept = new Department { DepartmentId = 1, DepartmentName = "Account" };
            var result = _departments.Create(dept);

            List<Department> departments = (List<Department>)_mock.Object.GetDepartments();

            CollectionAssert.Contains(departments, result);
        }

       
    }
}
