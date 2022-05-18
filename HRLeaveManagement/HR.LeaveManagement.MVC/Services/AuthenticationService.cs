using AutoMapper;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models;
using HR.LeaveManagement.MVC.Services.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace HR.LeaveManagement.MVC.Services
{
    public class AuthenticationService : BaseHttpService, Contracts.IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private JwtSecurityTokenHandler _tokenHandler;
        private readonly IMapper _mapper;

        public AuthenticationService(ILocalStorageService localStorage, IClient client, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
            : base(localStorage, client)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._tokenHandler = new JwtSecurityTokenHandler();
            this._mapper = mapper;
        }

        public async Task<bool> Authenticate(string email, string password)
        {
            try
            {
                AuthRequest authenticateRequest = new() {Email = email, Password = password};
                var authenticateResponse = await _client.LoginAsync(authenticateRequest);

                if (authenticateResponse.Token != string.Empty)
                {
                    var tokenContent = _tokenHandler.ReadJwtToken(authenticateResponse.Token);
                    var claims = ParseClaims(tokenContent);
                    var user = new ClaimsPrincipal(new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme));
                    if (_httpContextAccessor.HttpContext != null)
                    {
                        var login = _httpContextAccessor.HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme, user);
                    }

                    _localStorage.SetStorageValue("token", authenticateResponse.Token);

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Register(RegisterVM registration)
        {
            var registrationRequest = _mapper.Map<RegistrationRequest>(registration);

            var response = await _client.RegisterAsync(registrationRequest);

            if (!string.IsNullOrEmpty(response.UserId))
            {
                await Authenticate(registration.Email, registration.Password);
                return true;
            }
            
            return false;
        }

        public async Task Logout()
        {
            _localStorage.ClearStorage(new List<string>(){"roken"});
            if (_httpContextAccessor.HttpContext != null)
                await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        {
            var claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }
    }
}
