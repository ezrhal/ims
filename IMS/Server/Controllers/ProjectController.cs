using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Server.Services;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IMS.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : Controller
    {

        IMongoDBService _db;

        public ProjectController(IMongoDBService db)
        {
            _db = db;
        }

        [HttpGet("getprojects")]
        public Task<List<ProjectModel>> GetProjects(int typeid)
        {
            return _db.GetProjects();
        }

        [HttpGet("getproject")]
        public Task<ProjectModel> GetProject(string id)
        {
            return _db.GetProject(id);
        }

        [HttpPost("saveproject")]
        public Task<string> SaveProject(ProjectModel project)
        {
            return _db.SaveProject(project);
        }

        [HttpPost("updateproject")]
        public Task<string> UpdateProject(ProjectModel project)
        {
            return _db.UpdateProject(project);
        }


        [HttpPost("saveworkitemproject")]
        public async Task<string> SaveWorkItemProject(List<string> paramList)
        {
            WorkItemModel workItem = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItemModel>(paramList[0].ToString());
            string projectid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
            string isempty = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[2].ToString());

            return await _db.SaveWorkItemProject(workItem, projectid, isempty);

        }

        [HttpPost("removeproject")]
        public async Task<string> RemoveProject([FromBody] string id)
        {
            return await _db.RemoveProject(id);

        }

        [HttpPost("removeitem")]
        public async Task<string> RemoveMaterial(List<string> paramList)
        {
            string projectid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
            string workitemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
            string itemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[2].ToString());
            string type = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[3].ToString());

            return await _db.RemoveMaterial(projectid, workitemid, type, itemid);

        }

        [HttpPost("addmaterial")]
        public async Task<string> AddMaterial(List<string> paramList)
        {
            MaterialsModel material = Newtonsoft.Json.JsonConvert.DeserializeObject<MaterialsModel>(paramList[0].ToString());
            string projectid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
            string workitemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[2].ToString());
            string isempty = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[3].ToString());
            string type = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[4].ToString());

            return await _db.AddMaterial(material.ToBsonDocument(), projectid, workitemid, isempty, type);

        }

        [HttpPost("addequipment")]
        public async Task<string> AddEquipment(List<string> paramList)
        {
            EquipmentModel equipment = Newtonsoft.Json.JsonConvert.DeserializeObject<EquipmentModel>(paramList[0].ToString());
            string projectid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
            string workitemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[2].ToString());
            string isempty = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[3].ToString());
            string type = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[4].ToString());

            return await _db.AddMaterial(equipment.ToBsonDocument(), projectid, workitemid, isempty, type);

        }

        

        [HttpPost("addlabor")]
        public async Task<string> AddLabor(List<string> paramList)
        {
            LaborModel labor = Newtonsoft.Json.JsonConvert.DeserializeObject<LaborModel>(paramList[0].ToString());
            string projectid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
            string workitemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[2].ToString());
            string isempty = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[3].ToString());
            string type = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[4].ToString());

            return await _db.AddMaterial(labor.ToBsonDocument(), projectid, workitemid, isempty, type);

        }


        [HttpPost("updatequantity")]
        public async Task<string> UpdateQuantity(List<string> paramList)
        { 
            string projectid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
            string workitemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
            string itemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[2].ToString());
            string type = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[3].ToString());
            string quantity = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[4].ToString());

            return await _db.UpdateQuantity(projectid, workitemid, type, itemid, Convert.ToDouble(quantity));

        }

        [HttpPost("updateequipment")]
        public async Task<string> UpdateEquipment(List<string> paramList)
        { 
            string projectid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
            string workitemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());
            string itemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[2].ToString());
            string type = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[3].ToString());
            string quantity = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[4].ToString());
            string hours = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[5].ToString());

            return await _db.UpdateQuantityHours(projectid, workitemid, type, itemid, Convert.ToDouble(quantity), Convert.ToDouble(hours));

        }

        [HttpPost("deleteworkitem")]
        public async Task<string> DeleteWorkitem(List<string> paramList)
        {
            string projectid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[0].ToString());
            string workitemid = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(paramList[1].ToString());

            return await _db.DeleteWorkItem(projectid, workitemid);
        }



        //TRANSFER TO REPORT CONTROLLER

        [HttpGet("getreportproject")]
        public async Task<List<WorkItemModel>> GetReportPorject(string projectid)
        {
            return await _db.GetReportProject(projectid);
        }

        [HttpGet("getreportmaterials")]
        public async Task<List<MaterialsModel>> GetMaterials(string projectid, string workitemid)
        {
            return await _db.GetMaterials(projectid, workitemid);
        }

        [HttpGet("getreportequipment")]
        public async Task<List<EquipmentModel>> GetEquipment(string projectid, string workitemid)
        {
            return await _db.GetEquipment(projectid, workitemid);
        }

        [HttpGet("getreportlabor")]
        public async Task<List<LaborModel>> GetLabor(string projectid, string workitemid)
        {
            return await _db.GetLabor(projectid, workitemid);
        }

        [HttpPost("testresponse")]
        public async Task<int> Addnumber(TestNumber number)
        {
            int result = number.number1 + number.number2;
            return result;
        }

    }
}

