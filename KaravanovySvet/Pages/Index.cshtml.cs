using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KaravanovySvet.Models;
using KaravanovySvet.Data;
using JenikuvWeb.CloudStorage;
using Microsoft.Extensions.Caching.Memory;
using Google.Cloud.Storage.V1;

namespace KaravanovySvet.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly KaravanovySvetContext _context;
        private readonly ICloudStorage _cloudStorage;
        private readonly IMemoryCache _cache;
        public IndexModel(ILogger<IndexModel> logger, KaravanovySvetContext context, ICloudStorage cloudStorage, IMemoryCache memoryCache)
        {
            _logger = logger;
            _context = context;
            _cloudStorage = cloudStorage;
            _cache = memoryCache;
        }
        public IList<BlogViewModel> LatestBlogPosts { get; set; }
        public IList<BlogViewModel> MostReadPosts { get; set; }
        public IEnumerable<string> TagCloud { get; set; }

        public async Task OnGetAsync()
        {
            var mostReadPosts = await _context.Blog
                .OrderByDescending(b => b.TotalViews)
                .Take(3)
                .Include(blog => blog.Images)
                .ToListAsync();

            MostReadPosts = new List<BlogViewModel>();

            foreach (var mrPost in mostReadPosts)
            {
                var blogViewModel = new BlogViewModel
                {
                    Blog = mrPost,
                    BlogImage = mrPost.Images.FirstOrDefault()
                };
                if (blogViewModel.BlogImage != null)
                {
                    // Generate Signed URL for each image
                    string objectName = blogViewModel.BlogImage.ImageStorageName;
                    blogViewModel.BlogImage.ImagePath = _cloudStorage.GenerateSignedUrl(blogViewModel.BlogImage.ImageStorageName);
                }
                MostReadPosts.Add(blogViewModel);
            }

            var labels = MostReadPosts.SelectMany(b => b.Blog.LabelArray).Distinct();

            TagCloud = labels;

            var latestPosts = await _context.Blog
                .OrderByDescending(b => b.PublishDate)
                .Take(5)
                .Include(blog => blog.Images)
                .ToListAsync();

            LatestBlogPosts = new List<BlogViewModel>();

            foreach (var lPost in latestPosts)
            {
                var blogViewModel = new BlogViewModel
                {
                    Blog = lPost,
                    BlogImage = lPost.Images.FirstOrDefault()
                };
                if (blogViewModel.BlogImage != null)
                {
                    // Generate Signed URL for each image
                    string objectName = blogViewModel.BlogImage.ImageStorageName;
                    blogViewModel.BlogImage.ImagePath = _cloudStorage.GenerateSignedUrl(blogViewModel.BlogImage.ImageStorageName);
                }
                LatestBlogPosts.Add(blogViewModel);
            }
        }
    }
}
