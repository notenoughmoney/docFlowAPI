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
    public class DocTypesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DocTypesController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }


        // вот этот контроллер самый обычный
        // он отсылает данные (название и ссылку на файл)
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select * from get_doc_types()";

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

        // а вот этот контроллер чисто отсылает файл
        [HttpGet]
        [Route("download")]
        public async Task<FileStreamResult> Stream(string fileName)
        {
            string localFilePath = "UploadedFiles\\Shablons\\" + fileName;

            var stream = new MemoryStream(System.IO.File.ReadAllBytes(localFilePath));
            var response = File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            //var response = File(stream, "image/jpeg");
            return response;
        }


        // этот контроллер и следующий отправляют файл и данные в form-data
        // и они используют особую модель JsonModelBinder
        [HttpPost]
        public IActionResult Post(
            [ModelBinder(BinderType = typeof(JsonModelBinder))] DocTypes docTypes,
            IList<IFormFile> files)
        {
            // записываем файл в окружение веб хоста
            // а также устанавливаем ссылку на него
            string dir = "";
            if (files.Count != 0)
            {
                string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles\\Shablons");
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

            string query = @"call add_doc_type(@Title, @LinkToShablon)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Title", docTypes.Title);
                    myCommand.Parameters.AddWithValue("@LinkToShablon", dir);
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


        [HttpPut]
        public IActionResult Put(
            [ModelBinder(BinderType = typeof(JsonModelBinder))] DocTypes docTypes,
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
                string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles\\Shablons");
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

            string query = @"call update_doc_type(@DocTypeId ,@Title, @LinkToShablon)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@DocTypeId", docTypes.DocTypeId);
                    myCommand.Parameters.AddWithValue("@Title", docTypes.Title);
                    myCommand.Parameters.AddWithValue("@LinkToShablon", dir);
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

        // самый обычный delete
        // спасибо тебе госпади
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"call del_doc_type(@DocTypeId)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@DocTypeId", id);
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
    }
}
