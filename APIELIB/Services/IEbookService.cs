using APIELIB.DTOs;

namespace APIELIB.Services;

/// <summary>
/// Interface định nghĩa các nghiệp vụ của hệ thống thư viện số
/// </summary>
public interface IEbookService
{
    /// <summary>
    /// Tìm kiếm tài liệu theo nhiều tiêu chí, có hỗ trợ phân trang
    /// </summary>
    /// <param name="request">Tham số tìm kiếm</param>
    /// <returns>Kết quả tìm kiếm có phân trang</returns>
    Task<SearchResponse> SearchAsync(SearchRequest request);

    /// <summary>
    /// Lấy thông tin chi tiết một tài liệu theo ID
    /// </summary>
    /// <param name="id">ID tài liệu</param>
    /// <returns>Thông tin chi tiết tài liệu, hoặc null nếu không tìm thấy</returns>
    Task<BookDetailResponse?> GetBookAsync(long id);
}
