using FileExchanger.Helpers;
using FileExchanger.Models;
using FileExchanger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger
{
    public class DbApp : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ExchangeFileModel> ExchangeFiles { get; set; }
        public DbSet<UserFilesModel> UserFiles { get; set; }
        public DbSet<WorkingGroupModel> WorkingGroups { get; set; }
        public DbSet<UserInWorkingGroupModel> UserInWorkingGroups { get; set; }
        public DbSet<AuthClientModel> AuthClients { get; set; }
        public DbSet<AdminModel> Admins { get; set; }
        public DbSet<DirectoryModel> Directory { get; set; }
        public DbSet<StorageFileModel> StorageFiles { get; set; }
        public DbSet<TelegramUserModel> TelegramUsers { get; set; }
        public DbApp(DbContextOptions<DbApp> options):base(options)
        {
            configurationDb();
        }
        public DbApp(string dbConnect) : base(GetOptions(dbConnect))
        {
            configurationDb();
        }
        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }
        private void configurationDb()
        {
            Database.EnsureCreated();
            createAdminUser();
        }
        private void createAdminUser()
        {
            if (AuthClients.Any())
                return;
            var admin = AuthClients.Add(new AuthClientModel()
            {
                Email = "admin@example.com",
                Password = PasswordHelper.GetHash("admin"),
                Name = "admin"
            }).Entity;
            Admins.Add(new AdminModel() { AuthClient = admin });
            Directory.Add(new DirectoryModel()
            {
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Name = "/",
                Key = "root",
                Owner = admin,
                Root = null
            });
            SaveChanges();
        }
    }
}
