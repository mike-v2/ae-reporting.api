using System.ComponentModel.DataAnnotations;

namespace ae_reporting.api.Models;

public class Patient
{
    [Key]
    public string PatientId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
