using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IMS.Shared.Models;
using IMS.Server.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using Newtonsoft.Json;

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

        #region MENU

        [HttpGet("getmenus")]
        public async Task<List<MenuModel>> GetMenus()
        {
            return await _db.GetMenus();
        }
        
        [HttpPost("savemenu")]
        public async Task<string> SaveMenu(MenuModel menu)
        {
            return await _db.SaveMneu(menu);
        }

        [HttpGet("getparentmenus")]
        public async Task<List<MenuModel>> GetParentMenus()
        {
            return await _db.GetParentMenus();
        }
        
        #endregion
        
        
        [HttpGet("getallmedals")]
        public async Task<List<List<MedalTallyModel>>> GetMedals()
        {
            // URL of the website
            string url = "https://sports.depedcaraga.site/public/";

            // Create a new HtmlWeb instance
            var web = new HtmlWeb();

            // Load the HTML from the URL
            var doc = web.Load(url);

            // List to store table data
            var tablesData = new List<TableData>();

            // Get all table elements in the HTML document
            var tables = doc.DocumentNode.SelectNodes("//table");

            if (tables != null)
            {
                // Iterate through each table
                foreach (var table in tables)
                {
                    // Get the id attribute of the table
                    var tableId = table.GetAttributeValue("id", "");

                    // Create a new TableData instance
                    var tableData = new TableData
                    {
                        Id = tableId,
                        Rows = new List<List<string>>() // List to store rows
                    };

                    // Get all rows in the table
                    var rows = table.SelectNodes(".//tr");

                    if (rows != null)
                    {
                        // Iterate through each row
                        foreach (var row in rows)
                        {
                            // List to store cells
                            var rowCells = new List<string>();

                            // Get all cells in the row
                            var cells = row.SelectNodes(".//td | .//th");

                            if (cells != null)
                            {
                                // Iterate through each cell
                                foreach (var cell in cells)
                                {
                                    // Add cell content to rowCells list
                                    rowCells.Add(cell.InnerText);
                                }
                            }

                            // Add rowCells list to tableData
                            tableData.Rows.Add(rowCells);
                        }
                    }

                    // Add tableData to tablesData list
                    tablesData.Add(tableData);
                }
            }

            // Serialize tablesData to JSON
            //var json = JsonConvert.SerializeObject(tablesData, Formatting.Indented);

            TableData t1 = tablesData[0];

            List<MedalTallyModel> medalselem = new();
            List<MedalTallyModel> medalssec = new();
            List<MedalTallyModel> medalstotal = new();

            List<List<MedalTallyModel>> medals = new();
            
            for (int i = 3; i < 15; i++)
            {
                List<string> r = t1.Rows[i];
                
                MedalTallyModel m = new();

                m.Province = r[0];
                m.Gold = r[1];
                m.Silver = r[2];
                m.Bronze = r[3];
                m.rank = r[4];
                
                medalselem.Add(m);
                
                m = new();
                
                m.Province = r[0];
                m.Gold = r[5];
                m.Silver = r[6];
                m.Bronze = r[7];
                m.rank = r[8];
            
                medalssec.Add(m);
                m = new();

                m.Province = r[0];
                m.Gold = r[9];
                m.Silver = r[10];
                m.Bronze = r[11];
                m.rank = r[12];
                medalstotal.Add(m);

            }
            
            medals.Add(medalselem);
            medals.Add(medalssec);
            medals.Add(medalstotal);
            
            return medals;
        }

        [HttpGet("geteventmdedals")]
        public async Task<List<MedalTallyModel>> GetMedalsPerEvent()
        {
            string url = "https://sports.depedcaraga.site/public/";

            // Create a new HtmlWeb instance
            var web = new HtmlWeb();

            // Load the HTML from the URL
            var doc = web.Load(url);

            // List to store table data
            var tablesData = new List<TableData>();

            // Get all table elements in the HTML document
            var tables = doc.DocumentNode.SelectNodes("//table");

            if (tables != null)
            {
                // Iterate through each table
                foreach (var table in tables)
                {
                    // Get the id attribute of the table
                    var tableId = table.GetAttributeValue("id", "");

                    // Create a new TableData instance
                    var tableData = new TableData
                    {
                        Id = tableId,
                        Rows = new List<List<string>>() // List to store rows
                    };

                    // Get all rows in the table
                    var rows = table.SelectNodes(".//tr");

                    if (rows != null)
                    {
                        // Iterate through each row
                        foreach (var row in rows)
                        {
                            // List to store cells
                            var rowCells = new List<string>();

                            // Get all cells in the row
                            var cells = row.SelectNodes(".//td | .//th");

                            if (cells != null)
                            {
                                // Iterate through each cell
                                foreach (var cell in cells)
                                {
                                    // Add cell content to rowCells list
                                    rowCells.Add(cell.InnerText);
                                }
                            }

                            // Add rowCells list to tableData
                            tableData.Rows.Add(rowCells);
                        }
                    }

                    // Add tableData to tablesData list
                    tablesData.Add(tableData);
                }
            }

            TableData t1 = tablesData[5];

            List<MedalTallyModel> eventmedals = new();
            
            List<string> province = t1.Rows[0];

            
            List<List<int>> ids = new() { 
                new List<int>(){1, 2, 3} , 
                new List<int>(){4, 5, 6}, 
                new List<int>(){7, 8, 9}, 
                new List<int>(){10, 11, 12}, 
                new List<int>(){13, 14, 15}, 
                new List<int>(){16, 17, 18}, 
                new List<int>(){19, 20, 21}, 
                new List<int>(){22, 23, 24}, 
                new List<int>(){25, 26, 27}, 
                new List<int>(){28, 29, 30}, 
                new List<int>(){31, 32, 33}, 
                new List<int>(){34, 35, 36} 
            };
            
            for (int i = 2; i < t1.Rows.Count; i++)
            {
                for (int x = 1; x < province.Count; x++)
                {
                    MedalTallyModel eventmedal = new();
                    eventmedal.Province = province[x];
                    eventmedal.Gold = t1.Rows[i][ids[x-1][0]];
                    eventmedal.Silver = t1.Rows[i][ids[x-1][1]];
                    eventmedal.Bronze = t1.Rows[i][ids[x-1][2]];
                    eventmedal.sport = t1.Rows[i][0];
                    eventmedals.Add(eventmedal);
                }
            }

            return eventmedals;

        }

    // Data structure to hold table information
        class TableData
        {
            public string Id { get; set; }
            public List<List<string>> Rows { get; set; }
        }
    }
}

