using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Domain.Entities;
using Library.MVC.Data;

namespace Library.MVC.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Loans.Include(l => l.Book).Include(l => l.Member);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }
        //SHOWING ONLY THE AVAIBLE DATA
        // GET: Loans/Create
       
        public IActionResult Create()
        {
            var availableBooks = _context.Books
                .Where(b => b.IsAvailable)
                .ToList();

            ViewData["BookId"] = new SelectList(availableBooks, "Id", "Title");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName");

            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,MemberId,LoanDate,DueDate,ReturnedDate")] Loan loan)
        {
            var selectedBook = await _context.Books.FirstOrDefaultAsync(b => b.Id == loan.BookId);

            if (selectedBook == null)
            {
                ModelState.AddModelError("BookId", "Selected book was not found.");
            }
            else if (!selectedBook.IsAvailable)
            {
                ModelState.AddModelError("BookId", "This book is already on loan.");
            }

            if (loan.DueDate < loan.LoanDate)
            {
                ModelState.AddModelError("DueDate", "Due date cannot be earlier than loan date.");
            }

            if (ModelState.IsValid)
            {
                loan.ReturnedDate = null;
                selectedBook!.IsAvailable = false;
                loan.LoanDate = DateTime.Now;
                _context.Add(loan);
                _context.Update(selectedBook);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["BookId"] = new SelectList(
                _context.Books.Where(b => b.IsAvailable || b.Id == loan.BookId),
                "Id",
                "Title",
                loan.BookId
            );

            ViewData["MemberId"] = new SelectList(
                _context.Members,
                "Id",
                "FullName",
                loan.MemberId
            );

            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                return NotFound();
            }

            if (loan.ReturnedDate == null)
            {
                loan.ReturnedDate = DateTime.Now;

                if (loan.Book != null)
                {
                    loan.Book.IsAvailable = true;
                    _context.Update(loan.Book);
                }

                _context.Update(loan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan != null)
            {
                if (loan.ReturnedDate == null && loan.Book != null)
                {
                    loan.Book.IsAvailable = true;
                    _context.Update(loan.Book);
                }

                _context.Loans.Remove(loan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.Id == id);
        }
    }
}
