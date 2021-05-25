using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions;
using Microsoft.EntityFrameworkCore;
using Lab_JWT.Models;
using Lab_JWT.Services;
using DBLib.Models;
using DBLib.DAL;

namespace Lab_JWT
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
            services.AddControllers()
            // https://docs.microsoft.com/zh-tw/aspnet/core/web-api/advanced/formatting?view=aspnetcore-5.0#configure-systemtextjson-based-formatters
            .AddJsonOptions(options =>
            {
                // 設置Reponse內JSON key命名規則使用PascalCase而不是使用預設camelCase(駝峰式命名)
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            #region 注入DbContext

            string getSQLConnectionStr = Configuration.GetConnectionString("DefaultConnectionStrings");

            services.AddDbContext<AuthenTokenContext>(config =>
            {
                config.UseSqlite(getSQLConnectionStr);
            });

            #endregion

            #region  Set authentication type to jwt

            // 設置每一次Http request進來時驗證是否有帶Token
            // 驗證方式為Header內帶 'Authorization': 'Bearer ' + Token
            // You need to import package as follow
            // using Microsoft.AspNetCore.Authentication.JwtBearer;
            services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme
            )
            // 設置JWT驗證選項內容
            .AddJwtBearer(options =>
            {
                // You need to import package as follow
                // using Microsoft.IdentityModel.Tokens;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    #region  配置驗證發行者

                    ValidateIssuer = true, // 是否要啟用驗證發行者
                    ValidIssuer = Configuration.GetSection("JWTConfig").GetValue<string>("Issuer"),

                    #endregion

                    #region 配置驗證接收方

                    ValidateAudience = false, // 是否要啟用驗證接收者
                    // ValidAudience = "" // 如果不需要驗證接收者可以註解

                    #endregion

                    #region 配置驗證Token有效期間

                    ValidateLifetime = true, // 是否要啟用驗證有效時間

                    #endregion

                    #region 配置驗證金鑰

                    ValidateIssuerSigningKey = false, // 是否要啟用驗證金鑰，一般不需要去驗證，因為通常Token內只會有簽章

                    #endregion

                    #region 配置簽章驗證用金鑰

                    // 這裡配置是用來解Http Request內Token加密
                    // 如果Secret Key跟當初建立Token所使用的Secret Key不一樣的話會導致驗證失敗
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            Configuration.GetSection("JWTConfig").GetValue<string>("SignKey")
                        )
                    )

                    #endregion
                };
            });

            #endregion

            #region Config data bind(strong type data)
            // https://docs.microsoft.com/zh-tw/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0#ioptionsmonitor

            // 將jwt配置綁定強型別Data物件模型，後面Controller在Constructure會在注入此Data物件模型
            services.Configure<JWTConfig>(Configuration.GetSection("JWTConfig"));

            #endregion

            // 補齊.net core字元編碼類型，避免出現亂碼現象
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            #region Services register

            // 使用單例型態來進行服務註冊
            // 意謂者從app啟動時就會建立此服務，而後面所有的Http request都會用這個同一個服務實體
            // 當Http request進行response後，此服務實體還是會繼續存在不會被釋放掉
            services.AddTransient<JWTBase, JWTServices>();

            #endregion

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lab_JWT", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab_JWT v1"));
            }

            app.UseAuthentication(); // 驗證

            app.UseHttpsRedirection();

            app.UseRouting();

            // 授權必須要在'app.UseRouting()'跟'app.UseEndpoints'中間
            app.UseAuthorization(); // 授權

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
