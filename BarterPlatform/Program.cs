using BarterPlatform.Models;
using BarterPlatform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


//判斷是否為Azure環境變數，否則使用本地連接字串
var connectionString = Environment.GetEnvironmentVariable("BarterPlatformConnection")
                       ?? builder.Configuration.GetConnectionString("BarterPlatformConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("未設定 'BarterPlatformConnection' 連接字串。");
}
//註冊SQL Server連線，名稱來自appsettings.json設定
builder.Services.AddDbContext<BarterPlatformContext>(options =>
    options.UseSqlServer(connectionString));


//註冊Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    //會話選項，有需要的話
    options.IdleTimeout = TimeSpan.FromMinutes(20); // 20 分鐘
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // 必要的 cookie，不受 GDPR 影響
});
builder.Services.AddControllersWithViews();

//註冊RazorPages
builder.Services.AddRazorPages();
//註冊HttpContext服務
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen();

//註冊SmtpSettings配置，將secrets.json寫在Azure
builder.Services.Configure<BPEmail.SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

//
builder.Services.AddScoped<IDatabaseHealthService, DatabaseHealthService>();



/////////////////////////////////////////////////////////////////////

var app = builder.Build();

// 錯誤發生時的處理
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/ErrorProduction");

    // 捕捉 HTTP 狀態碼錯誤 (如 404、403 等)，導向同一錯誤頁面
    app.UseStatusCodePagesWithReExecute("/Home/ErrorProduction");
}

//開發環境才能用SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//CSP內容安全政策，防止XSS攻擊，建立白名單
//但如果使用CSP代表，CSS、JS等需要用外部引入的方式，否則會被擋下
//目前不使用
//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("Content-Security-Policy",
//        "default-src 'self'; " +  // 允許載入本地資源
//        "script-src 'self' https://cdn.jsdelivr.net https://example.com; " +  // 允許 JS 來源
//        "style-src 'self' https://cdn.jsdelivr.net https://fonts.googleapis.com; " +  // 允許 CSS 來源
//        "img-src 'self' https://images.unsplash.com data:; " +  // 允許圖片來源，含 Base64
//        "font-src 'self' https://fonts.gstatic.com; " +  // 允許字體來源
//        "connect-src 'self' https://api.example.com; " +  // 允許後端 API 請求
//        "frame-src 'self' https://www.youtube.com; " +  // 允許 iframe 來源
//        "object-src 'none';" // 禁止嵌入不安全的物件 (如 Flash)
//    );
//    await next();
//});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//啟用Session
app.UseSession();
// HttpsRedirection
app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Opening}/{id?}");

app.Run();
