using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMS.Web.API.Common
{
    public class MessageStatement
    {
        public static string Save { get { return "Record has been saved successfully"; } }
        public static string Update { get { return "Record has been updated successfully"; } }
        public static string Delete { get { return "Record has been deleted successfully"; } }
        public static string Return { get { return "Refund should be less then the amount"; } }
        public static string Post { get { return "Record has been posted successfully"; } }
        public static string Approved { get { return "Record has been Approved successfully"; } }
        public static string Rejected { get { return "Record has been Rejected successfully"; } }
        public static string RelationExists { get { return "You can't Delete a record because a related record is exists."; } }
        public static string NotFound { get { return "Record not found"; } }
        public static string BadRequest { get { return "Bad request"; } }
        public static string Conflict { get { return "Conflict, Record already exists."; } }
        public static string Processed { get { return "Your selected action is completed."; } }
        public static string Improted { get { return "Items have been imported successfully."; } }

    }
}