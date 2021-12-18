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
        public DbSet<FileModel> Files { get; set; }
        public DbSet<UserFilesModel> UserFiles { get; set; }
        public DbSet<WorkingGroupModel> WorkingGroups { get; set; }
        public DbSet<UserInWorkingGroupModel> UserInWorkingGroups { get; set; }
        public DbApp(DbContextOptions<DbApp> options):base(options)
        {
            Database.EnsureCreated();
        }

    }
}
