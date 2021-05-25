using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using DBLib.Models;

// 這裡是使用Code First
// 先Migrattions
// dotnet ef migrations add InitialCreate
// 之後再更新至DataBase
// dotnet ef database update InitialCreate

namespace DBLib.DAL
{
    public class AuthenTokenContext : DbContext
    {
        #region 註冊資料表

        public DbSet<Token> Tokens { set; get; }

        #endregion

        #region 覆寫DbContext OnConfiguring

        public AuthenTokenContext() { }

        public AuthenTokenContext(DbContextOptions<AuthenTokenContext> options)
            : base(options)
        {
        }

        // 覆寫原始DbContext OnConfiguring 設置application service provider
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 如果未設置application service provider就指定給予預設
            // 用在Migration後接DataBase Update指令用
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=Data Source=C:\\Users\\jyu\\Desktop\\Project\\Lab\\jyu.lab.jwt\\Lab_JWT\\AuthenToken.db");
            }
        }

        #endregion
    }
}