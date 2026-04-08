using Microsoft.AspNetCore.Mvc;
using APIELIB.DTOs;
using APIELIB.Services;

namespace APIELIB.Controllers;

/// <summary>
/// Controller xử lý các API liên quan đến tài liệu thư viện số
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EbookController : ControllerBase
{
    private readonly IEbookService _ebookService;
    private readonly ILogger<EbookController> _logger;

    public EbookController(IEbookService ebookService, ILogger<EbookController> logger)
    {
        _ebookService = ebookService;
        _logger = logger;
    }

    /// <summary>
    /// Tìm kiếm tài liệu theo nhiều tiêu chí
    /// </summary>
    /// <param name="request">Tham số tìm kiếm</param>
    /// <returns>Danh sách tài liệu phù hợp, có phân trang</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SearchResponse>> Search([FromBody] SearchRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Làm sạch giá trị trước khi ghi log để tránh log forging
        var safeTitle = request.Title?.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
        _logger.LogInformation("Search request: CollectionId={CollectionId}, Title={Title}, Page={Page}",
            request.CollectionId, safeTitle, request.CurrentPage);

        var result = await _ebookService.SearchAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một tài liệu theo ID
    /// </summary>
    /// <param name="id">ID tài liệu</param>
    /// <returns>Thông tin chi tiết tài liệu kèm metadata</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(BookDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDetailResponse>> GetBook(long id)
    {
        _logger.LogInformation("GetBook request: Id={Id}", id);

        var result = await _ebookService.GetBookAsync(id);
        if (result == null)
            return NotFound(new { message = $"Không tìm thấy tài liệu với ID = {id}" });

        return Ok(result);
    }
}
