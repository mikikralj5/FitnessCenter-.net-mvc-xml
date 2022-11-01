using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using WebApplication1.Models;

namespace WebApplication1.HelperClasses
{
    public class ReadXML
    {
        public static List<User> UsersRead()
        {
            List<User> users = new List<User>();
            var path = HttpContext.Current.Server.MapPath(@"~/App_Data/users.xml");

            if (File.Exists(path))
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    var XML = new XmlSerializer(typeof(List<User>));
                    users = (List<User>)XML.Deserialize(stream);
                }
            }

            return users;
        }
    }
}