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
using System.Data;

namespace JVMU.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private SqlConnection conn;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create (MatakuliahModel obj)
        {
            string constr = Configuration["ConnectionStrings:conString"];
            conn = new SqlConnection(constr);
            SqlCommand command = new SqlCommand("Matkul_List", conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@NamaMatkul", obj.NamaMatkul);
            command.Parameters.AddWithValue("@Pengampu", obj.Pengampu);
            conn.Open();
            int i = command.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Matakuliah");
        }

        public IActionResult Upload(int id)
        {
            string constr = Configuration["ConnectionStrings:conString"];
            var matakuliahids = new List<Models.MatakuliahModel>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string query = "SELECT MatKulID,NamaMatkul FROM Matakuliah WHERE MatKulID="+id;
                SqlCommand command = new SqlCommand(query, con);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matakuliahids.Add(new MatakuliahModel
                        {
                            MatKulID = (int)reader["MatKulID"],
                            NamaMatkul = reader["NamaMatkul"].ToString(),
                        });
                    }
                }
                con.Close();
            }
            ViewBag.matakuliahidlist = matakuliahids;
            return View();
        }

        public IActionResult Update(int id)
        {
            string connectionString = Configuration["ConnectionStrings:conString"];
            var matakuliahids = new List<Models.MatakuliahModel>();
            //MatakuliahModel matakuliahs = new MatakuliahModel();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select * From Matakuliah Where MatKulID='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matakuliahids.Add(new MatakuliahModel
                        {
                            MatKulID = (int)reader["MatKulID"],
                            NamaMatkul = reader["NamaMatkul"].ToString(),
                            Pengampu = reader["Pengampu"].ToString()
                        });
                        //matakuliahs.MatKulID = Convert.ToInt32(reader["MatKulID"]);
                        //matakuliahs.NamaMatkul = Convert.ToString(reader["NamaMatkul"]);
                    }
                }
                connection.Close();
            }
            ViewBag.matakuliahupdate = matakuliahids;
            return View();
        }

        //[HttpPost]
        //public IActionResult Update(MatakuliahModel matakuliah)
        //{
        //    string connectionString = Configuration["ConnectionStrings:conString"];
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string sql = $"Update Matakuliah SET NamaMatkul='{matakuliah.NamaMatkul}' Where MatKulID='{matakuliah.MatKulID}'";
        //        using (SqlCommand command = new SqlCommand(sql, connection))
        //        {
        //            connection.Open();
        //            command.ExecuteNonQuery();
        //            connection.Close();
        //        }
        //    }
        //    return RedirectToAction("Matakuliah");
        //}

        [HttpPost]
        public async Task<IActionResult> Update(MatakuliahModel obj, int id)
        {
            string constr = Configuration["ConnectionStrings:conString"];
            conn = new SqlConnection(constr);
            SqlCommand command = new SqlCommand("Update", conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@NamaMatkul", obj.NamaMatkul);
            command.Parameters.AddWithValue("@MatKulID", id);
            conn.Open();
            int i = command.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Matakuliah");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file,MatakuliahDetail obj)
        {

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FileName);
            string constr = Configuration["ConnectionStrings:conString"];
            conn = new SqlConnection(constr);
            
            
            //string query = "SELECT MatKulID,NamaMatkul FROM Matakuliah";
            SqlCommand command = new SqlCommand("Materi_List", conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MatKulID", obj.MatKulID);
            command.Parameters.AddWithValue("@JudulMateri", obj.JudulMateri);
            command.Parameters.AddWithValue("@DeskripsiMateri", obj.DeskripsiMateri);
            command.Parameters.AddWithValue("@LinkMateri", file.FileName);
            conn.Open();
            int i = command.ExecuteNonQuery();
            conn.Close();

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Matakuliah");
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
