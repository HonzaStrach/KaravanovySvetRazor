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
    public class IndexModel : PageModel
    {
        private readonly KaravanovySvet.Data.KaravanovySvetContext _context;

        public IndexModel(KaravanovySvet.Data.KaravanovySvetContext context)
        {
            _context = context;
        }

        public IList<BlogViewModel> BlogPosts { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedYear { get; set; }


        public async Task OnGetAsync()
        {
            var blogPostsQuery = _context.Blog.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                blogPostsQuery = blogPostsQuery.Where(b => b.Title.Contains(SearchString) || b.Labels.Contains(SearchString));
            }

            if (SelectedYear.HasValue)
            {
                blogPostsQuery = blogPostsQuery.Where(b => b.PublishDate.Year == SelectedYear.Value);
            }

            BlogPosts = await blogPostsQuery
                .OrderByDescending(b => b.PublishDate)
                .Select(b => new BlogViewModel
                {
                    Blog = b,
                    BlogImage = b.Images.FirstOrDefault()
                })
                .ToListAsync();
        }
    }
}