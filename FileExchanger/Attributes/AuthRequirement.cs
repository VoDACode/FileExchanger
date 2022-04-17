using FileExchanger.Configs;
using Microsoft.AspNetCore.Authorization;
using System;

namespace FileExchanger.Attributes
{
    public class AuthRequirement : IAuthorizationRequirement
    {
        public DefaultService Service { get; }

        public AuthRequirement(DefaultService service)
        {
            Service = service;
        }
    }
}
