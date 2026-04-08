using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIELIB.Models;

/// <summary>
/// Bảng chuyên mục / bộ sưu tập tài liệu
/// </summary>
[Table("collection", Schema = "Ebook")]
public class Collection
{
    [Key]
    public long Id { get; set; }

    [StringLength(300)]
    public string? Name { get; set; }

    public long? ParentId { get; set; }

    public long? Level { get; set; }

    public int? Status { get; set; }

    public int? SortOrder { get; set; }

    [StringLength(50)]
    public string? PortalId { get; set; }

    [StringLength(50)]
    public string? Language { get; set; }

    [StringLength(200)]
    public string? Link { get; set; }

    public int? Allowdownload { get; set; }

    // Navigation property
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
