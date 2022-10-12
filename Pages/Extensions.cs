using GreenLocator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GreenLocator.Pages
{
    public static class Extensions
    {
        public static bool CheckIfUsrNull(this AspNetUser current)
        {
            return current == null;
        }

        public static bool CheckIfUsrFieldsNull(this AspNetUser current)
        {
            if (current.City == null)
                return true;

            if (current.Street == null)
                return true;

            if (current.House == null)
                return true;

            return false;
        }

        public static bool CheckIfUsrStatusNull(this AspNetUser current)
        {
            if (current.ShareStatus == null)
                return true;

            if (current.ThingToShare == null)
                return true;

            return false;
        }
    }
}
