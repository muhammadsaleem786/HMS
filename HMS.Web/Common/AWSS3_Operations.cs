using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon;
using Amazon.S3;
using System.Configuration;
using Amazon.Runtime;
using Amazon.S3.Model;
using System.IO;

namespace HMS.Web.API.Common
{
    public class AWSS3_Operations
    {
        private RegionEndpoint Region;
        private IAmazonS3 S3Client;
        private string BucketName;
        private int S3UrlExpiry;

        public AWSS3_Operations()
        {
            string accessKey = ConfigurationManager.AppSettings["AWSS3ClientAccessId"].ToString();
            string secretAccessKey = ConfigurationManager.AppSettings["AWSS3ClientSecretId"].ToString();
            string givenRegion = ConfigurationManager.AppSettings["S3Region"].ToString();
            string expiry = ConfigurationManager.AppSettings["S3UrlExpiry"].ToString();
            BucketName = ConfigurationManager.AppSettings["S3BucketName"].ToString();

            Int32.TryParse(expiry, out S3UrlExpiry);
            Region = RegionEndpoint.GetBySystemName(givenRegion);
            BasicAWSCredentials credentials = new BasicAWSCredentials(accessKey, secretAccessKey);
            S3Client = new AmazonS3Client(credentials, Region);
        }

        public string S3UploadObject(string path, string key, string contentType = "")
        {
            try
            {
                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = key,
                    ContentType = contentType,
                    FilePath = path
                };
                PutObjectResponse response = S3Client.PutObject(request);

                File.Delete(path);

                GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = BucketName,
                    Key = key,
                    Expires = DateTime.Now.AddDays(S3UrlExpiry)
                };
                return S3Client.GetPreSignedURL(urlRequest);
            }
            catch(Exception ex)
            {
                return "Not OK";
            }
        }
        public string S3DeleteObject(string key)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = BucketName,
                    Key = key
                };
                S3Client.DeleteObject(request);
                return "OK";
            }
            catch(Exception ex)
            {
                return "Not OK";
            }
        }
        public string S3GetURL(string key)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = BucketName,
                    Key = key
                };
                GetObjectResponse response = S3Client.GetObject(request);
                GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = BucketName,
                    Key = key,
                    Expires = DateTime.Now.AddDays(S3UrlExpiry)
                };
                string url = S3Client.GetPreSignedURL(urlRequest);
                return url;
            }
            catch(Exception ex)
            {
                return "Not OK";
            }
        }
    }
}