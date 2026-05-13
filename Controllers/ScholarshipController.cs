using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BursTakip.Data;
using BursTakip.Models;

namespace Burs_Takip_Sistemi.Controllers
{
    public class ScholarshipController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScholarshipController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Scholarship
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ScholarshipPrograms.Include(s => s.Institution);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Scholarship/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarshipProgram = await _context.ScholarshipPrograms
                .Include(s => s.Institution)
                .FirstOrDefaultAsync(m => m.ProgramID == id);
            if (scholarshipProgram == null)
            {
                return NotFound();
            }

            return View(scholarshipProgram);
        }

        // GET: Scholarship/Create
        public IActionResult Create()
        {
            ViewData["InstitutionID"] = new SelectList(_context.InstitutionProfiles, "InstitutionID", "InstitutionName");
            return View();
        }

        // POST: Scholarship/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgramID,InstitutionID,ProgramName,Amount,DurationMonths,Quota,GenderCriteria,DepartmentCriteria,MinGPA,Status,ApplicationDeadline,SubmissionDeadline,AdminNote,CreatedAt,SubmittedAt,ApprovedAt")] ScholarshipProgram scholarshipProgram)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scholarshipProgram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstitutionID"] = new SelectList(_context.InstitutionProfiles, "InstitutionID", "InstitutionName", scholarshipProgram.InstitutionID);
            return View(scholarshipProgram);
        }

        // GET: Scholarship/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarshipProgram = await _context.ScholarshipPrograms.FindAsync(id);
            if (scholarshipProgram == null)
            {
                return NotFound();
            }
            ViewData["InstitutionID"] = new SelectList(_context.InstitutionProfiles, "InstitutionID", "InstitutionName", scholarshipProgram.InstitutionID);
            return View(scholarshipProgram);
        }

        // POST: Scholarship/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProgramID,InstitutionID,ProgramName,Amount,DurationMonths,Quota,GenderCriteria,DepartmentCriteria,MinGPA,Status,ApplicationDeadline,SubmissionDeadline,AdminNote,CreatedAt,SubmittedAt,ApprovedAt")] ScholarshipProgram scholarshipProgram)
        {
            if (id != scholarshipProgram.ProgramID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scholarshipProgram);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScholarshipProgramExists(scholarshipProgram.ProgramID))
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
            ViewData["InstitutionID"] = new SelectList(_context.InstitutionProfiles, "InstitutionID", "InstitutionName", scholarshipProgram.InstitutionID);
            return View(scholarshipProgram);
        }

        // GET: Scholarship/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarshipProgram = await _context.ScholarshipPrograms
                .Include(s => s.Institution)
                .FirstOrDefaultAsync(m => m.ProgramID == id);
            if (scholarshipProgram == null)
            {
                return NotFound();
            }

            return View(scholarshipProgram);
        }

        // POST: Scholarship/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scholarshipProgram = await _context.ScholarshipPrograms.FindAsync(id);
            if (scholarshipProgram != null)
            {
                _context.ScholarshipPrograms.Remove(scholarshipProgram);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScholarshipProgramExists(int id)
        {
            return _context.ScholarshipPrograms.Any(e => e.ProgramID == id);
        }
    }
}
