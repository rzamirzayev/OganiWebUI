using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class SubscribeService : ISubscribeService
    {
        private readonly DbContext db;
        private readonly IEmailService emailService;
        private readonly ICryptoService cryptoService;
        private readonly IHttpContextAccessor ctx;

        public SubscribeService(DbContext db,IEmailService emailservice,ICryptoService cryptoService,IHttpContextAccessor ctx)
        {
            this.db = db;
            this.emailService = emailservice;
            this.cryptoService = cryptoService;
            this.ctx = ctx;
        }
        public async Task<Tuple<bool,string>> Subscribe(string email)
        {
            var entity = await db.Set<Subscribe>().FirstOrDefaultAsync(x => x.Email.Equals(email));
            var data = Tuple.Create(true, "");
            if (entity?.ApprovedAt is not null)
            {
                return Tuple.Create(true, "Siz artiq abunesiniz");
            }
            else if (entity is not null)
            {
                return Tuple.Create(true, "Siz abunelik ucun epoct unvaninizi tesdiq etmelisiniz!");
            }
            entity = new Subscribe { Email = email };
            await db.Set<Subscribe>().AddAsync(entity);
            await db.SaveChangesAsync();
            string token = $"id={entity.Email}|expire={DateTime.Now.AddHours(1):yyyy.MM.dd HH:mm:ss}";
            token = cryptoService.Encrypt(token, true);
            string redirectUrl = $"{ctx.HttpContext.Request.Scheme}://{ctx.HttpContext.Request.Host}/subscribe-approve?token={token}";
            string msg = $"Salam <b>Rza</b>,<a href=\"{redirectUrl}\">Link</a> Abuneliyinizi tamamlayin";

            await emailService.SendEmail(entity.Email, "Ogani Subcripton", msg);
            return Tuple.Create(false, "E-poçt ünvanınıza təsdiq linki göndərildi. 1 saat erzinde tesdiq edin");

        }

        public async Task<Tuple<bool,string>> SubscribeApprove(string token)
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
                return Tuple.Create(true, "Sorgunun istifade muddeti bitmidir");
            }
            var entity = await db.Set<Subscribe>().FirstOrDefaultAsync(m => m.Email.Equals(email));
            if (entity is null)
                goto l1;

            if (entity.ApprovedAt is not null)
            {
                return Tuple.Create(true, "Artiq abune olmusunuz");

            }
            entity.ApprovedAt = DateTime.Now;
            await db.SaveChangesAsync();
            return Tuple.Create(false, "Abuneliyiniz tesdiq olundu");


        l1:
            return Tuple.Create(true, "Qadagan edilmis sorgu");
        }
    }
}
