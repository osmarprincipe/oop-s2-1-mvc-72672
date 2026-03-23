using Library.Domain.Entities;
using Library.Domain.Enums;

namespace Library.MVC.Models
{
    public class DashboardViewModel
    {
        public int InspectionsThisMonth { get; set; }
        public int FailedInspectionsThisMonth { get; set; }
        public int OverdueOpenFollowUps { get; set; }

        public string? SelectedTown { get; set; }
        public RiskRating? SelectedRiskRating { get; set; }

        public List<string> Towns { get; set; } = new();
        public List<Premises> PremisesResults { get; set; } = new();
    }
}