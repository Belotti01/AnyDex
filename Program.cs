using AnyDex.Areas.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Diagnostics;

namespace AnyDex {
	public static class Program {

		private static ILogger? Logger { get; set; }

		public static void Main(string[] args) {
			Logger = new LoggerFactory().CreateLogger(typeof(Program));

			WebApplication app;
			WebApplicationBuilder builder;
			string connectionString = "user id=root;host=localhost;database=anydex;password=root";

			Logger.LogInformation("Creating WebApplication Builder");
			builder = WebApplication.CreateBuilder(args);
			Logger.LogInformation("Configuring WebApplication Services");
			AddServices(builder, connectionString);
			ConfigureIdentity(builder);
			ConfigureLogging(builder);

			Logger.LogInformation("Building WebApplication");
			app = builder.Build();
			Logger.LogInformation("Configuring WebApplication");
			ConfigureHttp(app);
			ConfigureEndpoints(app);

			Logger.LogInformation("Initializing Database data");
			InitializeDatabase();

			Logger.LogInformation("Starting Application");
			app.Run();
		}

		private static void InitializeDatabase() {
			using AnyDexDb db = new();
			// Create or update database if needed
			db.Database.Migrate();

			if(Debugger.IsAttached) {
				// Generate test data in the database
				AnyDexDB.Testing.DummyGenerator.GenerateData();
			}
		}

		private static void AddServices(WebApplicationBuilder builder, string connectionString) {
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();
			builder.Services.AddRazorPages();
			builder.Services.AddServerSideBlazor();
			builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<User>>();
			builder.Services.AddMudServices();
			// Add support for ASP.NET MVC Localization
			builder.Services.AddLocalization(x => x.ResourcesPath = "Resources");
			// Suppress null-value warning for entity attributes with the [Required] data validation property
			builder.Services.AddControllers(
				options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true
			);
			// Inject DbContextFactory
			builder.Services.AddDbContextFactory<AnyDexDb>(options => 
				options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
			);
		}

		private static void ConfigureLogging(WebApplicationBuilder builder) {
			builder.Services.AddLogging(options => {
				options.SetMinimumLevel(LogLevel.Trace);
				options.AddConsole();
				options.AddDebug();
			});
		}

		/// <summary>
		/// Set options for the identity Authentication and Authorization services.
		/// </summary>
		private static void ConfigureIdentity(WebApplicationBuilder builder) {
			builder.Services.AddIdentity<User, Role>()
				.AddDefaultTokenProviders()
				.AddEntityFrameworkStores<AnyDexDb>()
				.AddDefaultUI()
				.AddDefaultTokenProviders();

			// Setup options for authentication & cookies
			builder.Services.Configure<IdentityOptions>(options => {
				// Password settings
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 2;

				// Lockout settings
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = false;

				// User settings
				options.User.AllowedUserNameCharacters =
				"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				options.User.RequireUniqueEmail = true;
			});

			builder.Services.ConfigureApplicationCookie(options => {
				// Cookie settings
				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

				options.LoginPath = "/Identity/Account/Login";
				options.AccessDeniedPath = "/Identity/Account/AccessDenied";
				options.SlidingExpiration = true;
			});
		}

		private static void ConfigureHttp(WebApplication app) {
			// Configure the HTTP request pipeline.
			if(app.Environment.IsDevelopment()) {
				app.UseMigrationsEndPoint();
			} else {
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
		}

		private static void ConfigureEndpoints(WebApplication app) {
			app.UseStaticFiles();
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();
			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");
		}
	}
}