using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIELIB.Models;

/// <summary>
/// Bảng chủ đề / môn học
/// </summary>
[Table("Subject", Schema = "Ebook")]
public class Subject
{
    [Key]
    public long Id { get; set; }

    [StringLength(300)]
    public string? Name { get; set; }

    public long? ParentId { get; set; }

    public long? Level { get; set; }

    public int? Status { get; set; }

    public int? Sortorder { get; set; }

    [StringLength(50)]
    public string? DDC { get; set; }

    [StringLength(50)]
    public string? PortalId { get; set; }

    [StringLength(50)]
    public string? Language { get; set; }

    [StringLength(200)]
    public string? Link { get; set; }

    // Navigation property
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
