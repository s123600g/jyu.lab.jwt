---
title: jyu.lab.jwt
tags: Github
description: Asp.net Core WebApi JWT小實作練習
---

# jyu.lab.jwt

`Asp.net Core WebApi JWT`小實作練習

使用`asp.net core 5.0`

### 參考連結

* https://blog.miniasp.com/post/2019/12/16/How-to-use-JWT-token-based-auth-in-aspnet-core-31
* https://blog.poychang.net/authenticating-jwt-tokens-in-asp-net-core-webapi/
* https://mgleon08.github.io/blog/2018/07/16/jwt/
* https://medium.com/%E9%BA%A5%E5%85%8B%E7%9A%84%E5%8D%8A%E8%B7%AF%E5%87%BA%E5%AE%B6%E7%AD%86%E8%A8%98/%E7%AD%86%E8%A8%98-%E9%80%8F%E9%81%8E-jwt-%E5%AF%A6%E4%BD%9C%E9%A9%97%E8%AD%89%E6%A9%9F%E5%88%B6-2e64d72594f8
* https://auth0.com/docs/tokens/json-web-tokens/json-web-token-claims#custom-claims

# 關於專案

## Nuget套件

| 套件 | 版本 |
| -------- | -------- |
| `Microsoft.AspNetCore.Authentication.JwtBearer`     | 5.0.6     |
| `NLog`     | 4.7.10     |
| `NLog.Web.AspNetCore`     | 4.12.0     |

## 跟JWT相關

| 程式檔                                            |    位置     | 用途                                              |
|:------------------------------------------------- |:-----------:|:------------------------------------------------- |
| `JWTServices.cs`                                  | `Services/` | JWT核心處理服務                                   |
| `JWTCliam.cs`                                     |  `Models/`  | 用來放User資訊聲明各項目內容Data Modal            |
| `JWTConfig.cs`                                    |  `Models/`  | 用來綁定對應JWT設定值項目內容Data Modal           |
| `appsettings.Development.json` `appsettings.json` |     `/`     | Key(`JWTConfig`)內容配置會跟產生Token與驗證有關連 |

### 如何導入JWT處理服務
此服務導入前必須要有NLog，請確認專案已經導入NLog實作。
> 有關於NLog導入可參考 <br/>
> https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-5

> 使用擴充方法來註冊服務群組 <br/>
> https://docs.microsoft.com/zh-tw/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0#register-groups-of-services-with-extension-methods

透過相依性注入(dependency injection)

* Step 1. 在`Startup.cs`進行服務註冊
```csharp=
services.AddSingleton<JWTBase, JWTServices>();
```
> 關於注入的服務留存期可參考，在這範例是使用`單一` <br/>
> https://docs.microsoft.com/zh-tw/dotnet/core/extensions/dependency-injection#service-lifetimes

* Step 2. 在需使用的Controller內`Constructure`進行服務的注入
```csharp=
private readonly JWTBase jwtHelper;

public LoginController(
    JWTBase jWTServices
            .
            .
){
            .
            .
    jwtHelper = jWTServices;
}
```

透過該服務內`GetJWT()`方法來進行產生JWT Token，需要給予的參數如下
* `jWTCliam` --> Token 資訊聲明內容物件
* `secretKey` --> 加密金鑰，用來做加密簽章用，內容長度最低16字元
* `issuer` --> Token 發行者資訊
* `expireMinutes` --> Token 有效期限(分鐘)，設置該Token可以保存多久

### 設置驗證方法
需要再`Startup.cs`內兩個區塊進行設置

* `ConfigureServices`區塊 <br/>
請記得要先引入 <br/>
1.`Microsoft.AspNetCore.Authentication.JwtBearer` <br/>
2.`Microsoft.IdentityModel.Tokens` <br/>
```csharp=
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
```
> 概念<br/>
> --> 設置每一次Http request進來時驗證是否有帶Token <br/>
> --> 驗證方式為Header內帶 'Authorization': 'Bearer ' + Token

```csharp=
 services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme
)
// 設置JWT驗證選項內容
.AddJwtBearer(options =>
    {
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
```

* `Configure`區塊 <br/>
要加入`Authentication`(驗證)和`Authorization`(授權) <br/>
在裡面加入`app.UseAuthentication()`和`app.UseAuthorization()`  <br/>
記得一個口訣，**先驗證在進行授權**

> 需要注意`UseAuthorization()`要包在`app.UseRouting()`和`app.UseEndpoints()`兩者之間
> 否則在發起Http request進來時會出現錯誤

```
2021-05-20 16:25:15.7919 [ERROR] An unhandled exception has occurred while executing the request. 
System.InvalidOperationException: Endpoint Lab_JWT.Controllers.LoginController.GetTokenInfo (Lab_JWT) contains authorization metadata, but a middleware was not found that supports authorization.
Configure your application startup by adding app.UseAuthorization() inside the call to Configure(..) in the application startup code. The call to app.UseAuthorization() must appear between app.UseRouting() and app.UseEndpoints(...).
   at Microsoft.AspNetCore.Routing.EndpointMiddleware.ThrowMissingAuthMiddlewareException(Endpoint endpoint)
   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)
   at Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware.Invoke(HttpContext httpContext)
   at Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)
   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)
```

### 關於如何使用此JWT處理服務
請參考`Controllers/LoginController.cs`

