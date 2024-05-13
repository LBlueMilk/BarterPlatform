using BarterPlatform.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//註冊連線，名稱來自appsettings.json設定
builder.Services.AddDbContext<BarterPlatformContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("BarterPlatformConnection")));

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

/////////////////////////////////////////////////////////////////////

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

//開發環境才能用SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
