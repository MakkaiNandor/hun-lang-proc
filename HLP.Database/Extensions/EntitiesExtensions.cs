using HLP.Database.Entities;
using HLP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Database.Extensions
{
    public static class EntitiesExtensions
    {
        public static Word ToModel(this WordEntity word)
        {
            return new Word
            {
                Text = word.Text,
                WordTypes = word.WordTypes.Split("|").ToList()
            };
        }

        public static Affix ToModel(this AffixEntity affix)
        {
            var db = DatabaseContext.GetInstance();

            return new Affix
            {
                OriginalText = affix.OriginalText,
                Text = affix.Text,
                Info = db.AffixInfos.SingleOrDefault(ai => ai.Code == affix.InfoCode)?.ToModel(),
                Requirements = affix.Requirements.Split(".").ToList(),
                Prevowel = affix.Prevowel,
                Assimilation = affix.Assimilation
            };
        }

        public static WordType ToModel(this WordTypeEntity wordType)
        {
            return new WordType
            {
                Code = wordType.Code,
                Name = wordType.Name,
                IncludedWordTypes = wordType.IncludedWordTypes.Split("|").ToList()
            };
        }

        public static AffixInfo ToModel(this AffixInfoEntity affixInfo)
        {
            return new AffixInfo
            {
                Code = affixInfo.Code,
                Type = affixInfo.Type,
                GroupNumber = affixInfo.GroupNumber,
                WordTypeBefore = affixInfo.WordTypeBefore,
                WordTypeAfter = affixInfo.WordTypeAfter,
                Description = affixInfo.Description
            };
        }

        public static OrderRule ToModel(this OrderRuleEntity orderRule)
        {
            return new OrderRule
            {
                RootWordType = orderRule.RootWordType,
                RulesBeforeRoot = orderRule.RulesBeforeRoot,
                RulesAfterRoot = orderRule.RulesAfterRoot
            };
        }

        public static MorphTest ToModel(this MorphTestEntity morphTest)
        {
            var values = morphTest.Analysis.Split("+").ToList();
            var index = values.IndexOf(values.Find(v => v.StartsWith("!")));
            return new MorphTest
            {
                Word = morphTest.Word,
                Stem = values[index],
                MorphCode = morphTest.MorphCode,
                Prefixes = values.Take(index).ToList(),
                Suffixes = values.TakeLast(values.Count() - index - 1).ToList()
            };
        }
    }
}
