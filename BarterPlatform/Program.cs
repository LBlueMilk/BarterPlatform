using BarterPlatform.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//���U�s�u�A�W�٨Ӧ�appsettings.json�]�w
builder.Services.AddDbContext<BarterPlatformContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("BarterPlatformConnection")));

//���USession
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    //�|�ܿﶵ�A���ݭn����
    options.IdleTimeout = TimeSpan.FromMinutes(20); // 20 ����
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // ���n�� cookie�A���� GDPR �v�T
});
builder.Services.AddControllersWithViews();

//���URazorPages
builder.Services.AddRazorPages();
//���UHttpContext�A��
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen();

/////////////////////////////////////////////////////////////////////

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

//�}�o���Ҥ~���SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//�ҥ�Session
app.UseSession();
// HttpsRedirection
app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Opening}/{id?}");

app.Run();