using Bogus;
using Library.Domain.Entities;

namespace Library.MVC.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            if (context.Books.Any() || context.Members.Any() || context.Loans.Any())
            {
                return;
            }

            var books = new Faker<Book>()
                .RuleFor(b => b.Title, f => f.Commerce.ProductName())
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Isbn, f => f.Random.ReplaceNumbers("978##########"))
                .RuleFor(b => b.Category, f => f.PickRandom(new[]
                {
                    "Programming",
                    "Software Engineering",
                    "Databases",
                    "Networking",
                    "Self Development"
                }))
                .RuleFor(b => b.IsAvailable, true)
                .Generate(20);

            var members = new Faker<Member>()
                .RuleFor(m => m.FullName, f => f.Name.FullName())
                .RuleFor(m => m.Email, f => f.Internet.Email())
                .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber("08########"))
                .Generate(10);

            context.Books.AddRange(books);
            context.Members.AddRange(members);
            await context.SaveChangesAsync();

            var random = new Random();
            var loans = new List<Loan>();

            var selectedBooks = books.OrderBy(b => random.Next()).Take(15).ToList();

            for (int i = 0; i < 15; i++)
            {
                var book = selectedBooks[i];
                var member = members[random.Next(members.Count)];

                var loanDate = DateTime.Now.AddDays(-random.Next(1, 30));
                var dueDate = loanDate.AddDays(7);

                DateTime? returnedDate = null;

                if (i < 5)
                {
                    returnedDate = loanDate.AddDays(random.Next(1, 6));
                    book.IsAvailable = true;
                }
                else if (i < 10)
                {
                    returnedDate = null;
                    book.IsAvailable = false;
                }
                else
                {
                    returnedDate = null;
                    dueDate = DateTime.Now.AddDays(-random.Next(1, 10));
                    book.IsAvailable = false;
                }

                loans.Add(new Loan
                {
                    BookId = book.Id,
                    MemberId = member.Id,
                    LoanDate = loanDate,
                    DueDate = dueDate,
                    ReturnedDate = returnedDate
                });
            }

            context.Loans.AddRange(loans);
            await context.SaveChangesAsync();
        }
    }
}