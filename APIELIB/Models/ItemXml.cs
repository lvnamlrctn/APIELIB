using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIELIB.Models;

/// <summary>
/// Bảng thông tin XML / mô tả chi tiết của tài liệu
/// </summary>
[Table("itemXml", Schema = "Ebook")]
public class ItemXml
{
    /// <summary>
    /// Khóa chính = ItemId (quan hệ 1-1 với bảng Item)
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(300)]
    public string? Title { get; set; }

    [StringLength(200)]
    public string? Author { get; set; }

    [StringLength(150)]
    public string? Publisher { get; set; }

    [StringLength(50)]
    public string? PublishDate { get; set; }

    [StringLength(300)]
    public string? Keyword { get; set; }

    [StringLength(2000)]
    public string? Xml { get; set; }

    [StringLength(300)]
    public string? OtherTitle { get; set; }

    [StringLength(50)]
    public string? Page { get; set; }

    [StringLength(200)]
    public string? OldAuthor { get; set; }

    // Navigation property (1-1 với Item)
    [ForeignKey("Id")]
    public Item? Item { get; set; }
}
