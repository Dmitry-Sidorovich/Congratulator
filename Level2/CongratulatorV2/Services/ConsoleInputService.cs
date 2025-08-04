using CongratulatorV2.Models;

namespace CongratulatorV2.Services;

public static class ConsoleInputService
{
    public static string GetName()
    {
        while (true)
        {
            Console.Write("Введите имя именинника: ");
            var inputName = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(inputName))
            {
                return inputName;
            }
            Console.WriteLine("Имя не может  быть пустым. Попробуйте снова.");
        } 
    }
    
    public static DateTime GetBirthDate()
    {
        int newInputYear, newInputMonth, newInputDay;

        while (true)
        {
            Console.Write("Введите год рождения (например, 1980): ");
            if (int.TryParse(Console.ReadLine(), out newInputYear) 
                && newInputYear >= 1900 && newInputYear < DateTime.Now.Year)
            {
                break;
            }
                
            Console.WriteLine("Неверный год. Введите год от 1900 до текущего.");
        }
            
        while (true)
        {
            Console.Write("Введите месяц рождения (1-12): ");
            if (int.TryParse(Console.ReadLine(), out newInputMonth) 
                && newInputMonth >= 1 && newInputMonth <= 12)
            {
                break;
            }
                
            Console.WriteLine("Неверный месяц. Введите месяц от 1 до 12.");
        }
            
        while (true)
        {         
            Console.Write("Введите день рождения: ");
            if (int.TryParse(Console.ReadLine(), out newInputDay) 
                && newInputDay >= 1 && newInputDay < DateTime.DaysInMonth(newInputYear, newInputMonth))
            {
                break;
            }
                
            Console.WriteLine("Неверный день. Попробуйте снова.");
        }
        
        return new DateTime(newInputYear, newInputMonth, newInputDay);
    }
    
    
    public static bool AskYesNo(string question)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine(question);
            Console.WriteLine("1 – да, 0 – нет.");
            Console.Write("Ваш выбор: ");

            if (!int.TryParse(Console.ReadLine(), out int answer))
            {
                Console.WriteLine("Некорректный ввод. Введите 1 или 0.");
                Console.ReadKey();
                Console.Clear();
                continue;
            }

            switch (answer)
            {
                case 1:
                    Console.Clear();
                    return true;
                case 0:
                    Console.WriteLine("Завершение..");
                    return false;
                default:
                    Console.WriteLine("Пожалуйста, введите 1 или 0.");
                    break;
            }
        }
    }
    
    public static int GetUpcomingDaysCount(int defaultUpcomingDaysCount)
    {
        while (true)
        {
            Console.Write($"Показывать ближайшие дни рождения в диапазоне дней (Enter — по умолчанию {defaultUpcomingDaysCount}): ");
            var inputUpcomingDaysCount = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(inputUpcomingDaysCount))
            {
                return defaultUpcomingDaysCount;
            }

            if (int.TryParse(inputUpcomingDaysCount, out int upcomingDaysCount) && upcomingDaysCount >= 0)
            {
                return upcomingDaysCount;
            }
            
            Console.WriteLine("Неверный ввод. Введите неотрицательное число дней или нажмите Enter.");
        }
    }
    
    public static int GetRecordNumber(List<Birthday> birthdays, string actionName)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Выберите номер записи для {actionName} (0 - выход):");
            for (int i = 0; i < birthdays.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {birthdays[i]} [ID: {birthdays[i].Id}]");
            }

            Console.Write("Номер записи: ");
            if (!int.TryParse(Console.ReadLine(), out int index))
            {
                Console.WriteLine("Некорректный номер. Попробуйте снова или нажмите 0 для выхода.");
                Console.ReadKey();
                continue;
            }

            if (index == 0)
            {
                return 0;
            }

            if (index < 1 || index > birthdays.Count)
            {
                Console.WriteLine($"Номер должен быть от 1 до {birthdays.Count}. Нажмите Enter для повтора.");
                Console.ReadLine();
                continue;
            }
            
            return index;
        }
    }
}