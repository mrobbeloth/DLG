
using DistributionListGenerator.Controllers;

var builder = WebApplication.CreateBuilder(args);
var EthosAPIKey = builder.Configuration.GetSection("Ethos_API")["key"];
IHostEnvironment env = builder.Environment;

// Add services to the container.
builder.Services.AddTransient<IAcademicPeriodsController, AcademicPeriodController>();
builder.Services.AddTransient<IStudentAcademicPeriodsController, StudentAcademicPeriodsController>();
builder.Services.AddTransient<IPersonController, PersonController>();
builder.Services.AddTransient<IStudentAcademicProgramsController, StudentAcademicProgramsController>();
builder.Services.AddTransient<IAcademicProgramsController, AcademicProgramsController>();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Start the application up, last step
app.Run();
