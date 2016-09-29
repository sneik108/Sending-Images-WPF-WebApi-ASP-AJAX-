using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;

namespace ASP_MVC___API.Controllers
{
    public class SaveFileController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            String filePath = HostingEnvironment.MapPath("~/Images/images.jpg");
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Image image = Image.FromStream(fileStream);
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            result.Content = new ByteArrayContent(memoryStream.ToArray());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            memoryStream.Close();
            fileStream.Close();
            return result;
        }
        public HttpResponseMessage Post()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            if (Request.Content.IsMimeMultipartContent())
            {
                
                Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(
                        new MultipartMemoryStreamProvider()).ContinueWith((task) =>
                        {
                            MultipartMemoryStreamProvider provider = task.Result;
                            foreach (HttpContent content in provider.Contents)
                            {
                                Stream stream = content.ReadAsStreamAsync().Result;
                                Image image = Image.FromStream(stream);
                                var testName = content.Headers.ContentDisposition.Name;
                                String filePath = HostingEnvironment.MapPath("~/Images/");
                                String fileName = "images" + ".jpg";
                                String fullPath = Path.Combine(filePath, fileName);
                                image.Save(fullPath);
                                stream.Close();
                            }
                        });
                return result;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(
                        HttpStatusCode.NotAcceptable,
                        "This request is not properly formatted"));
            }
        }
    }
}
