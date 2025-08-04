using CongratulatorV2.Models;

namespace CongratulatorV2.Interfaces;

public interface IBirthdayService
{
    List<Birthday> GetAll();
    Birthday? GetById(int id);
    List<Birthday> GetByMonth(int month);
    List<Birthday> SearchByName(string searchTerm);
    List<Birthday> GetUpcoming(int days);
    Birthday Add(string name, DateTime birthDate);
    Birthday Update(int id, string name, DateTime birthDate);
    bool Delete(int id);
    bool Exists(int id);
    int Count();
    List<Birthday> GetExpired();
    List<Birthday> Sort(List<Birthday> birthdays, SortOption sortOption);
}