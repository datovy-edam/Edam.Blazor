using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Edam.Data.Lexicon.Vocabulary;
using System.Linq.Expressions;
using voca = Edam.Data.Lexicon.Vocabulary;
using System.Runtime.CompilerServices;
using Edam.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Edam.Data.Lexicon
{

   public class LexiconContext : DbContext
   {

      public DbSet<LexiconItemInfo> Lexicon { get; set; }

      public DbSet<AreaItemInfo> Area { get; set; }
      public DbSet<EntityItemInfo> Entity { get; set; }
      public DbSet<ElementItemInfo> Element { get; set; }
      public DbSet<RelationshipItemInfo> Relationship { get; set; }
      public DbSet<TagItemInfo> Tag { get; set; }
      public DbSet<MetadataItemInfo> Metadata { get; set; }
      public DbSet<TermItemInfo> Term { get; set; }
      public DbSet<voca.UriItemInfo> Uri { get; set; }

      public string ConnectionString { get; }

      public LexiconContext()
      {
         ConnectionString = LexiconContextHelper.GetConnectionString();
      }

      public LexiconContext(string connectionString)
      {
         ConnectionString = connectionString;
      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         // Configure default schema
         modelBuilder.HasDefaultSchema("Vocabulary");
      }

      // Configures EF to create an SQL database using given connection string
      protected override void OnConfiguring(DbContextOptionsBuilder options)
          => options.UseSqlServer(ConnectionString);

   }

}
