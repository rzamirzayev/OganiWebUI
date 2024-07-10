using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using OganiWebUI.AppCode.Services;
using OganiWebUI.Models.Contexts;
using OganiWebUI.Models.Entities;
using System.Web;

namespace OganiWebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext db;
        private readonly IEmailService emailService;
        private readonly ICryptoService cryptoService;

        public HomeController(DataContext db,IEmailService emailService,ICryptoService cryptoService)
        {
            this.db = db;
            this.emailService=emailService;
            this.cryptoService=cryptoService;
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
        [HttpPost]
        public async Task<IActionResult> Subscribe(string email)
        {
            var entity=await db.Subscribers.FirstOrDefaultAsync(x=>x.Email.Equals(email));
            if (entity?.ApprovedAt is not null)
            {
                return Json(new
                {
                    error = true,
                    message = "Siz artiq abunesiniz"
                });
            }else if(entity is not null)
            {
                return Json(new
                {
                    error = true,
                    message = "Siz abunelik ucun epoct unvaninizi tesdiq etmelisiniz!"
                });
            }

            entity = new Subscribe { Email = email,CreatedAt=DateTime.Now };
            await db.Subscribers.AddAsync(entity);
            await db.SaveChangesAsync();
            string token = $"id={entity.Email}+expire={DateTime.Now.AddHours(1):yyyy.MM.dd HH:mm:ss}";
            token = cryptoService.Encrypt(token, true);
            string redirectUrl = $"https://localhost:44386/subscribe-approve?token={token}";

            string msg = $"Salam <b>Rza</b>,<a href=\"{redirectUrl}\">Link</a> Abuneliyinizi tamamlayin";

            await emailService.SendEmail(entity.Email, "Ogani Subcripton", msg);
            return Json(new
            {
                error = false,
                message = $"{email} sorgunu aldiq"
            }); ; 
        }
        [Route("/subscribe-approve")]
        public IActionResult S(string token)
        {
            token = cryptoService.Decrypt(token);
            return Content(token);
        }

        public string Encrypt(string text)
        {
            return cryptoService.Encrypt(text,true);
        }

        public string Decrypt(string text)
        {
            return cryptoService.Decrypt(text);
        }

    }
}
