using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BarterPlatform.Models;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Hosting;
using SkiaSharp;
using Org.BouncyCastle.Security;
using Microsoft.AspNetCore.Identity;

namespace BarterPlatform.Controllers
{
    public class LoginController : Controller
    {
        private readonly BarterPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        //ASP.NET Core 提供，依賴注入獲得根路徑IWebHostEnvironment
        public LoginController(BarterPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }



        // GET: Members/Create
        public IActionResult Create()
        {
            ViewData["AdminRegion_AdmID"] = new SelectList(_context.AdministrativeRegion, "AdmID", "AdminRegion");

            //顯示空白內容
            ViewBag.District_DisID = new SelectList(new List<SelectListItem>(), "Value", "Text");
            //直接顯示內容
            //ViewData["District_DisID"] = new SelectList(_context.District, "DisID", "DistrictName");


            return View();
        }


        //註冊會員並產生個人頁面
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemID,PersonalName,Nickname,Gender,BirthDate,PostalCode,AdminRegion_AdmID,District_DisID,OtherAddress,Username,Password,Email,Phone,NationalID,IDImage,CreationTime,DeletionTime,LastLogin,Status")] Member member, IFormFile uploadPhoto)
        {

            //檢查帳號是否存在
            var existingMemberUserName = await _context.Member.FirstOrDefaultAsync(m => m.Username == member.Username);
            if (existingMemberUserName != null)
            {
                ModelState.AddModelError("Username", "帳號已被註冊，請更換");
            }            

            //檢查圖片格式
            if (uploadPhoto != null)
            {
                if (uploadPhoto.Length > 2.4 * 1024 * 1024) // 2.4MB in bytes
                {
                    // 如果圖片大小超過2.4MB，進行壓縮
                    using (var memoryStream = new MemoryStream())
                    {
                        // 讀取上傳的圖片並進行壓縮
                        using (var inputStream = uploadPhoto.OpenReadStream())
                        {
                            using (var bitmap = SKBitmap.Decode(inputStream))
                            {
                                // 調整圖片大小
                                bitmap.Resize(new SKImageInfo(1024, 1024), SKFilterQuality.Medium);

                                // 將圖片編碼為 JPEG 格式
                                SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Jpeg, 75).SaveTo(memoryStream);
                            }
                        }

                        member.IDImage = memoryStream.ToArray();
                    }
                }
                else
                {
                    if (uploadPhoto.ContentType != "image/jpeg" && uploadPhoto.ContentType != "image/png")
                    {
                        ModelState.AddModelError("IDImage", "上傳的圖片格式不正確，請上傳 JPEG 或 PNG 格式的圖片。");
                    }
                    else
                    {
                        var memoryStream = new MemoryStream();
                        uploadPhoto.CopyTo(memoryStream);
                        member.IDImage = memoryStream.ToArray();
                    }
                }
            }

            //不檢查uploadPhoto，因為uploadPhoto是自己寫的，不然會一直報錯，即使規則正確，但不能直接把ModelState.IsValid驗證刪除，否則會更多問題
            ModelState.Remove("uploadPhoto");

            //通過驗證
            if (ModelState.IsValid)
            {

                //PasswordHasher包含雜湊和鹽的功能，使用PBKDF2
                //SQL格式未設定成VARCHAR(255)，所以無法使用
                //// 使用 PasswordHasher 對密碼進行哈希處理
                //var passwordHasher = new PasswordHasher<Member>();
                //member.Password = passwordHasher.HashPassword(member, member.Password); // 雜湊處理密碼

                //自動產生
                member.MemID = GenerateMemberID();
                //資料內容寫入
                member.CreationTime = DateTime.Now;
                member.LastLogin = DateTime.Now;
                member.Status = true;

                //建立前端Cookies
                Response.Cookies.Append("LoggedInMem", member.MemID, new CookieOptions
                {
                    //保持7天
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                // 新增資料到資料庫
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Personalpage");

            }
            //驗證失敗
            ViewData["AdminRegion_AdmID"] = new SelectList(_context.AdministrativeRegion, "AdmID", "AdminRegion", member.AdminRegion_AdmID);
            ViewData["District_DisID"] = new SelectList(_context.District, "DisID", "DistrictName", member.District_DisID);
            return View(member);
        }

        //自動產生MemberID
        private string GenerateMemberID()
        {
            //取得今天日期
            string todayDate = DateTime.Today.ToString("yyyyMMdd");

            //取得當天會員數量，利用CreationTime.Date(當天的日期)判斷
            int memberCount = _context.Member.Count(m => m.CreationTime.Date == DateTime.Today.Date);

            //生成後8碼的流水號
            string serialNumber = (memberCount + 1).ToString().PadLeft(8, '0');

            string newMemberID = todayDate + serialNumber;
            return newMemberID;
        }


        public IActionResult Login()
        {
            return View();
        }


        //會員登入
        [HttpPost]
        public async Task<IActionResult> LoginMem(LoginViewModel loginViewModel)
        {
            //定義是哪個資料庫
            Member loginMem = loginViewModel.Member;

            if (loginMem == null)
            {
                return View("Login", loginViewModel);
            }

            var result = await _context.Member
                .Where(x => x.Username == loginMem.Username && x.Password == loginMem.Password)
                .Select(x => x.MemID)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                ViewData["ErrorMsgMem"] = "帳號或密碼錯誤!!";
                return View("Login", loginViewModel);
            }

            ////建立前端Cookies
            Response.Cookies.Append("LoggedInMem", result, new CookieOptions
            {
                //保持7天
                Expires = DateTimeOffset.Now.AddDays(7)
            });

            return RedirectToAction("Index", "Home");
        }


        //員工登入
        [HttpPost]
        public async Task<IActionResult> LoginEmp(LoginViewModel loginViewModel)
        {
            //定義是哪個資料庫
            Employee loginEmp = loginViewModel.Employee;

            if (loginEmp == null)
            {
                return View();
            }

            var result = await _context.Employee
                .Where(x => x.Username == loginEmp.Username && x.Password == loginEmp.Password)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                ViewData["ErrorMsgEmp"] = "帳號或密碼錯誤!!有問題請洽主管人員";
                return View("Login", loginViewModel);
            }

            //建立後端Session
            HttpContext.Session.SetString("ManagerEmp", JsonConvert.SerializeObject(result));

            return RedirectToAction("Index", "Employees");
        }

        //登出移除
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("ManagerEmp");
            Response.Cookies.Delete("LoggedInMem");

            return RedirectToAction("Index", "Home");
        }



        private bool MemberExists(string id)
        {
            return (_context.Member?.Any(e => e.MemID == id)).GetValueOrDefault();
        }
    }
}
