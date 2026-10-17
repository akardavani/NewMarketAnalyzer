namespace MarketIntelligence.Domain
{
    public class TseIndexB2
    {
        public long InsCode { get; set; }
        public DateTime Date { get; set; }   // DEven
        public int Time { get; set; }        // HEven

        public decimal XNivInuClMresIbs { get; set; }
        public int HNivInuClMresIbs { get; set; }

        public decimal XNivInuPrDifMresIbs { get; set; }
        public int HNivInuPrDifMresIbs { get; set; }

        public decimal XNivInuPhMresIbs { get; set; }
        public int HNivInuPhMresIbs { get; set; }

        public decimal XVarIdxPhJClV { get; set; }

        public decimal XNivInuPbMresIbs { get; set; }
        public int HNivInuPbMresIbs { get; set; }

        public decimal XVarIdxPbJClV { get; set; }
        public decimal XVarDrInuClV { get; set; }

        public decimal QDvdNetJValIbs { get; set; }
        public decimal QCapBsRfVIbs { get; set; }
        public decimal KAjCapBzIbs { get; set; }
        public decimal XNivIrteNetClIbs { get; set; }
    }

}
