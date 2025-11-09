using MealPlanner.Application.DTOs.Users;
using MealPlanner.Application.Services;
using MealPlanner.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        IUserService userService,
        SignInManager<ApplicationUser> signInManager,
        ILogger<LoginModel> logger)
    {
        _userService = userService;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public LoginInputDto Input { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var dto = new LoginDto
            {
                Email = Input.Email,
                Password = Input.Password,
                RememberMe = Input.RememberMe
            };

            // Verify credentials with UserService
            var (succeeded, user) = await _userService.LoginAsync(dto);

            if (succeeded && user != null)
            {
                // Sign in using SignInManager
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in", Input.Email);
                    return LocalRedirect(returnUrl);
                }
            }

            ErrorMessage = "Invalid login attempt.";
            return Page();
        }

        return Page();
    }
}
