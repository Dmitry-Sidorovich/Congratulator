using CongratulatorV1.Interfaces;
using CongratulatorV1.Models;
using static CongratulatorV1.Services.ConsoleInputService;

namespace CongratulatorV1.Services;

public class ConsoleUIService(IBirthdayService birthdayService, IDataRepository jsonDataRepository) : IUserInterfaceService
{
    private const int DefaultUpcomingDaysCount = 7;
    private const int DefaultPageSize = 5;
    
    public void DisplayWelcomeScreen(List<Birthday> birthdays)
    {
        Console.Clear();
        Console.WriteLine("Добро пожаловать в Поздравлятор!");

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
            Console.WriteLine("=== Главное меню программы \"Поздравлятор\" ===" +
                              "\nВыберите:" +
                              "\n1 - Показать все дни рождения" +
                              "\n2 - Показать ближайшие дни рождения" +
                              "\n3 - Добавить новый день рождения" +
                              "\n4 - Редактировать день рождения" +
                              "\n5 - Удалить день рождения" +
                              "\n6 - Сортировать список" +
                              "\n7 - Фильтровать список" +
                              "\n0 - Выход");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Неверный ввод. Нажмите Enter.");
                Console.ReadKey();
                Console.Clear();
                continue;
            }

            Console.Clear();
            switch (choice)
            {
                case 1:
                    DisplayPaginatedBirthdays(birthdays, DefaultPageSize);
                    break;
                case 2:
                    int upcomingDaysCount = GetUpcomingDaysCount(DefaultUpcomingDaysCount);
                    var upcomingBirthdays = birthdayService.GetUpcomingBirthdays(birthdays, upcomingDaysCount);
                    DisplayUpcomingBirthdays(upcomingBirthdays, upcomingDaysCount);
                    break;
                case 3:
                    birthdayService.AddBirthday(birthdays);
                    jsonDataRepository.SaveBirthday(birthdays);
                    break;
                case 4:
                    birthdayService.EditBirthday(birthdays);
                    jsonDataRepository.SaveBirthday(birthdays);
                    break;
                case 5:
                    birthdayService.DeleteBirthday(birthdays);
                    jsonDataRepository.SaveBirthday(birthdays);
                    break;
                case 6:
                    DisplaySortMenu(birthdays);
                    break;
                case 7:
                    DisplayFilterMenu(birthdays);
                    break;
                case 0: 
                    Console.WriteLine("Завершение программы..");
                    return;
                default:
                    Console.WriteLine("Нет такого пункта. Нажмите Enter.");
                    Console.ReadLine();
                    Console.Clear();
                    break;
            }
            
            if (!AskYesNo("Желаете продолжить?"))
            {
                break;
            }
        }
    }

    public void DisplaySortMenu(List<Birthday> birthdays)
    {
        Console.WriteLine("Выберите сортировку:\n" +
                          "1 – По имени (А-Я)\n" +
                          "2 – По имени (Я-А)\n" +
                          "3 – По дате в году (янв-дек)\n" +
                          "4 – По дате в году (дек-янв)\n" +
                          "5 – По ближайшему ДР (сначала ближайшие)\n" +
                          "6 – По ближайшему ДР (сначала дальние)\n" +
                          "0 – Назад");

        var choice = ReadInt("Ваш выбор: ", 0, 6);
        if (choice == 0)
        {
            return;
        }

        var sortOption = choice switch
        {
            1 => SortOption.ByNameAsc,
            2 => SortOption.ByNameDesc,
            3 => SortOption.ByDateAsc,
            4 => SortOption.ByDateDesc,
            5 => SortOption.ByNextAsc,
            6 => SortOption.ByNextDesc,
            _ => SortOption.ByNameAsc
        };
        
        var sortedBirthdays = birthdayService.SortBirthdays(birthdays, sortOption);
        DisplayPaginatedBirthdays(sortedBirthdays);
    }

    public void DisplayFilterMenu(List<Birthday> birthdays)
    {
        Console.WriteLine(
            "Выберите фильтр:\n" +
            "1 – По месяцу рождения\n" +
            "2 – По части имени\n" +
            "3 – Просроченные в этом году\n" +
            "0 – Назад");
        int choice = ReadInt("Ваш выбор: ", min:0, max:3);
        Console.Clear();
        
        List<Birthday> result = birthdays;
        switch (choice)
        {
            case 1:
                int month = ReadInt("Введите месяц: ", min:1, max:12);
                result = birthdayService.FilterByMonth(birthdays, month);
                break;
            case 2:
                Console.Write("Введите часть имени: ");
                var termName = Console.ReadLine() ?? "";
                result = birthdayService.FilterByNameContains(birthdays, termName);
                break;
            case 3:
                result = birthdayService.GetExpiredBirthdays(birthdays);
                break;
            case 0:
                return;
        }
        
        Console.Clear();
        DisplayPaginatedBirthdays(result);
    }

    private int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value)
                && value >= min && value <= max)
                return value;
            Console.WriteLine($"Введите число от {min} до {max}.");
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
                return $"{b.Name} ({b.Age} лет) - {b.Date:dd MMMM yyyy} ({when})";
            });
    }
    
    public void DisplayUpcomingBirthdays(List<Birthday> birthdays)
        => DisplayUpcomingBirthdays(birthdays, DefaultUpcomingDaysCount);
    
    public void DisplayPaginatedBirthdays(List<Birthday> birthdays, int pageSize = 10)
    {
        DisplayPaginated(
            birthdays, 
            b => $"{b.Name} ({b.Age} лет) - {b.Date:dd MMMM yyyy}",
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
                Console.Clear();
                break;
            }
            Console.Clear();
        }
    }
}