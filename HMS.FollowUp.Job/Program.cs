using HMS.Entities.Models;
using HMS.FollowUp.Job.Implementations;
using System.Collections;
using System.ComponentModel.Design;
using System.Data;
using System.Net.Mail;
using System.Text;

internal class Program
{
    private static readonly string LogFilePath = "log.txt";
    private static StringBuilder LogBuffer = new StringBuilder();
    static async Task Main(string[] args)
    {
        try
        {
            LogMessage("Job started.");
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
                    var mobileNo = row["Mobile"].ToString();
                    var email = row["Email"] == DBNull.Value ? null : row["Email"].ToString();
                    var companyId = Convert.ToDecimal(row["CompanyId"]);
                    await SendRemindersAsync(reminderId, mobileNo, email, companyId);
                }
                LogMessage("Job completed successfully.");
            }
            else
            {
                LogMessage("This is the first run.");
            }
        }
        catch (Exception ex)
        {
            LogMessage("Job failed with exception", ex);
        }
        finally
        {
            WriteLogToFile(); // write everything once
        }
    }
    private static DateTime? ReadLastRunTime()
    {
        try
        {
            if (File.Exists(LogFilePath))
            {
                var lines = File.ReadAllLines(LogFilePath).Reverse();
                foreach (var line in lines)
                {
                    if (line.StartsWith("[LAST_RUN_TIME:"))
                    {
                        var dateStr = line.Replace("[LAST_RUN_TIME:", "").TrimEnd(']');
                        if (DateTime.TryParse(dateStr, out DateTime lastRun))
                            return lastRun;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage("Error reading last run time", ex);
        }
        return null;
    }
    private static void LogMessage(string message, Exception ex = null)
    {
        LogBuffer.AppendLine("--------------------------------------------------");
        LogBuffer.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        LogBuffer.AppendLine($"Message: {message}");
        if (ex != null)
        {
            LogBuffer.AppendLine($"Error: {ex.Message}");
            LogBuffer.AppendLine($"StackTrace: {ex.StackTrace}");
        }
        LogBuffer.AppendLine("--------------------------------------------------");
    }
    private static void WriteLogToFile()
    {
        try
        {
            LogBuffer.AppendLine($"[LAST_RUN_TIME:{DateTime.UtcNow:o}]"); // ISO format
            File.WriteAllText(LogFilePath, LogBuffer.ToString()); // Overwrite the file
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to write log: {ex.Message}");
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