using JVMU.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JVMU.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO;

namespace JVMU.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        public IConfiguration Configuration { get; }
        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public IActionResult Matakuliah()
        //{
        //    var matakuliahs = new List<Models.Matakuliah>();

        //    matakuliahs.Add(new Models.Matakuliah
        //    {
        //        MatKulID = 1,
        //        NamaMatkul = "Elektronika Analog",
        //        Pengampu = "Prapto Nugroho"
        //    });

        //    matakuliahs.Add(new Models.Matakuliah
        //    {
        //        MatKulID = 2,
        //        NamaMatkul = "Elektronika Dasar",
        //        Pengampu = "Prapto Nugroho"
        //    });

        //    ViewBag.matakuliahlist = matakuliahs;
        //    return View();
        //}
        //[Route("Matakuliah")]
        public IActionResult Matakuliah()
        {
            string constr = Configuration["ConnectionStrings:conString"];
            var matakuliahs = new List<Models.MatakuliahModel>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string query = "SELECT * FROM dbo.Matakuliah";
                SqlCommand command = new SqlCommand(query, con);  
                using (SqlDataReader reader=command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matakuliahs.Add(new MatakuliahModel
                        {
                            MatKulID = (int)reader["MatKulID"],
                            NamaMatkul = reader["NamaMatkul"].ToString(),
                            Pengampu = reader["Pengampu"].ToString()
                        });
                    }
                }
                con.Close();
                }
            ViewBag.matakuliahlist = matakuliahs;
            return View();
        }

        [Route("Matakuliah/Materi/{id:int}")]
        public IActionResult Detail(int id)
        {
            string constr = Configuration["ConnectionStrings:conString"];
            var matakuliahdetails = new List<Models.MatakuliahDetail>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string query = "SELECT Materi.* FROM Materi WHERE Materi.MatKulID = " + id;
                SqlCommand command = new SqlCommand(query, con);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matakuliahdetails.Add(new MatakuliahDetail
                        {
                            MateriID = (int)reader["MateriID"],
                            MatKulID = (int)reader["MatKulID"],
                            JudulMateri = reader["JudulMateri"].ToString(),
                            DeskripsiMateri = reader["DeskripsiMateri"].ToString(),
                            LinkMateri = reader["LinkMateri"].ToString()
                        });
                    }
                }
                con.Close();
            }
            ViewBag.matakuliahdetaillist = matakuliahdetails;
            return View();
        }

        //[HttpGet("download")]
        //public IActionResult GetBlobDownload(string link)
        //{
        //    var net = new System.Net.WebClient();
        //    var data = net.DownloadData(link);
        //    var content = new System.IO.MemoryStream(data);
        //    var contentType = "APPLICATION/octet-stream";
        //    var fileName = "something.bin";
        //    return File(content, contentType, fileName);
        //}

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Files");
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()  
        {  
            return new Dictionary<string, string>  
            {  
                {".txt", "text/plain"},  
                {".pdf", "application/pdf"},  
                {".doc", "application/vnd.ms-word"},  
                {".docx", "application/vnd.ms-word"},  
                {".xls", "application/vnd.ms-excel"},  
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},  
                {".jpg", "image/jpeg"},  
                {".jpeg", "image/jpeg"},  
                {".gif", "image/gif"},  
                {".csv", "text/csv"}  
            };  
        } 

    }
}
