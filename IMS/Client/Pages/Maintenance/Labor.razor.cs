using IMS.Shared.Models;
using System.Net.Http.Json;
using Radzen;
using Radzen.Blazor;
using IMS.Client.Shared;


namespace IMS.Client.Pages.Maintenance
{
    partial class Labor
    {
        List<ItemModel> labor = new List<ItemModel>();
        List<ItemModel> filteredlabor = new List<ItemModel>();
        bool isLoading = false;
        RadzenDataGrid<ItemModel> grid;

        IList<ItemModel> selectedItems;
        int cnt = 0;

        string pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        protected override async Task OnInitializedAsync()
        {
            labor = await httpClient.GetFromJsonAsync<List<ItemModel>>("maintenance/getitemsbytype?typeid=3");
            filteredlabor = labor;
            cnt = filteredlabor.Count();
        }

        void OnSearch(string Value)
        {
            if (Value.Length > 0)
            {
                filteredlabor = labor.Where(q => q.item.ToLower().Contains(Value.ToLower()) || q.description.ToLower().Contains(Value.ToLower())).ToList();
            }
            else
            {
                filteredlabor = labor;
            }
        }

        async Task LoadData(LoadDataArgs args)
        {
            StateHasChanged();
        }

        public async Task NewMaterial()
        {
            ItemModel item = new ItemModel();
            item.typeid = 3;

            var result = await DialogService.OpenAsync<AddMaterials>("Add new material",
                   new Dictionary<string, object>() { { "item", item }, {"itemtype", 3} },
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });

            if (result != null)
            {
                labor.Add(result);
                filteredlabor = labor;
                grid.Reload();
            }
        }

        #region EDIT

        public async Task EditMaterial(string id)
        {
            ItemModel item = labor.Find(e => e.Id.Equals(id)); //await httpClient.GetFromJsonAsync<ItemModel>("maintenance/getitem?id=" + id);

            var result = await DialogService.OpenAsync<AddMaterials>("Edit material",
                   new Dictionary<string, object>() { { "item", item }, { "edit", 1 } },
                   new DialogOptions() { Width = "500px", Resizable = false, Draggable = true });

            if (result != null)
            {
                item = result;
                filteredlabor = labor;
                grid.Reload();
            }
        }

        #endregion

        #region DELETE


        public async Task DeleteItems()
        {
            if (selectedItems != null && selectedItems.Count > 0)
            {
                var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                          new Dictionary<string, object>() { { "message", "Are you sure you want to delete the selected items?" } },
                          new DialogOptions() { Width = "350px", Resizable = false, Draggable = true });

                if (result != null && result)
                {
                    List<string> ids = new List<string>();

                    foreach (ItemModel selectedItem in selectedItems)
                    {
                        ids.Add(selectedItem.Id);
                    }

                    var res = await httpClient.PostAsJsonAsync<List<string>>("maintenance/deleteitem", ids);
                    int id = res.Content.ReadFromJsonAsync<int>().Result;

                    if (id > 0)
                    {
                        foreach (ItemModel selectedItem in selectedItems)
                        {
                            var itemToRemove = labor.Single(r => r.Id.Equals(selectedItem.Id));
                            labor.Remove(itemToRemove);
                        }

                        filteredlabor = labor;
                        grid.Reload();

                        NotificationService.Notify(
                          new NotificationMessage
                          {
                              Severity = NotificationSeverity.Success,
                              Summary = "Success",
                              Detail = "Materials has been deleted",
                              Duration = 3000
                          });
                    }
                }

            }

        }

        public async Task DeleteItem(string Id)
        {
             var result = await DialogService.OpenAsync<ConfirmDialog>("Confirm Delete",
                          new Dictionary<string, object>() { { "message", "Are you sure you want to delete the selected equipment?" } },
                          new DialogOptions() { Width = "350px", Resizable = false, Draggable = true });

            if (result != null && result)
            {
                List<string> ids = new List<string>();

                ids.Add(Id);

                var res =  await httpClient.PostAsJsonAsync<List<string>>("maintenance/deleteitem", ids);
                int id = res.Content.ReadFromJsonAsync<int>().Result;

                if (id > 0)
                {
                    var itemToRemove = labor.Single(r => r.Id.Equals(Id));
                    labor.Remove(itemToRemove);

                    filteredlabor = labor;
                    grid.Reload();

                    NotificationService.Notify(
                        new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Success",
                            Detail = "Material has been deleted",
                            Duration = 3000
                        });
                }
            }
            
        }

        #endregion

    }
}
