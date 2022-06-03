using DataLayer;
using DataLayer.Entityes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private ShopContext _context; //Добавляем поле, которое будет отражать зависимости.
        
      
        public HomeController(ShopContext context)
        {
            _context = context;
            
        }
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            
            List<OrderShop> _dirs = _context.orderShops.Include(x => x.Customers).Include(x=>x.Product).ToList();// выведем через linq, обращаемся к колекциям даннх и выводим их
            return new JsonResult(new { ResultFinal = _dirs });
        }
        

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ClassEdit model)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new { ResultFinal = model});
            if (await AddProduct(model.TypeProduct,model.CountProduct, model.SizeProduct, model.Price, model.Brend))
                return Redirect("/Home/Index");
            else
            {
                ModelState.AddModelError("Username", "Name of product in use");
                return new JsonResult(new { ResultFinal = model}); 
            }
        }

        private async Task<bool> AddProduct(string name, int countProduct, double sizeP, double price, Brend brend)
        {
            if (_context.Products.Any(p => p.TypeProduct == name))
                return false;
            Product pro = new Product()
            {
                TypeProduct = name, 
                CountProduct = countProduct, SizeProduct = sizeP, Price = price, Brend = brend
               
            };
            await _context.Products.AddAsync(pro);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IActionResult> EditProduct(ClassEdit model)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new { ResultFinal = model });
            Product pro = _context.Products.Find(model.Id);
            if (pro != null)
            {
                bool taken = pro.TypeProduct != model.TypeProduct && _context.Products.Any(p => p.TypeProduct == model.TypeProduct);
                if (taken)
                {
                    ModelState.AddModelError("Product", "Product in use");
                    return new JsonResult(new { ResultFinal = model });
                }
                pro.TypeProduct = model.TypeProduct;
                await _context.SaveChangesAsync();
                return Redirect("/Home/ListProduct");
            }
            else
            {
                ModelState.AddModelError("", "Invalid ID");
                return new JsonResult(new { ResultFinal = model });
            }
        }

        
        public async Task<IActionResult> DeleteProduct(int id)
        {
            Product prod = _context.Products.Find(id);
            if (prod != null)
            {
                _context.Products.Remove(prod);
                await _context.SaveChangesAsync();
            }
            return Redirect("/Account/ListFlight");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
