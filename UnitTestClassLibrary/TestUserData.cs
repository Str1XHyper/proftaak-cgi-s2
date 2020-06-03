using ClassLibrary.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestClassLibrary
{
    [TestClass]
    public class TestUserData
    {
        #region Constructor
        [TestMethod]
        public void TestUserData_Constructor_Happ()
        {
            //Arrange
            GetUserData userData;

            //Act
            userData = new GetUserData();

            //Assert
            Assert.AreEqual(typeof(GetUserData), userData.GetType());
        }
        #endregion

        #region RoleNameAuth
        [TestMethod] //Happy
        public void TestUserData_RoleNameAuth_Happy()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string authcode = "EZ4Str1XA2";

            //Act
            string rolNaam = userData.RoleNameAuth(authcode);

            //Assert
            Assert.AreEqual(rolNaam.ToLower(), "roostermaker");
        }

        [TestMethod] //Unhappy
        public void TestUserData_RolNameAuth_Null()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string authcode = null;

            //Act
            string rolNaam = userData.RoleNameAuth(authcode);

            //Assert
            Assert.AreEqual(rolNaam.ToLower(), "nietingelogd");
        }

        [TestMethod] //Unhappy
        public void TestUserData_RolNameAuth_Empty()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string authcode = string.Empty;

            //Act
            string rolNaam = userData.RoleNameAuth(authcode);

            //Assert
            Assert.AreEqual(rolNaam, "");
        }

        [TestMethod] //Unhappy
        public void TestUserData_RolNameAuth_NotFound()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string authcode = "42345";

            //Act
            string rolNaam = userData.RoleNameAuth(authcode);

            //Assert
            Assert.AreEqual(rolNaam, "");
        }
        #endregion

        #region RoleNameID
        [TestMethod] //Happy
        public void TestUserData_RoleNameID_Happy()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string userID = "1";

            //Act
            string rolNaam = userData.RoleNameID(userID);

            //Assert
            Assert.AreEqual(rolNaam.ToLower(), "roostermaker");
        }

        [TestMethod] //Unhappy
        public void TestUserData_RoleNameID_Null()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string userID = null;

            //Act
            string rolNaam = userData.RoleNameID(userID);

            //Assert
            Assert.AreEqual(rolNaam.ToLower(), "geenuserid");
        }

        [TestMethod] //Unhappy
        public void TestUserData_RoleNameID_NotFound()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string userID = "0";

            //Act
            string rolNaam = userData.RoleNameID(userID);

            //Assert
            Assert.AreEqual(rolNaam.ToLower(), "");
        }
        #endregion

        #region UserIDAuth
        [TestMethod] //Happy
        public void TestUserData_UserIDAuth_Happy()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string AuthCode = "EZ4Str1XA2";

            //Act
            string rolNaam = userData.UserIDAuth(AuthCode);

            //Assert
            Assert.AreEqual(rolNaam, "1");
        }

        [TestMethod] //Unhappy
        public void TestUserData_UserIDAuth_Null()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string AuthCode = null;

            //Act
            string rolNaam = userData.UserIDAuth(AuthCode);

            //Assert
            Assert.AreEqual(rolNaam, "geenAuthCode");
        }

        [TestMethod] //Unhappy
        public void TestUserData_UserIDAuth_NotFound()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string AuthCode = "asdfasdfasdf";

            //Act
            string rolNaam = userData.UserIDAuth(AuthCode);

            //Assert
            Assert.AreEqual(rolNaam, "");
        }

        [TestMethod] //Unhappy
        public void TestUserData_UserIDAuth_Empty()
        {
            //Arrange
            GetUserData userData = new GetUserData();
            string AuthCode = "";

            //Act
            string rolNaam = userData.UserIDAuth(AuthCode);

            //Assert
            Assert.AreEqual(rolNaam, "");
        }
        #endregion
    }

}