using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Practice.Data;
using Practice.Models;
using Practice.ViewModels;

namespace Practice.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<BasketVM> baskets;

            if (Request.Cookies["basket"] != null)
            {
                baskets = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);   
            }
            else
            {
                baskets = new List<BasketVM>();
            }

            foreach (var item in baskets)
            {
                var product = _context.Products
                    .Include(p=>p.Images)
                    .Where(p => !p.SoftDelete)
                    .FirstOrDefault(p => p.Id == item.Id);

                item.Product = product;
            }
            return View(baskets);
        }
    }
}
