using CongratulatorV2.Models;

namespace CongratulatorV2.Interfaces;

public interface IBirthdayRepository
{
    List<Birthday> GetAll();
    Birthday? GetById(int id);
    List<Birthday> GetByMonth(int month);
    List<Birthday> SearchByName(string searchTerm);
    Birthday Add(string name, DateTime birthDate);
    Birthday Update(int id, string name, DateTime birthDate);
    bool Delete(int id);
    bool Exists(int id);
    int Count();
    List<Birthday> GetExpired(DateTime startOfYear, DateTime today);
}