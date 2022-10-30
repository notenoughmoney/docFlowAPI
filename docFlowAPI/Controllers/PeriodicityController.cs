using docFlowAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System.Data;
using TodoApi.Models;

namespace docFlowAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodicitiesController : ControllerBase {
        private readonly IConfiguration _configuration;
        public PeriodicitiesController(IConfiguration configuration) {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get() {
            string query = @"select * from get_periodicities()";

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
        public JsonResult Post(Periodicities periodicities) {
            string query = @"call add_periodicity(@PeriodicityTitle, @Dates)";
                
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource)) {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon)) {
                    myCommand.Parameters.AddWithValue("@PeriodicityTitle", periodicities.PeriodicityTitle);
                    myCommand.Parameters.AddWithValue("@Dates", periodicities.Dates);
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
        public JsonResult Put(Periodicities periodicities) {
            string query = @"call update_periodicity(@PeriodicityId, @PeriodicityTitle, @Dates)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource)) {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon)) {
                    myCommand.Parameters.AddWithValue("@PeriodicityId", periodicities.PeriodicityId);
                    myCommand.Parameters.AddWithValue("@PeriodicityTitle", periodicities.PeriodicityTitle);
                    myCommand.Parameters.AddWithValue("@Dates", periodicities.Dates);
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
            string query = @"call del_periodicity(@PeriodicityId)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource)) {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon)) {
                    myCommand.Parameters.AddWithValue("@PeriodicityId", id);
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
