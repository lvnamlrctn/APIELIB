using Microsoft.EntityFrameworkCore;
using APIELIB.Data;
using APIELIB.Services;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Controllers
builder.Services.AddControllers();

// Cấu hình Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "APIELIB - API Thư viện số",
        Version = "v1",
        Description = "ASP.NET Core Web API cho hệ thống quản lý thư viện số (Electronic Library), sử dụng EF Core + SQL Server."
    });
});

// Đăng ký EF Core DbContext với SQL Server
builder.Services.AddDbContext<EbookDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký dịch vụ nghiệp vụ theo pattern Dependency Injection
builder.Services.AddScoped<IEbookService, EbookService>();

var app = builder.Build();

// Kích hoạt Swagger UI (cả trong môi trường production để dễ kiểm thử)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "APIELIB v1");
    options.RoutePrefix = string.Empty; // Swagger UI tại root "/"
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
