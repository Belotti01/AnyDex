using AnyDex.Areas.Identity;
using AnyDex.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace AnyDex {
	public static class Program {
		public static void Main(string[] args) {
			WebApplication app;
			WebApplicationBuilder builder;
			string connectionString = "user id=root;host=localhost;database=anydex;password=root";

			builder = WebApplication.CreateBuilder(args);
			AddServices(builder, connectionString);
			ConfigureIdentity(builder);

			app = builder.Build();
			ConfigureHttp(app);
			ConfigureEndpoints(app);

			app.Run();
		}

		private static void AddServices(WebApplicationBuilder builder, string connectionString) {
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();
			builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddEntityFrameworkStores<ApplicationDbContext>();
			builder.Services.AddRazorPages();
			builder.Services.AddServerSideBlazor();
			builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
			builder.Services.AddSingleton<WeatherForecastService>();        // <- To remove
			builder.Services.AddAntDesign();
			// Add support for ASP.NET MVC Localization
			builder.Services.AddLocalization();
			// Suppress null-value warning for entity attributes with the [Required] data validation property
			builder.Services.AddControllers(
				options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true
			);
			// Inject DbContextFactory
			builder.Services.AddDbContextFactory<AnyDexDb>(options => 
				options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
			);
		}

		/// <summary>
		/// Set options for the identity Authentication and Authorization services.
		/// </summary>
		private static void ConfigureIdentity(WebApplicationBuilder builder) {
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

			builder.Services.ConfigureApplicationCookie(options =>
			{
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