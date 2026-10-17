namespace MarketIntelligence.Domain
{
    public class TseIndexB1LastDayLastData
    {
        public string LVal30 { get; set; }              // نام گروه (مثلاً محصولات فلزی)
        public long InsCode { get; set; }               // کد نماد
        public DateTime Date { get; set; }              // تاریخ (DEven)
        public int Time { get; set; }                   // ساعت (HEven)

        public int ZValIdxCot { get; set; }
        public int ZValBaiIbs { get; set; }
        public int ZValHauIbs { get; set; }
        public int ZValIchgIbs { get; set; }
        public int ZValNonCotIbs { get; set; }
        public int ZValResIbs { get; set; }
        public int ZValSuIbs { get; set; }
        public int ZTotValIbs { get; set; }

        public decimal HPhJIdx { get; set; }
        public decimal HPbJIdx { get; set; }
        public decimal XDrNivJIdx004 { get; set; }
        public decimal XPhNivJIdx004 { get; set; }
        public decimal XPbNivJIdx004 { get; set; }

        public decimal XNivIrteNetIbs { get; set; }
        public decimal XVarIdxJ { get; set; }
        public decimal XVarIdxJRfV { get; set; }
        public decimal XMoyVarValIbs { get; set; }
        public decimal XMoyVarValBaiIbs { get; set; }
        public decimal XmoyVarValHauIbs { get; set; }
    }

}
