using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BarterPlatform.Models;
using System.Diagnostics.Metrics;

namespace BarterPlatform.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly BarterPlatformContext _context;

        public EmployeesController(BarterPlatformContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var barterPlatformContext = _context.Employee.Include(e => e.AdminRegion_Adm).Include(e => e.District_Dis);
            return View(await barterPlatformContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.AdminRegion_Adm)
                .Include(e => e.District_Dis)
                .FirstOrDefaultAsync(m => m.EmpID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }





        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["AdminRegion_AdmID"] = new SelectList(_context.AdministrativeRegion, "AdmID", "AdminRegion");

            //顯示空白內容
            ViewBag.District_DisID = new SelectList(new List<SelectListItem>(), "Value", "Text");
            //直接顯示內容
            //ViewData["District_DisID"] = new SelectList(_context.District, "DisID", "DistrictName");
            
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpID,PersonalName,Nickname,Gender,BirthDate,PostalCode,AdminRegion_AdmID,District_DisID,OtherAddress,Username,Password,Email,Phone,NationalID,HireDate,TermDate,Salary,Status")] Employee employee)
        {

            //檢查EmpID是否存在
            var existingEmpID = await _context.Employee.FirstOrDefaultAsync(m => m.EmpID == employee.EmpID);
            if (existingEmpID != null)
            {
                ModelState.AddModelError("EmpID", "員工編號已被使用，請更換");
            }

            //檢查帳號是否存在
            var existingemployeeUserName = await _context.Member.FirstOrDefaultAsync(m => m.Username == employee.Username);
            if (existingemployeeUserName != null)
            {
                ModelState.AddModelError("UserName", "帳號已被註冊，請更換");
            }


            //通過驗證
            if (ModelState.IsValid)
            {
                //自動產生
                employee.HireDate = DateTime.Now;
                employee.Status = true;

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminRegion_AdmID"] = new SelectList(_context.AdministrativeRegion, "AdmID", "AdminRegion", employee.AdminRegion_AdmID);
            ViewData["District_DisID"] = new SelectList(_context.District, "DisID", "DistrictName", employee.District_DisID);
            return View(employee);
        }


        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["AdminRegion_AdmID"] = new SelectList(_context.AdministrativeRegion, "AdmID", "AdminRegion", employee.AdminRegion_AdmID);
            ViewData["District_DisID"] = new SelectList(_context.District, "DisID", "DistrictName", employee.District_DisID);
            //獲得暱稱資料給Edit用
            ViewData["Nickname"] = employee.Nickname;
            //獲得名稱資料給Edit用
            ViewData["PersonalName"] = employee.PersonalName;
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("EmpID,PersonalName,Nickname,Gender,BirthDate,PostalCode,AdminRegion_AdmID,District_DisID,OtherAddress,Username,Password,Email,Phone,NationalID,HireDate,TermDate,Salary,Status")] Employee employee)
        {
            if (id != employee.EmpID)
            {
                return NotFound();
            }

            // 檢查帳號是否存在(排除自身)
            var existingemployeeUserName = await _context.Employee.FirstOrDefaultAsync(m => m.EmpID != employee.EmpID && m.Username == employee.Username);
            if (existingemployeeUserName != null)
            {
                ModelState.AddModelError("UserName", "帳號已被註冊，請更換");
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmpID))
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
            ViewData["AdminRegion_AdmID"] = new SelectList(_context.AdministrativeRegion, "AdmID", "AdminRegion", employee.AdminRegion_AdmID);
            ViewData["District_DisID"] = new SelectList(_context.District, "DisID", "DistrictName", employee.District_DisID);
            return View(employee);
        }


        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Employee == null)
            {
                return Problem("Entity set 'BarterPlatformContext.Employee'  is null.");
            }
            var employee = await _context.Employee.FindAsync(id);
            if (employee != null)
            {
                _context.Employee.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(string id)
        {
          return (_context.Employee?.Any(e => e.EmpID == id)).GetValueOrDefault();
        }
    }
}
