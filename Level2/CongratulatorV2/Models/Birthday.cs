namespace CongratulatorV2.Models;

public class Birthday(string name  = "", DateTime date = default)
{
    public int Id { get; set; }
    public string Name { get; set; } = name;
    public DateTime Date { get; set; } = date;
    
    public override string ToString()
    {
        return $"{Name} - {Date:dd MMMM yyyy}";
    }
    
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            int age = today.Year - Date.Year;
            if (today < new DateTime(today.Year, Date.Month, Date.Day))
                age--;
            return age;
        }
    }
}