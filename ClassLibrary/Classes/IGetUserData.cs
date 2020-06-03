namespace ClassLibrary.Classes
{
    public interface IGetUserData
    {
        string RoleNameAuth(string authcode);
        string RoleNameID(string userID);
        string UserIDAuth(string authcode); 
    }
}