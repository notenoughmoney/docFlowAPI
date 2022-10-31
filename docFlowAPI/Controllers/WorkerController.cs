using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using TodoApi.Models;

namespace docFlowAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase {
        private readonly IConfiguration _configuration;
        public WorkersController(IConfiguration configuration) {
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

        [HttpPost]
        public JsonResult Post(Workers workers) {
            string query = @"call add_worker(@WorkerFullname, @JobId, @IsClassroomTeacher, @Mail, @Password)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource)) {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon)) {
                    myCommand.Parameters.AddWithValue("@WorkerFullname", workers.WorkerFullname);
                    myCommand.Parameters.AddWithValue("@JobId", workers.JobId);
                    myCommand.Parameters.AddWithValue("@IsClassroomTeacher", workers.IsClassroomTeacher);
                    myCommand.Parameters.AddWithValue("@Mail", workers.Mail);
                    myCommand.Parameters.AddWithValue("@Password", workers.Password);
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
        public JsonResult Put(Workers workers) {
            string query = @"call update_worker(@WorkerId, @WorkerFullname, @JobId, @IsClassroomTeacher, @Mail, @Password)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource)) {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon)) {
                    myCommand.Parameters.AddWithValue("@WorkerId", workers.WorkerId);
                    myCommand.Parameters.AddWithValue("@WorkerFullname", workers.WorkerFullname);
                    myCommand.Parameters.AddWithValue("@JobId", workers.JobId);
                    myCommand.Parameters.AddWithValue("@IsClassroomTeacher", workers.IsClassroomTeacher);
                    myCommand.Parameters.AddWithValue("@Mail", workers.Mail);
                    myCommand.Parameters.AddWithValue("@Password", workers.Password);
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
            string query = @"call del_worker(@WorkerId)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource)) {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon)) {
                    myCommand.Parameters.AddWithValue("@WorkerId", id);
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
