using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorIdentity.Identity.Controllers
{
    public class IdentityController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger logger;

        public IdentityController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<Startup> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpPost("Identity/LogIn")]
        public async Task<IActionResult> Signin([FromForm] string username, [FromForm] string password)
        {
            var result = await signInManager.PasswordSignInAsync(username, password, true, lockoutOnFailure: true);
            
            if (result.Succeeded)
            {
                return Redirect("/");
            }

            if (result.RequiresTwoFactor)
            {
                return Redirect("/Identity/LoginWith2fa");
            }

            if (result.IsLockedOut)
            {
                return Redirect("/Identity/Lockout");
            }

            return Redirect("/Identity/Login/Failed");  
        }

        [HttpPost("Identity/LogOut")]
        public async Task<IActionResult> Index()
        {
            await signInManager.SignOutAsync();
            return Redirect("/");
        }

        [Authorize]
        [HttpGet("Identity/Manage/DownloadPersonalData")]
        public async Task<IActionResult> DownloadPersonalData()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            logger.LogInformation("User with ID '{UserId}' asked for their personal data.", userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(IdentityUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
        }
    }
}
