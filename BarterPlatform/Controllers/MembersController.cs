using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BarterPlatform.Models;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Identity;
using System.Text;
using SkiaSharp;
using SkiaSharp.Extended;
//using System.Security.Cryptography; //使用雜湊(未使用)

namespace BarterPlatform.Controllers
{
    public class MembersController : Controller
    {
        private readonly BarterPlatformContext _context;

        public MembersController(BarterPlatformContext context)
        {
            _context = context;
        }


        ////未使用功能
        ////鹽+雜湊處理
        //public class PasswordHasher
        //{
        //    // 生成隨機鹽
        //    public static string GenerateSalt()
        //    {
        //        byte[] saltBytes = new byte[16]; // 鹽長度設16字
        //        using (var rng = RandomNumberGenerator.Create())
        //        {
        //            rng.GetBytes(saltBytes);
        //        }
        //        return Convert.ToBase64String(saltBytes);
        //    }

        //    // 雜湊處理
        //    public static string HashPassword(string password, string salt)
        //    {
        //        using (SHA256 sha256 = SHA256.Create())
        //        {
        //            byte[] bytes = Encoding.UTF8.GetBytes(salt + password); // 鹽和密碼組合
        //            byte[] hash = sha256.ComputeHash(bytes);
        //            return Convert.ToBase64String(hash);
        //        }
        //    }
        //}


        [HttpPost]
        public IActionResult GetFilteredDistricts(string selectedValue)
        {
            // 提取前兩個字
            string prefix = selectedValue.Substring(0, 2);

            // 獲取以修改後的前綴開頭的 District_DisID
            var filteredDistricts = _context.District
                .Where(d => d.DisID.StartsWith(prefix))
                .Select(d => new { Value = d.DisID, Text = d.DistrictName })
                .ToList();

            // 返回 JSON 格式的篩選結果
            return Json(filteredDistricts);
        }


        [HttpPost]
        public IActionResult GetDistrictDisID(string selectedAdmID)
        {
            // 提取 AdminRegion_AdmID 的第一個字符
            char prefix = selectedAdmID[0];

            // 在第一個字符前面新增 'A'
            string modifiedPrefix = "A" + prefix;

            // 獲取以修改後的前綴開頭的 District_DisID
            var districtDisIDs = _context.District
                                        .Where(d => d.DisID.StartsWith(modifiedPrefix))
                                        .Select(d => new { Value = d.DisID, Text = d.DistrictName }) // 將 DisID 做為 Value，DistrictName 做為 Text
                                        .ToList();

            // 返回 JSON 格式的篩選結果
            return Json(districtDisIDs);
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            var barterPlatformContext = _context.Member.Include(m => m.AdminRegion_Adm).Include(m => m.District_Dis);

            //ViewData["District"] = _context.District.Find(disid).DistrictName;           

            return View(await barterPlatformContext.ToListAsync());
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


        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

            ////未使用加密
            //// 取得鹽值
            //string salt = PasswordHasher.GenerateSalt();
            //// 對密碼進行雜湊處理
            //string hashedPassword = PasswordHasher.HashPassword(member.Password, salt);

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
                //自動產生
                member.MemID = GenerateMemberID();
                member.CreationTime = DateTime.Now;
                member.LastLogin = DateTime.Now;
                member.Status = true;

                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminRegion_AdmID"] = new SelectList(_context.AdministrativeRegion, "AdmID", "AdminRegion", member.AdminRegion_AdmID);
            ViewData["District_DisID"] = new SelectList(_context.District, "DisID", "DistrictName", member.District_DisID);
            return View(member);
        }




        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            ViewData["AdminRegion_AdmID"] = new SelectList(_context.AdministrativeRegion, "AdmID", "AdminRegion", member.AdminRegion_AdmID);
            ViewData["District_DisID"] = new SelectList(_context.District, "DisID", "DistrictName", member.District_DisID);
            //獲得暱稱資料給Edit用
            ViewData["Nickname"] = member.Nickname;
            //獲得名稱資料給Edit用
            ViewData["PersonalName"] = member.PersonalName;
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MemID,PersonalName,Nickname,Gender,BirthDate,PostalCode,AdminRegion_AdmID,District_DisID,OtherAddress,Username,Password,Email,Phone,NationalID,IDImage,CreationTime,DeletionTime,LastLogin,Status")] Member member)
        {
            if (id != member.MemID)
            {
                return NotFound();
            }


            // 檢查帳號是否存在(且排除自身)
            var existingMemberUserName = await _context.Member.FirstOrDefaultAsync(m => m.Username == member.Username && m.MemID != member.MemID);
            if (existingMemberUserName != null)
            {
                ModelState.AddModelError("Username", "帳號已被註冊，請更換");
            }


            //將name叫uploadPhoto的地方取得資料
            var uploadPhoto = Request.Form.Files["uploadPhoto"];

            //如果有資料且長度超過0
            if (uploadPhoto != null && uploadPhoto.Length > 0)
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
                                // 調整圖片大小為1024x1024
                                bitmap.Resize(new SKImageInfo(1024, 1024), SKFilterQuality.Medium);

                                // 將圖片編碼為 JPEG 格式，品質設為75
                                SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Jpeg, 75).SaveTo(memoryStream);
                            }
                        }

                        // 將壓縮後的圖片陣列賦值給資料欄位IDImage
                        member.IDImage = memoryStream.ToArray();
                    }
                }
                else
                {
                    // 檢查上傳的圖片格式
                    if (uploadPhoto.ContentType != "image/jpeg" && uploadPhoto.ContentType != "image/png")
                    {
                        ModelState.AddModelError("IDImage", "上傳的圖片格式不正確，請上傳 JPEG 或 PNG 格式的圖片。");
                    }
                    else
                    {
                        // 創建內存流並將上傳的圖片複製到內存流
                        var memoryStream = new MemoryStream();
                        uploadPhoto.CopyTo(memoryStream);
                        // 將內存流轉換為陣列並賦值給資料欄位IDImage
                        member.IDImage = memoryStream.ToArray();
                    }
                }
            }
            else
            {
                // 如果沒有上傳新檔案，則不覆寫原始IDImage
                var originalIDImage = await _context.Member.FindAsync(member.MemID);
                member.IDImage = originalIDImage.IDImage;
            }



            //不檢查uploadPhoto，因為uploadPhoto是自己寫的，不然會一直報錯，即使規則正確，但不能直接把ModelState.IsValid驗證刪除，否則會更多問題
            ModelState.Remove("uploadPhoto");

            if (ModelState.IsValid)
            {
                try
                {
                    //根據主鍵決定是否可能衝突
                    var pictureUpdated = await _context.Member.FindAsync(member.MemID);
                    if (pictureUpdated != null)
                    {
                        //將資料內容分離，這樣才不會有兩筆相同key的資料衝突
                        _context.Entry(pictureUpdated).State = EntityState.Detached;
                    }

                    //更新資料
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminRegion_AdmID"] = new SelectList(_context.AdministrativeRegion, "AdmID", "AdminRegion", member.AdminRegion_AdmID);
            ViewData["District_DisID"] = new SelectList(_context.District, "DisID", "DistrictName", member.District_DisID);
            return View(member);
        }


        //後端用
        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Member == null)
            {
                return Problem("Entity set 'BarterPlatformContext.Member'  is null.");
            }
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //前端用
        // POST: Members/Delete/5
        [HttpPost, ActionName("DeleteFE")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFrontEnd(string id)
        {
            if (_context.Member == null)
            {
                return Problem("Entity set 'BarterPlatformContext.Member'  is null.");
            }
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
            }

            //刪除Cookie
            Response.Cookies.Delete("LoggedInMem");

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool MemberExists(string id)
        {
            return (_context.Member?.Any(e => e.MemID == id)).GetValueOrDefault();
        }


    }
}
