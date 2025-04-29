To do this you may need to follow this step:
+ Open Visual Studio and create new ASP.NET core MVC
    - choose the last version is 9.0
+ Set up databasee in appsettings.json and that is the example:
    
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=(Your server name);Initial Catalog=(Your database);Integrated Security=True;Trust Server Certificate=True;"
    },
+ install package :
    - look at the top of the Visual Studio click on tool and choose Nuget Package Manager and then choose Package Manager Console then install these:
        Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
        Install-Package Microsoft.EntityFrameworkCore.SqlServer
        Install-Package Microsoft.EntityFrameworkCore.Tools
        Install-Package Microsoft.AspNetCore.Identity.UI

        (the reason why I recommand you to choose 9.0 version when create new ASP.NET core MVC is when you run these command it always install the last version of Indentiry Framework Core 9.0 So if you create new project with another version that not much with Identity Framework Core you will get an error)
+ Set up DbContext
    - create folder name Data inside right click on solution at the right panel and create it
    - inside that folder name Data (Data/ApplicationDbContext.cs) create new class name ApplicationDbContext.cs
    - inside that class past this:
        using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
        using Microsoft.EntityFrameworkCore;

        namespace Yournamespace
        {
            public class ApplicationDbContext : IdentityDbContext
            {
                public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                    : base(options)
                {
                }
            }
        }

+ Create a login model :
    using System.ComponentModel.DataAnnotations;

    namespace Yournamespace
    {
        public class LoginViewModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
+ Create form login Views/Home/Index.cshtml:
    @model Yournamespace.LoginViewModel

    @{
        ViewData["Title"] = "Login Page";
        Layout = null;
    }

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <!-- Tailwind CSS -->
    <script src="https://cdn.tailwindcss.com"></script>

    <div class="container-md  d-flex justify-content-center align-items-center shadow-lg p-5 " style="overflow: hidden; background-color: white; width:60%; margin-top:130px">
        <div class="row w-100">
            <!-- Image Column (Float Right) -->
            <div class="col-md-6 image-container">
                <img src="./images/login.jpg" alt="Login illustration" class="login-image" style="width: 100%;">
            </div>
            <!-- Form Column (Float Left) -->
            <div class="col-md-6 form-container">
                <div class="bg-white p-6 rounded-lg shadow-lg w-full max-w-sm">
                    <h2 class="text-2xl font-bold text-center mb-6">LOGIN</h2>

                    @if (ViewData["Error"] != null)
                    {
                        <p class="text-red-500 text-center">@ViewData["Error"]</p>
                    }

                    <form asp-action="Index" method="post">
                        <div class="mb-4">
                            <label for="Username" class="sr-only">Username</label>
                            <input type="text" id="Username" name="Username" class="w-full p-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="Enter your username" required value="@Model.Username">
                        </div>
                        <div class="mb-4">
                            <label for="Password" class="sr-only">Password</label>
                            <input type="password" id="Password" name="Password" class="w-full p-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="Enter your password" required value="@Model.Password">
                        </div>
                        <button type="submit" class="w-full bg-blue-600 text-white p-2 rounded hover:bg-blue-500 transition">Login</button>
                    </form>
                </div>
            </div>
        </div>
    </div>
+ Create dasboard after login success Views/Home/Dashboard.cshtml:
    @{
    var userName = User.Identity.Name;  // This gives the logged-in user's name (could be email or username depending on your setup)
    }

    <h1>Welcome, @userName!</h1>

+ Modify HomeController
    using Yournamespace
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    namespace Yournamespace
    {
        public class HomeController : Controller
        {
            private readonly UserManager<IdentityUser> _userManager;
            private readonly SignInManager<IdentityUser> _signInManager;

            public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
            }

            [HttpGet]
            public IActionResult Index()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Index(LoginViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user != null)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Dashboard", "Home");
                        }
                    }
                    ViewData["Error"] = "Invalid username or password.";
                }
                return View(model);
            }

            public IActionResult Dashboard()
            {
                return View();
            }
        }
    }


+ Fixed program.cs:
    using Yournamespace
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // FIXED: Correct way to add Identity
    builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // Middleware
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication(); // <-- Make sure Authentication is here
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
+ and use this command in Package Manager Console to migrate your database:
    dotnet ef migrations add InitialCreate
    dotnet ef database update
+ Extra point if you want to test your login but you didn't have any user in database yet you can use this :
    - create this method in homeController 
        public async Task<IActionResult> CreateTestUser()
        {
            var user = new IdentityUser { UserName = "admin" };
            var result = await _userManager.CreateAsync(user, "Password123!");
            return Content(result.Succeeded ? "Test user created successfully" : "Error creating user");
        }
    - then run your project and visit https://localhost:xxxx/Home/CreateTestUser
    - After login → you will redirect to Dashboard page! you will see @User.Identity.Name will show admin at the Dashboard.

