﻿
using System.Net.Http.Headers;
using HR.LeaveManagement.MVC.Contracts;

namespace HR.LeaveManagement.MVC.Services.Base
{
    public class BaseHttpService
    {
        protected readonly ILocalStorageService _localStorage;
        protected IClient _client;

        public BaseHttpService(ILocalStorageService localStorage, IClient client)
        {
            _localStorage = localStorage;
            _client = client;
        }

        protected Response<Guid> ConvertApiExceptions<Guid>(ApiException ex)
        {
            return ex.StatusCode switch
            {
                400 => new Response<Guid>() { Message = "Validation erros have occured.", ValidationErrors = ex.Response, Success = false},
                404 => new Response<Guid>() {Message = "The requested item could not be found.", Success = false},
                _ => new Response<Guid>() {Message = "Something went wrong, please try again.", Success = false}
            };
        }

        protected void AddBearerToken()
        {
            if (_localStorage.Exits("token"))
                _client.HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _localStorage.GetStorageValue<string>("token"));
        }
    }
}
