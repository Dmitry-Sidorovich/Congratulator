using CongratulatorV2.Data;
using CongratulatorV2.Interfaces;
using CongratulatorV2.Repositories;
using CongratulatorV2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var services = new ServiceCollection();
services.AddDbContext<AppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("Default")));
services.AddScoped<IBirthdayRepository, BirthdayRepository>();
services.AddScoped<IBirthdayService, BirthdayService>();
services.AddScoped<IUserInterfaceService, ConsoleUIService>();
services.AddSingleton<IConfiguration>(configuration);

await using var serviceProvider = services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

using (var scope = serviceProvider.CreateScope())
{
    var uiService = scope.ServiceProvider.GetRequiredService<IUserInterfaceService>();
    uiService.DisplayWelcomeScreen();
    uiService.DisplayMainMenu();
}

Console.WriteLine("Спасибо за использование программы Поздравлятор!");