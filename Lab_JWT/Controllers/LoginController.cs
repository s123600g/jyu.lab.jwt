using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Lab_JWT.Models;
using Lab_JWT.Services;

namespace Lab_JWT.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> log;
        private readonly IConfiguration config;
        private readonly JWTConfig jwtConfig;
        private readonly JWTBase jwtHelper;

        public LoginController(
            ILogger<LoginController> logger,
            IOptions<JWTConfig> jwtOptions,
            IConfiguration configuration,
            JWTBase jWTServices
        )
        {
            log = logger;
            jwtConfig = jwtOptions.Value;
            config = configuration;
            jwtHelper = jWTServices;
        }

        /// <summary>
        /// 登入取得Token
        /// 簡易Sample 沒有做任何登入驗證自動給予內容
        /// </summary>
        /// <returns></returns>
        [Route("api/signin")]
        [HttpPost]
        [AllowAnonymous]
        public Response SignIn()
        {
            Response res = new Response();
            JWTCliam cliam = new JWTCliam();

            try
            {
                DateTime getExpireDateTime = DateTime.UtcNow.AddMinutes(jwtConfig.ExpireDateTime);

                cliam.iss = jwtConfig.Issuer;
                cliam.sub = Guid.NewGuid().ToString(); // 放User內容，這裡放自動產生Guid值
                cliam.iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                cliam.nbf = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                cliam.jti = Guid.NewGuid().ToString(); // Token 識別獨立ID
                cliam.exp = getExpireDateTime.ToString();

                // 執行產生JWT，取得回應結果
                RunStatus getJWTResponse = jwtHelper.GetJWT(
                    cliam,
                    jwtConfig.SignKey,
                    jwtConfig.Issuer,
                    jwtConfig.ExpireDateTime
                );

                switch (getJWTResponse.isSuccess)
                {
                    case true:

                        res.Status = true;
                        res.JwtToken = getJWTResponse.jwt;
                        res.Msg = "Done.";

                        break;

                    case false:

                        res.Status = false;
                        res.JwtToken = string.Empty;
                        res.Msg = "驗證過程發生錯誤.";

                        break;
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}/{ex.StackTrace}");
                res.Status = false;
                res.JwtToken = string.Empty;
                res.Msg = "處理過程發生錯誤.";
            }

            return res;
        }

        [Route("api/get/info")]
        [HttpGet]
        [Authorize]
        public Response GetTokenInfo()
        {
            Response res = new Response();

            try
            {
                // 這裡簡單查詢User資訊聲明內發行者資訊(Issuer)
                var getIssuer = User.Claims.Where(
                    data => data.Type == "iss"
                ).FirstOrDefault();

                res.Status = true;
                res.Msg = getIssuer.Value.ToString();
                res.JwtToken = string.Empty;
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}\n{ex.StackTrace}");
            }

            return res;
        }
    }
}