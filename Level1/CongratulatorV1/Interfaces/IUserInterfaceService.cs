using CongratulatorV1.Models;

namespace CongratulatorV1.Interfaces;

public interface IUserInterfaceService
{
    void DisplayWelcomeScreen(List<Birthday> birthdays);
    void DisplayMainMenu(List<Birthday> birthdays);
    void DisplayAllBirthdays(List<Birthday> birthdays);
    void DisplayUpcomingBirthdays(List<Birthday> birthdays);
    void DisplayUpcomingBirthdays(List<Birthday> birthdays, int upcomingDaysCount);
    void DisplayPaginatedBirthdays(List<Birthday> birthdays, int pageSize);
}