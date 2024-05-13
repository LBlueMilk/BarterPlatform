using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BarterPlatform.Models;
using SkiaSharp;

namespace BarterPlatform.Controllers
{

    public class PersonalPageController : Controller
    {
        private readonly BarterPlatformContext _context;

        public PersonalPageController(BarterPlatformContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {

            //  Cookie 取得 MemID
            var memberId = Request.Cookies["LoggedInMem"];

            if (string.IsNullOrEmpty(memberId))
            {
                // 沒找到 MemID，導向
                return RedirectToAction("Login", "Login");
            }

            // 根據 MemID 調資料
            var memberIdData = await _context.Member
                .Include(m => m.AdminRegion_Adm)
                .Include(m => m.District_Dis)
                .FirstOrDefaultAsync(m => m.MemID == memberId);

            // 根據 Comment資料表的Member_MemID 調資料
            var commentsIdData = await _context.Comment
                .Include(c => c.Item_ID)    //包含關聯資料
                .Where(c => c.Member_MemID == memberId)
                .ToListAsync();

            // 根據 Item資料表的Member_MemID 調資料
            var itemIdData = await _context.Item
                .Where(i => i.Member_MemID == memberId)
                .ToListAsync();

            var viewModel = new PersonalPageViewModel
            {
                Member = memberIdData,
                Comments = commentsIdData,
                Items = itemIdData
            };


            return View(viewModel);
        }



        // GET: PersonalPage/Edit/5
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

        // POST: PersonalPage/Edit/5
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


            // 檢查圖片格式
            var uploadPhoto = Request.Form.Files["uploadPhoto"];
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


        // POST: PersonalPage/Delete/5
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

        private bool MemberExists(string id)
        {
            return (_context.Member?.Any(e => e.MemID == id)).GetValueOrDefault();
        }
    }
}
