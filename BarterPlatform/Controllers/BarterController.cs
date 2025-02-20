using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BarterPlatform.Models;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;
using SkiaSharp;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using BarterPlatform.DTOs;

namespace BarterPlatform.Controllers
{
    public class BarterController : Controller
    {
        private readonly BarterPlatformContext _context;

        public BarterController(BarterPlatformContext context)
        {
            _context = context;
        }

        // GET: Barter
        public async Task<IActionResult> Index()
        {
            // 取得 "SearchResults" 在 Session 中儲存的 JSON 格式字串
            var jsonSearchResults = HttpContext.Session.GetString("SearchResults");

            // 用來儲存搜尋結果的清單型態，類型為 ItemDTO
            List<ItemDTO> searchResultsDTO = null;

            // 檢查是否有搜尋結果
            if (jsonSearchResults != null)
            {
                // 如果有，將 JSON 格式字串反序列化為 List<ItemDTO>
                searchResultsDTO = JsonConvert.DeserializeObject<List<ItemDTO>>(jsonSearchResults);
                // 刪除 Session 中的 "SearchResults" 項目，因為已經處理完
                HttpContext.Session.Remove("SearchResults");
            }

            // 創建一個 IQueryable<Item> 來查詢資料庫中的 Item 並包含關聯的 Member_Mem
            IQueryable<Item> barterPlatformContext = _context.Item.Include(i => i.Member_Mem);

            // 如果有搜尋結果，則進行篩選
            if (searchResultsDTO != null && searchResultsDTO.Any())
            {
                // 提取搜尋結果中所有的 ItemName
                var itemNames = searchResultsDTO.Select(dto => dto.ItemName).ToList();
                // 根據 ItemName 篩選 Item，僅保留那些在搜尋結果中的項目
                barterPlatformContext = barterPlatformContext.Where(item => itemNames.Contains(item.ItemName));
            }

            // 將篩選後的結果轉換為 List 並傳遞給視圖進行顯示
            return View(await barterPlatformContext.ToListAsync());
        }


        //搜尋功能
        public async Task<IActionResult> SearchFE(string searchText)
        {

            //尋找物品名稱和敘述欄位
            var searchResults = await _context.Item
                .Include(i => i.Member_Mem)
                .Where(e => e.ItemName.Contains(searchText) || e.IteDesc.Contains(searchText) || e.Member_Mem.PersonalName.Contains(searchText))
                .Select(e => new ItemDTO // 將結果投影為DTO，寫在DTOs底下
                {
                    ItemName = e.ItemName,
                    IteDesc = e.IteDesc,
                    PersonalName = e.Member_Mem.PersonalName
                })
                .ToListAsync();

            //將searchResults取得的值和jsonSettings，物件序列化成JSON格式字串
            var jsonSearchResults = JsonConvert.SerializeObject(searchResults);
            //將jsonSearchResults儲存在叫SearchResults的Session中
            HttpContext.Session.SetString("SearchResults", jsonSearchResults);

            return RedirectToAction("Index", "Barter");
        }


        // GET: Barter/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Item == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Member_Mem)
                .FirstOrDefaultAsync(m => m.IteID == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Barter/Create
        public IActionResult Create()
        {
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID");
            return View();
        }

        // POST: Barter/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IteID,ItemName,IteImage,IteDesc,UploadTime,Bargain,Member_MemID")] Item item, IFormFile uploadPhoto)
        {

            //檢查圖片格式
            if (uploadPhoto != null)
            {
                if (uploadPhoto.Length > 1.5 * 1024 * 1024) // 1.5MB in bytes
                {
                    // 如果圖片大小超過1.5MB，進行壓縮
                    using (var memoryStream = new MemoryStream())
                    {
                        // 讀取上傳的圖片並進行壓縮
                        using (var inputStream = uploadPhoto.OpenReadStream())
                        {
                            using (var bitmap = SKBitmap.Decode(inputStream))
                            {
                                // 調整圖片大小
                                bitmap.Resize(new SKImageInfo(1024, 1024), SKFilterQuality.Medium);

                                // 將圖片編碼為 JPEG 格式，並將品質設為 75
                                using (var image = SKImage.FromBitmap(bitmap))
                                {
                                    image.Encode(SKEncodedImageFormat.Jpeg, 75).SaveTo(memoryStream);
                                }
                            }
                        }
                        // 將壓縮後的圖片陣列賦值給資料欄位IDImage
                        item.IteImage = memoryStream.ToArray();
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
                        item.IteImage = memoryStream.ToArray();
                    }
                }
            }

            //不檢查uploadPhoto，因為uploadPhoto是自己寫的，不然會一直報錯，即使規則正確，但不能直接把ModelState.IsValid驗證刪除，否則會更多問題
            ModelState.Remove("uploadPhoto");

            //取得登入者ID
            string memID = Request.Cookies["LoggedInMem"];
            //將登入者ID和資料庫資料比對
            var memberData = _context.Member
     .FirstOrDefault(member => member.MemID == memID);

            //如果有就寫入Member_MemID
            if (memberData != null && memberData.MemID == memID)
            {
                item.Member_MemID = memID;
            }

            //不驗證，我也不知道怎麼改
            ModelState.Remove("Member_MemID");
            ModelState.Remove("Member_Mem");

            if (ModelState.IsValid)
            {

                item.IteID = GenerateItemID();
                item.UploadTime = DateTime.Now;

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", item.Member_MemID);
            return View(item);
        }


        //自動產生ItemID
        private int GenerateItemID()
        {
            //從資料庫中取得目前的Item的最大數
            int maxIteID = _context.Item.Max(item => item.IteID);

            //生成新的ItemID
            int newItemID = maxIteID + 1;

            return newItemID;
        }


        // GET: Barter/Edit/5
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

            var loggedInMemID = HttpContext.Request.Cookies["LoggedInMem"];

            //如果當前 LoggedInMem 和 MemID 不匹配，禁止訪問
            if (loggedInMemID != item.Member_MemID)
            {
                return Forbid();
            }

            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", item.Member_MemID);
            return View(item);
        }

        // POST: Barter/Edit/5
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
                if (uploadPhoto.Length > 1.5 * 1024 * 1024) // 1.5MB in bytes
                {
                    // 如果圖片大小超過1.5MB，進行壓縮
                    using (var memoryStream = new MemoryStream())
                    {
                        // 讀取上傳的圖片並進行壓縮
                        using (var inputStream = uploadPhoto.OpenReadStream())
                        {
                            using (var bitmap = SKBitmap.Decode(inputStream))
                            {
                                // 調整圖片大小
                                bitmap.Resize(new SKImageInfo(1024, 1024), SKFilterQuality.Medium);

                                // 將圖片編碼為 JPEG 格式，並將品質設為 75
                                using (var image = SKImage.FromBitmap(bitmap))
                                {
                                    image.Encode(SKEncodedImageFormat.Jpeg, 75).SaveTo(memoryStream);
                                }
                            }
                        }
                        // 將壓縮後的圖片陣列賦值給資料欄位IDImage
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
                return RedirectToAction("BarterChat", "Barter", new { id = item.IteID });
            }
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", item.Member_MemID);
            return View(item);
        }


        // GET: Barter/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Item == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Member_Mem)
                .FirstOrDefaultAsync(m => m.IteID == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Barter/Delete/5
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


        public async Task<IActionResult> BarterChat(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var item = await _context.Item
            .Include(i => i.Member_Mem) // 加載 Member_Mem 屬性
            .FirstOrDefaultAsync(m => m.IteID == id);

            if (item == null)
            {
                return NotFound();
            }

            //取得當前物品留言ID相同的
            var comments = await _context.Comment
                .Include(c => c.Member_Mem)
                .Where(c => c.Item_IteID == id) //只選擇和當前物品留言ID相同的
                .ToListAsync();


            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = new BarterIteComModel
            {
                Item = item,
                Comments = comments
            };

            return View(model);
        }

        public async Task<IActionResult> FEComEdit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            ViewData["Item_IteID"] = new SelectList(_context.Item, "IteID", "IteID", comment.Item_IteID);
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", comment.Member_MemID);

            return View(comment);
        }



        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FEComEdit(int id, [Bind("ComID,ComContent,ComTime,Member_MemID,Item_IteID")] Comment comment)
        {
            if (id != comment.ComID)
            {
                return NotFound();
            }

            ModelState.Remove("Item_ID");
            ModelState.Remove("Member_Mem");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(comment.ComID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("BarterChat", "Barter", new { id = comment.Item_IteID });
            }
            ViewData["Item_IteID"] = new SelectList(_context.Item, "IteID", "IteID", comment.Item_IteID);
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", comment.Member_MemID);
            return View(comment);
        }

        [HttpPost, ActionName("FEComDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FEComDelete(int id)
        {
            if (_context.Comment == null)
            {
                return Problem("Entity set 'BarterPlatformContext.Comment'  is null.");
            }
            var comment = await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                _context.Comment.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("BarterChat", "Barter", new { id = comment.Item_IteID });
        }


        private bool ItemExists(int id)
        {
            return (_context.Item?.Any(e => e.IteID == id)).GetValueOrDefault();
        }
    }
}
