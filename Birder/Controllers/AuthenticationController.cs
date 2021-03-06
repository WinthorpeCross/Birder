﻿using Birder.Data.Model;
using Birder.Helpers;
using Birder.Services;
using Birder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Birder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISystemClockService _systemClock;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AuthenticationController(UserManager<ApplicationUser> userManager
                                        , SignInManager<ApplicationUser> signInManager
                                        , ILogger<AuthenticationController> logger
                                        , ISystemClockService systemClock
                                        , IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _systemClock = systemClock;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel loginViewModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.UserName);

                if (user is null)
                {
                    _logger.LogError(LoggingEvents.GetItemNotFound, "Login failed: User not found");
                    return StatusCode(500, new AuthenticationResultDto() { FailureReason = AuthenticationFailureReason.Other });
                }

                if (user.EmailConfirmed == false)
                {
                    _logger.LogInformation("EmailNotConfirmed", "You cannot login until you confirm your email.");
                    return StatusCode(500, new AuthenticationResultDto() { FailureReason = AuthenticationFailureReason.EmailConfirmationRequired });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginViewModel.Password, false);

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return StatusCode(500, new AuthenticationResultDto() { FailureReason = AuthenticationFailureReason.LockedOut });
                }

                //ToDo: move to separate Service?
                if (result.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim("ImageUrl", user.Avatar),
                        new Claim("DefaultLatitude", user.DefaultLocationLatitude.ToString()),
                        new Claim("DefaultLongitude", user.DefaultLocationLongitude.ToString()),
                        new Claim("FlickrKey", _configuration["FlickrApiKey"]),
                        new Claim("MapKey", _configuration["MapApiKey"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"]));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                    var baseUrl = string.Concat(_configuration["Scheme"], _configuration["Domain"]);

                    var tokenOptions = new JwtSecurityToken(
                        issuer: baseUrl,
                        audience: baseUrl,
                        claims: claims,
                        expires: _systemClock.GetNow.AddDays(2),
                        signingCredentials: signinCredentials);

                    var viewModel = new AuthenticationResultDto()
                    {
                        FailureReason = AuthenticationFailureReason.None,
                        AuthenticationToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions)
                    };

                    return Ok(viewModel);
                }

                _logger.LogWarning(LoggingEvents.GenerateItems, "Other authentication failure");
                return StatusCode(500, new AuthenticationResultDto() { FailureReason = AuthenticationFailureReason.Other });
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.Exception, ex, "An unexpeceted error occurred");
                return StatusCode(500, new AuthenticationResultDto() { FailureReason = AuthenticationFailureReason.Other });
            }
        }
    }
}
