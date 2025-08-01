using CongratulatorV2.Models;

namespace CongratulatorV2.Interfaces;

public interface IBirthdayRepository
{
    List<Birthday> LoadBirthdays();
    void SaveBirthdays(List<Birthday> birthday);
    //void DeleteBirthday(int id);
}