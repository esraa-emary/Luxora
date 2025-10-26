using Microsoft.AspNetCore.Http;

namespace Bookify.Helpers
{
    public static class SessionHelper
    {
        private const string UserIdKey = "UserId";
        private const string UserNameKey = "UserName";
        private const string UserEmailKey = "UserEmail";
        private const string IsAdminKey = "IsAdmin";

        public static void SetUserSession(ISession session, int userId, string userName, string email, bool isAdmin)
        {
            session.SetInt32(UserIdKey, userId);
            session.SetString(UserNameKey, userName);
            session.SetString(UserEmailKey, email);
            session.SetString(IsAdminKey, isAdmin.ToString());
        }

        public static int? GetUserId(ISession session)
        {
            return session.GetInt32(UserIdKey);
        }

        public static string? GetUserName(ISession session)
        {
            return session.GetString(UserNameKey);
        }

        public static string? GetUserEmail(ISession session)
        {
            return session.GetString(UserEmailKey);
        }

        public static bool IsAdmin(ISession session)
        {
            var isAdminValue = session.GetString(IsAdminKey);
            return isAdminValue != null && bool.Parse(isAdminValue);
        }

        public static bool IsLoggedIn(ISession session)
        {
            return session.GetInt32(UserIdKey).HasValue;
        }

        public static void ClearSession(ISession session)
        {
            session.Clear();
        }
    }
}




