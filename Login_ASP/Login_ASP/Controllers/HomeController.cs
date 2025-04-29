using Login_ASP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Login_ASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        // Inject the UserManager and SignInManager
        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Home/Index
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        // POST: /Home/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the user exists
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    // Attempt to sign in the user with the provided password
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        // If login is successful, you can redirect to a different page (e.g., Home/Index or Dashboard)
                        return RedirectToAction("Dashboard", "Home");  // Replace with your target page
                    }
                    else
                    {
                        // If login fails, show an error message
                        ViewData["Error"] = "Invalid username or password.";
                    }
                }
                else
                {
                    // If the user doesn't exist, show an error message
                    ViewData["Error"] = "Invalid username or password.";
                }
            }

            // If validation fails or login fails, redisplay the form
            return View(model);
        }

        // Redirect to dashboard or other pages for logged-in users
        public IActionResult Dashboard()
        {
            return View();  // Redirect to your actual dashboard view here
        }

        // Optional: Privacy page
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
