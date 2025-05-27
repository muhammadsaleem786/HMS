using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System.Threading.Tasks;
using Repository.Pattern.Ef6;
using Repository.Pattern.DataContext;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Hosting;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.end;
using HMS.Entities.Models;
using HMS.Entities.CustomModel;
using iTextSharp.tool.xml.css;
using System.Net.Http;

namespace HMS.Web.API.Common
{
    public class EmailService : IDisposable
    {
        int EmailPort;
        decimal sys_notificationID;
        string webUrl, EmailFrom, EmailDisplayName;
        string EmailUserName, EmailPassword, EmailSMTP;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public EmailService()
        {
            sys_notificationID = 1;
            webUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"];
            EmailFrom = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"];
            EmailDisplayName = System.Configuration.ConfigurationManager.AppSettings["EmailDisplayName"];

            EmailUserName = System.Configuration.ConfigurationManager.AppSettings["EmailUserName"];
            EmailPassword = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"];
            EmailSMTP = System.Configuration.ConfigurationManager.AppSettings["EmailSMTP"];
            EmailPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["EmailPort"]);
            IDataContextAsync context = new HMSContext();
            _unitOfWorkAsync = new UnitOfWork(context);
        }

        public void SendEmailCofirm(string Path, string Email, string Name, decimal CompanyID, string EncryptionToken, decimal SenderID, string Subject)
        {
            Task.Run(() => SendEmailConfirm(Path, Email, Name, CompanyID, EncryptionToken, SenderID, Subject));
        }
        private void SendEmailConfirm(string Path, string Email, string Name, decimal CompanyID, string EncryptionToken, decimal SenderID, string Subject)
        {
            decimal sys_notificationID = 1;
            sys_notification_alert sys_notification = new sys_notification_alert();

            if (_unitOfWorkAsync.Repository<sys_notification_alert>().Queryable().Count() > 0)
                sys_notificationID = _unitOfWorkAsync.Repository<sys_notification_alert>().Queryable().Max(e => e.ID) + 1;


            string webUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"];
            string EmailFrom = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"];
            string EmailDisplayName = System.Configuration.ConfigurationManager.AppSettings["EmailDisplayName"];

            string EmailUserName = System.Configuration.ConfigurationManager.AppSettings["EmailUserName"];
            string EmailPassword = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"];
            string EmailSMTP = System.Configuration.ConfigurationManager.AppSettings["EmailSMTP"];
            int EmailPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["EmailPort"]);


            sys_notification.ID = sys_notificationID;
            sys_notification.CompanyID = CompanyID;
            sys_notification.TypeID = 2;
            sys_notification.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"];
            sys_notification.EmailTo = Email;
            sys_notification.Subject = Subject + ' ' + webUrl;
            string link1 = webUrl + "/#/resetpassword?" + EncryptionToken;
            sys_notification.Body = ResolveEmailConfirm(Name, Path, link1);
            sys_notification.CreatedBy = Convert.ToInt32(SenderID);
            sys_notification.CreatedDate = DateTime.Now;
            sys_notification.ObjectState = ObjectState.Added;

            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;

            //From address to send email
            if (string.IsNullOrEmpty(EmailDisplayName))
                message.From = new MailAddress(EmailFrom);
            else
                message.From = new MailAddress(EmailFrom, EmailDisplayName);

            //To address to send email
            message.To.Add(new MailAddress(Email));
            message.Subject = sys_notification.Subject;
            message.Body = sys_notification.Body;

            using (var client = new System.Net.Mail.SmtpClient(EmailSMTP, EmailPort))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(EmailUserName, EmailPassword);

                // Enable SSL encryption
                client.EnableSsl = true;

                // Try to send the message. Show status in console.
                try
                {
                    client.Send(message);
                    sys_notification.SentTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    sys_notification.FailureCount = 1;
                    sys_notification.SentTime = null;
                }
            }

            //_sys_notification_alertService.Insert(sys_notification);
            _unitOfWorkAsync.Repository<sys_notification_alert>().Insert(sys_notification);
            try
            {
                _unitOfWorkAsync.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Trace.Error(ex);
            }
            finally
            {
                Dispose();
            }
        }

        private string GetEmailTemplate(string Path, string Name)
        {
            //string path = HttpContext.Current.Server.MapPath("");
            string template = File.ReadAllText(Path);
            string webUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"];

            if (template.Contains("[[UserName]]"))
            {
                template = template.Replace("[[UserName]]", Name);
            }
            if (template.Contains("[[WebUrl]]"))
            {
                template = template.Replace("[[WebUrl]]", webUrl);
            }
            return template;
        }

        public void Dispose()
        {
            _unitOfWorkAsync.Dispose();
        }
        public static string ResolveEmailConfirm(string Name, string path, string TokenLink = "")
        {
            //string pat = HttpContext.Current.Server.MapPath(path);
            string template = File.ReadAllText(path);
            string webUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"];
            if (template.Contains("[[UserName]]"))
            {
                template = template.Replace("[[UserName]]", Name);
            }

            if (template.Contains("[[TokenLinkForShowTotalLink]]"))
            {
                template = template.Replace("[[TokenLinkForShowTotalLink]]", TokenLink);
            }
            if (template.Contains("[[tokenLink]]"))
            {
                template = template.Replace("[[tokenLink]]", TokenLink);
            }

            if (template.Contains("[[ResetLinkForShowTotalLink]]"))
            {
                template = template.Replace("[[ResetLinkForShowTotalLink]]", TokenLink);
            }
            if (template.Contains("[[ResetLink]]"))
            {
                template = template.Replace("[[ResetLink]]", TokenLink);
            }
            if (template.Contains("[[WebUrl]]"))
            {
                template = template.Replace("[[WebUrl]]", webUrl);
            }
            return template;
        }

        public void SendEmail(EmailModel model, bool IsAttachPDF)
        {
            Task.Run(() => SendEmailInvoice(model, IsAttachPDF));
        }
        private void SendEmailInvoice(EmailModel model, bool IsAttachPDF)
        {
            string Body = "", EmailTo = "";
            Body = model.Body;
            var images = GetImagesInHTMLString(Body);
            //var iframes = GetIframeInHTMLString(Body);
            var iframesAndSources = GetIframeAndSources(Body);
            Body = DoIt(Body);
            var cidImages = GetImagesInHTMLString(Body);

            // Create and build a new MailMessage object
            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;

            //From address to send email
            if (string.IsNullOrEmpty(EmailDisplayName))
                message.From = new MailAddress(EmailFrom);
            else
                message.From = new MailAddress(EmailFrom, EmailDisplayName);

            //To address to send email
            if (!string.IsNullOrEmpty(model.SendTo))
                message.To.Add(new MailAddress(model.SendTo));
            if (!string.IsNullOrEmpty(model.CC))
                message.CC.Add(new MailAddress(model.CC));
            if (!string.IsNullOrEmpty(model.Bcc))
                message.Bcc.Add(new MailAddress(model.Bcc));

            message.Subject = model.Subject;


            if (images.Count > 0)
            {
                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html);
                int i = 0;
                foreach (var image in images)
                {
                    string src = Regex.Match(image, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;
                    Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(src.Split(',')[1]));
                    System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
                    var imageToInline = new LinkedResource(streamBitmap, MediaTypeNames.Image.Jpeg);
                    string cidsrc = Regex.Match(cidImages[i], "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;
                    imageToInline.ContentId = cidsrc.Split(':')[1]; //"MyImage";
                    alternateView.LinkedResources.Add(imageToInline);
                    i++;
                }
                message.AlternateViews.Add(alternateView);
            }
            if (model.PdfBody != null && IsAttachPDF == true)
            {
                var ms = GetPdf(Body, false);
                ms.Position = 0;
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(ms, "Document.pdf");
                message.Attachments.Add(attachment);
            }


            message.Body = Body;
            using (var client = new SmtpClient(EmailSMTP, EmailPort))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(EmailUserName, EmailPassword);

                // Enable SSL encryption
                client.EnableSsl = true;

                // Try to send the message. Show status in console.
                try
                {
                    client.Send(message);
                }
                catch (Exception ex)
                {
                }
            }
        }
        public static MemoryStream GetPdf(string html, bool IsSavePdfInFolder)
        {
            var ms = new MemoryStream();

            var document = new Document();

            PdfWriter writer;
            string filePath = "";
            string baseFilePath = "";
            if (IsSavePdfInFolder)
            {
                baseFilePath = HostingEnvironment.MapPath("~/");
                filePath = DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                writer = PdfWriter.GetInstance(document, new FileStream(baseFilePath + filePath, FileMode.Create));
            }
            else
            {
                //below one line is used for pdf file to send email as attachment without saving in folder
                writer = PdfWriter.GetInstance(document, ms);
            }
            //writer = PdfWriter.GetInstance(document, ms);
            writer.CloseStream = false;
            // HTMLWorker htmlparser = new HTMLWorker(document);
            document.Open();
            var strReader = new StringReader(html);
            //Set factories
            HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
            //Set css
            ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
            //cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/assets/css/main.css"), true);

            // cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/Assets/css/main.css"), true);

            IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));

            var worker = new XMLWorker(pipeline, true);
            var xmlParse = new XMLParser(true, worker);
            xmlParse.Parse(strReader);
            xmlParse.Flush();
            document.Close();
            //if (IsSavePdfInFolder)
            //    return filePath;
            //else
            return ms;
        }
        public static MemoryStream GeneratePDF(string html)
        {
            var pdfDoc = new Document(PageSize.A3);
            var memoryStream = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, memoryStream);

            pdfWriter.RgbTransparencyBlending = true;
            pdfDoc.Open();

            var cssResolver = new StyleAttrCSSResolver();
            XMLWorkerFontProvider fontProvider = new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS);
            //Following line will be required in case of lanuguages other than english i.e I have added following for arabic font.
            //fontProvider.Register(HttpContext.Server.MapPath("~/Tahoma Regular font.ttf"));
            CssAppliers cssAppliers = new CssAppliersImpl(fontProvider);
            HtmlPipelineContext htmlContext = new HtmlPipelineContext(cssAppliers);
            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
            // Pipelines
            PdfWriterPipeline pdf = new PdfWriterPipeline(pdfDoc, pdfWriter);
            HtmlPipeline html1 = new HtmlPipeline(htmlContext, pdf);
            CssResolverPipeline css = new CssResolverPipeline(cssResolver, html1);
            // XML Worker
            XMLWorker worker = new XMLWorker(css, true);
            XMLParser p = new XMLParser(worker);
            p.Parse(new StringReader(html));
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            memoryStream.Position = 0;
            return memoryStream;

        }

        public static string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", string.Empty); sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }
        private List<string> GetImagesInHTMLString(string htmlString)
        {
            List<string> images = new List<string>();
            string pattern = @"<(img)\b[^>]*>";

            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(htmlString);

            for (int i = 0, l = matches.Count; i < l; i++)
            {
                images.Add(matches[i].Value);
            }

            return images;
        }
        private List<string> GetIframeInHTMLString(string htmlString)
        {
            List<string> images = new List<string>();
            string pattern = @"(?<=<iframe[^>]*?)(?:\s*width=[""'](?<width>[^""']+)[""']|\s*height=[""'](?<height>[^'""]+)[""']|\s*src=[""'](?<src>[^'""]+[""']))+[^>]*?>";

            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(htmlString);

            for (int i = 0, l = matches.Count; i < l; i++)
            {
                images.Add(matches[i].Value);
            }

            return images;
        }
        public static Dictionary<string, string> GetIframeAndSources(string htmlString)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlString);
            Dictionary<string, string> lstIframes = new Dictionary<string, string>();
            document.DocumentNode.Descendants("iframe")
                                .Where(e =>
                                {
                                    string src = e.GetAttributeValue("src", null) ?? "";
                                    return !string.IsNullOrEmpty(src);
                                })
                                .ToList()
                                .ForEach(x =>
                                {
                                    string currentSrcValue = x.GetAttributeValue("src", null);
                                    lstIframes.Add(x.OuterHtml, currentSrcValue);
                                });
            return lstIframes;
        }
        public static string DoIt(string htmlString)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlString);
            document.DocumentNode.Descendants("img")
                                .Where(e =>
                                {
                                    string src = e.GetAttributeValue("src", null) ?? "";
                                    return !string.IsNullOrEmpty(src) && src.StartsWith("data:image");
                                })
                                .ToList()
                                .ForEach(x =>
                                {
                                    string currentSrcValue = x.GetAttributeValue("src", null);
                                    currentSrcValue = currentSrcValue.Split(',')[1];//Base64 part of string
                                    byte[] imageData = Convert.FromBase64String(currentSrcValue);
                                    string contentId = Guid.NewGuid().ToString();
                                    LinkedResource inline = new LinkedResource(new MemoryStream(imageData), "image/jpeg");
                                    inline.ContentId = contentId;
                                    inline.TransferEncoding = TransferEncoding.Base64;

                                    x.SetAttributeValue("src", "cid:" + inline.ContentId);
                                });


            return document.DocumentNode.OuterHtml;
        }
        public void SendEmailPrescription(string template, string emailto)
        {

            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;

            //From address to send email
            if (string.IsNullOrEmpty(EmailDisplayName))
                message.From = new MailAddress(EmailFrom);
            else
                message.From = new MailAddress(EmailFrom, EmailDisplayName);

            //To address to send email
            if (!string.IsNullOrEmpty(emailto))
                message.To.Add(new MailAddress(emailto));

            message.Subject = "Prescription";

            if (template != null && template != "")
            {
                var ms = GeneratePDF(template);
                ms.Position = 0;
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(ms, "Document.pdf");
                message.Attachments.Add(attachment);
            }
            message.Body = "test";
            using (var client = new SmtpClient(EmailSMTP, EmailPort))
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.Credentials = new NetworkCredential(EmailUserName, EmailPassword);
                client.EnableSsl = true;
                try
                {
                    client.Send(message);
                }
                catch (Exception ex)
                {
                }
            }
        }
        public void SendEmailReport(string sourcePath, string emailto)
        {

            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;

            //From address to send email
            if (string.IsNullOrEmpty(EmailDisplayName))
                message.From = new MailAddress(EmailFrom);
            else
                message.From = new MailAddress(EmailFrom, EmailDisplayName);

            //To address to send email
            if (!string.IsNullOrEmpty(emailto))
                message.To.Add(new MailAddress(emailto));

            message.Subject = "Prescription";

            if (sourcePath != null && sourcePath != "")
            {
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(sourcePath);
                message.Attachments.Add(attachment);
            }
            message.Body = "test";
            using (var client = new SmtpClient(EmailSMTP, EmailPort))
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.Credentials = new NetworkCredential(EmailUserName, EmailPassword);
                client.EnableSsl = true;
                try
                {
                    client.Send(message);
                }
                catch (Exception ex)
                {
                }
            }
        }

    }
}