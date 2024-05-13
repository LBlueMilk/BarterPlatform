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
    public class AuthenticationRecordsController : Controller
    {
        private readonly BarterPlatformContext _context;

        public AuthenticationRecordsController(BarterPlatformContext context)
        {
            _context = context;
        }

        // GET: AuthenticationRecords
        public async Task<IActionResult> Index()
        {
            var barterPlatformContext = _context.AuthenticationRecords.Include(a => a.Employee_Emp).Include(a => a.Member_Mem);
            return View(await barterPlatformContext.ToListAsync());
        }

        // GET: AuthenticationRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AuthenticationRecords == null)
            {
                return NotFound();
            }

            var authenticationRecords = await _context.AuthenticationRecords
                .Include(a => a.Employee_Emp)
                .Include(a => a.Member_Mem)
                .FirstOrDefaultAsync(m => m.AuthID == id);
            if (authenticationRecords == null)
            {
                return NotFound();
            }

            return View(authenticationRecords);
        }

        // GET: AuthenticationRecords/Create
        public IActionResult Create()
        {
            ViewData["Employee_EmpID"] = new SelectList(_context.Employee, "EmpID", "EmpID");
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID");
            return View();
        }

        // POST: AuthenticationRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthID,Employee_EmpID,Member_MemID,AuthStatus,AuthTime")] AuthenticationRecords authenticationRecords)
        {
            if (ModelState.IsValid)
            {
                _context.Add(authenticationRecords);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Employee_EmpID"] = new SelectList(_context.Employee, "EmpID", "EmpID", authenticationRecords.Employee_EmpID);
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", authenticationRecords.Member_MemID);
            return View(authenticationRecords);
        }

        // GET: AuthenticationRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AuthenticationRecords == null)
            {
                return NotFound();
            }

            var authenticationRecords = await _context.AuthenticationRecords.FindAsync(id);
            if (authenticationRecords == null)
            {
                return NotFound();
            }
            ViewData["Employee_EmpID"] = new SelectList(_context.Employee, "EmpID", "EmpID", authenticationRecords.Employee_EmpID);
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", authenticationRecords.Member_MemID);
            return View(authenticationRecords);
        }

        // POST: AuthenticationRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthID,Employee_EmpID,Member_MemID,AuthStatus,AuthTime")] AuthenticationRecords authenticationRecords)
        {
            if (id != authenticationRecords.AuthID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(authenticationRecords);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthenticationRecordsExists(authenticationRecords.AuthID))
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
            ViewData["Employee_EmpID"] = new SelectList(_context.Employee, "EmpID", "EmpID", authenticationRecords.Employee_EmpID);
            ViewData["Member_MemID"] = new SelectList(_context.Member, "MemID", "MemID", authenticationRecords.Member_MemID);
            return View(authenticationRecords);
        }

        // GET: AuthenticationRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AuthenticationRecords == null)
            {
                return NotFound();
            }

            var authenticationRecords = await _context.AuthenticationRecords
                .Include(a => a.Employee_Emp)
                .Include(a => a.Member_Mem)
                .FirstOrDefaultAsync(m => m.AuthID == id);
            if (authenticationRecords == null)
            {
                return NotFound();
            }

            return View(authenticationRecords);
        }

        // POST: AuthenticationRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AuthenticationRecords == null)
            {
                return Problem("Entity set 'BarterPlatformContext.AuthenticationRecords'  is null.");
            }
            var authenticationRecords = await _context.AuthenticationRecords.FindAsync(id);
            if (authenticationRecords != null)
            {
                _context.AuthenticationRecords.Remove(authenticationRecords);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthenticationRecordsExists(int id)
        {
          return (_context.AuthenticationRecords?.Any(e => e.AuthID == id)).GetValueOrDefault();
        }
    }
}
