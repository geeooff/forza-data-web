using ForzaData.Web.Configuration;

namespace ForzaData.Web;

public class Startup(IConfiguration configuration)
{
	public IConfiguration Configuration { get; } = configuration;

	public void ConfigureServices(IServiceCollection services)
	{
		services.Configure<CookiePolicyOptions>(options =>
		{
			options.CheckConsentNeeded = context => true;
			options.MinimumSameSitePolicy = SameSiteMode.None;
		});

		services.Configure<AppSettings>("App", Configuration);

		services.AddControllersWithViews();
		services.AddRazorPages();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseCookiePolicy();

		app.UseRouting();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
			endpoints.MapRazorPages();
		});
	}
}
