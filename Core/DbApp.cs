using Core.Helpers;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core
{
    public class DbApp : DbContext
    {
        public DbApp(DbContextOptions<DbApp> options):base(options)
        {
            configurationDb();
        }
        public DbApp(string dbConnect) : base(GetOptions(dbConnect))
        {
            configurationDb();
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ExchangeFileModel> ExchangeFiles { get; set; }
        public DbSet<UserFilesModel> UserFiles { get; set; }
        public DbSet<WorkingGroupModel> WorkingGroups { get; set; }
        public DbSet<UserInWorkingGroupModel> UserInWorkingGroups { get; set; }
        public DbSet<AuthClientModel> AuthClients { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
        public DbSet<AdminModel> Admins { get; set; }
        public DbSet<DirectoryModel> Directory { get; set; }
        public DbSet<StorageFileModel> StorageFiles { get; set; }
        public DbSet<TelegramUserModel> TelegramUsers { get; set; }
        public DbSet<ShareItemModel> ShareItems { get; set; }


        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }
        private void configurationDb()
        {
            Database.EnsureCreated();
            createDefaultUsers();
        }
        private void createDefaultUsers()
        {
            if (AuthClients.Any())
                return;
            var sait = PasswordHelper.GetSecureSalt;
            var admin = AuthClients.Add(new AuthClientModel()
            {
                Email = "admin@example.com",
                Password = PasswordHelper.GetHash("admin", sait),
                PasswordSalt = Convert.ToBase64String(sait),
                Name = "admin",
                Active = true,
            }).Entity;
            var guest = AuthClients.Add(new AuthClientModel()
            {
                Email = "guest",
                Password = PasswordHelper.GetHash("guest", sait),
                PasswordSalt = Convert.ToBase64String(sait),
                Name = "guest",
                Active = false
            }).Entity;
            Admins.Add(new AdminModel() { AuthClient = admin });
            Directory.Add(new DirectoryModel()
            {
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Name = "/",
                Key = "".RandomString(96),
                Owner = admin,
                Root = null
            });
            Directory.Add(new DirectoryModel()
            {
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Name = "/",
                Key = "".RandomString(96),
                Owner = guest,
                Root = null,
            });
            SaveChanges();
        }
    }
}
