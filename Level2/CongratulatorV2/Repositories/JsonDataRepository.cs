using System.Text.Encodings.Web;
using System.Text.Json;
using CongratulatorV2.Interfaces;
using CongratulatorV2.Models;

namespace CongratulatorV2.Repositories;

public class JsonDataRepository : IDataRepository
{
    private const string FileName = "birthdays.json";
    
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };
    
    public List<Birthday> LoadBirthdays()
    {
        try
        {
            if (!File.Exists(FileName))
            {
                return [];
            }
            
            var json = File.ReadAllText(FileName);
            
            return JsonSerializer.Deserialize<List<Birthday>>(json, _serializerOptions) ?? [];
        }
        catch (Exception e) when
           (e is IOException or UnauthorizedAccessException or JsonException)
        {
            Console.WriteLine($"Ошибка загрузки: {e.Message}");
            return [];
        }
    }
    
    public void SaveBirthday(List<Birthday> birthdays)
    {
        try
        {
            var json = JsonSerializer.Serialize(birthdays, _serializerOptions);
            
            File.WriteAllText(FileName, json);
        }
        catch (Exception e) when
            (e is IOException or UnauthorizedAccessException or JsonException)
        {
            Console.WriteLine($"Ошибка сохранения: {e.Message}");
        }
    }

    public bool DataFileExists()
    {
        return File.Exists(FileName);
    }
}