using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using Web_1670.Data;
using Web_1670.Models;

namespace Web_1670.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index( string search)
        {
            var books = from m in _context.Book select m;
            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(s => s.NameBook.Contains(search));
            }
            /*var _book = getAllBook();
            ViewBag.book = _book;*/
            return View(await books.ToListAsync());
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
        //GET ALL PRODUCT
        public List<Book> getAllProduct()
        {
            return _context.Book.ToList();
        }

        //GET DETAIL PRODUCT
        public Book getDetailProduct(int id)
        {
            var product = _context.Book.Find(id);
            return product;
        }

        //ADD CART
        public IActionResult addCart(int id)
        {
            var cart = HttpContext.Session.GetString("cart");//get key cart
            if (cart == null)
            {
                var book = getDetailProduct(id);
                List<Cart> listCart = new List<Cart>()
               {
                   new Cart
                   {
                       Book = book,
                       Quantity = 1
                   }
               };
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));

            }
            else
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                bool check = true;
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Book.Id == id)
                    {
                        dataCart[i].Quantity++;
                        check = false;
                    }
                }
                if (check)
                {
                    dataCart.Add(new Cart
                    {
                        Book = getDetailProduct(id),
                        Quantity = 1
                    });
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                // var cart2 = HttpContext.Session.GetString("cart");//get key cart
                //  return Json(cart2);
            }

            return RedirectToAction(nameof(Index));

        }

        public IActionResult ListCart()
        {
            var cart = HttpContext.Session.GetString("cart");//get key cart
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                if (dataCart.Count > 0)
                {
                    ViewBag.carts = dataCart;
                    return View();
                }
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult updateCart(int id, int quantity)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                if (quantity > 0)
                {
                    for (int i = 0; i < dataCart.Count; i++)
                    {
                        if (dataCart[i].Book.Id == id)
                        {
                            dataCart[i].Quantity = quantity;
                        }
                    }


                    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                }
                var cart2 = HttpContext.Session.GetString("cart");
                return Ok(quantity);
            }
            return BadRequest();

        }

        public IActionResult deleteCart(int id)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Book.Id == id)
                    {
                        dataCart.RemoveAt(i);
                    }
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                return RedirectToAction(nameof(ListCart));
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
        public async Task<ActionResult> CheckOut()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("~/Login");
            }
            /*if (User.Identity.Name == null)
            {
                return RedirectToRoute(new PathString("/Login/"));
            }*/
            if (ModelState.IsValid)
            {
                var cart = HttpContext.Session.GetString("cart");
                int count = 0;
                if (count ==null) {
                    count = _context.Order.Max(m => m.Count); ;
                }
                
                if (cart != null)
                {
                    List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                    for (int i = 0; i < dataCart.Count; i++)
                    {

                        Order order = new Order()
                        {

                            IdentityUserId = user.Id,
                            BookId = dataCart[i].Book.Id,
                            Qty = dataCart[i].Quantity,
                            Price = Convert.ToDouble(dataCart[i].Quantity * dataCart[i].Book.Price),
                            OrderTime = DateTime.Now,
                            Count = count + 1
                        };
                        _context.Order.Add(order);
                        _context.SaveChanges();
                        deleteCart(dataCart[i].Book.Id);
                    }

                }

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

    }
}