﻿using System;
namespace IMS.Shared.Models
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }

        public LoginModel UserLogin { get; set; }
    }
}

