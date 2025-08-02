using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSyncService.DataAccess
{
    public class DataAccess
    {

        public static string Tokenkey = ConfigurationManager.AppSettings["Tokenkey"];


    }
}
