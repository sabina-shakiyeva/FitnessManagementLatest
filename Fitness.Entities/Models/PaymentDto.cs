namespace FitnessManagement.Dtos
{
    public class PaymentDto
    {
        public string CardNumber { get; set; }
        public string CardHolder { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
        public decimal Amount { get; set; }
        public bool IsMonthlyPayment { get; set; }


        //public string CardToken { get; set; } 
        //public decimal Amount { get; set; }
        //public bool IsMonthlyPayment { get; set; }
        //public string Currency { get; set; } = "usd";

    }
}
