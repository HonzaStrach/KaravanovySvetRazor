using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KaravanovySvet.Models;
using KaravanovySvet.Data;

namespace KaravanovySvet.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly BlobServiceClient _blobServiceClient;

        private readonly KaravanovySvetContext _context;

        public IndexModel(ILogger<IndexModel> logger, BlobServiceClient blobServiceClient, KaravanovySvetContext context)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
            _context = context;
        }
        public IList<BlogViewModel> LatestBlogPosts { get; set; }


        public async Task OnGetAsync()
        {
            LatestBlogPosts = await _context.Blog
                .OrderByDescending(b => b.PublishDate)
                .Take(5)
                .Select(b => new BlogViewModel
                {
                Blog = b,
                BlogImage = b.Images.FirstOrDefault()
                })
            .ToListAsync();
        }
    }
}
