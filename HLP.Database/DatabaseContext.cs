using HLP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace HLP.Database
{
    public class DatabaseContext
    {
        private static DatabaseContext Instance = null;

        public static DatabaseContext GetInstance()
        {
            if (Instance == null)
            {
                Instance = new DatabaseContext();
            }
            return Instance;
        }

        public List<Word> Words { get; set; } = new List<Word>();
        public List<Affix> Affixes { get; set; } = new List<Affix>();
        public List<WordType> WordTypes { get; set; } = new List<WordType>();
        public List<AffixInfo> AffixInfos { get; set; } = new List<AffixInfo>();
        public List<OrderRule> OrderRules { get; set; } = new List<OrderRule>();
        public List<MorphTest> MorphTests { get; set; } = new List<MorphTest>();

        private DatabaseContext() { }

        public static void Dispose()
        {
            if (Instance != null)
            {
                Instance.Words = null;
                Instance.Affixes = null;
                Instance.WordTypes = null;
                Instance.AffixInfos = null;
                Instance.OrderRules = null;
                Instance.MorphTests = null;
                Instance = null;
            }
        }

        public List<string> SearchWordInDatabase(string word, string type)
        {
            var wordResult = Words.Find(w => w.Text == word);

            if (wordResult == null) return new List<string>();

            var types = GetCompatibleWordTypes(type);

            if (types.Count == 0) return new List<string>(wordResult.WordTypes);

            return new List<string>(wordResult.WordTypes.Intersect(types));
        }

        public List<string> GetCompatibleWordTypes(string typeCode)
        {
            var result = new List<string>();

            var type = WordTypes.Find(t => t.Code == typeCode);

            if (type == null) return result;

            result.Add(typeCode);

            type.IncludedWordTypes.ForEach(t => result.AddRange(GetCompatibleWordTypes(t)));

            //result.AddRange(WordTypes.Where(t => t.IncludedWordTypes.Contains(typeCode)).Select(t => t.Code));

            return result.Distinct().ToList();
        }

        /*protected override void OnConfiguring(DbContextOptionsBuilder options)
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
        }*/
    }
}
