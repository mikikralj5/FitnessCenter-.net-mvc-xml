using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using WebApplication1.Models;

namespace WebApplication1.HelperClasses
{
    public class WriteXML
    {
        public static void UsersWrite(List<User> users)
        {
            var path = HttpContext.Current.Server.MapPath(@"~/App_Data/users.xml");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (StreamWriter sw = File.AppendText(path))
            {
                new XmlSerializer(typeof(List<User>)).Serialize(sw, users);

            }
        }
    }
}