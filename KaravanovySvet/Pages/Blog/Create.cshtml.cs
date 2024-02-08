using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KaravanovySvet.Data;
using KaravanovySvet.Models;

namespace KaravanovySvet.Pages.Blog
{
    public class CreateModel : PageModel
    {
        private readonly KaravanovySvet.Data.KaravanovySvetContext _context;

        public CreateModel(KaravanovySvet.Data.KaravanovySvetContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public BlogViewModel BlogViewModel { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        // Log the error or examine it in the debugger
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }

            // Add the Blog to the context
            _context.Blog.Add(BlogViewModel.Blog);
            await _context.SaveChangesAsync();

            // Set the BlogId for the BlogImage and add it to the context
            BlogViewModel.BlogImage.BlogId = BlogViewModel.Blog.Id;
            _context.BlogImage.Add(BlogViewModel.BlogImage);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
