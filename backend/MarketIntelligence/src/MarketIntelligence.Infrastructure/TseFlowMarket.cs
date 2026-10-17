using System.ComponentModel;

namespace MarketIntelligence.Infrastructure
{
    public enum TseFlowMarket
    {
        Unknown = -1,

        //عمومي - مشترک بين بورس و فرابورس
        [Description("همه")]
        All = 0,

        //بورس TSE
        [Description("بورس")]
        Bourse = 1,

        //فرابورس OTC
        [Description("فرابورس")]
        FaraBourse = 2,

        //آتی
        [Description("آتی")]
        Future = 3,

        //پایه فرابورس OTC
        [Description("پایه فرابورس")]
        OTCBase = 4,

        //پایه فرابورس (منتشر نمی شود) OTC
        [Description("پایه فرابورس(منتشر نمی شود)")]
        OTCBase_NotPublished = 5,

        //بورس انرژی
        [Description("انرژی")]
        Energy = 6,

        //بورس کالا
        [Description("کالا")]
        Mercantile = 7
    }
}
