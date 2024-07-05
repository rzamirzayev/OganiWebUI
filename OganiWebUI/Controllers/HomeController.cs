using Microsoft.AspNetCore.Mvc;
using OganiWebUI.Models.Contexts;
using OganiWebUI.Models.Entities;

namespace OganiWebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext db;

        public HomeController(DataContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(string fullName,string email,string message)
        {
            var post=new ContactPost {FullName=fullName, Email= email, Message = message,CreatedAt=DateTime.Now };
            db.ContactPosts.Add(post);
            db.SaveChanges();

            return Json(new
            {
                error=false,
                message="Muracietiniz qebul edildi"
            });
        }
    }
}
