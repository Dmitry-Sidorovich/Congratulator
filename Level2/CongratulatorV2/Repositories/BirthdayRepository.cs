using CongratulatorV2.Data;
using CongratulatorV2.Interfaces;
using CongratulatorV2.Models;

namespace CongratulatorV2.Repositories;

public class BirthdayRepository : IBirthdayRepository
{
    private readonly AppDbContext _context;

    public BirthdayRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public List<Birthday> GetAll()
    {
        return _context.Birthdays
            .OrderBy(b => b.Name)
            .ToList();
    }

    public Birthday? GetById(int id)
    {
        return _context.Birthdays.Find(id);
    }

    public List<Birthday> GetByMonth(int month)
    {
        return _context.Birthdays
            .Where(b => b.Date.Month == month)
            .OrderBy(b => b.Date.Day)
            .ToList();
    }
    
    public List<Birthday> SearchByName(string searchTerm)
    {
        return _context.Birthdays
            .Where(b => b.Name.ToLower().Contains(searchTerm))
            .OrderBy(b => b.Name)
            .ToList();
    }
    
    public Birthday Add(string name, DateTime birthDate)
    {
        var birthday = new Birthday
        {
            Name = name,
            Date = birthDate
        };
            
        _context.Birthdays.Add(birthday);
        _context.SaveChanges();
        return birthday;
    }
    
    public Birthday Update(int id, string name, DateTime birthDate)
    {
        var existing = _context.Birthdays.Find(id);
        if (existing == null)
        {
            throw new InvalidOperationException($"День рождения с ID {id} не найден");
        }
            
        existing.Name = name;
        existing.Date = birthDate;
        _context.SaveChanges();
        return existing;
    }
    
    public bool Delete(int id)
    {
        var birthday = _context.Birthdays.Find(id);
        if (birthday == null)
        {
            return false;
        }

        _context.Birthdays.Remove(birthday);
        _context.SaveChanges();
        return true;
    }
    
    public bool Exists(int id)
    {
        return _context.Birthdays.Any(b => b.Id == id);
    }
    
    public int Count()
    {
        return _context.Birthdays.Count();
    }

    public List<Birthday> GetExpired(DateTime startOfYear, DateTime today)
    {
        return _context.Birthdays
            .Where(b => b.Date >= startOfYear && b.Date <= today)
            .OrderBy(b => new DateTime(today.Year, b.Date.Month, b.Date.Day))
            .ToList();
    }
}