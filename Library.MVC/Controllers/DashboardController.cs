using Library.Domain.Enums;
using Library.MVC.Data;
using Library.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? town, RiskRating? riskRating)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var premisesQuery = _context.Premises.AsQueryable();

            if (!string.IsNullOrEmpty(town))
            {
                premisesQuery = premisesQuery.Where(p => p.Town == town);
            }

            if (riskRating.HasValue)
            {
                premisesQuery = premisesQuery.Where(p => p.RiskRating == riskRating.Value);
            }

            var filteredPremisesIds = await premisesQuery
                .Select(p => p.Id)
                .ToListAsync();

            var inspectionsThisMonth = await _context.Inspections
                .Where(i => i.InspectionDate >= startOfMonth &&
                            i.InspectionDate < startOfNextMonth &&
                            filteredPremisesIds.Contains(i.PremisesId))
                .CountAsync();

            var failedInspectionsThisMonth = await _context.Inspections
                .Where(i => i.InspectionDate >= startOfMonth &&
                            i.InspectionDate < startOfNextMonth &&
                            i.Outcome == InspectionOutcome.Fail &&
                            filteredPremisesIds.Contains(i.PremisesId))
                .CountAsync();

            var overdueOpenFollowUps = await _context.FollowUps
                .Include(f => f.Inspection)
                .Where(f => f.Status == FollowUpStatus.Open &&
                            f.DueDate < now &&
                            f.Inspection != null &&
                            filteredPremisesIds.Contains(f.Inspection.PremisesId))
                .CountAsync();

            var towns = await _context.Premises
                .Select(p => p.Town)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            var premisesResults = await premisesQuery
                .OrderBy(p => p.Name)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                InspectionsThisMonth = inspectionsThisMonth,
                FailedInspectionsThisMonth = failedInspectionsThisMonth,
                OverdueOpenFollowUps = overdueOpenFollowUps,
                SelectedTown = town,
                SelectedRiskRating = riskRating,
                Towns = towns,
                PremisesResults = premisesResults
            };

            return View(viewModel);
        }
    }
}