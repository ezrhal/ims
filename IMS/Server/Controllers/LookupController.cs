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
        
        [HttpGet("getroles")]
        public Task<List<RoleModel>> GetRoles()
        {
            return _db.GetRoles();
        }

        [HttpGet("getworkcategories")]
        public Task<List<WorkCategoryModel>> GetWorkCategories()
        {
            return _db.GetCategories();
        }
        
        [HttpGet("Test")]
        public async Task<string> Test()
        {
            string url = "https://reports.pgas.ph/Report/View/VENDOR/Vendor_Application?uniqueName=6642d3771374b429388bb67c";
            byte[] pdfBinary = await DownloadPdfAsync(url);
            string base64String = Convert.ToBase64String(pdfBinary);
            return base64String;
        }

        public static async Task<byte[]> DownloadPdfAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/pdf"));
                return await client.GetByteArrayAsync(url);
            }
        }
        
       
    }
}

