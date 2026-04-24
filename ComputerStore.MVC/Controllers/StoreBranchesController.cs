using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComputerStore.MVC.Models;

namespace ComputerStore.MVC.Controllers
{
    public class StoreBranchesController : Controller
    {
        private readonly ComputerStoreDbContext _context;

        public StoreBranchesController(ComputerStoreDbContext context)
        {
            _context = context;
        }

        // GET: StoreBranches
        public async Task<IActionResult> Index()
        {
            return View(await _context.StoreBranches.ToListAsync());
        }

        // GET: StoreBranches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeBranch = await _context.StoreBranches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeBranch == null)
            {
                return NotFound();
            }

            return View(storeBranch);
        }

        // GET: StoreBranches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StoreBranches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,PhoneNumber,IsMainBranch,IsActive")] StoreBranch storeBranch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storeBranch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(storeBranch);
        }

        // GET: StoreBranches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeBranch = await _context.StoreBranches.FindAsync(id);
            if (storeBranch == null)
            {
                return NotFound();
            }
            return View(storeBranch);
        }

        // POST: StoreBranches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,PhoneNumber,IsMainBranch,IsActive")] StoreBranch storeBranch)
        {
            if (id != storeBranch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storeBranch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreBranchExists(storeBranch.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(storeBranch);
        }

        // GET: StoreBranches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeBranch = await _context.StoreBranches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeBranch == null)
            {
                return NotFound();
            }

            return View(storeBranch);
        }

        // POST: StoreBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storeBranch = await _context.StoreBranches.FindAsync(id);
            if (storeBranch != null)
            {
                _context.StoreBranches.Remove(storeBranch);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreBranchExists(int id)
        {
            return _context.StoreBranches.Any(e => e.Id == id);
        }
    }
}
