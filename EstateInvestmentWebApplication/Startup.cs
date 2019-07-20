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
using ReflectionIT.Mvc.Paging;
using EstateInvestmentWebApplication.Models.DatabaseEntitiesModel;
using Microsoft.AspNetCore.Mvc;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<ApplicationDbContext>(option =>
                option.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddPaging();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            
            //Số lượng user access
            int countUserAccess = 0;

            //Mỗi ngày fakeTokenServer được làm mới
            Random ran = new Random();
            int fakeToken = ran.Next() * 10000;
            string fakeTokenServer = fakeToken.ToString();

            //Lấy ngày hiện tại để check đã qua ngày chưa
            int currentDay = DateTime.Now.DayOfYear;


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
                //Check nếu vào Ngày mới save countUserAccess vào db và reset
                if (currentDay != DateTime.Now.DayOfYear)
                {
                    currentDay = DateTime.Now.DayOfYear;
                    ApplicationDbContext dbContext = context.RequestServices.GetService<ApplicationDbContext>();
                    NumberUserAccess userAccessInDay = new NumberUserAccess();
                    userAccessInDay.Date = DateTime.Now.AddDays(-1);
                    userAccessInDay.UserNumber = countUserAccess;
                    dbContext.Add(userAccessInDay);
                    dbContext.SaveChanges();

                    //Reset Token
                    fakeToken = ran.Next() * 10000;
                    fakeTokenServer = fakeToken.ToString();
                    countUserAccess = 0;

                }

                //Count user
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

            DataSeeder.Initialize(roleManager, userManager).Wait();

        }

    }


}
