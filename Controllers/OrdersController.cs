using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/Card")]
        public IActionResult Card() { 
            // get string from cookie = "card"
            // change it to list<books>
            // send them to viewbag
            return View();
        }
        [HttpPost("AddToCard")]
        public IActionResult AddToCard()
        {
            //get id book
            // get string from cookie = "card" 
            // add that string to card 
            // send them to client => set string to cookie ="card"
            return Json("");
        }

        
    }
}
