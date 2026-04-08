namespace APIELIB.DTOs;

/// <summary>
/// Thông tin chi tiết một tài liệu (dùng cho GetBook API)
/// </summary>
public class BookDetailResponse
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string? PublishDate { get; set; }
    public string? Keyword { get; set; }
    public string? Xml { get; set; }
    public string? OtherTitle { get; set; }
    public DateTime? Submited { get; set; }
    public long? CreatedBy { get; set; }
    public long? CollectionId { get; set; }
    public string? CollectionName { get; set; }
    public string? Images { get; set; }
    public int? TotalView { get; set; }
    public int? TotalDownload { get; set; }
    public byte? Status { get; set; }
    public long? SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public long? TopicId { get; set; }
    public string? TopicName { get; set; }
    public DateTime? LastUpdate { get; set; }
    public long? TypeId { get; set; }
    public long? UpdateBy { get; set; }
    public int? AllowDownload { get; set; }
    public int? Show { get; set; }
    public string? Page { get; set; }

    /// <summary>Danh sách metadata mở rộng của tài liệu</summary>
    public List<MetaDataDto> MetaData { get; set; } = new();
}

/// <summary>
/// Một trường metadata của tài liệu
/// </summary>
public class MetaDataDto
{
    public int? MetaDataFieldId { get; set; }
    public string? Value { get; set; }
    public string? Language { get; set; }
    public int? SortOrder { get; set; }
}
