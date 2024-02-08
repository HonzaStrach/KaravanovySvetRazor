using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KaravanovySvet.Data;
using KaravanovySvet.Models;

namespace KaravanovySvet.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly KaravanovySvet.Data.KaravanovySvetContext _context;

        public DetailsModel(KaravanovySvet.Data.KaravanovySvetContext context)
        {
            _context = context;
        }

        public BlogViewModel BlogPost { get; set; } = default!;

        public IList<Blogs> MostReadBlogPosts { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var blogPost = await _context.Blog
                .Include(b => b.Images) // Ensure images are included
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            // Fetch the three most-read blog posts
            MostReadBlogPosts = await _context.Blog
                .OrderByDescending(b => b.TotalViews)
                .Take(3)
                .ToListAsync();

            // Cookie key specific to this blog post
            string cookieKey = $"BlogViewed_{id}";

            // Check if the cookie exists and is valid
            if (!Request.Cookies.ContainsKey(cookieKey) || Request.Cookies[cookieKey] != "true")
            {
                // Increment TotalViews if not viewed in last 7 days or no cookie found
                blogPost.TotalViews = (blogPost.TotalViews ?? 0) + 1;
                await _context.SaveChangesAsync();

                // Set a cookie to expire in 7 days
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                };
                Response.Cookies.Append(cookieKey, "true", cookieOptions);
            }

            BlogPost = new BlogViewModel
            {
                Blog = blogPost,
                BlogImage = blogPost.Images.FirstOrDefault() // Assumes you want the first image
            };

            return Page();
        }
    }
}
