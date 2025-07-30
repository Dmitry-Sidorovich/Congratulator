using CongratulatorV1.Models;

namespace CongratulatorV1.Interfaces;

public interface IDataRepository
{
    List<Birthday> LoadBirthdays();
    void SaveBirthday(List<Birthday> birthday);
    bool DataFileExists();
}