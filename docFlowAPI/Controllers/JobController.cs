using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using TodoApi.Models;

namespace docFlowAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase {
        private readonly IConfiguration _configuration;
        public JobsController(IConfiguration configuration) {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get() {
            string query = @"select * from get_jobs()";

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

        [HttpPost]
        public JsonResult Post(Jobs jobs) {
            string query = @"call add_job(@JobTitle)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource)) {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon)) {
                    myCommand.Parameters.AddWithValue("@JobTitle", jobs.JobTitle);
                    try {
                        myReader = myCommand.ExecuteReader();
                    } catch (PostgresException ex) {
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
        public JsonResult Put(Jobs jobs) {
            string query = @"call update_job(@JobId, @JobTitle)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource)) {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon)) {
                    myCommand.Parameters.AddWithValue("@JobId", jobs.JobId);
                    myCommand.Parameters.AddWithValue("@JobTitle", jobs.JobTitle);
                    try {
                        myReader = myCommand.ExecuteReader();
                    } catch (PostgresException ex) {
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

        [HttpDelete("{id}")]
        public JsonResult Delete(int id) {
            string query = @"call del_job(@JobId)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource)) {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon)) {
                    myCommand.Parameters.AddWithValue("@JobId", id);
                    try {
                        myReader = myCommand.ExecuteReader();
                    } catch (PostgresException ex) {
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
