using HMS.Web.API.Filters;
using HMS.Entities.CustomModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace HMS.Web.API.Areas.FileServer.Controllers
{
    [JwtAuthentication]
    public class UploadController : ApiController
    {

        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        [ActionName("Temporary")]
        public ResponseInfo Temporary()
        {
            var objResponse = new ResponseInfo() { IsSuccess = false };
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                List<string> FileNamesList = new List<string>();

                // Get the uploaded image from the Files collection
                HttpFileCollection httpPostedFile = HttpContext.Current.Request.Files;

                string fileSavePath, fileName;
                var FolderPath = HttpContext.Current.Server.MapPath("~/Files");
                FolderPath = Path.Combine(FolderPath, "Temp");

                foreach (string i in httpPostedFile)
                {
                    HttpPostedFile UFD = httpPostedFile[i] as HttpPostedFile;
                    string fileExtension = Path.GetExtension(UFD.FileName);
                    fileName = DateTime.UtcNow.ToString("yyyyMMddHHmmsss")+ fileExtension;
                    FileNamesList.Add(fileName);
                    fileSavePath = Path.Combine(FolderPath, fileName);
                    UFD.SaveAs(fileSavePath);

                }
                objResponse.IsSuccess = true;
                objResponse.ResultSet = FileNamesList.ToArray();
            }

            return objResponse;
        }
    }
}