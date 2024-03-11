using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KaravanovySvet.Data;
using KaravanovySvet.Models;
using JenikuvWeb.CloudStorage;

namespace KaravanovySvet.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly KaravanovySvet.Data.KaravanovySvetContext _context;
        private readonly ICloudStorage _cloudStorage;

        public DetailsModel(KaravanovySvet.Data.KaravanovySvetContext context, ICloudStorage cloudStorage)
        {
            _context = context;
            _cloudStorage = cloudStorage;
        }

        public BlogViewModel BlogPost { get; set; } = default!;
        public IList<Blogs> MostReadBlogPosts { get; set; }
        public IEnumerable<string> TagCloud { get; set; }
        public Blogs PreviousBlogPost { get; set;} = default!;
        public Blogs NextBlogPost { get; set; } = default!;

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

            BlogPost.BlogImage.ImagePath = _cloudStorage.GenerateSignedUrl(BlogPost.BlogImage.ImageStorageName);

            var allPosts = await _context.Blog
                .OrderBy(b => b.PublishDate) // or another ordering logic that suits your application
                .ToListAsync();

            int currentIndex = allPosts.FindIndex(b => b.Id == id);
            PreviousBlogPost = currentIndex > 0 ? allPosts[currentIndex - 1] : null;
            NextBlogPost = currentIndex + 1 < allPosts.Count ? allPosts[currentIndex + 1] : null;

            var labels = MostReadBlogPosts.SelectMany(b => b.LabelArray).Distinct();

            TagCloud = labels;

            return Page();
        }
    }
}
