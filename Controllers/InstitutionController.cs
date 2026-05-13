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
    public class InstitutionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstitutionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Institution
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.InstitutionProfiles.Include(i => i.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Institution/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var institutionProfile = await _context.InstitutionProfiles
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.InstitutionID == id);
            if (institutionProfile == null)
            {
                return NotFound();
            }

            return View(institutionProfile);
        }

        // GET: Institution/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email");
            return View();
        }

        // POST: Institution/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstitutionID,UserID,InstitutionName,EntityType,IdentityNumber,TaxCertificatePath,AuthorizedPersonName,AuthorizedPersonPhone,AuthorizedPersonEmail")] InstitutionProfile institutionProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(institutionProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", institutionProfile.UserID);
            return View(institutionProfile);
        }

        // GET: Institution/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var institutionProfile = await _context.InstitutionProfiles.FindAsync(id);
            if (institutionProfile == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", institutionProfile.UserID);
            return View(institutionProfile);
        }

        // POST: Institution/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstitutionID,UserID,InstitutionName,EntityType,IdentityNumber,TaxCertificatePath,AuthorizedPersonName,AuthorizedPersonPhone,AuthorizedPersonEmail")] InstitutionProfile institutionProfile)
        {
            if (id != institutionProfile.InstitutionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(institutionProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstitutionProfileExists(institutionProfile.InstitutionID))
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
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", institutionProfile.UserID);
            return View(institutionProfile);
        }

        // GET: Institution/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var institutionProfile = await _context.InstitutionProfiles
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.InstitutionID == id);
            if (institutionProfile == null)
            {
                return NotFound();
            }

            return View(institutionProfile);
        }

        // POST: Institution/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var institutionProfile = await _context.InstitutionProfiles.FindAsync(id);
            if (institutionProfile != null)
            {
                _context.InstitutionProfiles.Remove(institutionProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstitutionProfileExists(int id)
        {
            return _context.InstitutionProfiles.Any(e => e.InstitutionID == id);
        }
    }
}
