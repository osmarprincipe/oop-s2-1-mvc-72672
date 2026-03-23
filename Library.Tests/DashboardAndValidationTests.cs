using FluentAssertions;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.MVC.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library.Tests
{
    public class DashboardAndValidationTests
    {
        private ApplicationDbContext GetContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void OverdueOpenFollowUps_Should_Be_Countable_Correctly()
        {
            using var context = GetContext(nameof(OverdueOpenFollowUps_Should_Be_Countable_Correctly));

            var premises = new Premises
            {
                Name = "Test Premises",
                Address = "1 Main Street",
                Town = "Dublin",
                RiskRating = RiskRating.High
            };

            context.Premises.Add(premises);
            context.SaveChanges();

            var inspection = new Inspection
            {
                PremisesId = premises.Id,
                InspectionDate = DateTime.Today.AddDays(-10),
                Score = 50,
                Outcome = InspectionOutcome.Fail,
                Notes = "Test"
            };

            context.Inspections.Add(inspection);
            context.SaveChanges();

            context.FollowUps.AddRange(
                new FollowUp
                {
                    InspectionId = inspection.Id,
                    DueDate = DateTime.Today.AddDays(-3),
                    Status = FollowUpStatus.Open,
                    ClosedDate = null
                },
                new FollowUp
                {
                    InspectionId = inspection.Id,
                    DueDate = DateTime.Today.AddDays(2),
                    Status = FollowUpStatus.Open,
                    ClosedDate = null
                },
                new FollowUp
                {
                    InspectionId = inspection.Id,
                    DueDate = DateTime.Today.AddDays(-1),
                    Status = FollowUpStatus.Closed,
                    ClosedDate = DateTime.Today
                }
            );

            context.SaveChanges();

            var overdueCount = context.FollowUps
                .Count(f => f.Status == FollowUpStatus.Open && f.DueDate < DateTime.Today);

            overdueCount.Should().Be(1);
        }

        [Fact]
        public void ClosedFollowUp_WithoutClosedDate_Should_Be_Invalid()
        {
            var followUp = new FollowUp
            {
                InspectionId = 1,
                DueDate = DateTime.Today,
                Status = FollowUpStatus.Closed,
                ClosedDate = null
            };

            var isValid = !(followUp.Status == FollowUpStatus.Closed && followUp.ClosedDate == null);

            isValid.Should().BeFalse();
        }

        [Fact]
        public void FailedInspectionsThisMonth_Should_Be_Countable_Correctly()
        {
            using var context = GetContext(nameof(FailedInspectionsThisMonth_Should_Be_Countable_Correctly));

            var premises = new Premises
            {
                Name = "Fail Test Premises",
                Address = "2 Main Street",
                Town = "Cork",
                RiskRating = RiskRating.Medium
            };

            context.Premises.Add(premises);
            context.SaveChanges();

            var now = DateTime.Today;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var nextMonth = startOfMonth.AddMonths(1);

            context.Inspections.AddRange(
                new Inspection
                {
                    PremisesId = premises.Id,
                    InspectionDate = startOfMonth.AddDays(1),
                    Score = 40,
                    Outcome = InspectionOutcome.Fail,
                    Notes = "Fail 1"
                },
                new Inspection
                {
                    PremisesId = premises.Id,
                    InspectionDate = startOfMonth.AddDays(2),
                    Score = 45,
                    Outcome = InspectionOutcome.Fail,
                    Notes = "Fail 2"
                },
                new Inspection
                {
                    PremisesId = premises.Id,
                    InspectionDate = startOfMonth.AddDays(3),
                    Score = 80,
                    Outcome = InspectionOutcome.Pass,
                    Notes = "Pass"
                },
                new Inspection
                {
                    PremisesId = premises.Id,
                    InspectionDate = startOfMonth.AddMonths(-1),
                    Score = 35,
                    Outcome = InspectionOutcome.Fail,
                    Notes = "Old Fail"
                }
            );

            context.SaveChanges();

            var failedThisMonth = context.Inspections.Count(i =>
                i.InspectionDate >= startOfMonth &&
                i.InspectionDate < nextMonth &&
                i.Outcome == InspectionOutcome.Fail);

            failedThisMonth.Should().Be(2);
        }

        [Fact]
        public void InspectionsThisMonth_Should_Be_Countable_Correctly()
        {
            using var context = GetContext(nameof(InspectionsThisMonth_Should_Be_Countable_Correctly));

            var premises = new Premises
            {
                Name = "Month Test Premises",
                Address = "3 Main Street",
                Town = "Galway",
                RiskRating = RiskRating.Low
            };

            context.Premises.Add(premises);
            context.SaveChanges();

            var now = DateTime.Today;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var nextMonth = startOfMonth.AddMonths(1);

            context.Inspections.AddRange(
                new Inspection
                {
                    PremisesId = premises.Id,
                    InspectionDate = startOfMonth.AddDays(1),
                    Score = 70,
                    Outcome = InspectionOutcome.Pass,
                    Notes = "This month 1"
                },
                new Inspection
                {
                    PremisesId = premises.Id,
                    InspectionDate = startOfMonth.AddDays(5),
                    Score = 75,
                    Outcome = InspectionOutcome.Pass,
                    Notes = "This month 2"
                },
                new Inspection
                {
                    PremisesId = premises.Id,
                    InspectionDate = startOfMonth.AddMonths(-1),
                    Score = 60,
                    Outcome = InspectionOutcome.Fail,
                    Notes = "Previous month"
                }
            );

            context.SaveChanges();

            var inspectionsThisMonth = context.Inspections.Count(i =>
                i.InspectionDate >= startOfMonth &&
                i.InspectionDate < nextMonth);

            inspectionsThisMonth.Should().Be(2);
        }
    }
}