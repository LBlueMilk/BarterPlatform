using BarterPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace BarterPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //注入信箱資料
        private readonly BPEmail.SmtpSettings _smtpSettings;

        private readonly BarterPlatformContext _context; // 注入 BarterPlatformContext


        public HomeController(ILogger<HomeController> logger,IConfiguration configuration, BarterPlatformContext context)
        {
            _logger = logger;
            //取得資料後注入
            _smtpSettings = configuration.GetSection("SmtpSettings").Get<BPEmail.SmtpSettings>();
            _context = context; 
        }

        public IActionResult Index()
        {
            // 從 Items 資料表中取得最新的三筆資料(圖片)
            var items = _context.Item
                                .OrderByDescending(i => i.UploadTime)   //用時間分辨
                                .Take(5)
                                .ToList();

            return View(items);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ManagementRules()
        {
            return View();
        }

        public IActionResult BBS()
        {
            return View();
        }

        public IActionResult WebmastersWords() {
            return View();
        }

        public IActionResult Opening() {
            return View();
        }


        //訪客信箱留言
        [HttpPost]
        public async Task<IActionResult> SendComment(EMailCommentModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                        Subject = "新留言",
                        Body = $"姓名: {model.Name}\n電子郵件: {model.Email}\n留言:\n{model.Message}",
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add("barterplatform19111010@gmail.com");

                    using (var smtpClient = new SmtpClient(_smtpSettings.Host)
                    {
                        Port = _smtpSettings.Port,
                        Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                        EnableSsl = _smtpSettings.EnableSsl,
                    })
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }

                    TempData["Success"] = "留言已成功發送！";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString(), "Error sending email.");
                    TempData["Error"] = "發送留言時出現錯誤。請稍後再試。";
                }
            }

            return RedirectToAction("WebmastersWords");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // 用於正式環境錯誤頁面（不顯示 RequestId）
        [AllowAnonymous]
        public IActionResult ErrorProduction()
        {
            return View(); // 對應 Views/Shared/ErrorProduction.cshtml
        }

    }
}
