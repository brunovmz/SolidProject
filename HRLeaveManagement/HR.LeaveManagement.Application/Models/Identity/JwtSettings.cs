﻿using System;

namespace HR.LeaveManagement.Application.Models.Identity
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public Double DurationInMinutes { get; set; }
    }
}
