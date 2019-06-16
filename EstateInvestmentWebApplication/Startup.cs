using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EstateInvestmentWebApplication.Data;
using EstateInvestmentWebApplication.Models;
using EstateInvestmentWebApplication.Services;
using Microsoft.AspNetCore.Http;
using System.Timers;

namespace EstateInvestmentWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Số lượng user access
            int countUserAccess = 0;

            //Mỗi ngày fakeTokenServer được làm mới
            Random ran = new Random();
            int fakeToken = ran.Next() * 10000;
            string fakeTokenServer = fakeToken.ToString();

            //Tạo Timer reset fakeTokenServer
            double resetTime = (23-DateTime.Now.Hour)*(3.6e+6) +(59 - DateTime.Now.Minute)* 60000;
            Timer timer = new Timer(resetTime);
            timer.Elapsed += new ElapsedEventHandler((sender, e) => ResetCountUserAccess(sender, e,ref countUserAccess,ref fakeTokenServer));
            timer.Start();



            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();


            //Middleware count số lượng User truy cập vào page
            app.Use(async (context, next) =>
            {

                var fakeTokenClient = context.Request.Cookies["Fake_token"];
                if (fakeTokenClient != fakeTokenServer)
                {
                    CookieOptions options = new CookieOptions();

                    //Set Expires time cho cookie qua ngày là hết hạn
                    options.Expires = DateTimeOffset.Now.AddHours(23-DateTime.Now.Hour).AddMinutes(59-DateTime.Now.Minute).AddSeconds(59-DateTime.Now.Second);
                    context.Response.Cookies.Append("Fake_token", fakeTokenServer,options);
                    countUserAccess++;
                }

                //Truyền biến countUserAccess qua các controller
                context.Items["countUserAccess"] = countUserAccess;

                await next.Invoke();

            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void ResetCountUserAccess(Object source, ElapsedEventArgs e,ref int countUserAccess,ref string fakeTokenServer)
        {
            var timer = source as Timer;
            if(timer != null)
            {
                //Reset Token
                Random ran = new Random();
                int fakeToken = ran.Next() * 10000;
                fakeTokenServer = fakeToken.ToString();

                countUserAccess = 0;

                //Set lại Interval
                double resetTime = (23 - DateTime.Now.Hour) * (3.6e+6) + (59 - DateTime.Now.Minute) * 60000;
                timer.Interval = resetTime;
                timer.Start();

            }
        }

    }
}
