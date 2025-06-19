using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AttendanceSyncService.ZKT_Device.ZKT_Service
{

    public class ZKTService
    {
        public static decimal CompID = Convert.ToInt64(ConfigurationManager.AppSettings["CompanyID"]);
        public static string LoctID = ConfigurationManager.AppSettings["LocationCode"];
        public static string Tokenkey = ConfigurationManager.AppSettings["Tokenkey"];       

        public ZkemClient objCZKEM;
        private int PortNumber;
        private DateTime LastDataSync;
        private string IPAddress, LocationCode, Password;
        private DataTable AttMachine = new DataTable();
        private decimal CompanyID, DeviceID;

        public async void GetAttendanceFromMachine()
        {
            try
            {
                 var geturl = "http://localhost:8081/api/AttendanceSync/GetAllAttendanceMachine?CompanyID=" + CompID + "&tokenKey=" + Tokenkey + "&LocationCode=" + LoctID +"";
              
                List<Attendance> obj2 =GetDeviceData(geturl).Select(z=>z).ToList();                
                AttMachine = ToDataTable<Attendance>(obj2);
                if (AttMachine == null) return;
                Console.WriteLine("Found No# Of device is " + AttMachine.Rows.Count);
                Console.WriteLine();
                bool IsResult = false;
                int iMachineNumber, idwInOutMode, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond, idwVerifyMode, idwWorkcode;
                string idwEmployeeCode = "", StringDate = "";
                var url = "";
                var inputJson = "";
                DataTable DTAttendance = new DataTable();
                DateTime TDate, AttendanceDate = DateTime.Now;
                List<AttendanceModel> AttendanceList = new List<AttendanceModel>();
                foreach (DataRow dr in AttMachine.Rows)
                {
                    try
                    {
                        AttendanceList = new List<AttendanceModel>();
                        DeviceID = 0;
                        CompanyID = Convert.ToDecimal(dr["CompanyID"]);
                        LocationCode = dr["LocationCode"].ToString();
                        IPAddress = dr["IPAddress"].ToString();
                        Password = dr["Password"].ToString();
                        PortNumber = Convert.ToInt32(dr["PortNo"]);

                        if (dr["LastDataSync"] == DBNull.Value)
                            LastDataSync = Convert.ToDateTime("01/01/1900");
                        else
                            LastDataSync = Convert.ToDateTime(dr["LastDataSync"]);
                        if (IsAttendanceMachineConnected(CompanyID, LocationCode, DeviceID, Convert.ToString(dr["Password"]), LastDataSync) == false) continue;

                        iMachineNumber = 1; idwWorkcode = 0;
                        IsResult = objCZKEM.EnableDevice(iMachineNumber, false);
                        if (IsResult == false) ErrorShow("Device Disable ");
                        IsResult = objCZKEM.ReadAllGLogData(iMachineNumber);

                        if (IsResult)//read all the attendance records to the memory
                        {
                            while (objCZKEM.SSR_GetGeneralLogData(iMachineNumber, out idwEmployeeCode, out idwVerifyMode,
                                       out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                            {
                                StringDate = idwDay.ToString() + "/" + idwMonth.ToString() + "/" + idwYear.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond;
                                TDate = DateTime.ParseExact(StringDate, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture);

                                if (TDate.Date >= LastDataSync.Date && TDate.Date <= AttendanceDate.Date)
                                {
                                    AttendanceModel obj = new AttendanceModel();
                                    obj.CompanyID = CompanyID;
                                    obj.LocationCode = LocationCode;
                                    obj.EmployeeCode = idwEmployeeCode;
                                    obj.AttendanceMode = (short)idwInOutMode;
                                    obj.AttendanceTime = JsonConvert.SerializeObject(TDate);
                                    AttendanceList.Add(obj);
                                }
                            }
                        }
                        else
                            ErrorShow("Read All Log Data ");

                        IsResult = objCZKEM.EnableDevice(iMachineNumber, true);
                        if (IsResult == false) ErrorShow("Device Enable ");

                        if (AttendanceList.Count() == 0) continue;
                        DTAttendance = ToDataTable<AttendanceModel>(AttendanceList);

                        inputJson = (new JavaScriptSerializer()).Serialize(AttendanceList).Replace("\\\"", "");
                        Library.WriteErrorLog("Json Data:" + inputJson);

                         url = $"http://localhost:8081/api/AttendanceSync/DatabaseSyncProcess?CompanyID={CompanyID}&DeviceID={DeviceID}&LocationCode={LocationCode}&Tokenkey={Tokenkey}";

                        await PostData(url, inputJson);
                        Library.WriteErrorLog("insert sucessfully Machine Data");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static List<Attendance> GetDeviceData(string url)
        {
            try
            {
                List<Attendance> obj1 = new List<Attendance>();
                string html = string.Empty;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                WebResponse response1 = request.GetResponse();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                 obj1 = JsonConvert.DeserializeObject<List<Attendance>>(reader.ReadToEnd());

                    html = reader.ReadToEnd();
                }
                Library.WriteErrorLog("Sussessfully Get Device Data");
                return obj1;
            }
            catch (Exception ex)
            {
                throw ex;
                Library.WriteErrorLog("Error Get Device Data");
            }
        }
        static async Task<string> PostData(string url, string json)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string jsson = json;
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var resultt = streamReader.ReadToEnd();
                }               
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
                Library.WriteErrorLog(ex);
            }
        }
        private bool IsAttendanceMachineConnected(decimal CompanyID, string LocationCode, decimal DeviceID, string CommPassword, DateTime LastDataSync)
        {
            objCZKEM = new ZkemClient(RaiseDeviceEvent);
            bool bIsCommPassword = false;
            if (CommPassword != "")
                bIsCommPassword = objCZKEM.SetCommPassword(Convert.ToInt32(CommPassword));

            bool bIsConnected = objCZKEM.Connect_Net(IPAddress, PortNumber);

            //objCZKEM.Connect_Net(IPAddress, PortNumber);
            //bool bIsConnected = objCZKEM.SetCommPassword(2370);
            Console.WriteLine("");
            Console.WriteLine("Device Info");
            Console.WriteLine("IP Address : " + IPAddress);
            Console.WriteLine("Port No : " + PortNumber.ToString());
            Console.WriteLine("Sync Date Start : " + LastDataSync.ToString("MMMM dd,yyyy"));
            Console.WriteLine("Comm Password Status : " + (CommPassword == "" ? "Password Ignore" : (bIsCommPassword ? "Correct" : "Not Correct")));
            Console.WriteLine("Connection Status : " + (bIsConnected ? "Ok" : "Error"));

            if (bIsConnected == false)
            {
                Library.WriteErrorLog("Zkt not connected.");
                ErrorShow("Connection Status ");
                //DataAccess.DataAccess.DeviceFailureProcess(CompanyID, LocationID, DeviceID);
            }
            if (bIsConnected) return true;
            Library.WriteErrorLog("Zkt connected.");
            //EventLog.WriteEntry("Attendance Machine is not connected.");

            return bIsConnected;
        }

        private void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                        Console.WriteLine("The device is switched off");
                        break;
                    }

                default:
                    break;
            }
        }

        private void ErrorShow(string MSG)
        {
            int ErrorCode = 0;
            objCZKEM.GetLastError(ref ErrorCode);
            Console.WriteLine(MSG + "Error Code = " + ErrorCode.ToString());
        }

        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
