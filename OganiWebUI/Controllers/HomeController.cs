using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using OganiWebUI.AppCode.Services;
using OganiWebUI.Models.Contexts;
using OganiWebUI.Models.Entities;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace OganiWebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext db;
        private readonly IEmailService emailService;
        private readonly ICryptoService cryptoService;

        public HomeController(DataContext db, IEmailService emailService, ICryptoService cryptoService)
        {
            this.db = db;
            this.emailService = emailService;
            this.cryptoService = cryptoService;
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
        public IActionResult Contact(string fullName, string email, string message)
        {
            var post = new ContactPost { FullName = fullName, Email = email, Message = message, CreatedAt = DateTime.Now };
            db.ContactPosts.Add(post);
            db.SaveChanges();

            return Json(new
            {
                error = false,
                message = "Muracietiniz qebul edildi"
            });
        }
        [HttpPost]
        public async Task<IActionResult> Subscribe(string email)
        {
            var entity = await db.Subscribers.FirstOrDefaultAsync(x => x.Email.Equals(email));
            if (entity?.ApprovedAt is not null)
            {
                return Json(new
                {
                    error = true,
                    message = "Siz artiq abunesiniz"
                });
            } else if (entity is not null)
            {
                return Json(new
                {
                    error = true,
                    message = "Siz abunelik ucun epoct unvaninizi tesdiq etmelisiniz!"
                });
            }

            entity = new Subscribe { Email = email, CreatedAt = DateTime.Now };
            await db.Subscribers.AddAsync(entity);
            await db.SaveChangesAsync();
            string token = $"id={entity.Email}|expire={DateTime.Now.AddHours(1):yyyy.MM.dd HH:mm:ss}";
            token = cryptoService.Encrypt(token, true);
            string redirectUrl = $"{Request.Scheme}://{Request.Host}/subscribe-approve?token={token}";

            string msg = $"Salam <b>Rza</b>,<a href=\"{redirectUrl}\">Link</a> Abuneliyinizi tamamlayin";

            await emailService.SendEmail(entity.Email, "Ogani Subcripton", msg);
            return Json(new
            {
                error = false,
                message = $"E-poçt ünvanınıza təsdiq linki göndərildi. 1 saat erzinde tesdiq edin"
            }); ;
        }
        [Route("/subscribe-approve")]
        public async Task<IActionResult> SubscribeApprove(string token)
        {
            token = cryptoService.Decrypt(token);
            string patterns = @"id=(?<email>[^|]*)\|expire=(?<date>\d{4}\.\d{2}\.\d{2}\s\d{2}:\d{2})";
            var match = Regex.Match(token, patterns);
            if (!match.Success)
                goto l1;

            var email = match.Groups["email"].Value;
            var date = match.Groups["date"].Value;
            if (string.IsNullOrWhiteSpace(email))
                goto l1;


            //Burda xeta verir expire date 00 00 00 gelir deye
            if (string.IsNullOrWhiteSpace(date) || !DateTime.TryParseExact(date, "yyyy.MM.dd HH:mm:ss", null, DateTimeStyles.None, out DateTime expireDate))
                goto l1;

            if (expireDate < DateTime.Now)
            {
                ViewBag.ErrorMessage = "Sorgunun istifade muddeti bitmidir";
                return View();
            }
            var entity = await db.Subscribers.FirstOrDefaultAsync(m => m.Email.Equals(email));
            if(entity is null)
                goto l1;

            if (entity.ApprovedAt is not null)
            {
                ViewBag.ErrorMessage = "Artiq abune olmusunuz";
                return View();
            }
            entity.ApprovedAt = DateTime.Now;
            await db.SaveChangesAsync();
            TempData["Message"] = "Abuneliyiniz tesdiq olundu";
            return RedirectToAction(nameof(Index), "Home");

            l1:
            ViewBag.ErrorMessage = "Qadagan edilmis sorgu";
            return View();
        }



    }
}
