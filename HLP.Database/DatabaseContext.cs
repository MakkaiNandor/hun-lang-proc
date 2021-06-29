using HLP.Database.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Text;
using HLP.Database.Extensions;

namespace HLP.Database
{
    public class DatabaseContext : DbContext
    {
        public static readonly string DbPath = @".\my_database.db";

        public static DatabaseContext Instance = null;

        public static DatabaseContext GetInstance()
        {
            if (Instance == null)
            {
                Instance = new DatabaseContext();
            }
            return Instance;
        }

        public DbSet<WordEntity> Words { get; set; }
        public DbSet<AffixEntity> Affixes { get; set; }
        public DbSet<WordTypeEntity> WordTypes { get; set; }
        public DbSet<AffixInfoEntity> AffixInfos { get; set; }
        public DbSet<OrderRuleEntity> OrderRules { get; set; }
        public DbSet<MorphTestEntity> MorphTests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<WordEntity>().ToTable("Words");
            builder.Entity<WordEntity>().HasKey(e => e.Id);
            builder.Entity<WordEntity>().Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Entity<WordEntity>().HasIndex(e => e.Text);

            builder.Entity<AffixEntity>().ToTable("Affixes");
            builder.Entity<AffixEntity>().HasKey(e => e.Id);
            builder.Entity<AffixEntity>().Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Entity<AffixEntity>().HasIndex(a => a.Text);

            builder.Entity<WordTypeEntity>().ToTable("WordTypes");
            builder.Entity<WordTypeEntity>().HasKey(e => e.Id);
            builder.Entity<WordTypeEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Entity<AffixInfoEntity>().ToTable("AffixInfos");
            builder.Entity<AffixInfoEntity>().HasKey(e => e.Id);
            builder.Entity<AffixInfoEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Entity<OrderRuleEntity>().ToTable("OrderRules");
            builder.Entity<OrderRuleEntity>().HasKey(e => e.Id);
            builder.Entity<OrderRuleEntity>().Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Entity<MorphTestEntity>().ToTable("MorphTests");
            builder.Entity<MorphTestEntity>().HasKey(e => e.Id);
            builder.Entity<MorphTestEntity>().Property(e => e.Id).ValueGeneratedOnAdd();
        }

        public List<string> SearchInDatabase(string word, string type)
        {
            var wordResult = Words.SingleOrDefault(w => w.Text == word).ToModel();

            if (wordResult == null) return new List<string>();

            var types = GetCompatibleWordTypes(type);

            if (types.Count == 0) return new List<string>(wordResult.WordTypes);

            return new List<string>(wordResult.WordTypes.Intersect(types));
        }

        public List<string> GetCompatibleWordTypes(string typeCode)
        {
            var result = new List<string>();

            var type = WordTypes.SingleOrDefault(t => t.Code == typeCode).ToModel();

            if (type == null) return result;

            result.Add(typeCode);

            type.IncludedWordTypes.ForEach(t => result.AddRange(GetCompatibleWordTypes(t)));

            result.AddRange(WordTypes.Where(t => t.IncludedWordTypes.Contains(typeCode)).Select(t => t.Code));

            return result.Distinct().ToList();
        }

        /*private static DatabaseContext DBInstance = null;

        public static DatabaseContext GetInstance()
        {
            if(DBInstance is null)
            {
                DBInstance = new DatabaseContext();
            }
            return DBInstance;
        }

        public List<WordType> WordTypes { get; } = new List<WordType>();
        public List<Word> Words { get; } = new List<Word>();
        public List<Affix> Affixes { get; } = new List<Affix>();
        public List<AffixInfo> AffixInfos { get; } = new List<AffixInfo>();

        private DatabaseContext()
        {
            var loader = new DatabaseLoader();
            loader.LoadDataAsync(WordTypes, Words, Affixes, AffixInfos);
        }

        public List<string> SearchInDatabase(string word, string type)
        {
            var wordResult = Words.Find(w => w.Text == word);

            if (wordResult == null) return new List<string>();

            var types = GetCompatibleWordTypes(type);

            if (types.Count == 0) return new List<string>(wordResult.Types);

            return new List<string>(wordResult.Types.Intersect(types));
        }

        public List<string> GetCompatibleWordTypes(string typeCode)
        {
            var result = new List<string>();

            var type = WordTypes.Find(t => t.Code == typeCode);

            if (type == null) return result;

            result.Add(typeCode);

            type.Includes.ForEach(t => result.AddRange(GetCompatibleWordTypes(t)));

            result.AddRange(WordTypes.Where(t => t.Includes.Contains(typeCode)).Select(t => t.Code));

            return result.Distinct().ToList();
        }*/
    }
}
