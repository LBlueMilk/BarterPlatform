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
    public class AdministrativeRegionsController : Controller
    {
        private readonly BarterPlatformContext _context;

        public AdministrativeRegionsController(BarterPlatformContext context)
        {
            _context = context;
        }

        // GET: AdministrativeRegions
        public async Task<IActionResult> Index()
        {
              return _context.AdministrativeRegion != null ? 
                          View(await _context.AdministrativeRegion.ToListAsync()) :
                          Problem("Entity set 'BarterPlatformContext.AdministrativeRegion'  is null.");
        }

        // GET: AdministrativeRegions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.AdministrativeRegion == null)
            {
                return NotFound();
            }

            var administrativeRegion = await _context.AdministrativeRegion
                .FirstOrDefaultAsync(m => m.AdmID == id);
            if (administrativeRegion == null)
            {
                return NotFound();
            }

            return View(administrativeRegion);
        }

        //驗證編號是否重複
        [HttpPost]
        public IActionResult CheckAdmID(string AdmID)
        {
            if (_context.AdministrativeRegion == null)
            {
                return Json(false); // or return Json("Entity set 'BarterPlatformContext.AdministrativeRegion'  is null.");
            }

            bool isAdmIDExist = _context.AdministrativeRegion.Any(a => a.AdmID == AdmID);
            return Json(!isAdmIDExist);
        }

        //驗證名稱是否重複
        [HttpPost]
        public IActionResult CheckAdminRegion(string AdminRegion)
        {
            if (_context.AdministrativeRegion == null)
            {
                return Json(false); // or return Json("Entity set 'BarterPlatformContext.AdministrativeRegion'  is null.");
            }

            bool isAdminRegionExist = _context.AdministrativeRegion.Any(a => a.AdminRegion == AdminRegion);
            return Json(!isAdminRegionExist);
        }




        // GET: AdministrativeRegions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdministrativeRegions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdmID,AdminRegion")] AdministrativeRegion administrativeRegion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(administrativeRegion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(administrativeRegion);
        }

        // GET: AdministrativeRegions/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.AdministrativeRegion == null)
            {
                return NotFound();
            }

            var administrativeRegion = await _context.AdministrativeRegion.FindAsync(id);
            if (administrativeRegion == null)
            {
                return NotFound();
            }
            return View(administrativeRegion);
        }

        // POST: AdministrativeRegions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AdmID,AdminRegion")] AdministrativeRegion administrativeRegion)
        {
            if (id != administrativeRegion.AdmID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(administrativeRegion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministrativeRegionExists(administrativeRegion.AdmID))
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
            return View(administrativeRegion);
        }

        // GET: AdministrativeRegions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.AdministrativeRegion == null)
            {
                return NotFound();
            }

            var administrativeRegion = await _context.AdministrativeRegion
                .FirstOrDefaultAsync(m => m.AdmID == id);
            if (administrativeRegion == null)
            {
                return NotFound();
            }

            return View(administrativeRegion);
        }

        // POST: AdministrativeRegions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.AdministrativeRegion == null)
            {
                return Problem("Entity set 'BarterPlatformContext.AdministrativeRegion'  is null.");
            }
            var administrativeRegion = await _context.AdministrativeRegion.FindAsync(id);
            if (administrativeRegion != null)
            {
                _context.AdministrativeRegion.Remove(administrativeRegion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministrativeRegionExists(string id)
        {
          return (_context.AdministrativeRegion?.Any(e => e.AdmID == id)).GetValueOrDefault();
        }
    }
}
