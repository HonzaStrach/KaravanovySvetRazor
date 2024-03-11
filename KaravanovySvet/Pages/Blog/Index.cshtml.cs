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
    public class IndexModel : PageModel
    {
        private readonly KaravanovySvet.Data.KaravanovySvetContext _context;
        private readonly ICloudStorage _cloudStorage;
        public IList<BlogViewModel> BlogPosts { get; set; }
        public IList<BlogViewModel> MostReadPosts { get; set; }
        public IEnumerable<string> TagCloud { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        private const int PageSize = 10;
        public IndexModel(KaravanovySvet.Data.KaravanovySvetContext context, ICloudStorage cloudStorage)
        {
            _context = context;
            _cloudStorage = cloudStorage;
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

            var blogPosts = await blogPostsQuery
                .OrderByDescending(b => b.PublishDate)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Include(blog => blog.Images)
                .ToListAsync();

            BlogPosts = new List<BlogViewModel>();

            foreach (var post in blogPosts)
            {
                var blogViewModel = new BlogViewModel
                {
                    Blog = post,
                    BlogImage = post.Images.FirstOrDefault()
                };
                if (blogViewModel.BlogImage != null)
                {
                    // Generate Signed URL for each image
                    string objectName = blogViewModel.BlogImage.ImageStorageName;
                    blogViewModel.BlogImage.ImagePath = _cloudStorage.GenerateSignedUrl(blogViewModel.BlogImage.ImageStorageName);
                }
                BlogPosts.Add(blogViewModel);
            }

            MostReadPosts = await _context.Blog
                .OrderByDescending(b => b.TotalViews)
                .Take(3)
                .Select(b => new BlogViewModel
                {
                    Blog = b
                })
                .ToListAsync();

            var labels = MostReadPosts.SelectMany(b => b.Blog.LabelArray).Distinct();

            TagCloud = labels;
        }
    }
}