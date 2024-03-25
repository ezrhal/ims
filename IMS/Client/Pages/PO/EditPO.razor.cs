using System.Net.Http.Json;
using System.Text.Json;
using DnsClient;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace IMS.Client.Pages.PO
{
	public partial class EditPO
	{
		[Parameter] public POModel po { get; set; }
        [Parameter] public RadzenDataGrid<POModel> poGrid { get; set; }
        List<SupplierModel> suppliers;
        

		protected override async Task OnInitializedAsync()
		{
            suppliers = await httpClient.GetFromJsonAsync<List<SupplierModel>>("purchaseorder/getsuppliers");
        }

        public async Task SavePO()
        {
            SupplierModel supplier = suppliers.Find(q => q.Id.Equals(po.supplierid));
            po.supplier = supplier.supplier;
            po.supplieraddress = supplier.address;

            List<string> paramList = new List<string>();
            paramList.Add(Newtonsoft.Json.JsonConvert.SerializeObject(po));

            var response = await httpClient.PostAsJsonAsync("purchaseorder/editpo", paramList);

            NotificationService.Notify(
                new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "PO has been save",
                    Duration = 3000
                });

            poGrid.Reload();
            DialogService.Close();

        }

        void CloseDialog()
        {
            DialogService.Close();
        }
    }
}

