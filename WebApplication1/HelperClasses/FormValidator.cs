using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.HelperClasses
{
    public class FormValidator
    {
        public static bool IsEmpty(FormCollection fc)
        {

            foreach (var item in fc.Keys)
            {
                if (fc.Get((string)item).Trim() == string.Empty)
                {
                    return true;
                }
            }

            return false;
        }
    }
}