using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using GeradorFrameweb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class UploadFiles : Controller
    {

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post()
        {
            long size = HttpContext.Request.Form.Files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in HttpContext.Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            //return Ok(new { count = HttpContext.Request.Form.Files.Count, size, filePath });

            var logfile = "./log.txt";
            if (!Directory.Exists("build"))
                Directory.CreateDirectory("build");

            StreamWriter writer = new StreamWriter(logfile);

                Console.SetOut(writer);
                FWStart.Main(filePath);

           
            System.IO.File.Move(logfile, "./build/"+ logfile);

            string startPath = @"./build";
            string zipPath = Guid.NewGuid() + ".zip";

            ZipFile.CreateFromDirectory(startPath, "./" + zipPath);

            const string contentType = "application/zip";
            HttpContext.Response.ContentType = contentType;
            var result = new FileContentResult(System.IO.File.ReadAllBytes(@"./" + zipPath), contentType)
            {
                FileDownloadName = zipPath
            };

            return result;
        }
    }
}
