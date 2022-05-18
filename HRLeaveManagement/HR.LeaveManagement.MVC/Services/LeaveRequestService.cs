using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models;
using HR.LeaveManagement.MVC.Services.Base;

namespace HR.LeaveManagement.MVC.Services
{
    public class LeaveRequestService : BaseHttpService, ILeaveRequestService
    {
        private readonly IMapper _mapper;
        private readonly ILocalStorageService _localStorageService;
        private readonly IClient _httpClient;
        public LeaveRequestService(ILocalStorageService localStorageService, IClient httpClient, IMapper mapper) : base(localStorageService, httpClient)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public async Task<AdminLeaveRequestViewVM> GetAdminLeaveRequestList()
        {
            throw new NotImplementedException();
        }

        public async Task<EmployeeLeaveRequestViewVM> GetUserLeaveRequests()
        {
            throw new NotImplementedException();
        }

        public async Task<Response<int>> CreateLeaveRequest(CreateLeaveRequestVM leaveRequest)
        {
            try
            {
                var response = new Response<int>();
                var createLeaveRequest = _mapper.Map<CreateLeaveRequestDto>(leaveRequest);
                AddBearerToken();
                var apiResponse = await _client.LeaveRequestsPOSTAsync(createLeaveRequest);
                if (apiResponse.Success)
                {
                    response.Data = apiResponse.Id;
                    response.Success = true;
                }
                else
                {
                    foreach (var error in apiResponse.Errors)
                    {
                        response.ValidationErrors += error + Environment.NewLine;
                    }
                }

                return response;
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<int>(ex);
            }
        }

        public async Task DeleteLeaveRequest(int id)
        {
            throw new NotImplementedException();
        }

    
    }
}
