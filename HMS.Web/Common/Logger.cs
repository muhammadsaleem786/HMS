using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMS.Web.API
{
    public static class Logger
    {
        public static log4net.ILog Trace { get; set; }
    }
}