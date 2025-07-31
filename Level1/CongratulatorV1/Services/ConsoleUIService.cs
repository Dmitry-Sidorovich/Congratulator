using CongratulatorV1.Interfaces;
using CongratulatorV1.Models;
using static CongratulatorV1.Services.ConsoleInputService;

namespace CongratulatorV1.Services;

public class ConsoleUIService(IBirthdayService birthdayService, IDataRepository jsonDataRepository) : IUserInterfaceService
{
    private const int DefaultUpcomingDaysCount = 7;
    
    public void DisplayWelcomeScreen(List<Birthday> birthdays)
    {
        Console.Clear();
        Console.WriteLine("Добро пожаловать в Поздравлятор!\n");

        if (!jsonDataRepository.DataFileExists())
        {
            Console.WriteLine("Файл данных не найден.");
            Console.WriteLine("Чтобы создать файл, добавьте хотя бы одну запись (пункт меню \"Добавить новое ДР\").\n");
            return;
        }
        
        if (birthdays.Count == 0)
        {
            Console.WriteLine("Файл данных пуст. Добавьте первую запись через пункт меню «Добавить новое ДР».\n");
            return;
        }
        
        var upcomingBirthdays = birthdayService.GetUpcomingBirthdays(birthdays);
        DisplayUpcomingBirthdays(upcomingBirthdays);
    }
    
    public void DisplayMainMenu(List<Birthday> birthdays)
    {
        while (true)
        {
            Console.WriteLine("\n\n=== Главное меню программы \"Поздравлятор\" ===\nВыберите:" +
                              "\n1 - Добавить новое ДР\n2 - Редактировать ДР\n3 - Удалить ДР" +
                              "\n4 - Список всех ДР\n5 - Список ближайших ДР\n0 - Выход");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Неверный ввод. Нажмите Enter.");
                Console.ReadLine();
                continue;
            }
            Console.Clear();
            switch (choice)
            {
                case 1:
                    birthdayService.AddBirthday(birthdays);
                    jsonDataRepository.SaveBirthday(birthdays);
                    break;
                case 2:
                    birthdayService.EditBirthday(birthdays);
                    jsonDataRepository.SaveBirthday(birthdays);
                    break;
                case 3: 
                    birthdayService.DeleteBirthday(birthdays);
                    jsonDataRepository.SaveBirthday(birthdays);
                    break;
                case 4:
                    DisplayPaginatedBirthdays(birthdays, 5);
                    break;
                case 5:
                    int upcomingDaysCount = GetUpcomingDaysCount(7);
                    var upcomingBirthdays = birthdayService.GetUpcomingBirthdays(birthdays, upcomingDaysCount);
                    DisplayUpcomingBirthdays(upcomingBirthdays, upcomingDaysCount);
                    break;
                case 0: 
                    return;
                default:
                    Console.WriteLine("Нет такого пункта. Нажмите Enter.");
                    Console.ReadLine();
                    break;
            }
            
            if (!AskYesNo("Желаете продолжить?"))
            {
                break;
            }
        }
    }
    
    public void DisplayAllBirthdays(List<Birthday> birthdays)
    {
        if (birthdays == null || birthdays.Count == 0)
        {
            Console.WriteLine("Список дней рождения пуст.");
            return;
        }

        Console.WriteLine("Список дней рождения");
        for (int i = 0; i < birthdays.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {birthdays[i]}");
        }
    }
    
    public void DisplayUpcomingBirthdays(List<Birthday> birthdays, int upcomingDaysCount)
    {
        var today = DateTime.Today;

        if (birthdays == null || birthdays.Count == 0)
        {
            Console.WriteLine($"Нет дней рождения в ближайшие {upcomingDaysCount} дней ({today:dd.MM.yyyy}–{today.AddDays(upcomingDaysCount):dd.MM.yyyy}).");
            return;
        }
        
        
        string header = $"Дни рождения сегодня и ближайшие {upcomingDaysCount} дней ({today:dd.MM.yyyy}–{today.AddDays(upcomingDaysCount):dd.MM.yyyy}):";
        Console.WriteLine(header);
        Console.WriteLine();
        
        DisplayPaginated(
            birthdays,
            b =>
            {
                var next = new DateTime(today.Year, b.Date.Month, b.Date.Day);
                if (next < today) 
                    next = next.AddYears(1);
                int delta = (next - today).Days;
                string when = delta == 0 ? "сегодня" : $"через {delta} дн.";
                return $"{b.Name} - {b.Date:dd MMMM yyyy} ({when})";
            });
    }
    
    public void DisplayUpcomingBirthdays(List<Birthday> birthdays)
        => DisplayUpcomingBirthdays(birthdays, DefaultUpcomingDaysCount);
    
    public void DisplayPaginatedBirthdays(List<Birthday> birthdays, int pageSize = 10)
    {
        DisplayPaginated(
            birthdays, 
            b => $"{b.Name} - {b.Date:dd MMMM yyyy}",
            pageSize);
    }
    
    public void DisplayPaginated<T>(
        List<T> items,
        Func<T,string> formatter,
        int pageSize = 10)
    {
        if (items == null || items.Count == 0)
        {
            Console.WriteLine("Нечего отображать — список пуст.");
            return;
        }

        int totalItems  = items.Count;
        int totalPages  = (totalItems + pageSize - 1) / pageSize;
        int currentPage = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Страница {currentPage + 1}/{totalPages} — всего записей: {totalItems}\n");

            var pageItems = items.Skip(currentPage * pageSize).Take(pageSize);

            int itemNumber = currentPage * pageSize;
            foreach (var item in pageItems)
            {
                Console.WriteLine($"{++itemNumber}. {formatter(item)}");
            }

            Console.WriteLine("\n[N] — следующая, [P] — предыдущая, [Q] — выход");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.N && currentPage < totalPages - 1)
            {
                currentPage++;
            }
            else if (key == ConsoleKey.P && currentPage > 0)
            {
                currentPage--;
            }
            else if (key == ConsoleKey.Q)
            {
                break;
            }
        }
    }
}