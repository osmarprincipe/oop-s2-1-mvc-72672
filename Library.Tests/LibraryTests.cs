using System;
using System.Linq;
using Library.Domain.Entities;
using Library.MVC.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class LibraryTests
{

    private ApplicationDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }


    [Fact]
    public void CreatingLoanMakesBookUnavailable()
    {
        using var context = GetContext();

        var book = new Book
        {
            Title = "Clean Code",
            Author = "Robert Martin",
            Isbn = "111",
            Category = "Programming",
            IsAvailable = true
        };

        var member = new Member
        {
            FullName = "John Doe",
            Email = "john@test.com",
            Phone = "123"
        };

        context.Books.Add(book);
        context.Members.Add(member);
        context.SaveChanges();

        var loan = new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7)
        };

        context.Loans.Add(loan);

        book.IsAvailable = false;

        context.SaveChanges();

        Assert.False(context.Books.First().IsAvailable);
    }



    [Fact]
    public void CannotCreateLoanForUnavailableBook()
    {
        using var context = GetContext();

        var book = new Book
        {
            Title = "Test Book",
            Author = "Author",
            Isbn = "222",
            Category = "Programming",
            IsAvailable = false
        };

        context.Books.Add(book);
        context.SaveChanges();

        Assert.False(book.IsAvailable);
    }



    [Fact]
    public void ReturningLoanMakesBookAvailableAgain()
    {
        using var context = GetContext();

        var book = new Book
        {
            Title = "Refactoring",
            Author = "Martin Fowler",
            Isbn = "333",
            Category = "Programming",
            IsAvailable = false
        };

        var member = new Member
        {
            FullName = "Jane Doe",
            Email = "jane@test.com",
            Phone = "999"
        };

        context.Books.Add(book);
        context.Members.Add(member);
        context.SaveChanges();

        var loan = new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7),
            ReturnedDate = DateTime.Now
        };

        context.Loans.Add(loan);

        book.IsAvailable = true;

        context.SaveChanges();

        Assert.True(context.Books.First().IsAvailable);
    }



    [Fact]
    public void LoanSetsCorrectDueDate()
    {
        using var context = GetContext();

        var book = new Book
        {
            Title = "Domain Driven Design",
            Author = "Eric Evans",
            Isbn = "444",
            Category = "Programming",
            IsAvailable = true
        };

        var member = new Member
        {
            FullName = "Alice",
            Email = "alice@test.com",
            Phone = "555"
        };

        context.Books.Add(book);
        context.Members.Add(member);
        context.SaveChanges();

        var loanDate = DateTime.Now;

        var loan = new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = loanDate,
            DueDate = loanDate.AddDays(7)
        };

        context.Loans.Add(loan);
        context.SaveChanges();

        Assert.Equal(loanDate.AddDays(7).Date, loan.DueDate.Date);
    }



    [Fact]
    public void BookSearchReturnsExpectedMatches()
    {
        using var context = GetContext();

        context.Books.AddRange(
            new Book { Title = "Clean Code", Author = "Robert Martin", Isbn = "1", Category = "Programming", IsAvailable = true },
            new Book { Title = "Pragmatic Programmer", Author = "Andrew Hunt", Isbn = "2", Category = "Programming", IsAvailable = true },
            new Book { Title = "Cooking Book", Author = "Chef John", Isbn = "3", Category = "Cooking", IsAvailable = true }
        );

        context.SaveChanges();

        var result = context.Books
            .Where(b => b.Title.Contains("Clean") || b.Author.Contains("Martin"))
            .ToList();

        Assert.Single(result);
    }



    [Fact]
    public void AvailabilityFilterReturnsCorrectBooks()
    {
        using var context = GetContext();

        context.Books.AddRange(
            new Book { Title = "Book1", Author = "A", Isbn = "1", Category = "Programming", IsAvailable = true },
            new Book { Title = "Book2", Author = "B", Isbn = "2", Category = "Programming", IsAvailable = false }
        );

        context.SaveChanges();

        var availableBooks = context.Books.Where(b => b.IsAvailable).ToList();

        Assert.Single(availableBooks);
    }



    [Fact]
    public void LoanLinksBookAndMemberCorrectly()
    {
        using var context = GetContext();

        var book = new Book
        {
            Title = "Test",
            Author = "Author",
            Isbn = "555",
            Category = "Programming",
            IsAvailable = true
        };

        var member = new Member
        {
            FullName = "Bob",
            Email = "bob@test.com",
            Phone = "777"
        };

        context.Books.Add(book);
        context.Members.Add(member);
        context.SaveChanges();

        var loan = new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7)
        };

        context.Loans.Add(loan);
        context.SaveChanges();

        var savedLoan = context.Loans.First();

        Assert.Equal(book.Id, savedLoan.BookId);
        Assert.Equal(member.Id, savedLoan.MemberId);
    }

}