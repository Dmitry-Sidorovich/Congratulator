using CongratulatorV2.Models;

namespace CongratulatorV2.Interfaces;

public interface IBirthdayService
{
    void AddBirthday(List<Birthday> birthdays);
    void EditBirthday(List<Birthday> birthdays);
    void DeleteBirthday(List<Birthday> birthdays);
    List<Birthday> GetUpcomingBirthdays(List<Birthday> birthdays);
    List<Birthday> GetUpcomingBirthdays(List<Birthday> birthdays, int upcomingDaysCount);
    List<Birthday> SortBirthdays(List<Birthday> birthdays, SortOption sortOption);
    List<Birthday> FilterByMonth(List<Birthday> birthdays, int month);
    List<Birthday> FilterByNameContains(List<Birthday> birthdays, string substring);
    List<Birthday> GetExpiredBirthdays(List<Birthday> birthdays);
}