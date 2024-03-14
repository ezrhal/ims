using System;
using System.Net.Http;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Radzen;

namespace IMS.Client.Pages.Maintenance
{
	partial class AddMaterials
	{
		[Parameter] public ItemModel item { get; set; }
        [Parameter] public int OrderID { get; set; }
        [Parameter] public int edit { get; set; } = 0;
        [Parameter] public int itemtype {get; set;} = 0;

        public List<UnitModel> units = new List<UnitModel>();
        
        protected override async Task OnInitializedAsync()
        {
            units = await httpClient.GetFromJsonAsync<List<UnitModel>>("lookup/getunits?itemtype=" + itemtype);
        }

        public async Task SaveMaterial(ItemModel args)
        {
            if (edit == 0)
            {
                var result = await httpClient.PostAsJsonAsync<ItemModel>("maintenance/savematerial", args);
                //string _id = result.Content.ReadFromJsonAsync<string>().Result;

                string _id = await result.Content.ReadAsStringAsync();
                args.Id = _id;
            }
            else
            {
                await httpClient.PostAsJsonAsync<ItemModel>("maintenance/saveeditmaterial", args);
            }

            NotificationService.Notify(
                   new NotificationMessage
                   {
                       Severity = NotificationSeverity.Success,
                       Summary = "Success",
                       Detail = "Material has been save",
                       Duration = 3000
                   });

            dialog.Close(args);

        }

		void CloseDialog()
		{
            dialog.Close();
        }
    }
}

