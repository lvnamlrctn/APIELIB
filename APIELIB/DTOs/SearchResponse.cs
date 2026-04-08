namespace APIELIB.DTOs;

/// <summary>
/// Thông tin mỗi tài liệu trong kết quả tìm kiếm
/// </summary>
public class SearchItemDto
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string? PublishDate { get; set; }
    public string? Keyword { get; set; }
    public string? OtherTitle { get; set; }
    public DateTime? Submited { get; set; }
    public long? CreatedBy { get; set; }
    public long? CollectionId { get; set; }
    public string? Images { get; set; }
    public int? TotalView { get; set; }
    public int? TotalDownload { get; set; }
    public byte? Status { get; set; }
    public long? SubjectId { get; set; }
    public DateTime? LastUpdate { get; set; }
    public long? TypeId { get; set; }
    public long? UpdateBy { get; set; }
    public int? AllowDownload { get; set; }
    public string? CollectionName { get; set; }
    public string? SubjectName { get; set; }
    public string? TopicName { get; set; }
    public int? Show { get; set; }
    public string? Page { get; set; }
}

/// <summary>
/// Kết quả trả về từ API tìm kiếm (có phân trang)
/// </summary>
public class SearchResponse
{
    public List<SearchItemDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int ItemPerPage { get; set; }
    public int TotalPages { get; set; }
}
