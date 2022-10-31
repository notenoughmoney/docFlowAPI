using docFlowAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using TodoApi.Models;

namespace docFlowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ScheduleController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select * from get_schedule()";

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

        [HttpPost]
        public JsonResult Post(Schedule schedule)
        {
            string query = @"call add_schedule(@DocTypeId, @JobId, @PeriodId)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@DocTypeId", schedule.DocTypeId);
                    myCommand.Parameters.AddWithValue("@JobId", schedule.JobId);
                    myCommand.Parameters.AddWithValue("@PeriodId", schedule.PeriodId);
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
        public JsonResult Put(Schedule schedule)
        {
            string query = @"call update_schedule(@ScheduleId, @DocTypeId, @JobId, @PeriodId)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ScheduleId", schedule.ScheduleId);
                    myCommand.Parameters.AddWithValue("@DocTypeId", schedule.DocTypeId);
                    myCommand.Parameters.AddWithValue("@JobId", schedule.JobId);
                    myCommand.Parameters.AddWithValue("@PeriodId", schedule.PeriodId);
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

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"call del_schedule(@ScheduleId)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("docFlow");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ScheduleId", id);
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
