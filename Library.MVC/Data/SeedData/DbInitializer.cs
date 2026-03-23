using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.MVC.Data;

namespace Library.MVC.SeedData
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            var hasPremises = context.Premises.Any();
            var hasInspections = context.Inspections.Any();
            var hasFollowUps = context.FollowUps.Any();

            var premises = new List<Premises>
            {
                new Premises { Name = "Green Garden Cafe", Address = "12 Main Street", Town = "Dublin", RiskRating = RiskRating.Low },
                new Premises { Name = "River View Restaurant", Address = "45 River Road", Town = "Dublin", RiskRating = RiskRating.Medium },
                new Premises { Name = "Sunny Side Bakery", Address = "8 Market Lane", Town = "Dublin", RiskRating = RiskRating.High },
                new Premises { Name = "Ocean Breeze Hotel", Address = "22 Coast Avenue", Town = "Cork", RiskRating = RiskRating.High },

                new Premises { Name = "Fresh Farm Bistro", Address = "10 Hill Street", Town = "Cork", RiskRating = RiskRating.Medium },
                new Premises { Name = "City Deli", Address = "31 King Street", Town = "Cork", RiskRating = RiskRating.Low },
                new Premises { Name = "Golden Spoon", Address = "5 Bridge Road", Town = "Cork", RiskRating = RiskRating.High },
                new Premises { Name = "Blue Sky Cafe", Address = "18 Park Avenue", Town = "Galway", RiskRating = RiskRating.Medium },

                new Premises { Name = "Happy Meals Takeaway", Address = "27 Station Road", Town = "Galway", RiskRating = RiskRating.High },
                new Premises { Name = "The Breakfast House", Address = "14 Church Street", Town = "Galway", RiskRating = RiskRating.Low },
                new Premises { Name = "Silver Fork", Address = "9 Harbour Road", Town = "Galway", RiskRating = RiskRating.Medium },
                new Premises { Name = "Family Table", Address = "50 Oak Drive", Town = "Dublin", RiskRating = RiskRating.Low }
            };

            context.Premises.AddRange(premises);
            context.SaveChanges();

            var inspections = new List<Inspection>
            {
                new Inspection { PremisesId = 1, InspectionDate = new DateTime(2026, 3, 3), Score = 92, Outcome = InspectionOutcome.Pass, Notes = "Good hygiene standards." },
                new Inspection { PremisesId = 1, InspectionDate = new DateTime(2026, 3, 15), Score = 88, Outcome = InspectionOutcome.Pass, Notes = "Minor storage issue." },

                new Inspection { PremisesId = 2, InspectionDate = new DateTime(2026, 3, 5), Score = 74, Outcome = InspectionOutcome.Pass, Notes = "Paperwork incomplete." },
                new Inspection { PremisesId = 2, InspectionDate = new DateTime(2026, 3, 20), Score = 61, Outcome = InspectionOutcome.Fail, Notes = "Cleaning schedule missing." },

                new Inspection { PremisesId = 3, InspectionDate = new DateTime(2026, 2, 25), Score = 95, Outcome = InspectionOutcome.Pass, Notes = "Excellent standards." },
                new Inspection { PremisesId = 3, InspectionDate = new DateTime(2026, 3, 10), Score = 91, Outcome = InspectionOutcome.Pass, Notes = "Very good compliance." },

                new Inspection { PremisesId = 4, InspectionDate = new DateTime(2026, 3, 1), Score = 58, Outcome = InspectionOutcome.Fail, Notes = "Raw food storage issue." },
                new Inspection { PremisesId = 4, InspectionDate = new DateTime(2026, 3, 18), Score = 67, Outcome = InspectionOutcome.Fail, Notes = "Pest control records incomplete." },
                new Inspection { PremisesId = 4, InspectionDate = new DateTime(2026, 3, 22), Score = 79, Outcome = InspectionOutcome.Pass, Notes = "Improved after corrections." },

                new Inspection { PremisesId = 5, InspectionDate = new DateTime(2026, 3, 2), Score = 81, Outcome = InspectionOutcome.Pass, Notes = "Good overall." },
                new Inspection { PremisesId = 5, InspectionDate = new DateTime(2026, 3, 21), Score = 76, Outcome = InspectionOutcome.Pass, Notes = "One sink area needed attention." },

                new Inspection { PremisesId = 6, InspectionDate = new DateTime(2026, 3, 4), Score = 89, Outcome = InspectionOutcome.Pass, Notes = "Clean prep area." },
                new Inspection { PremisesId = 6, InspectionDate = new DateTime(2026, 3, 24), Score = 84, Outcome = InspectionOutcome.Pass, Notes = "Training records updated." },

                new Inspection { PremisesId = 7, InspectionDate = new DateTime(2026, 3, 6), Score = 54, Outcome = InspectionOutcome.Fail, Notes = "Serious sanitation issues." },
                new Inspection { PremisesId = 7, InspectionDate = new DateTime(2026, 3, 26), Score = 63, Outcome = InspectionOutcome.Fail, Notes = "Follow-up needed again." },

                new Inspection { PremisesId = 8, InspectionDate = new DateTime(2026, 3, 7), Score = 78, Outcome = InspectionOutcome.Pass, Notes = "Acceptable with small improvements." },
                new Inspection { PremisesId = 8, InspectionDate = new DateTime(2026, 3, 27), Score = 82, Outcome = InspectionOutcome.Pass, Notes = "Better stock rotation." },

                new Inspection { PremisesId = 9, InspectionDate = new DateTime(2026, 3, 8), Score = 49, Outcome = InspectionOutcome.Fail, Notes = "Unsafe holding temperature." },
                new Inspection { PremisesId = 9, InspectionDate = new DateTime(2026, 3, 19), Score = 57, Outcome = InspectionOutcome.Fail, Notes = "Cleaning below standard." },
                new Inspection { PremisesId = 9, InspectionDate = new DateTime(2026, 3, 28), Score = 73, Outcome = InspectionOutcome.Pass, Notes = "Passed after major actions." },

                new Inspection { PremisesId = 10, InspectionDate = new DateTime(2026, 3, 9), Score = 90, Outcome = InspectionOutcome.Pass, Notes = "Strong daily controls." },
                new Inspection { PremisesId = 10, InspectionDate = new DateTime(2026, 3, 29), Score = 87, Outcome = InspectionOutcome.Pass, Notes = "Good standards maintained." },

                new Inspection { PremisesId = 11, InspectionDate = new DateTime(2026, 3, 11), Score = 72, Outcome = InspectionOutcome.Pass, Notes = "Some labels missing." },
                new Inspection { PremisesId = 12, InspectionDate = new DateTime(2026, 3, 12), Score = 93, Outcome = InspectionOutcome.Pass, Notes = "Very strong hygiene controls." },
                new Inspection { PremisesId = 12, InspectionDate = new DateTime(2026, 3, 30), Score = 91, Outcome = InspectionOutcome.Pass, Notes = "Good standards maintained." }
            };

            context.Inspections.AddRange(inspections);
            context.SaveChanges();

            var today = DateTime.Today;

            var followUps = new List<FollowUp>
            {
                new FollowUp { InspectionId = 4, DueDate = today.AddDays(-10), Status = FollowUpStatus.Open, ClosedDate = null },
                new FollowUp { InspectionId = 7, DueDate = today.AddDays(-5), Status = FollowUpStatus.Closed, ClosedDate = today.AddDays(-4) },
                new FollowUp { InspectionId = 8, DueDate = today.AddDays(-2), Status = FollowUpStatus.Open, ClosedDate = null },
                new FollowUp { InspectionId = 14, DueDate = today.AddDays(-8), Status = FollowUpStatus.Closed, ClosedDate = today.AddDays(-7) },
                new FollowUp { InspectionId = 15, DueDate = today.AddDays(5), Status = FollowUpStatus.Open, ClosedDate = null },

                new FollowUp { InspectionId = 18, DueDate = today.AddDays(-12), Status = FollowUpStatus.Closed, ClosedDate = today.AddDays(-11) },
                new FollowUp { InspectionId = 19, DueDate = today.AddDays(-6), Status = FollowUpStatus.Open, ClosedDate = null },
                new FollowUp { InspectionId = 20, DueDate = today.AddDays(7), Status = FollowUpStatus.Open, ClosedDate = null },
                new FollowUp { InspectionId = 23, DueDate = today.AddDays(-3), Status = FollowUpStatus.Closed, ClosedDate = today.AddDays(-2) },
                new FollowUp { InspectionId = 11, DueDate = today.AddDays(3), Status = FollowUpStatus.Closed, ClosedDate = today.AddDays(2) }
            };

            context.FollowUps.AddRange(followUps);
            context.SaveChanges();
        }
    }
}