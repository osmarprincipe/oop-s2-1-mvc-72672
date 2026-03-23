using Library.Domain.Entities;
using Library.MVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Library.MVC.Controllers
{
    [Authorize]
    public class FollowUpsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FollowUpsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FollowUps
        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FollowUps.Include(f => f.Inspection);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FollowUps/Details/5
        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var followUp = await _context.FollowUps
                .Include(f => f.Inspection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (followUp == null)
            {
                return NotFound();
            }

            return View(followUp);
        }

        // GET: FollowUps/Create
        [Authorize(Roles = "Admin,Inspector")]
        public IActionResult Create()
        {
            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id");
            return View();
        }

        // POST: FollowUps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Inspector")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create([Bind("Id,DueDate,Status,ClosedDate,InspectionId")] FollowUp followUp)
        {
            var inspection = await _context.Inspections.FindAsync(followUp.InspectionId);

            if (inspection != null && followUp.DueDate < inspection.InspectionDate)
            {
                Log.Warning("Invalid FollowUp creation attempt. DueDate {DueDate} is before InspectionDate {InspectionDate}. InspectionId: {InspectionId}",
                    followUp.DueDate, inspection.InspectionDate, followUp.InspectionId);

                ModelState.AddModelError("DueDate", "Due date cannot be before the inspection date.");
            }

            if (followUp.Status == Library.Domain.Enums.FollowUpStatus.Closed && followUp.ClosedDate == null)
            {
                Log.Warning("Invalid FollowUp creation attempt. Status is Closed but ClosedDate is missing. InspectionId: {InspectionId}",
                    followUp.InspectionId);

                ModelState.AddModelError("ClosedDate", "Closed date is required when status is Closed.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(followUp);
                await _context.SaveChangesAsync();

                Log.Information("FollowUp created. Id: {Id}, InspectionId: {InspectionId}", followUp.Id, followUp.InspectionId);

                return RedirectToAction(nameof(Index));
            }

            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
            return View(followUp);
        }

        // GET: FollowUps/Edit/5
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var followUp = await _context.FollowUps.FindAsync(id);
            if (followUp == null)
            {
                return NotFound();
            }
            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
            return View(followUp);
        }

        // POST: FollowUps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DueDate,Status,ClosedDate,InspectionId")] FollowUp followUp)
        {
            if (id != followUp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(followUp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FollowUpExists(followUp.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                Log.Information("Item updated. ID: {Id}", id);
                return RedirectToAction(nameof(Index));
            }
            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
            return View(followUp);
        }

        // GET: FollowUps/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var followUp = await _context.FollowUps
                .Include(f => f.Inspection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (followUp == null)
            {
                return NotFound();
            }

            return View(followUp);
        }

        // POST: FollowUps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var followUp = await _context.FollowUps.FindAsync(id);
            if (followUp != null)
            {
                _context.FollowUps.Remove(followUp);
            }

            await _context.SaveChangesAsync();
            Log.Information("Item deleted. ID: {Id}", id);
            return RedirectToAction(nameof(Index));
        }

        private bool FollowUpExists(int id)
        {
            return _context.FollowUps.Any(e => e.Id == id);
        }
    }
}
