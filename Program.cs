using AnyDex.Areas.Identity;
using AnyDex.Data;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.Diagnostics;

namespace AnyDex {
	public static class Program {
		private const bool RELOAD_DB_DATA = false;

		private static IConfiguration? _configuration;

		public static async Task Main(string[] args) {
			LoadConfiguration();
			Console.ForegroundColor = ConsoleColor.Cyan;

			string separatorLine = new('-', 50);
			string connectionStringAlias = "MainConnection";
			WebApplication app;
			WebApplicationBuilder builder;
			string connectionString = _configuration.GetConnectionString(connectionStringAlias);
			ServerVersion dbServerVersion = ServerVersion.AutoDetect(connectionString);
			Console.WriteLine(separatorLine);
			PrintStartupInformation(connectionStringAlias, dbServerVersion);
			Console.WriteLine(separatorLine);

			Console.Write("Creating WebApplication Builder...");
			builder = WebApplication.CreateBuilder(args);
			Console.WriteLine(" Done");
			Console.Write("Configuring WebApplication Services...");
			AddServices(builder, connectionString, dbServerVersion);
			ConfigureIdentity(builder);
			ConfigureLogging(builder);
			Console.WriteLine(" Done");
			Console.WriteLine(separatorLine);

			Console.Write("Building WebApplication...");
			app = builder.Build();
			Console.WriteLine(" Done");
			Console.Write("Configuring WebApplication...");
			ConfigureHttp(app);
			ConfigureEndpoints(app);
			ConfigureLocalization(app);
			Console.WriteLine(" Done");
			Console.WriteLine(separatorLine);

			Console.Write("Loading Database data:\n");
			await InitializeDatabaseAsync();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(separatorLine);
			Console.Write("\n\n\n");
			Console.ResetColor();

			app.Run();
		}

		private static void PrintStartupInformation(string connectionStringAlias, ServerVersion dbServerVersion) {
			Console.Title = $"AnyDex v{Environment.Version}";

			// Log startup information
			Console.WriteLine($"Running on {Environment.OSVersion.VersionString}");
			Console.WriteLine($"Database Server Version: {dbServerVersion}");
			Console.WriteLine($"Using connection string \"{connectionStringAlias}\"");
		}

		[MemberNotNull(nameof(_configuration))]
		private static void LoadConfiguration() {
			ConfigurationBuilder configBuilder = new();
			configBuilder
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");
			_configuration = configBuilder.Build();
		}

		private static async Task InitializeDatabaseAsync() {
			using AnyDexDb db = new();
			// Create or update database if needed
			await db.Database.MigrateAsync();

			if(Debugger.IsAttached) {
				// Generate test data in the database
				bool createUsers = !db.Users.Any();
				AnyDexDB.Testing.DummyGenerator.GenerateData(createUsers, RELOAD_DB_DATA);
				AnyDexDB.Testing.DummyGenerator.LogData();
			}
		}

		private static void ConfigureLocalization(WebApplication app) {
			// Add support for ASP.NET MVC Localization
			var cultures = new string[] {
				"en-US", "it-IT"
			};
			int defaultCultureIndex = 0;
			
			var localizationOptions = new RequestLocalizationOptions()
				.SetDefaultCulture(cultures[defaultCultureIndex])
				.AddSupportedCultures(cultures)
				.AddSupportedUICultures(cultures);

			app.UseRequestLocalization(localizationOptions);
		}

		private static void AddServices(WebApplicationBuilder builder, string connectionString, ServerVersion serverVersion) {
			builder.Services.AddScoped<DialogService>();
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();
			builder.Services.AddRazorPages();
			builder.Services.AddServerSideBlazor();
			builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<User>>();
			builder.Services.AddMudServices(config => {
				config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;

				config.SnackbarConfiguration.PreventDuplicates = false;
				config.SnackbarConfiguration.NewestOnTop = false;
				config.SnackbarConfiguration.ShowCloseIcon = true;
				config.SnackbarConfiguration.VisibleStateDuration = 5000;
				config.SnackbarConfiguration.HideTransitionDuration = 500;
				config.SnackbarConfiguration.ShowTransitionDuration = 500;
				config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
				config.SnackbarConfiguration.ClearAfterNavigation = true;
				config.SnackbarConfiguration.MaxDisplayedSnackbars = 4;
			});
			// Suppress null-value warning for entity attributes with the [Required] data validation property
			builder.Services.AddControllers(
				options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true
			);
			// Inject DbContextFactory
			builder.Services.AddDbContextFactory<AnyDexDb>(options => {
				options.UseLazyLoadingProxies(true);
				options.UseMySql(connectionString, serverVersion);
			});
			// Enable Localization
			builder.Services.AddLocalization();
			// Inject DateTime Client Time Zone converter
			builder.Services.AddScoped<DateTimeLocalizer>();
		}

		private static void ConfigureLogging(WebApplicationBuilder builder) {
			builder.Logging
				.ClearProviders()
				.AddConsole()
				.SetMinimumLevel(Debugger.IsAttached ? LogLevel.Debug : LogLevel.Information);
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

				// Lockout (= account locked after multiple failed attempts) settings
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
			app.UseRequestLocalization();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();
			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");
		}
	}
}