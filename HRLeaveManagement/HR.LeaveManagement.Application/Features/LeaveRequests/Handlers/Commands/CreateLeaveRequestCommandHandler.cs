﻿
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Infrastructure;
using HR.LeaveManagement.Application.DTOs.LeaveRequest.Validators;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Commands;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Models;
using HR.LeaveManagement.Application.Responses;
using HR.LeaveManagement.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Commands
{
    public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, BaseCommandResponse>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository, 
            ILeaveTypeRepository leaveTypeRepository,
            ILeaveAllocationRepository leaveAllocationRepository,
            IMapper mapper,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _mapper = mapper;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
            _leaveAllocationRepository = leaveAllocationRepository;
        }
        public async Task<BaseCommandResponse> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var validator = new CreateLeaveRequestDtoValidator(_leaveTypeRepository);
            var validationResult = await validator.ValidateAsync(request.LeaveRequestDto, cancellationToken);
            var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(q => q.Type == "uid")?.Value;

            var allocation =
                await _leaveAllocationRepository.GetUserAllocations(userId, request.LeaveRequestDto.LeaveTypeId);
            var daysRequested = (int) (request.LeaveRequestDto.EndDate - request.LeaveRequestDto.StartDate).TotalDays;

            if (daysRequested > allocation.NumberOfDays)
            {
                validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(
                    nameof(request.LeaveRequestDto.EndDate), "You do not have enough days for this request"));
            }

            if (validationResult.IsValid == false)
            {
                response.Success = false;
                response.Message = "Creation Failed";
                response.Errors = validationResult.Errors.Select(q => q.ErrorMessage).ToList();
            }
            else
            {
                var leaveRequest = _mapper.Map<LeaveRequest>(request.LeaveRequestDto);
                leaveRequest.RequestingEmplpoyeeId = userId;
                leaveRequest = await _leaveRequestRepository.Add(leaveRequest);

                response.Success = true;
                response.Message = "Request Created Successfully";
                response.Id = leaveRequest.Id;
                
                try
                {
                    var emailAddress = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                    var email = new Email
                    {
                        To = emailAddress,
                        Body =
                            $"Your leave request for {request.LeaveRequestDto.StartDate:D} to {request.LeaveRequestDto.EndDate:D} " +
                            $"has been submitted successfully",
                        Subject = "Leave Request Submitted"
                    };
                    await _emailSender.SendEmail(email);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            
            return response;
        }
    }
}
