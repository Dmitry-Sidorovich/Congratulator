using CongratulatorV2.Interfaces;
using CongratulatorV2.Models;
using Microsoft.Extensions.Configuration;
using static CongratulatorV2.Services.ConsoleInputService;

namespace CongratulatorV2.Services;

public class ConsoleUIService : IUserInterfaceService
{
    private readonly IBirthdayService _birthdayService;
    private readonly IConfiguration _configuration;

    public ConsoleUIService(IBirthdayService birthdayService, IConfiguration configuration)
    {
        _birthdayService = birthdayService;
        _configuration = configuration;
    }
    
    private int DefaultUpcomingDaysCount => _configuration.GetValue<int>("AppSettings:DefaultUpcomingDaysCount");
    private int DefaultPageSize => _configuration.GetValue<int>("AppSettings:DefaultPageSize");
    
    public void DisplayWelcomeScreen()
    {
        Console.Clear();
        Console.WriteLine("=== Добро пожаловать в Поздравлятор Level 2! ===");
        Console.WriteLine();

        try
        {
            var birthdaysCount = _birthdayService.Count();
            if (birthdaysCount == 0)
            {
                Console.WriteLine("База данных пуста. Добавьте первую запись через пункт меню «Добавить день рождения».\n");
                return;
            }

            Console.WriteLine($"В базе данных {birthdaysCount} записей.");
            var upcomingBirthdays = _birthdayService.GetUpcoming(DefaultUpcomingDaysCount);
            DisplayUpcomingBirthdays(upcomingBirthdays, DefaultUpcomingDaysCount);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
        }
    }
    
    public void DisplayMainMenu()
    {
        while (true)
        {
            Console.WriteLine("=== Главное меню ===" +
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
                case 1: DisplayAllBirthdays(); break;
                case 2: DisplayUpcomingBirthdays(); break;
                case 3: AddBirthday(); break;
                case 4: UpdateBirthday(); break;
                case 5: DeleteBirthday(); break;
                case 6: DisplaySortMenu(); break;
                case 7: DisplayFilterMenu(); break;
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

    private void DisplaySortMenu()
    {
        var birthdays = _birthdayService.GetAll();
        if (birthdays.Count == 0)
        {
            Console.WriteLine("Список пуст, нечего сортировать.");
            return;
        }
        
        Console.WriteLine("=== Сортировка списка ===");
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
        
        var sortedBirthdays = _birthdayService.Sort(birthdays, sortOption);
        Console.Clear();
        Console.WriteLine("=== Отсортированный список ===");
        DisplayPaginatedBirthdays(sortedBirthdays);
    }

    private void DisplayFilterMenu()
    {
        Console.WriteLine("=== Фильтрация списка ===");
        Console.WriteLine(
            "Выберите фильтр:\n" +
            "1 – По месяцу рождения\n" +
            "2 – По части имени\n" +
            "3 – Просроченные в этом году\n" +
            "0 – Назад");
        int choice = ReadInt("Ваш выбор: ", 0, 3);
        Console.Clear();
        
        List<Birthday> result;
        switch (choice)
        {
            case 1:
                int month = ReadInt("Введите месяц (1-12): ", min:1, max:12);
                result = _birthdayService.GetByMonth(month);
                Console.WriteLine($"=== Дни рождения в {GetMonthName(month)} ===");
                break;
            case 2:
                Console.Write("Введите часть имени: ");
                var searchTerm = Console.ReadLine() ?? "";
                result = _birthdayService.SearchByName(searchTerm);
                Console.WriteLine($"=== Результаты поиска по '{searchTerm}' ===");
                break;
            case 3:
                result = _birthdayService.GetExpired();
                Console.WriteLine("=== Просроченные дни рождения в этом году ===");
                break;
            default:
                return;
        }
        
        Console.Clear();
        DisplayPaginatedBirthdays(result);
    }
    
    private string GetMonthName(int month)
    {
        return month switch
        {
            1 => "Январь", 2 => "Февраль", 3 => "Март", 4 => "Апрель",
            5 => "Май", 6 => "Июнь", 7 => "Июль", 8 => "Август",
            9 => "Сентябрь", 10 => "Октябрь", 11 => "Ноябрь", 12 => "Декабрь",
            _ => "Неизвестно"
        };
    }

    private void DisplayAllBirthdays()
    {
        var birthdays = _birthdayService.GetAll(); 
        DisplayPaginatedBirthdays(birthdays, DefaultPageSize);
    }

    private void DisplayUpcomingBirthdays()
    {
        int upcomingDaysCount = GetUpcomingDaysCount(DefaultUpcomingDaysCount);
        var upcomingBirthdays = _birthdayService.GetUpcoming(upcomingDaysCount);
        DisplayUpcomingBirthdays(upcomingBirthdays, upcomingDaysCount);
    }

    private void AddBirthday()
    {
        Console.WriteLine("=== Добавление нового дня рождения ===");
        string name = GetName();
        DateTime birthDate = GetBirthDate();
        
        var addedBirthday = _birthdayService.Add(name, birthDate);
        Console.WriteLine($"Именинник {addedBirthday.Name} с датой рождения {addedBirthday.Date:dd MMMM yyyy} года успешно добавлен!");
        Console.WriteLine($"Присвоен ID: {addedBirthday.Id}");
    }
    
    private void UpdateBirthday()
    {
        var birthdays = _birthdayService.GetAll();
        if (birthdays.Count == 0)
        {
            Console.WriteLine("Список пуст, нечего редактировать.");
            return;
        }

        Console.WriteLine("=== Редактирование дня рождения ===");
        int index = GetRecordNumber(birthdays, "редактирования");
        if (index == 0)
        {
            return;
        }

        var chosenBirthday = birthdays[index - 1];
        Console.Clear();
        Console.WriteLine($"Редактирование: {chosenBirthday}");

        string newName = chosenBirthday.Name;
        DateTime newBirthDate = chosenBirthday.Date;

        if (AskYesNo($"Желаете изменить имя «{chosenBirthday.Name}»?"))
        {
            newName = GetName();
        }

        if (AskYesNo($"Желаете изменить дату рождения «{chosenBirthday.Date:dd MMMM yyyy}»?"))
        {
            newBirthDate = GetBirthDate();
        }

        var updatedBirthday = _birthdayService.Update(chosenBirthday.Id, newName, newBirthDate);
        Console.WriteLine($"Запись успешно обновлена: {updatedBirthday}");
    }
    
    private void DeleteBirthday()
    {
        var birthdays = _birthdayService.GetAll();
        if (birthdays.Count == 0)
        {
            Console.WriteLine("Список пуст, нечего удалять.");
            return;
        }

        Console.WriteLine("=== Удаление дня рождения ===");
        int index = GetRecordNumber(birthdays, "удаления");
        if (index == 0)
        {
            return;
        }

        var chosenBirthday = birthdays[index - 1];
        if (AskYesNo($"Вы уверены, что хотите удалить «{chosenBirthday}»?"))
        {
            bool deleted = _birthdayService.Delete(chosenBirthday.Id);
            if (deleted)
            {
                Console.WriteLine("Запись успешно удалена.");
            }
            else
            {
                Console.WriteLine("Ошибка при удалении записи.");
            }
        }
        else
        {
            Console.WriteLine("Удаление отменено.");
        }
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
    
    private void DisplayUpcomingBirthdays(List<Birthday> birthdays, int upcomingDaysCount)
    {
        var today = DateTime.Today;

        if (birthdays == null || birthdays.Count == 0)
        {
            Console.WriteLine($"Нет дней рождения в ближайшие {upcomingDaysCount} дней ({today:dd.MM.yyyy}–{today.AddDays(upcomingDaysCount):dd.MM.yyyy}).");
            return;
        }


        Console.WriteLine($"Дни рождения сегодня и ближайшие {upcomingDaysCount} дней ({today:dd.MM.yyyy}–{today.AddDays(upcomingDaysCount):dd.MM.yyyy}):");
        Console.WriteLine();
        
        DisplayPaginated(birthdays,b => FormatWithUpcoming(b, upcomingDaysCount));
    }
    
    private void DisplayPaginatedBirthdays(List<Birthday> birthdays, int pageSize = 10)
    {
        DisplayPaginated(birthdays,b => FormatBirthday(b), pageSize);
    }
    
    public void DisplayPaginated<T>(
        List<T> items,
        Func<T,string> formatter,
        int pageSize = -1)
    {
        if (pageSize == -1)
        {
            pageSize = DefaultPageSize;
        }
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
    
    private string FormatBirthday(Birthday birthday)
    {
        return $"{birthday.Name}- {birthday.Date:dd MMMM yyyy}";
    }

    private string FormatWithUpcoming(Birthday birthday, int upcomingDaysCount)
    {
        string when = upcomingDaysCount == 0 ? "СЕГОДНЯ!" : $"через {upcomingDaysCount} дн.";
        return $"{birthday.Name} - {birthday.Date:dd MMMM yyyy} ({when})";
    }
}