using IMS.Shared.Models;

namespace IMS.Client.Shared;

public partial class NavigationMenu
{
   public LoginModel userLogin;
   
   protected override async Task OnInitializedAsync()
   { 
      userLogin =  await _localStorage.GetItemAsync<LoginModel>("userLogin");
      
   }
}