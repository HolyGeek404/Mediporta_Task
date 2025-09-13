using System.ComponentModel.DataAnnotations;

namespace Model.DataAccess.Entities;

public class Tag
{
    [Key]
    public int TagId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}