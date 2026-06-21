namespace Final_Exam_1;

public class Transaction
{
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public required string TransactionType { get; set; }
    public decimal AmountGEL { get; set; }
    public decimal AmountUSD { get; set; }
    public decimal AmountEUR { get; set; }
}