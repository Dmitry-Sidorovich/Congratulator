using CongratulatorV1.Models;

namespace CongratulatorV1.Interfaces;

public interface IBirthdayService
{
    void AddBirthday(List<Birthday> birthdays);
    void EditBirthday(List<Birthday> birthdays);
    void DeleteBirthday(List<Birthday> birthdays);
    List<Birthday> GetUpcomingBirthdays(List<Birthday> birthdays);
    List<Birthday> GetUpcomingBirthdays(List<Birthday> birthdays, int upcomingDaysCount);
}