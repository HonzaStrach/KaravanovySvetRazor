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
        public IList<BlogViewModel> BlogPosts { get; set; }
        public IList<BlogViewModel> MostReadPosts { get; set; }
        public IEnumerable<string> TagCloud { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        private const int PageSize = 1;
        public IndexModel(KaravanovySvet.Data.KaravanovySvetContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }


        public async Task OnGetAsync(int currentPage = 1)
        {
            CurrentPage = currentPage;

            var blogPostsQuery = _context.Blog.AsQueryable();
            int selectedYear;
            if (!string.IsNullOrEmpty(SearchString))
            {
                if (int.TryParse(SearchString, out selectedYear))
                {
                    blogPostsQuery = blogPostsQuery.Where(b => b.PublishDate.Year == selectedYear);
                }
                else
                {
                    blogPostsQuery = blogPostsQuery.Where(b => b.Title.Contains(SearchString) || b.Labels.Contains(SearchString));
                }
            }

            var totalPosts = await blogPostsQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalPosts / (double)PageSize);

            BlogPosts = await blogPostsQuery
                .OrderByDescending(b => b.PublishDate)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(b => new BlogViewModel
                {
                    Blog = b,
                    BlogImage = b.Images.FirstOrDefault()
                })
                .ToListAsync();

            MostReadPosts = await _context.Blog
                .OrderByDescending(b => b.TotalViews)
                .Take(3)
                .Select(b => new BlogViewModel
                {
                    Blog = b,
                    BlogImage = b.Images.FirstOrDefault()
                })
                .ToListAsync();

            var labels = MostReadPosts.SelectMany(b => b.Blog.LabelArray).Distinct();

            TagCloud = labels;
        }
    }
}