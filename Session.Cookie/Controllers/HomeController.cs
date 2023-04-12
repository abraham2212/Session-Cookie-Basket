using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Practice.Data;
using Practice.Models;
using Practice.ViewModels;

namespace Practice.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //HttpContext.Session.SetString("name", "Chinara");
            //var data = HttpContext.Session.GetString("name");
            //ViewBag.Data = data;

            //Response.Cookies.Append("surname", "Ibadova");
            //var datacookie = Request.Cookies["surname"];
            //ViewBag.Cookie = datacookie;

            IEnumerable<Slider> sliders = await _context.Sliders
                .Where(p => !p.SoftDelete)
                .ToListAsync();
            SliderInfo sliderInfo = await _context.SliderInfos
                .Where(p => !p.SoftDelete)
                .FirstOrDefaultAsync();
            IEnumerable<Product> products = await _context.Products
                .Include(p=>p.Images)
                .Where(p=>!p.SoftDelete)
                .Take(8)
                .ToListAsync();
            IEnumerable<Category> categories = await _context.Categories
                .Where(p => !p.SoftDelete)
                .ToListAsync();
            About about = await _context.Abouts
                .Where(p => !p.SoftDelete)
                .FirstOrDefaultAsync();
            ExpertHeader expertHeader = await _context.ExpertHeaders
                .Where(p => !p.SoftDelete)
                .FirstOrDefaultAsync();
            IEnumerable<ExpertExpertPosition> expertExpertPosition = await _context.ExpertExpertPositions
                .Include(e=>e.Expert)
                .Include(e=>e.ExpertPosition)
                .Where(p => !p.SoftDelete)
                .ToListAsync();
            Subscribe subscribe = await _context.Subscribes
                .Where(p => !p.SoftDelete)
                .FirstOrDefaultAsync();
            IEnumerable<Blog> blogs = await _context.Blogs
                .Where(p => !p.SoftDelete)
                .ToListAsync();
            BlogHeader blogHeader = await _context.BlogHeaders
                .Where(p => !p.SoftDelete)
                .FirstOrDefaultAsync();
            IEnumerable<Author> authors = await _context.Authors
                .Include(a=>a.Says)
                .Where(p => !p.SoftDelete)
                .ToListAsync();
            IEnumerable<Instagram> instagrams = await _context.Instagrams
                .Where(p => !p.SoftDelete)
                .ToListAsync();


            HomeVM model = new()
            {
                Slider = sliders,
                SliderInfo = sliderInfo,
                Products = products,
                Categories = categories,
                About = about,
                ExpertHeader = expertHeader,
                ExpertExpertPositions = expertExpertPosition,
                Subscribe = subscribe,
                Blogs = blogs,
                BlogHeader = blogHeader,
                Authors = authors,
                Instagrams = instagrams
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id is null) return BadRequest();
            Product dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null) return NotFound();

            List<BasketVM> baskets;

            if (Request.Cookies["basket"] != null)
            {
                baskets = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            else
            {
                baskets = new List<BasketVM>();
            }


            BasketVM existProduct = baskets.FirstOrDefault(p => p.Id == id);

            if (existProduct == null)
            {
                baskets.Add(new BasketVM
                {
                    Id = dbProduct.Id,
                    Count = 1
                });
            }
            else
            {
                existProduct.Count++;
            }
            Response.Cookies.Append("basket",JsonConvert.SerializeObject(baskets));

            return RedirectToAction(nameof(Index));
        }
       
      
    }
}