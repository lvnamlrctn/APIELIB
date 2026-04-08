namespace APIELIB.DTOs;

/// <summary>
/// Tham số đầu vào cho API tìm kiếm tài liệu
/// Tương ứng với các tham số của stored procedure [Ebook].[SearchEbook1]
/// </summary>
public class SearchRequest
{
    /// <summary>ID chuyên mục (0 = tất cả, -1 = trả về rỗng, >0 = lọc theo collection)</summary>
    public long CollectionId { get; set; } = 0;

    /// <summary>ID người dùng</summary>
    public long UserId { get; set; } = 0;

    /// <summary>Từ khóa tìm theo tựa đề</summary>
    public string? Title { get; set; }

    /// <summary>Từ khóa tìm theo tác giả</summary>
    public string? Author { get; set; }

    /// <summary>Từ khóa tìm theo nhà xuất bản</summary>
    public string? Publisher { get; set; }

    /// <summary>Năm xuất bản</summary>
    public string? PublishDate { get; set; }

    /// <summary>Từ khóa tìm kiếm</summary>
    public string? Keyword { get; set; }

    /// <summary>Ngày nộp từ</summary>
    public string? SubmitedFrom { get; set; }

    /// <summary>Ngày nộp đến</summary>
    public string? SubmitedTo { get; set; }

    /// <summary>Trạng thái tài liệu</summary>
    public int Status { get; set; } = 0;

    /// <summary>ID chủ đề (0 = tất cả, !=0 = lọc theo topic và topic con)</summary>
    public long TopicId { get; set; } = 0;

    /// <summary>ID môn học (0 = tất cả, >0 = lọc theo subject và subject con)</summary>
    public long SubjectId { get; set; } = 0;

    /// <summary>Thứ tự sắp xếp (ví dụ: "submited desc", "totalView desc")</summary>
    public string? Order { get; set; }

    /// <summary>ID portal</summary>
    public string? PortalId { get; set; }

    /// <summary>Ngôn ngữ tài liệu</summary>
    public string? Language { get; set; }

    /// <summary>ID tài liệu cụ thể (nếu muốn lấy 1 item)</summary>
    public long Id { get; set; } = 0;

    /// <summary>Số tạp chí</summary>
    public string? SoTapChi { get; set; }

    /// <summary>Tên tạp chí</summary>
    public string? TenTapChi { get; set; }

    /// <summary>Ngày xuất bản từ</summary>
    public string? PublishDateFrom { get; set; }

    /// <summary>Ngày xuất bản đến</summary>
    public string? PublishDateTo { get; set; }

    /// <summary>Số item trên mỗi trang (mặc định 20)</summary>
    public int ItemPerPage { get; set; } = 20;

    /// <summary>Trang hiện tại (bắt đầu từ 1)</summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>ID ngành học</summary>
    public long NganHocId { get; set; } = 0;

    /// <summary>ID môn học</summary>
    public long MonHocId { get; set; } = 0;

    /// <summary>ID chương trình</summary>
    public long ProgramId { get; set; } = 0;

    /// <summary>ID loại tài liệu số</summary>
    public long DigTypeId { get; set; } = 0;

    /// <summary>Danh sách ID tài liệu (phân tách bởi dấu phẩy)</summary>
    public string? ItemIdList { get; set; }
}
