using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIELIB.Models;

/// <summary>
/// Bảng chủ đề / topic tài liệu
/// </summary>
[Table("topic", Schema = "Ebook")]
public class Topic
{
    [Key]
    public long Id { get; set; }

    [StringLength(100)]
    public string? Name { get; set; }

    public long? ParentId { get; set; }

    public long? Level { get; set; }

    public int? Status { get; set; }

    [Column("order")]
    public int? Order { get; set; }

    [StringLength(50)]
    public string? PortalId { get; set; }

    public int? IsLogin { get; set; }

    [StringLength(150)]
    public string? Description { get; set; }

    [StringLength(150)]
    public string? Keyword { get; set; }

    [StringLength(150)]
    public string? PageTitle { get; set; }

    [StringLength(150)]
    public string? MetaDescription { get; set; }

    [StringLength(50)]
    public string? Language { get; set; }

    [StringLength(50)]
    public string? DDC { get; set; }

    // Navigation property
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
