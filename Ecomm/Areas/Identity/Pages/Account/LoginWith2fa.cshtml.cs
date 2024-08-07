﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.DataAccess.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Ecomm.Areas.Identity.Pages.Account
{
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginWith2faModel> _logger;
        private readonly ISMSSenderService _smSSenderService;
        private readonly IEmailSender _emailSender;

        public LoginWith2faModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginWith2faModel> logger,
            ISMSSenderService sMSSenderService,
            IEmailSender emailSender)
           
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _smSSenderService = sMSSenderService;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
            public string TwoFactAuthProviderName {  get; set; }    
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);

                if (providers.Any(_ => _ == "Phone"))
                {
                    Input.TwoFactAuthProviderName = "Phone";
                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
                    await _smSSenderService.SendSmsAsync(user.PhoneNumber, $"OTP Code: ${token}");
                }
                else if (providers.Any(_ => _ == "Email"))
                {
                    Input.TwoFactAuthProviderName = "Email";
                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    await _emailSender.SendEmailAsync(user.Email, "2FA Code",
                     $"<h3 >{token}</h3>.");
                }
            }
            else
            {
                ReturnUrl = returnUrl;
                RememberMe = rememberMe;
            }
                return Page();
        }
        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {

                var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

                //var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);
                var result = await _signInManager.TwoFactorSignInAsync(Input.TwoFactAuthProviderName, authenticatorCode, rememberMe, Input.RememberMachine);
                var userId = await _userManager.GetUserIdAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                    return LocalRedirect(returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                    return RedirectToPage("./Lockout");
                }
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return Page();
            }
            return Page();
        }
    }
}
