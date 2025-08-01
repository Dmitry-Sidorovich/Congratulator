using CongratulatorV2.Data;
using CongratulatorV2.Interfaces;
using CongratulatorV2.Repositories;
using CongratulatorV2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();           

var services = new ServiceCollection();
services.AddSingleton<IBirthdayRepository, JsonBirthdayRepository>();
services.AddSingleton<IBirthdayService, BirthdayService>();
services.AddSingleton<IUserInterfaceService, ConsoleUIService>();
services.AddDbContext<AppDbContext>(options => options.UseSqlite(config.GetConnectionString("Default")));

using var serviceProvider = services.BuildServiceProvider();

using var db = serviceProvider.GetRequiredService<AppDbContext>();
db.Database.Migrate();

var repository = serviceProvider.GetRequiredService<IBirthdayRepository>();
var ui = serviceProvider.GetRequiredService<IUserInterfaceService>();

var birthdays = repository.LoadBirthdays();

ui.DisplayWelcomeScreen(birthdays);
ui.DisplayMainMenu(birthdays);