using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIELIB.Models;

/// <summary>
/// Bảng tài liệu chính (Item)
/// </summary>
[Table("Item", Schema = "Ebook")]
public class Item
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public DateTime? Submited { get; set; }

    public long? CreatedBy { get; set; }

    public long? CollectionId { get; set; }

    [StringLength(200)]
    public string? Images { get; set; }

    public int? TotalView { get; set; }

    public int? TotalDownload { get; set; }

    public byte? Status { get; set; }

    public long? SubjectId { get; set; }

    public DateTime? LastUpdate { get; set; }

    public long? TypeId { get; set; }

    public long? UpdateBy { get; set; }

    public int? AllowDownload { get; set; }

    public int? Free { get; set; }

    public long? TopicId { get; set; }

    [StringLength(50)]
    public string? PortalId { get; set; }

    [StringLength(50)]
    public string? Language { get; set; }

    public int? Show { get; set; }

    // Navigation properties
    [ForeignKey("CollectionId")]
    public Collection? Collection { get; set; }

    [ForeignKey("SubjectId")]
    public Subject? Subject { get; set; }

    [ForeignKey("TopicId")]
    public Topic? Topic { get; set; }

    public ItemXml? ItemXml { get; set; }

    public ICollection<MetaDataValue> MetaDataValues { get; set; } = new List<MetaDataValue>();
}
