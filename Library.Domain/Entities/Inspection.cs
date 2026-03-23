using System.ComponentModel.DataAnnotations;
using Library.Domain.Enums;

namespace Library.Domain.Entities
{
    public class Inspection
    {
        public int Id { get; set; }

        [Required]
        public DateTime InspectionDate { get; set; }

        [Required]
        [Range(0, 100)]
        public int Score { get; set; }

        [Required]
        public InspectionOutcome Outcome { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public int PremisesId { get; set; }

        public Premises? Premises { get; set; }

        public ICollection<FollowUp> FollowUps { get; set; } = new List<FollowUp>();
    }
}