using IMS.Shared.Models;
using System.Net.Http.Json;
using Radzen;
using Radzen.Blazor;
using IMS.Client.Shared;
using Microsoft.AspNetCore.Components;
using System;

namespace IMS.Client.Pages.Maintenance
{
	partial class Unit
	{
        List<UnitModel> units = new List<UnitModel>();
        List<UnitModel> filteredunits = new List<UnitModel>();
        bool isLoading = false;
        RadzenDataGrid<UnitModel> grid;
        IList<UnitModel> selectedUnits;

        int cnt = 0;


        protected override async Task OnInitializedAsync()
        {
            units = await httpClient.GetFromJsonAsync<List<UnitModel>>("lookup/getunits");
            filteredunits = units;
            cnt = filteredunits.Count();

        }

        void OnSearch(string Value)
        {
            if (Value.Length > 0)
            {
                filteredunits = units.Where(q => q.unit.ToLower().Contains(Value.ToLower())).ToList();
            }
            else
            {
                filteredunits = units;
            }
        }

        public async Task NewUnit()
        {
            ItemModel item = new ItemModel();

            var result = await DialogService.OpenAsync<AddMaterials>("Add new Unit",
                   new Dictionary<string, object>() { { "item", item } },
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });

            if (result != null)
            {
                units.Add(result);
                filteredunits = units;
                grid.Reload();
            }
        }


        #region EDIT

        public async Task EditUnit(string id)
        {
            UnitModel unit = units.Find(e => e.Id.Equals(id)); //await httpClient.GetFromJsonAsync<ItemModel>("maintenance/getitem?id=" + id);

            var result = await DialogService.OpenAsync<AddMaterials>("Edit unit",
                   new Dictionary<string, object>() { { "item", unit }, { "edit", 1 } },
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });

            if (result != null)
            {
                unit = result;
                filteredunits = units;
                grid.Reload();
            }
        }

        #endregion

        #region DELETE


        public async Task DeleteItems()
        {
            if (selectedUnits != null && selectedUnits.Count > 0)
            {
                var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                          new Dictionary<string, object>() { { "message", "Are you sure you want to delete the selected unit?" } },
                          new DialogOptions() { Width = "350px", Resizable = false, Draggable = true });

                if (result != null && result)
                {
                    List<string> ids = new List<string>();

                    foreach (UnitModel selectedUnit in selectedUnits)
                    {
                        ids.Add(selectedUnit.Id);
                    }

                    var res = await httpClient.PostAsJsonAsync<List<string>>("maintenance/deleteunit", ids);
                    int id = res.Content.ReadFromJsonAsync<int>().Result;

                    if (id > 0)
                    {
                        foreach (UnitModel selectedUnit in selectedUnits)
                        {
                            var unitToRemove = units.Single(r => r.Id.Equals(selectedUnit.Id));
                            units.Remove(unitToRemove);
                        }

                        filteredunits = units;
                        grid.Reload();

                        NotificationService.Notify(
                          new NotificationMessage
                          {
                              Severity = NotificationSeverity.Success,
                              Summary = "Success",
                              Detail = "Unit has been deleted",
                              Duration = 3000
                          });
                    }
                }

            }

        }

        #endregion
    }
}