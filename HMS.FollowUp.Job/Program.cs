using HMS.Entities.Models;
using HMS.FollowUp.Job.Implementations;
using System.Collections;
using System.ComponentModel.Design;
using System.Data;
using System.Net.Mail;

internal class Program
{
    private static readonly string LastRunFilePath = "lastrun.txt";

    static async Task Main(string[] args)
    {
        try
        {
            DateTime? lastRun = ReadLastRunTime();
            if (lastRun.HasValue)
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@Date", lastRun);
                var followUpData = dataAccessManager.GetDataSet("SP_GetFollowUpData", ht);
                DataTable followUpTable = followUpData.Tables[0];

                foreach (DataRow row in followUpTable.Rows)
                {
                    var reminderId = Convert.ToDecimal(row["ReminderId"]);
                    var mobileNo = row["MobileNo"].ToString();
                    var email = row["Email"] == DBNull.Value ? null : row["Email"].ToString();
                    var companyId = Convert.ToDecimal(row["CompanyId"]);
                    await SendRemindersAsync(reminderId, mobileNo, email, companyId);
                }
                Console.WriteLine($"Last run was at: {lastRun.Value}");
            }
            else
            {
                Console.WriteLine("This is the first run.");
            }
            WriteCurrentRunTime();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
        }
    }
    private static DateTime? ReadLastRunTime()
    {
        try
        {
            if (File.Exists(LastRunFilePath))
            {
                string content = File.ReadAllText(LastRunFilePath);
                if (DateTime.TryParse(content, out DateTime lastRun))
                {
                    return lastRun;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading last run time: {ex.Message}");
        }
        return null;
    }
    private static void WriteCurrentRunTime()
    {
        try
        {
            File.WriteAllText(LastRunFilePath, DateTime.Now.ToString("o")); // ISO 8601 format
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing last run time: {ex.Message}");
        }
    }
    public static async Task SendRemindersAsync(decimal? ReminderId, string mobileno, string email, decimal CompanyID)
    {
        var service = new SmsService();
        DataAccessManager dataAccessManager = new DataAccessManager();
        var ht = new Hashtable();
        ht.Add("@CompanyID", CompanyID);
        ht.Add("@ReminderId", ReminderId);
        var allData = dataAccessManager.GetDataSet("SP_GetReminderData", ht);
        var reminderTable = allData.Tables[0];
        var integrationTable = allData.Tables[1];
        bool isUnicode = false;
        if (reminderTable.Rows.Count > 0)
        {
            var smsRow = integrationTable.AsEnumerable()
       .FirstOrDefault(r => r["Type"].ToString() == "T" && Convert.ToBoolean(r["IsActive"]));
            var emailRow = integrationTable.AsEnumerable()
                .FirstOrDefault(r => r["Type"].ToString() == "E" && Convert.ToBoolean(r["IsActive"]));
            foreach (DataRow row in reminderTable.Rows)
            {
                int smsTypeId = Convert.ToInt32(row["SMSTypeId"]);
                string messageBody = row["MessageBody"].ToString();
                if (Convert.ToBoolean(row["IsUrdu"]))
                    isUnicode = true;
                if (smsTypeId == 1 && smsRow != null)
                {
                    await service.AuthenticateAsync(
                         smsRow["UserName"].ToString(),
                         smsRow["Password"].ToString(),
                         mobileno,
                         messageBody,
                         smsRow["Masking"].ToString(),
                         isUnicode
                     );
                }
                else if (emailRow != null && !string.IsNullOrEmpty(email))
                {
                    await SendEmailNotify(
                         emailRow["UserName"].ToString(),
                         emailRow["Password"].ToString(),
                         emailRow["SMTP"].ToString(),
                         Convert.ToInt32(emailRow["PortNo"]),
                         email,
                         messageBody
                     );
                }
            }
        }
    }
    private static async Task SendEmailNotify(string EmailFrom, string EmailPassword, string EmailSMTP, int? EmailPort, string toemail, string message)
    {
        MailMessage mail = new MailMessage();
        SmtpClient smtpC = new SmtpClient(EmailSMTP);
        smtpC.EnableSsl = true;
        smtpC.Port = Convert.ToInt32(EmailPort);
        smtpC.Credentials = new System.Net.NetworkCredential(EmailFrom, EmailPassword);
        mail.From = new MailAddress(EmailFrom);
        mail.To.Add(toemail);
        mail.Subject = "";
        mail.Body = message;

        try
        {
            //Send Email
            smtpC.Send(mail);
        }
        catch (Exception ex)
        {
        }
    }

}