 using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IMS.Server.Services;
using IMS.Shared;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace IMS.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PurchaseRequestController : Controller
    {
        IMongoDBService _db;

        public PurchaseRequestController(IMongoDBService db)
        {
            _db = db;
        }

        [HttpGet("getprs")]
        public async Task<List<PRModel>> GetPRs(string projectid = "")
        {
           return await _db.GetPRs(projectid);
           
        }

        [HttpGet("getpr")]
        public async Task<PRModel> GetPR(string id)
        {
            return await _db.GetPR(id);
        }

        [HttpPost("savepr")]
        public async Task<string> SavePR(PRModel pr)
        {
           await _db.SavePR(pr);
           return pr.Id;
        }

        [HttpGet("getprojectname")]
        public Task<List<ProjectModel>> GetProjectname()
        {
            return _db.GetProjectname();
        }

        [HttpGet("getmaterialsquantity")]
        public async Task<List<MaterialsModel>> GetMaterialsQuantity(string projectid, string itemid = "")
        {
            return await _db.GetMaterialsQuantity(projectid, itemid);
        }

        [HttpPost("savepritems")]
        public async Task<string> SavePRItem(List<string> paramList)
        {
            string prid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
            List<BalanceMaterialModel> items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BalanceMaterialModel>>(paramList[1].ToString());
            List<PRItemModel> existing = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PRItemModel>>(paramList[2].ToString());

            return await _db.SavePRItem(prid, items, existing);

            //string prid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
            //PRItemModel pritem = Newtonsoft.Json.JsonConvert.DeserializeObject<PRItemModel>(paramList[1].ToString());
            //string isempty = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[2].ToString());

            //return await _db.SavePRItem(prid, pritem, isempty);
        }

        [HttpPost("removepritem")]
        public async Task<string> RemovePRItem(List<string> paramList)
        {
            string id = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
            string pritemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());

            return await _db.RemovePRItem(id, pritemid);
        }

        
        [HttpPost("removepritem1")]
        public async Task<string> RemovePRItem1(string id, string pritemid)
        {
            //string id = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
            //string pritemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());

            return await _db.RemovePRItem(id, pritemid);
        }

        [HttpPost("updatepritem")]
        public async Task<string> UpdatePRItem(List<string> paramList)
        {
            string id = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
            string pritemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
            double quantity = Newtonsoft.Json.JsonConvert.DeserializeObject<double>(paramList[2].ToString());

            return await _db.UpdatePRItem(id, pritemid, quantity);
        }

        [HttpGet("gettotalprocured")]
        public  async Task<TotalProcuredModel> GetTotalProcured(string projectid, string itemid)
        {
            return await _db.GetTotalProcured(projectid, itemid);
        }

        [HttpPost("deletepr")]
        public async Task<string> DeletePR([FromBody]string id)
        {
            return await _db.DeletePR(id);
        }

        [HttpPost("submitpr")]
        public async Task<string> SubmitPR([FromBody]string id)
        {
            return await _db.SubmitPR(id);
        } 

        [HttpGet("getbalancematerials")]
        public async Task<List<BalanceMaterialModel>> GetBalanceMaterials(string projectid, string workitemid)
        {
            return await _db.GetBalanceMaterials(projectid, workitemid);
        }

        [HttpGet("getworkitemsinfo")]
        public async Task<List<WorkItemInfoModel>> GetWorkitems(string projectid)
        {
            return await _db.GetWorkItemsInfo(projectid);
        }

        
    }

}
