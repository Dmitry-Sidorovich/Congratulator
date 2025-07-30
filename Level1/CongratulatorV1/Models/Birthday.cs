namespace CongratulatorV1.Models;

public class Birthday(string name  = "", DateTime date = default)
{
    public string Name { get; set; } = name;
    public DateTime Date { get; set; } = date;
    
    public override string ToString()
    {
        return $"{Name} - {Date:dd MMMM yyyy}";
    }
}