using Microsoft.EntityFrameworkCore;

using Edam.Data.FileSystemModel;

namespace Edam.Data.FileSystemDb;

public class FileSystemContext : DbContext
{

   public DbSet<ContentTypeInfo> ContentTypes { get; set; }
   public DbSet<ContainerInfo> Containers { get; set; }
   public DbSet<FileItemInfo> FileItems { get; set; }
   public DbSet<FileItemDataInfo> DataItems { get; set; }

   public string ConnectionString { get; }

   public FileSystemContext()
   {
      //ConnectionString = Configuration["Connnectionstrings:MyConnection"];
      //ConnectionString = LexiconContextHelper.GetConnectionString();
   }

   public FileSystemContext(string connectionString)
   {
      ConnectionString = connectionString;
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      // Configure default schema
      modelBuilder.HasDefaultSchema("FileSystem");
   }

   // Configures EF to create an SQL database using given connection string
   protected override void OnConfiguring(DbContextOptionsBuilder options)
   {
      options.UseSqlServer(ConnectionString);
   }

}
