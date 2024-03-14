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
    public class LookupController : Controller
    {
        IMongoDBService _db;

        public LookupController(IMongoDBService db)
        {
            _db = db;
        }

        [HttpGet("getunits")]
        public Task<List<UnitModel>> GetUnits(int itemtype)
        {
            return _db.GetUnits(itemtype);
        }

        [HttpGet("getworkcategories")]
        public Task<List<WorkCategoryModel>> GetWorkCategories()
        {
            return _db.GetCategories();
        }
    }
}

