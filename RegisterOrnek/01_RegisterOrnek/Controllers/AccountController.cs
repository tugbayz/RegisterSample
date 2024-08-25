using _01_RegisterOrnek.MailServices;
using _01_RegisterOrnek.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _01_RegisterOrnek.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMailService _mailService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(IMailService mailService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _mailService = mailService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var codeToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Home", new { userId = user.Id, code = codeToken }, protocol: Request.Scheme);
                var mailmessage = $"Please confirm your account by<br/> <a href='{callbackUrl}'clicking here<a/>";
                await _mailService.SendMailAsync(model.Email, "Confirm your email", mailmessage);

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("kullanıcı geçersiz");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "Index" : "Error");
        }
    }
}
