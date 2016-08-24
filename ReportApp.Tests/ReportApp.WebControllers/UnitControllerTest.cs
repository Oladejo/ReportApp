using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReportApp.Core.Abstract;
using ReportApp.Core.Entities;
using ReportApp.Web.Controllers;

namespace ReportApp.Tests.ReportApp.WebControllers
{
    [TestClass]
    public class UnitControllerTest
    {
        //[TestMethod]
        //public void UnitIndexViewContainListOfUnit()
        //{
        //    //Arrange
        //    Mock<IUnit> mock = new Mock<IUnit>();

        //    mock.Setup(m => m.GetUnits()).Returns(new List<Unit>
        //    {
        //        new Unit { UnitId = 1, UnitName = "Sales"},
        //        new Unit { UnitId = 2, UnitName = "Marketer"} 
        //    }.AsEnumerable());

        //    UnitController units = new UnitController();

        //    //Act
        //    var model = units.Index() as ViewResult;
        //    var actual = (List<Unit>)model.Model;

        //    //Assert
        //    Assert.IsInstanceOf<List<Unit>>(actual);
        //    Assert.IsNotNull(actual);
        //    Assert.AreEqual(2, actual.Count()); //Failed test here
        //}
    }
}
