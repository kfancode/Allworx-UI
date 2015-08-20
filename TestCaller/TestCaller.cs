using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allworx.Lib;


namespace TestCaller
{
    [TestClass]
    public class TestCaller
    {
        [TestMethod]
        public void TestCallerPhoneNumber()
        {
            //  arrange
            string cPhoneNum = "(401) 433-3100";
            bool cReg = true;
            Caller myCaller = new Caller("(401) 433-3100", true);

            //  act


            //  assert
            Assert.AreEqual(myCaller.PhoneNumber, cPhoneNum);
        }

        [TestMethod]
        public void TestCallerRegistered()
        {
            //  arrange
            bool cReg = true;
            Caller myCaller = new Caller("(401) 433-3100", true);

            //  act


            //  assert
            Assert.AreEqual(myCaller.Registered, cReg);
        }
    }
}
