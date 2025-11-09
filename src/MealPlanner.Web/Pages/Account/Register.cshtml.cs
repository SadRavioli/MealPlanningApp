using MealPlanner.Application.DTOs.Users;
using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        IUserService userService,
        ILogger<RegisterModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public RegisterInputDto Input { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var dto = new RegisterDto
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                Password = Input.Password
            };

            var (succeeded, user, errors) = await _userService.RegisterAsync(dto);

            if (succeeded && user != null)
            {
                _logger.LogInformation("User {Email} registered successfully", Input.Email);
                // Redirect to login page after successful registration
                return RedirectToPage("/Account/Login", new { returnUrl });
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            ErrorMessage = "Unable to create account. Please check the errors below.";
        }

        return Page();
    }
}
