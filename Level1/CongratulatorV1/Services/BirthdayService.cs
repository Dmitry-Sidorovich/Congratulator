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
                var nextBirthday = new DateTime(today.Year, birthday.Date.Month, birthday.Date.Day);
                if (nextBirthday < today)
                {
                    nextBirthday = nextBirthday.AddYears(1);
                }
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
}