using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace HMS.Web.API.Areas.FileServer.Controllers
{
    public class DownloadController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        [ActionName("DownloadFile")]
        public HttpResponseMessage DownloadFile(string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                if (File.Exists(FilePath))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var fileStream = new FileStream(FilePath, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(FilePath);
                    return response;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
       
    }
}
