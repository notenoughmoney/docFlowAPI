using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace docFlowAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase {
        private readonly IConfiguration _configuration;
        public WorkerController(IConfiguration configuration) {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get() {
            string query = @"select * from get_workers()";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource)) {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon)) {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }
    }
}
