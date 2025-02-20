using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BarterPlatform.Models;
using System.Diagnostics.Metrics;
using SkiaSharp;

namespace BarterPlatform.Controllers
{
    public class ItemsController : Controller
    {
        private readonly BarterPlatformContext _context;

        public ItemsController(BarterPlatformContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var barterPlatformContext = _context.Item.Include(i => i.Member_Mem);
            return View(await barterPlatformContext.ToListAsync());
        }



        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Item == null)
            {
                return NotFound();
            }

            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", item.Member_MemID);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IteID,ItemName,IteImage,IteDesc,UploadTime,Bargain,Member_MemID")] Item item)
        {
            if (id != item.IteID)
            {
                return NotFound();
            }

            //將name叫uploadPhoto的地方取得資料
            var uploadPhoto = Request.Form.Files["uploadPhoto"];
            //如果有資料且長度超過0
            if (uploadPhoto != null && uploadPhoto.Length > 0)
            {
                if (uploadPhoto.Length > 100 * 512) // 100KB in bytes
                {
                    // 如果圖片大小超過100KB，進行壓縮
                    using (var memoryStream = new MemoryStream())
                    {
                        // 讀取上傳的圖片並進行壓縮
                        using (var inputStream = uploadPhoto.OpenReadStream())
                        {
                            using (var bitmap = SKBitmap.Decode(inputStream))
                            {
                                // 調整圖片大小
                                bitmap.Resize(new SKImageInfo(512, 512), SKFilterQuality.Medium);

                                //將圖片編碼為 JPEG 格式，並將品質設為 75
                                int quality = 75;                                 
                                SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Jpeg;

                                do
                                {
                                    //清空內存流
                                    memoryStream.SetLength(0); 
                                    //壓縮儲存
                                    using (var image = SKImage.FromBitmap(bitmap))
                                    {
                                        image.Encode(imageFormat, quality).SaveTo(memoryStream);
                                    }

                                    //如果壓縮後仍然超過100KB，則降低質量
                                    quality -= 5;
                                } while (memoryStream.Length > 100 * 512 && quality > 0);
                            }
                        }
                        item.IteImage = memoryStream.ToArray();
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
                        item.IteImage = memoryStream.ToArray();
                    }
                }
            }
            else
            {
                // 如果沒有上傳新檔案，則不覆寫原始IDImage
                var originalIDImage = await _context.Item.FindAsync(item.IteID);
                item.IteImage = originalIDImage.IteImage;
            }


            //不檢查uploadPhoto，因為uploadPhoto是自己寫的，不然會一直報錯，即使規則正確，但不能直接把ModelState.IsValid驗證刪除，否則會更多問題
            ModelState.Remove("uploadPhoto");

            //不驗證，我也不知道怎麼改
            ModelState.Remove("Member_MemID");
            ModelState.Remove("Member_Mem");
            
            if (ModelState.IsValid)
            {
                try
                {
                    //根據主鍵決定是否可能衝突
                    var pictureUpdated = await _context.Item.FindAsync(item.IteID);
                    if (pictureUpdated != null)
                    {
                        //將資料內容分離，這樣才不會有兩筆相同key的資料衝突
                        _context.Entry(pictureUpdated).State = EntityState.Detached;
                    }

                    //更新資料
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.IteID))
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
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", item.Member_MemID);
            return View(item);
        }



        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 查找要删除的项目
            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            //找和項目關聯的評論
            var relatedComments = _context.Comment.Where(c => c.Item_IteID == id).ToList();

            //刪除關聯評論
            foreach (var comment in relatedComments)
            {
                _context.Comment.Remove(comment);
            }

            //刪除該項目
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
          return (_context.Item?.Any(e => e.IteID == id)).GetValueOrDefault();
        }
    }
}
