using System.ComponentModel.DataAnnotations;
using Library.Domain.Enums;

namespace Library.Domain.Entities
{
    public class FollowUp
    {
        public int Id { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public FollowUpStatus Status { get; set; }

        public DateTime? ClosedDate { get; set; }

        [Required]
        public int InspectionId { get; set; }

        public Inspection? Inspection { get; set; }
    }
}