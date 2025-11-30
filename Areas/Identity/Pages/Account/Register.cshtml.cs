using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BeFit.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email jest wymagany.")]
            [EmailAddress(ErrorMessage = "Nieprawidłowy format email.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Hasło jest wymagane.")]
            [StringLength(100, ErrorMessage = "Hasło musi mieć od {2} do {1} znaków.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło")]
            [Compare("Password", ErrorMessage = "Hasła nie są identyczne.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            return Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            
            if (ModelState.IsValid)
            {
                var user = new IdentityUser 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    EmailConfirmed = true // Auto-confirm for development
                };
                
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Utworzono nowe konto użytkownika.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                
                var addedMessages = new HashSet<string>();
                foreach (var error in result.Errors)
                {
                    var errorMessage = error.Code switch
                    {
                        "DuplicateUserName" => "Ten email jest już zajęty.",
                        "DuplicateEmail" => "Ten email jest już zajęty.",
                        "PasswordTooShort" => "Hasło jest za krótkie.",
                        "PasswordRequiresNonAlphanumeric" => "Hasło musi zawierać znak specjalny.",
                        "PasswordRequiresDigit" => "Hasło musi zawierać cyfrę.",
                        "PasswordRequiresLower" => "Hasło musi zawierać małą literę.",
                        "PasswordRequiresUpper" => "Hasło musi zawierać wielką literę.",
                        _ => error.Description
                    };
                    
                    // Avoid duplicate messages
                    if (addedMessages.Add(errorMessage))
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                    }
                }
            }

            return Page();
        }
    }
}

