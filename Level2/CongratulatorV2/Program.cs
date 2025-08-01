using CongratulatorV2.Interfaces;
using CongratulatorV2.Repositories;
using CongratulatorV2.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<IDataRepository, JsonDataRepository>();
services.AddSingleton<IBirthdayService, BirthdayService>();
services.AddSingleton<IUserInterfaceService, ConsoleUIService>();

using var serviceProvider = services.BuildServiceProvider();
var repository = serviceProvider.GetRequiredService<IDataRepository>();
var ui = serviceProvider.GetRequiredService<IUserInterfaceService>();

var birthdays = repository.LoadBirthdays();

ui.DisplayWelcomeScreen(birthdays);
ui.DisplayMainMenu(birthdays);