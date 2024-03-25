using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using IMS.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;

namespace IMS.Client.Pages
{
	public partial class Login
	{
        public bool IsMedium = false;
        public bool IsSmall = false;
        LoginModel login = new();
        
        public async Task AuthUser()
		{
            bool invalidInput = false;
            string msg = "";
            string style = "";

            if (login.Username.Trim().Length == 0)
            {
                invalidInput = true;
                msg = "Username is required!";
            }
            else if (login.Password.Trim().Length == 0)
            {
                invalidInput = true;
                msg = "Password is required!";
            }

            if (IsSmall)
            {
                style = "position: fixed; top: 5%; left: 50%; transform: translate(-50%, -50%);";
            }

            if (!invalidInput)
            {
                var loginAsJson = JsonSerializer.Serialize(login);
                var response = await httpClient.PostAsJsonAsync("api/account/login", login);

                var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Console.WriteLine(loginResult.Successful);

                if (loginResult.Successful)
                {
                    await _localStorage.SetItemAsync("authToken", loginResult.Token);
                    ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(login.Username);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    NotificationService.Notify(
                        new NotificationMessage
                        {
                            Style = style,
                            Severity = NotificationSeverity.Error,
                            Summary = "Invalid",
                            Detail = "Invalid username or password.",
                            Duration = 3000
                        }); ;
                }
            }
            else
            {
                NotificationService.Notify(
                new NotificationMessage
                {
                    Style = style,
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = msg,
                    Duration = 3000
                }) ;

                invalidInput = false;
            }
                        

            
        }

        public async Task OnChange(bool match)
        {
            IsSmall = match;
        }

	}
}

