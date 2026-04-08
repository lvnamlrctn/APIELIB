using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIELIB.Models;

/// <summary>
/// Bảng giá trị metadata mở rộng của tài liệu
/// </summary>
[Table("MetaDataValue", Schema = "Ebook")]
public class MetaDataValue
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public int? MetaDataFieldId { get; set; }

    [StringLength(3000)]
    public string? Value { get; set; }

    [StringLength(100)]
    public string? Language { get; set; }

    public long? ItemId { get; set; }

    [StringLength(3000)]
    public string? ValueUnSign { get; set; }

    public int? SortOrder { get; set; }

    // Navigation property
    [ForeignKey("ItemId")]
    public Item? Item { get; set; }
}
