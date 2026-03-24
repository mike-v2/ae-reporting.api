using System.ComponentModel.DataAnnotations;

namespace ae_reporting.api.Models;

public class AdverseEvent
{
    [Key]
    public int Id { get; set; }
    public string PatientId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateOfOnset { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string RelationshipToStudyDrug { get; set; } = string.Empty;

    // Navigation property
    public Patient? Patient { get; set; }
}
