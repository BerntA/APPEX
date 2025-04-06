using System.ComponentModel.DataAnnotations;

namespace database.Entities;

public class Customer
{
    [Key]
    public long Id { get; set; }
    public string OrganizationNumber { get; set; }
    public string Name { get; set; }
    public string Note { get; set; }
}