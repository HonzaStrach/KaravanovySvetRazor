using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaravanovySvet.Data; // Update this using directive to use your actual data context namespace

public class BlogYearsViewComponent : ViewComponent
{
    private readonly KaravanovySvetContext _context; // Use your actual DbContext

    public BlogYearsViewComponent(KaravanovySvetContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var years = await _context.Blog
            .Select(b => b.PublishDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();

        return View(years); // Passes the years to the default view for this component
    }
}