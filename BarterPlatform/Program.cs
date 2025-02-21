using BarterPlatform.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//���USQL Server�s�u�A�W�٨Ӧ�appsettings.json�]�w
//builder.Services.AddDbContext<BarterPlatformContext>(option => 
//    option.UseSqlServer(builder.Configuration.GetConnectionString("BarterPlatformConnection")));

//���UNpgsql PostgresSQL�s�u�A�W�٨Ӧ�appsettings.json�]�w
builder.Services.AddDbContext<BarterPlatformContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BarterPlatformConnection")));

//���UNpgsql PostgresSQL�s�u�A�W�٨Ӧ�appsettings.json�]�w�A�Ω�Azure�����ܼƪ���
//builder.Services.AddDbContext<BarterPlatformContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING")));



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

//���USmtpSettings�t�m�A�Nsecrets.json�g�bAzure
builder.Services.Configure<BPEmail.SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


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

//CSP���e�w���F���A����XSS�����A�إߥզW��
//���p�G�ϥ�CSP�N��ACSS�BJS���ݭn�Υ~���ޤJ���覡�A�_�h�|�Q�פU
//�ثe���ϥ�
//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("Content-Security-Policy",
//        "default-src 'self'; " +  // ���\���J���a�귽
//        "script-src 'self' https://cdn.jsdelivr.net https://example.com; " +  // ���\ JS �ӷ�
//        "style-src 'self' https://cdn.jsdelivr.net https://fonts.googleapis.com; " +  // ���\ CSS �ӷ�
//        "img-src 'self' https://images.unsplash.com data:; " +  // ���\�Ϥ��ӷ��A�t Base64
//        "font-src 'self' https://fonts.gstatic.com; " +  // ���\�r��ӷ�
//        "connect-src 'self' https://api.example.com; " +  // ���\��� API �ШD
//        "frame-src 'self' https://www.youtube.com; " +  // ���\ iframe �ӷ�
//        "object-src 'none';" // �T��O�J���w�������� (�p Flash)
//    );
//    await next();
//});

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
