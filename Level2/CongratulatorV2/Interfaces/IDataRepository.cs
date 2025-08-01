using CongratulatorV2.Models;

namespace CongratulatorV2.Interfaces;

public interface IDataRepository
{
    List<Birthday> LoadBirthdays();
    void SaveBirthday(List<Birthday> birthday);
    bool DataFileExists();
}