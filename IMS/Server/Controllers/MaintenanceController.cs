using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IMS.Shared.Models;
using IMS.Server.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IMS.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MaintenanceController : Controller
    {
        IMongoDBService _db;

        public MaintenanceController(IMongoDBService db)
        {
            _db = db;
        }

        #region ITEMS

        [HttpGet("getitemsbytype")]
        public Task<List<ItemModel>> GetItemsByType(int typeid)
        {
            return _db.GetItemsByType(typeid);
        }

        [HttpPost]
        [Route("savematerial")]
        public Task<string> SaveMaterial(ItemModel item)
        {
            return _db.SaveMaterial(item);
        }

        [HttpGet("getitem")]
        public Task<ItemModel> GetItem(string id)
        {
            return _db.GetItem(id);
        }

        [HttpPost]
        [Route("saveeditmaterial")]
        public Task<int> EditItem(ItemModel item)
        {
            return _db.UpdateItem(item);
        }

        [HttpPost]
        [Route("deleteitem")]
        public Task<int> DeleteItem(string[] ids)
        {
            return _db.DeleteItem(ids);
        }

        #endregion

        #region WORKITEMS

        [HttpGet("getworkitems")]
        public Task<List<WorkItemModel>> GetWorkitems()
        {
            return _db.GetWorkItems();
        }

        [HttpPost]
        [Route("saveworkitem")]
        public Task<string> SaveWorkItem(WorkItemModel workitem)
        {
            return _db.SaveWorkitem(workitem);
        }

        [HttpPost]
        [Route("saveeditworkitem")]
        public Task<string> EditWorkItem(WorkItemModel workitem)
        {
            return _db.EditWorkItem(workitem);
        }

        [HttpPost]
        [Route("deleteworkitem")]
        public Task DeleteWorkItem([FromBody]string id)
        {
            return _db.DeleteWorkItem(id);
        }

        #endregion
    }
}

