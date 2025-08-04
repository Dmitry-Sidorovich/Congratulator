using CongratulatorV2.Interfaces;
using CongratulatorV2.Models;

namespace CongratulatorV2.Services;

public class BirthdayService : IBirthdayService
{
    private const int DefaultUpcomingDaysCount = 7;
    private readonly IBirthdayRepository _birthdayRepository;

    public BirthdayService(IBirthdayRepository birthdayRepository)
    {
        _birthdayRepository = birthdayRepository;
    }

    public List<Birthday> GetAll()
    {
        return _birthdayRepository.GetAll();
    }

    public Birthday? GetById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("ID должен быть положительным числом", nameof(id));
        }
        return _birthdayRepository.GetById(id);
    }
    
    public Birthday Add(string name, DateTime birthDate)
    {
        return _birthdayRepository.Add(name, birthDate);
    }

    public Birthday Update(int id, string name, DateTime birthDate)
    {
        return _birthdayRepository.Update(id, name, birthDate);
    }

    public bool Delete(int birthdayId)
    {
        return _birthdayRepository.Delete(birthdayId);
    }

    public List<Birthday> GetUpcoming(int days = DefaultUpcomingDaysCount)
    {
        if (days < 0)
            throw new ArgumentException("Количество дней не может быть отрицательным", nameof(days));

        var today = DateTime.Today;
        
        return _birthdayRepository.GetAll()
            .Where(b => CalculateDaysUntilBirthday(b, today) <= days) 
            .OrderBy(b => CalculateDaysUntilBirthday(b, today))
            .ThenBy(b => b.Name)
            .ToList();
    }
    
    public List<Birthday> Sort(List<Birthday> birthdays, SortOption sortOption)
    {
        if (birthdays == null || birthdays.Count == 0)
        {
            return new List<Birthday>();
        }
        
        var today = DateTime.Today;

        return sortOption switch
        {
            SortOption.ByNameAsc => birthdays.OrderBy(b => b.Name).ToList(),
            SortOption.ByNameDesc => birthdays.OrderByDescending(b => b.Name).ToList(),
            SortOption.ByDateAsc => birthdays.OrderBy(b => b.Date.Month).ThenBy(b => b.Date.Day).ToList(),
            SortOption.ByDateDesc => birthdays.OrderByDescending(b => b.Date.Month).ThenByDescending(b => b.Date.Day).ToList(),
            SortOption.ByNextAsc => birthdays.OrderBy(b => CalculateNextBirthday(b, today)).ToList(),
            SortOption.ByNextDesc => birthdays.OrderByDescending(b => CalculateNextBirthday(b, today)).ToList(),
            _ => birthdays.ToList()
        };
    }

    public List<Birthday> GetByMonth(int month)
    {
        return _birthdayRepository.GetByMonth(month);
    }

    public List<Birthday> SearchByName(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return new List<Birthday>();
        }

        var term = searchTerm.Trim().ToLower();
        
        return _birthdayRepository.SearchByName(term);
    }
    
    public List<Birthday> GetExpired()
    {
       var today = DateTime.Today;
       var startOfYear = new DateTime(today.Year, 1, 1);

       return _birthdayRepository.GetExpired(startOfYear, today);
    }

    public int Count()
    {
        return _birthdayRepository.Count();
    }

    public bool Exists(int birthdayId)
    {
        return _birthdayRepository.Exists(birthdayId);
    }
    
    private DateTime CalculateNextBirthday(Birthday birthday, DateTime fromDate)
    {
        var nextBirthday = new DateTime(fromDate.Year, birthday.Date.Month, birthday.Date.Day);
        if (nextBirthday < fromDate.Date)
        {
            nextBirthday = nextBirthday.AddYears(1);
        }
        return nextBirthday;
    }
    
    private int CalculateDaysUntilBirthday(Birthday birthday, DateTime fromDate)
    {
        var nextBirthday = CalculateNextBirthday(birthday, fromDate);
        return (nextBirthday - fromDate).Days;
    }
}