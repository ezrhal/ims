using System.Net.Http.Json;
using IMS.Shared.Models;

namespace IMS.Client.Pages.Project;

public partial class Sample
{
    private string myname = "Hero";

    private TestNumber number = new();

    protected override async Task OnInitializedAsync()
    {
        number.number1 = 1;
        number.number2 = 4;
        var response = await httpClient.PostAsJsonAsync("project/testresponse", number);

        int result = await response.Content.ReadFromJsonAsync<int>();
        
        Console.WriteLine(result);
    }
    
}