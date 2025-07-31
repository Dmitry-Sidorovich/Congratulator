using CongratulatorV1.Interfaces;
using CongratulatorV1.Models;
using static CongratulatorV1.Services.ConsoleInputService;

namespace CongratulatorV1.Services;

public class BirthdayService : IBirthdayService
{
    private const int DefaultUpcomingDaysCount = 7;
    
    public void AddBirthday(List<Birthday> birthdays)
    {
        string name = GetName();
        DateTime birthDate = GetBirthDate();
        birthdays.Add(new Birthday(name, birthDate));

        Console.WriteLine($"Именник {name} с датой рождения {birthDate:dd MMMM yyyy} года успешно добавлен.");
    }

    public void EditBirthday(List<Birthday> birthdays)
    {
        if (birthdays.Count == 0)
        {
            Console.WriteLine("Список пуст, нечего редактировать.");
            return;
        }

        int index = GetRecordNumber(birthdays, "редактирования");
        if (index == 0)
        {
            return;
        }
        
        Console.Clear();
        var chosenBirthday = birthdays[index - 1];
        
        
        if (AskYesNo($"Желаете изменить имя {chosenBirthday.Name}?"))
        {
            string newInputName = GetName();
            chosenBirthday.Name = newInputName;
            Console.WriteLine("Имя обновлено.");
        }

        if (AskYesNo($"Желаете изменить дату рождения «{chosenBirthday.Date:dd MMMM yyyy}»?"))
        {
            DateTime newBirthDate = GetBirthDate();
            chosenBirthday.Date = newBirthDate;
            Console.WriteLine("Дата рождения обновлена. ");
        }

        Console.WriteLine("Редактирование завершено.");
    }

    public void DeleteBirthday(List<Birthday> birthdays)
    {
        if (birthdays.Count == 0)
        {
            Console.WriteLine("Список пуст, нечего удалять.");
            return;
        }
        
        int index = GetRecordNumber(birthdays, "удаления");
        if (index == 0)
        {
            return;
        }
        
        var chosenBirthday = birthdays[index - 1];
        if (AskYesNo($"Вы уверены, что хотите удалить \"{chosenBirthday.Name} - {chosenBirthday.Date:dd MMMM yyyy}\"?"))
        {
            birthdays.RemoveAt(index - 1);
            Console.WriteLine("Запись успешно удалена.");
        }
        else
        {
            Console.WriteLine("Удаление отменено.");
        }
    }

    public List<Birthday> GetUpcomingBirthdays(List<Birthday> birthdays, int upcomingDaysCount)
    {
        var today = DateTime.Today;
        
        return birthdays.Select(birthday =>
            {
                DateTime nextBirthday = CalculateNextBirthday(birthday, today);
                int delta = (nextBirthday - today).Days;

                return (Birthday: birthday, DaysUntil: delta);
            })
            .Where(birthdayInfo => birthdayInfo.DaysUntil <= upcomingDaysCount)
            .OrderBy(birthdayInfo => birthdayInfo.DaysUntil)
            .Select(birthdayInfo => birthdayInfo.Birthday)
            .ToList();
    }
    
    public List<Birthday> GetUpcomingBirthdays(List<Birthday> birthdays)
        => GetUpcomingBirthdays(birthdays, DefaultUpcomingDaysCount);

    public List<Birthday> SortBirthdays(List<Birthday> birthdays, SortOption sortOption)
    { 
        var birthdaysList = birthdays.ToList();
        var today = DateTime.Today;

        return sortOption switch
        {
            SortOption.ByNameAsc => birthdaysList.OrderBy(b => b.Name).ToList(),
            SortOption.ByNameDesc => birthdaysList.OrderByDescending(b => b.Name).ToList(),
            SortOption.ByDateAsc => birthdaysList.OrderBy(b => b.Date.Month).ThenBy(b => b.Date.Day).ToList(),
            SortOption.ByDateDesc => birthdaysList.OrderByDescending(b => b.Date.Month).ThenByDescending(b => b.Date.Day).ToList(),
            SortOption.ByNextAsc => birthdaysList.OrderBy(b => CalculateNextBirthday(b, today)).ToList(),
            SortOption.ByNextDesc => birthdaysList.OrderByDescending(b => CalculateNextBirthday(b, today)).ToList(),
            _ => birthdaysList
        };
    }

    private DateTime CalculateNextBirthday(Birthday birthday, DateTime today)
    {
        var nextBirthday = new DateTime(today.Year, birthday.Date.Month, birthday.Date.Day);
        if (nextBirthday < today)
        {
            nextBirthday = nextBirthday.AddYears(1);
        }

        return nextBirthday;
    }

    public List<Birthday> FilterByMonth(List<Birthday> birthdays, int month)
    {
        return birthdays
            .Where(b => b.Date.Month == month)
            .OrderBy(b => b.Date.Day)
            .ToList();
    }

    public List<Birthday> FilterByNameContains(List<Birthday> birthdays, string substring)
    {
        if (string.IsNullOrWhiteSpace(substring))
        {
            return birthdays;
        }
        
        substring = substring.Trim().ToLower();
        return birthdays
            .Where(b=> b.Name.ToLower().Contains(substring))
            .OrderBy(b => b.Name)
            .ToList();
    }
    
    public List<Birthday> GetExpiredBirthdays(List<Birthday> birthdays)
    {
        var today = DateTime.Today;

        return birthdays
            .Where(b =>
            {
                var thisYearBirthday = new DateTime(
                    today.Year,
                    b.Date.Month,
                    b.Date.Day);
                return thisYearBirthday < today;
            })
            .OrderBy(b =>
                new DateTime(today.Year, b.Date.Month, b.Date.Day))
            .ToList();
    }
}