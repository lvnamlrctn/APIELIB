using Microsoft.EntityFrameworkCore;
using APIELIB.Data;
using APIELIB.DTOs;
using APIELIB.Models;

namespace APIELIB.Services;

/// <summary>
/// Triển khai nghiệp vụ thư viện số, chuyển đổi logic từ stored procedure sang EF Core LINQ
/// </summary>
public class EbookService : IEbookService
{
    private readonly EbookDbContext _context;

    public EbookService(EbookDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Tìm kiếm tài liệu theo nhiều tiêu chí.
    /// Logic tương đương stored procedure [Ebook].[SearchEbook1].
    /// </summary>
    public async Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        // Khi collectionId = -1, trả về kết quả rỗng ngay
        if (request.CollectionId == -1)
        {
            return new SearchResponse
            {
                Data = new List<SearchItemDto>(),
                TotalCount = 0,
                CurrentPage = request.CurrentPage,
                ItemPerPage = request.ItemPerPage,
                TotalPages = 0
            };
        }

        // Bắt đầu query từ bảng Item, join với ItemXml
        var query = _context.Items
            .Include(i => i.ItemXml)
            .Include(i => i.Collection)
            .Include(i => i.Subject)
            .Include(i => i.Topic)
            .AsQueryable();

        // Lọc theo portalId nếu có
        if (!string.IsNullOrWhiteSpace(request.PortalId))
        {
            query = query.Where(i => i.PortalId == request.PortalId);
        }

        // Lọc theo ngôn ngữ nếu có
        if (!string.IsNullOrWhiteSpace(request.Language))
        {
            query = query.Where(i => i.Language == request.Language);
        }

        // Lọc theo collectionId (bao gồm cả collection con)
        if (request.CollectionId > 0)
        {
            var collectionIds = await GetChildCollectionIds(request.CollectionId);
            query = query.Where(i => i.CollectionId.HasValue && collectionIds.Contains(i.CollectionId.Value));
        }

        // Lọc theo topicId (bao gồm cả topic con)
        if (request.TopicId != 0)
        {
            var topicIds = await GetChildTopicIds(request.TopicId);
            query = query.Where(i => i.TopicId.HasValue && topicIds.Contains(i.TopicId.Value));
        }

        // Lọc theo subjectId (bao gồm cả subject con)
        if (request.SubjectId > 0)
        {
            var subjectIds = await GetChildSubjectIds(request.SubjectId);
            query = query.Where(i => i.SubjectId.HasValue && subjectIds.Contains(i.SubjectId.Value));
        }

        // Lọc theo status nếu có (status != 0 mới lọc)
        if (request.Status != 0)
        {
            query = query.Where(i => i.Status == (byte)request.Status);
        }

        // Lọc theo ID cụ thể
        if (request.Id > 0)
        {
            query = query.Where(i => i.Id == request.Id);
        }

        // Lọc theo loại tài liệu số
        if (request.DigTypeId > 0)
        {
            query = query.Where(i => i.TypeId == request.DigTypeId);
        }

        // Lọc theo ngày nộp (submited) từ - đến
        if (!string.IsNullOrWhiteSpace(request.SubmitedFrom) && DateTime.TryParse(request.SubmitedFrom, out var submitedFrom))
        {
            query = query.Where(i => i.Submited >= submitedFrom);
        }
        if (!string.IsNullOrWhiteSpace(request.SubmitedTo) && DateTime.TryParse(request.SubmitedTo, out var submitedTo))
        {
            query = query.Where(i => i.Submited <= submitedTo.AddDays(1));
        }

        // Lọc theo danh sách ID cụ thể
        if (!string.IsNullOrWhiteSpace(request.ItemIdList))
        {
            var ids = request.ItemIdList.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => long.TryParse(s.Trim(), out var v) ? v : (long?)null)
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();
            if (ids.Count > 0)
            {
                query = query.Where(i => ids.Contains(i.Id));
            }
        }

        // --- Lọc theo MetaDataValue ---

        // Tìm theo Title (MetaDataFieldId = 64)
        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            var titleLower = request.Title.ToLower();
            var titleItemIds = _context.MetaDataValues
                .Where(m => m.MetaDataFieldId == 64 &&
                            m.Value != null &&
                            m.Value.ToLower().Contains(titleLower))
                .Select(m => m.ItemId);
            query = query.Where(i => titleItemIds.Contains(i.Id));
        }

        // Tìm theo Keyword (MetaDataFieldId = 57)
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var kwLower = request.Keyword.ToLower();
            var kwItemIds = _context.MetaDataValues
                .Where(m => m.MetaDataFieldId == 57 &&
                            m.Value != null &&
                            m.Value.ToLower().Contains(kwLower))
                .Select(m => m.ItemId);
            query = query.Where(i => kwItemIds.Contains(i.Id));
        }

        // Tìm theo Author: qua cột author của itemXml VÀ qua MetaDataValue (MetaDataFieldId = 3)
        if (!string.IsNullOrWhiteSpace(request.Author))
        {
            var authorLower = request.Author.ToLower();
            var authorItemIds = _context.MetaDataValues
                .Where(m => m.MetaDataFieldId == 3 &&
                            m.Value != null &&
                            m.Value.ToLower().Contains(authorLower))
                .Select(m => m.ItemId);
            query = query.Where(i =>
                (i.ItemXml != null && i.ItemXml.Author != null && i.ItemXml.Author.ToLower().Contains(authorLower))
                || authorItemIds.Contains(i.Id));
        }

        // Tìm theo Publisher (MetaDataFieldId = 39)
        if (!string.IsNullOrWhiteSpace(request.Publisher))
        {
            var pubLower = request.Publisher.ToLower();
            var pubItemIds = _context.MetaDataValues
                .Where(m => m.MetaDataFieldId == 39 &&
                            m.Value != null &&
                            m.Value.ToLower().Contains(pubLower))
                .Select(m => m.ItemId);
            query = query.Where(i => pubItemIds.Contains(i.Id));
        }

        // Tìm theo PublishDate (MetaDataFieldId = 15)
        if (!string.IsNullOrWhiteSpace(request.PublishDate))
        {
            var pdLower = request.PublishDate.ToLower();
            var pdItemIds = _context.MetaDataValues
                .Where(m => m.MetaDataFieldId == 15 &&
                            m.Value != null &&
                            m.Value.ToLower().Contains(pdLower))
                .Select(m => m.ItemId);
            query = query.Where(i => pdItemIds.Contains(i.Id));
        }

        // Tìm theo TenTapChi / SoTapChi (MetaDataFieldId = 43)
        if (!string.IsNullOrWhiteSpace(request.TenTapChi))
        {
            var tapChiLower = request.TenTapChi.ToLower();
            var tapChiItemIds = _context.MetaDataValues
                .Where(m => m.MetaDataFieldId == 43 &&
                            m.Value != null &&
                            m.Value.ToLower().Contains(tapChiLower))
                .Select(m => m.ItemId);
            query = query.Where(i => tapChiItemIds.Contains(i.Id));
        }

        if (!string.IsNullOrWhiteSpace(request.SoTapChi))
        {
            var soTapChiLower = request.SoTapChi.ToLower();
            var soTapChiItemIds = _context.MetaDataValues
                .Where(m => m.MetaDataFieldId == 43 &&
                            m.Value != null &&
                            m.Value.ToLower().Contains(soTapChiLower))
                .Select(m => m.ItemId);
            query = query.Where(i => soTapChiItemIds.Contains(i.Id));
        }

        // --- Đếm tổng số kết quả ---
        var totalCount = await query.CountAsync();

        // --- Sắp xếp động theo tham số order ---
        query = ApplyOrdering(query, request.Order);

        // --- Phân trang ---
        var currentPage = request.CurrentPage < 1 ? 1 : request.CurrentPage;
        var itemPerPage = request.ItemPerPage < 1 ? 20 : request.ItemPerPage;
        var skip = (currentPage - 1) * itemPerPage;

        var items = await query
            .Skip(skip)
            .Take(itemPerPage)
            .ToListAsync();

        // --- Map sang DTO ---
        var data = items.Select(i => new SearchItemDto
        {
            Id = i.Id,
            Title = i.ItemXml?.Title,
            Author = i.ItemXml?.Author,
            Publisher = i.ItemXml?.Publisher,
            PublishDate = i.ItemXml?.PublishDate,
            Keyword = i.ItemXml?.Keyword,
            OtherTitle = i.ItemXml?.OtherTitle,
            Page = i.ItemXml?.Page,
            Submited = i.Submited,
            CreatedBy = i.CreatedBy,
            CollectionId = i.CollectionId,
            Images = i.Images,
            TotalView = i.TotalView,
            TotalDownload = i.TotalDownload,
            Status = i.Status,
            SubjectId = i.SubjectId,
            LastUpdate = i.LastUpdate,
            TypeId = i.TypeId,
            UpdateBy = i.UpdateBy,
            AllowDownload = i.AllowDownload,
            CollectionName = i.Collection?.Name,
            SubjectName = i.Subject?.Name,
            TopicName = i.Topic?.Name,
            Show = i.Show
        }).ToList();

        return new SearchResponse
        {
            Data = data,
            TotalCount = totalCount,
            CurrentPage = currentPage,
            ItemPerPage = itemPerPage,
            TotalPages = (int)Math.Ceiling((double)totalCount / itemPerPage)
        };
    }

    /// <summary>
    /// Lấy thông tin chi tiết một tài liệu theo ID, kèm metadata mở rộng
    /// </summary>
    public async Task<BookDetailResponse?> GetBookAsync(long id)
    {
        var item = await _context.Items
            .Include(i => i.ItemXml)
            .Include(i => i.Collection)
            .Include(i => i.Subject)
            .Include(i => i.Topic)
            .Include(i => i.MetaDataValues)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
            return null;

        return new BookDetailResponse
        {
            Id = item.Id,
            Title = item.ItemXml?.Title,
            Author = item.ItemXml?.Author,
            Publisher = item.ItemXml?.Publisher,
            PublishDate = item.ItemXml?.PublishDate,
            Keyword = item.ItemXml?.Keyword,
            Xml = item.ItemXml?.Xml,
            OtherTitle = item.ItemXml?.OtherTitle,
            Page = item.ItemXml?.Page,
            Submited = item.Submited,
            CreatedBy = item.CreatedBy,
            CollectionId = item.CollectionId,
            CollectionName = item.Collection?.Name,
            Images = item.Images,
            TotalView = item.TotalView,
            TotalDownload = item.TotalDownload,
            Status = item.Status,
            SubjectId = item.SubjectId,
            SubjectName = item.Subject?.Name,
            TopicId = item.TopicId,
            TopicName = item.Topic?.Name,
            LastUpdate = item.LastUpdate,
            TypeId = item.TypeId,
            UpdateBy = item.UpdateBy,
            AllowDownload = item.AllowDownload,
            Show = item.Show,
            MetaData = item.MetaDataValues.Select(m => new MetaDataDto
            {
                MetaDataFieldId = m.MetaDataFieldId,
                Value = m.Value,
                Language = m.Language,
                SortOrder = m.SortOrder
            }).ToList()
        };
    }

    /// <summary>
    /// Lấy danh sách ID các collection bao gồm collection cha và tất cả collection con
    /// </summary>
    private async Task<HashSet<long>> GetChildCollectionIds(long parentId)
    {
        var allIds = new HashSet<long> { parentId };
        var allCollections = await _context.Collections
            .Select(c => new { c.Id, c.ParentId })
            .ToListAsync();

        // Duyệt đệ quy để tìm tất cả collection con
        var queue = new Queue<long>();
        queue.Enqueue(parentId);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var children = allCollections
                .Where(c => c.ParentId == current)
                .Select(c => c.Id)
                .ToList();
            foreach (var child in children)
            {
                if (allIds.Add(child))
                {
                    queue.Enqueue(child);
                }
            }
        }
        return allIds;
    }

    /// <summary>
    /// Lấy danh sách ID các topic bao gồm topic cha và tất cả topic con
    /// </summary>
    private async Task<HashSet<long>> GetChildTopicIds(long parentId)
    {
        var allIds = new HashSet<long> { parentId };
        var allTopics = await _context.Topics
            .Select(t => new { t.Id, t.ParentId })
            .ToListAsync();

        var queue = new Queue<long>();
        queue.Enqueue(parentId);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var children = allTopics
                .Where(t => t.ParentId == current)
                .Select(t => t.Id)
                .ToList();
            foreach (var child in children)
            {
                if (allIds.Add(child))
                {
                    queue.Enqueue(child);
                }
            }
        }
        return allIds;
    }

    /// <summary>
    /// Lấy danh sách ID các subject bao gồm subject cha và tất cả subject con
    /// </summary>
    private async Task<HashSet<long>> GetChildSubjectIds(long parentId)
    {
        var allIds = new HashSet<long> { parentId };
        var allSubjects = await _context.Subjects
            .Select(s => new { s.Id, s.ParentId })
            .ToListAsync();

        var queue = new Queue<long>();
        queue.Enqueue(parentId);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var children = allSubjects
                .Where(s => s.ParentId == current)
                .Select(s => s.Id)
                .ToList();
            foreach (var child in children)
            {
                if (allIds.Add(child))
                {
                    queue.Enqueue(child);
                }
            }
        }
        return allIds;
    }

    /// <summary>
    /// Áp dụng sắp xếp động dựa trên tham số order
    /// Ví dụ: "submited desc", "totalView desc", "title asc"
    /// </summary>
    private IQueryable<Item> ApplyOrdering(IQueryable<Item> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
        {
            // Mặc định sắp xếp theo ngày nộp mới nhất
            return query.OrderByDescending(i => i.Submited);
        }

        var parts = order.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var field = parts[0];
        var direction = parts.Length > 1 ? parts[1] : "asc";

        return (field, direction) switch
        {
            ("submited", "desc") => query.OrderByDescending(i => i.Submited),
            ("submited", _)      => query.OrderBy(i => i.Submited),
            ("totalview", "desc") => query.OrderByDescending(i => i.TotalView),
            ("totalview", _)      => query.OrderBy(i => i.TotalView),
            ("totaldownload", "desc") => query.OrderByDescending(i => i.TotalDownload),
            ("totaldownload", _)      => query.OrderBy(i => i.TotalDownload),
            ("lastupdate", "desc") => query.OrderByDescending(i => i.LastUpdate),
            ("lastupdate", _)      => query.OrderBy(i => i.LastUpdate),
            ("id", "desc") => query.OrderByDescending(i => i.Id),
            ("id", _)      => query.OrderBy(i => i.Id),
            _ => query.OrderByDescending(i => i.Submited)
        };
    }
}
