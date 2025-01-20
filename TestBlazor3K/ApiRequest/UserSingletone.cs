using TestBlazor3K.ApiRequest.Model;
using TestBlazor3K.ApiRequest;

namespace TestBlazor3K.ApiRequest
{
    public static class UserSingletone
    {
        public static CurUser us;

        public static void InitUser(CurUser user)
        {
            us = user;
        }

        public static CurUser GetUser()
        {
            return us;
        }

        public static void exitUser()
        {
            us = null;
        }
    }
}
