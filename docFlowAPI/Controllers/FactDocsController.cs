using docFlowAPI.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using TodoApi.Models;
using MimeTypes;

namespace docFlowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactDocsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FactDocsController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }


        // получить все документы к сдаче
        [HttpGet]
        public JsonResult GetAll()
        {
            string query = @"select * from get_fact_docs()";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        // получить документы к сдаче по пользоваетлю
        [HttpGet("{workerId}")]
        public JsonResult GetBy(int workerId)
        {
            string query = @"select * from get_fact_docs_by_worker(@WorkerId)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@WorkerId", workerId);
                    try
                    {
                        myReader = myCommand.ExecuteReader();
                    } catch (PostgresException ex)
                    {
                        return new JsonResult(ex.Message);  
                    }
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        // сдать документ и соответственно загрузить его
        [HttpPut("{id}")]
        public IActionResult Put(
            [ModelBinder(BinderType = typeof(JsonModelBinder))] int id,
            IList<IFormFile> files)
        {
            // пускай при обновлении записи существующий файл не будет обновляться
            // мне пофиг
            // всё равно ссылка переустановится

            // записываем файл в окружение веб хоста
            // а также устанавливаем ссылку на него
            string dir = "";
            if (files.Count != 0)
            {
                string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles\\FactDocs");
                foreach (var file in files)
                {
                    dir = Utils.RandomString(5) + "_" + file.FileName;
                    string filePath = Path.Combine(directoryPath, dir);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }

            string query = @"call pass_fact_doc(@FactDocId, @LinkToFile)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@FactDocId", id);
                    myCommand.Parameters.AddWithValue("@LinkToFile", dir);
                    try
                    {
                        myReader = myCommand.ExecuteReader();
                    }
                    catch (PostgresException ex)
                    {
                        Response.StatusCode = 500;
                        return new JsonResult(ex.Message);
                    }
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        //скачать загруженный (сданный) документ
        [HttpGet]
        [Route("download")]
        public async Task<FileStreamResult> Stream(string fileName)
        {
            string localFilePath = "UploadedFiles\\FactDocs\\" + fileName;

            var stream = new MemoryStream(System.IO.File.ReadAllBytes(localFilePath));
            var response = File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            //var response = File(stream, "image/jpeg");
            return response;
        }

    }
}
