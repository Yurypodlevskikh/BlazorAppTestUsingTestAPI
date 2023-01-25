using DeviesReadsBlazorApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace DeviesReadsBlazorApp.Data
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private ProtectedLocalStorage _localStorage;
        private readonly IDeviesReadAuthentication _drAuthentication;
        private readonly IDeviesReadUser _drUser;

        public CustomAuthStateProvider(
            ProtectedLocalStorage localStorage, 
            IDeviesReadAuthentication drAuthentication,
            IDeviesReadUser drUser)
        {
            _localStorage = localStorage;
            _drAuthentication = drAuthentication;
            _drUser = drUser;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //UserLocalStorage oldUser = new UserLocalStorage
            //{
            //    UserId = "5ad9c27b-70a3-4a99-abe2-40ae45e25fba",
            //    UserName = "Yurkinsson",
            //    AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Ill1cmtpbnNzb24iLCJzdWIiOiI1YWQ5YzI3Yi03MGEzLTRhOTktYWJlMi00MGFlNDVlMjVmYmEiLCJpYXQiOjE2NzQxNjY4NzMsImV4cCI6MTY4MTk0Mjg3M30.seNgkjBu71rWcitCvYEjh77zitcbZEdoQGwXOAUQLXc"
            //};

            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var userLocalStorage = await _localStorage.GetAsync<UserLocalStorage>("UserLocalStorage");
            
            //if (await _drAuthentication.IsLoggedIn(oldUser.AccessToken))
            //{
            //    claimsIdentity = new ClaimsIdentity(new[]
            //    {
            //                new Claim(ClaimTypes.Name, oldUser.UserName),
            //            }, "Devies api authentication");
            //}

            if (userLocalStorage.Success)
            {
                var userSession = userLocalStorage.Value;

                // Check if the user has been removed.
                if (await _drUser.UserExists(userLocalStorage.Value.UserId))
                {
                    //if (await _drAuthentication.IsLoggedIn(userSession.AccessToken))
                    //{
                    //    claimsIdentity = new ClaimsIdentity(new[]
                    //    {
                    //        new Claim(ClaimTypes.Name, userSession.UserName),
                    //    }, "Devies api authentication");
                    //}

                    claimsIdentity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, userSession.UserName),
                        }, "Devies api authentication");
                }
                else
                {
                    // The User has been removed. Clear the session.
                    await _localStorage.DeleteAsync("UserLocalStorage");
                }
            }

            var user = new ClaimsPrincipal(claimsIdentity);
            
            return await Task.FromResult(new AuthenticationState(user));
        }

        public async Task<bool> Login(RegisterDTO user)
        {
            var userLocalStorage = await _drAuthentication.Login(user);
            if (userLocalStorage != null && !string.IsNullOrEmpty(userLocalStorage.AccessToken))
            {
                await _localStorage.SetAsync("UserLocalStorage", userLocalStorage);

                var claimsIdentity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, userLocalStorage.UserName),
                        }, "Devies api authentication");

                var userClaims = new ClaimsPrincipal(claimsIdentity);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(userClaims)));

                return true;
            }

            return false;
        }

        public async Task<bool> Register(RegisterDTO user)
        {
            UserAuthResponseDTO response = await _drAuthentication.Register(user);
            if(response != null && (response.UserId != null && response.AccessToken != null)) 
            {
                UserLocalStorage userLocalStorage = new UserLocalStorage
                {
                    UserId = response.UserId,
                    AccessToken = response.AccessToken
                };
                await _localStorage.SetAsync("UserLocalStorage", userLocalStorage);
                return true;
            }
            return false;
        }
    }
}
