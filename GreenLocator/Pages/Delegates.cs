using GreenLocator.Models;

namespace GreenLocator.Pages
{
    public class Delegates
    {
        public delegate string StringToString(string x);
        public delegate int IntIntToInt(int x, int y);

        public delegate bool UserToBool(AspNetUser x);
        public static bool CheckUserInfo(UserToBool myDelegate, AspNetUser current)
        {
            return myDelegate(current);
        }
    }
}
