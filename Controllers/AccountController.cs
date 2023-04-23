using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PojistovnaApp.Models;
using System.Diagnostics;

namespace PojistovnaApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Zobrazení register formuláře
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Zpracování register formuláře
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            // Ověří, zda jsou data z modelu validní
            if (ModelState.IsValid)
            {
                // Vytvoří nový IdentityUser objekt s názvem uživatele a e-mailem nastaveným na hodnotu z modelu
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                // Vytvoří nový uživatelský účet s heslem z modelu
                var result = await _userManager.CreateAsync(user, model.Password);
                // Pokud se úspěšně podaří vytvořit uživatelský účet
                if (result.Succeeded)
                {
                    // Uživatel se přihlásí
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    // Přesměruje se na akci Index v HomeControlleru
                    return RedirectToAction("Index", "Home");
                }
                // Pokud došlo k chybě při vytváření uživatele
                foreach (var error in result.Errors)
                {
                    // Chyba se přidá do objektu ModelState a zobrazí se ve formuláři
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // Pokud došlo k chybě validace nebo při vytváření uživatele,
            // metoda se vrátí na původní stránku s modelem, aby uživatel mohl opravit chyby a odeslat formulář znovu
            return View(model);
        }

        // Zobrazení login formuláře
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Zpracování login formuláře
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            // Nastavení návratového URL pro zobrazení na další stránce
            ViewData["ReturnUrl"] = returnUrl;
            // Ověří, zda jsou data z modelu validní
            if (ModelState.IsValid)
            {
                // Ověří přihlašovací údaje pomocí _signInManager.PasswordSignInAsync metody
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                // Pokud jsou údaje správné
                if (result.Succeeded)
                {
                    // Uživatele přesměruje na cílovou stránku uloženou v returnUrl parametru
                    return RedirectToLocal(returnUrl);
                }
                // Pokud jsou údaje neplatné
                else
                {
                    // Chyba se přidá do objektu ModelState a zobrazí se ve formuláři
                    ModelState.AddModelError(string.Empty, "Neplatné přihlašovací údaje.");
                    return View(model);
                }
            }

            // Pokud byly odeslány neplatné údaje, vrátíme uživatele k přihlašovacímu formuláři
            return View(model);
        }

        // Zpracování odhlášení
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Odhlášení uživatele pomocí _signInManager.SignOutAsync metody
            await _signInManager.SignOutAsync();
            // Přesměrování na akci Index v HomeControlleru
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            // Pokud je cílové URL místní, uživatel je na toto URL přesměrován
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            // Jinak je uživatel přesměrován na akci Index v HomeControlleru
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
