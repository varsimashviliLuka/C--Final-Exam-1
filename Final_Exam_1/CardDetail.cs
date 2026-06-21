namespace Final_Exam_1;

public class CardDetail
{
    public required string CardNumber { get; set; }
    public required string CardValidDate { get; set; }
    public required string CVC { get; set; }

    public bool ValidateCardNumber(string CardNumber)
    {
        return CardNumber == this.CardNumber;
    }

    public bool ValidateCardValidDate(string CardValidDate)
    {
        return CardValidDate == this.CardValidDate;
    }
    public bool ValidateCVC(string CVC)
    {
        return CVC == this.CVC;
    }
}