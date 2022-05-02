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
			string connectionString;

			builder = WebApplication.CreateBuilder(args);
			connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			AddServices(builder, connectionString);

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