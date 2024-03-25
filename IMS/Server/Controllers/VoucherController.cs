using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Server.Services;
using Microsoft.AspNetCore.Mvc;
using IMS.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IMS.Server.Controllers
{
    [Route("[controller]")]
    public class VoucherController : Controller
    {
        IMongoDBService _db;

        public VoucherController(IMongoDBService db)
        {
            _db = db;
        }

        [HttpGet("getcvs")]
        public async Task<List<POModel>> GetCVs(string projectid)
        {
            return await _db.GetCVs(projectid);
        }

        [HttpGet("getcv")]
        public async Task<POModel> GetCV(string poid)
        {
            return await _db.GetCV(poid);
        }

        [HttpGet("getposdetails")]
        public async Task<List<POModel>> GetPODetails(string projectid)
        {
            return await _db.GetPOSDetails(projectid);
        }

        [HttpPost("savecv")]
        public async Task<string> SaveCV([FromBody] string data)
        {
            POModel cv = Newtonsoft.Json.JsonConvert.DeserializeObject<POModel>(data);

            return await _db.SaveCV(cv);
        }

        [HttpPost("deletecv")]
        public async Task<string> DeleteCV([FromBody] string data)
        {
            POModel cv = Newtonsoft.Json.JsonConvert.DeserializeObject<POModel>(data);

            return await _db.DeleteCV(cv);
        }

        [HttpPost("submitcv")]
        public async Task<string> SubmitCV([FromBody]List<string> paramList)
        {
            string prid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
            string poid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
            int action = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(paramList[2].ToString());

            return await _db.SubmitCV(prid, poid, action);
        }
    }
}

