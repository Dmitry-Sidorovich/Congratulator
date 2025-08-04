namespace CongratulatorV2.Models;

public class Birthday
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public Birthday() { }

    public Birthday(string name, DateTime date)
    {
        Name = name.Trim() ?? string.Empty;
        Date = date;
    }
    
    public override string ToString()
    {
        return $"{Name} - {Date:dd MMMM yyyy}";
    }
}