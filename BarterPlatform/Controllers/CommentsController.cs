using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BarterPlatform.Models;

namespace BarterPlatform.Controllers
{
    public class CommentsController : Controller
    {
        private readonly BarterPlatformContext _context;

        public CommentsController(BarterPlatformContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var barterPlatformContext = _context.Comment.Include(c => c.Item_ID).Include(c => c.Member_Mem);
            return View(await barterPlatformContext.ToListAsync());
        }

        //自動產生ComID
        private int GenerateComID()
        {
            //從資料庫中取得目前的Comment的最大值
            int maxComID = _context.Comment.Any() ? _context.Comment.Max(m => m.ComID) : 0;

            //生成新的ItemID
            int newCommentID = maxComID + 1;

            return newCommentID;
        }


        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["Item_IteID"] = new SelectList(_context.Item, "IteID", "IteDesc");
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ComID,ComContent,ComTime,Member_MemID,Item_IteID")] Comment comment, int itemPageId)
        {
            

            //取得登入者ID
            string memID = Request.Cookies["LoggedInMem"];
            //寫入Member_MemID留言者
            comment.Member_MemID = memID;
            //取得留言內容
            

            ModelState.Remove("Item_ID");
            ModelState.Remove("Member_Mem");
            ModelState.Remove("Member_MemID");

            try
            {
                if (ModelState.IsValid)
                {
                    comment.ComID = GenerateComID();
                    comment.Item_IteID = itemPageId;
                    comment.ComTime = DateTime.Now;

                    _context.Add(comment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                //紀錄異常狀態
                Console.WriteLine($"異常狀態：{ex.Message}");
                throw;
            }

            ViewData["Item_IteID"] = new SelectList(_context.Item, "IteID", "IteDesc", comment.Item_IteID);
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", comment.Member_MemID);
            return View(comment);
        }



        public async Task<IActionResult> Edit(int id)
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
        public async Task<IActionResult> Edit(int id, [Bind("ComID,ComContent,ComTime,Member_MemID,Item_IteID")] Comment comment)
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
                    if (!CommentExists(comment.ComID))
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
            ViewData["Item_IteID"] = new SelectList(_context.Item, "IteID", "IteID", comment.Item_IteID);
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", comment.Member_MemID);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Item_ID)
                .Include(c => c.Member_Mem)
                .FirstOrDefaultAsync(m => m.ComID == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
            return RedirectToAction(nameof(Index));
        }


        private bool CommentExists(int id)
        {
            return (_context.Comment?.Any(e => e.ComID == id)).GetValueOrDefault();
        }
    }
}
