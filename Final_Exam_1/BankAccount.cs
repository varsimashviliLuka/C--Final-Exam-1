namespace Final_Exam_1;

public class BankAccount
{
    
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required CardDetail CardDetails { get; set; }
    public required string Pin { get; set; }
    
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();

    private Dictionary<int,string> CURRENCY = new Dictionary<int, string>() {{1, "GEL"}, {2, "USD"}, {3, "EUR"}};

    private Dictionary<string, decimal> EXCHANGE_RATES = new Dictionary<string, decimal>()
    {
        {"GEL", 1},
        {"USD", 0.38m},
        {"EUR", 0.33m}
    };

    // 1
    public Transaction? checkDeposit()
    {
        addTransaction(createTransaction("Check Deposit"));
        return getLatestTransaction();
    }
    // 2
    public (string, bool) getAmount(int currency, decimal amount)
    {
        var message = "";
        var status = false;

        if (amount <= 0)
        {
            message = "Please Enter Positive Number";
            return (message, status);
        }

        if (!CURRENCY.TryGetValue(currency, out var cur))
        {
            message = "Please Enter Correct Currency";
            return (message, status);
        }

        var latestTransaction = getLatestTransaction();
        if (latestTransaction is null)
        {
            message = "No Latest Transaction Found!";
            status = false;
            return (message, status);
        }

        var newTransaction = createTransaction("Get Amount");
        
        message = "Insufficient Funds";
        
        switch (cur)
        {
            case "GEL":
                if (latestTransaction.AmountGEL < amount)
                {
                    return (message, status);
                }
                
                newTransaction.AmountGEL -= amount;

                break;
            case "USD":
                if (latestTransaction.AmountUSD < amount)
                {
                    return (message, status);
                }
                newTransaction.AmountUSD -= amount;
                
                break;
            case "EUR":
                if (latestTransaction.AmountEUR < amount)
                {
                    return (message, status);
                }
                newTransaction.AmountEUR -= amount;
                break;
        }
        addTransaction(newTransaction);
        status = true;
        message = $"You've Withdrawed {amount}{cur} | Remaining: GEL: {newTransaction.AmountGEL} | USD: {newTransaction.AmountUSD} | EUR: {newTransaction.AmountEUR}";
        return (message, status);
    }

    // 3
    public List<Transaction>? getLastFiveTransactions()
    {
        sortTransactions();
        return Transactions;
    }
    
    // 4

    public (string, bool) addAmount(int currency, decimal amount)
    {
        var status = false;
        var message = "";
        
        if (amount <= 0)
        {
            message = "Please Enter Positive Number";
            return (message, status);
        }

        if (!CURRENCY.TryGetValue(currency, out var cur))
        {
            message = "Please Enter Correct Currency";
            return (message, status);
        }
        
        var newTransaction = createTransaction("Get Amount");

        switch (cur)
        {
            case "GEL":
                newTransaction.AmountGEL += amount;
                break;
            case "EUR":
                newTransaction.AmountEUR += amount;
                break;
            case "USD":
                newTransaction.AmountUSD += amount;
                break;
        }
        
        addTransaction(newTransaction);
        status = true;
        message = $"You've Deposited {amount}{cur} | Current Balance: GEL: {newTransaction.AmountGEL} | USD: {newTransaction.AmountUSD} | EUR: {newTransaction.AmountEUR}";
        return (message, status);


    }

    // 5
    public bool changePin(string oldPin, string newPin)
    {
        var status = false;
        if (checkPin(oldPin))
        {
            Pin = newPin;
            addTransaction(createTransaction("Change PIN Code"));
            status = true;
        }

        return status;
    }
    
    // 6

    public (string, bool) exchangeCurrency(int fromCurrency, int toCurrency, decimal purchase)
    {
        var status = false;
        var message = "Incorrect Currency Index";
        if (!CURRENCY.TryGetValue(fromCurrency, out var fromCur) || !CURRENCY.TryGetValue(toCurrency, out var toCur) )
        {
            return (message, status);
        }

        message = "No Latest Transaction Found!";
        var newTransaction = createTransaction("Exchange Currency");
        var latestTransaction = getLatestTransaction();
        if (latestTransaction is null)
        {
            return (message,status);
        }
        message = "Insufficient Funds";
        
        var converted = purchase / EXCHANGE_RATES[toCur];
        
        switch (fromCur)
        {
            case "GEL":
                if (latestTransaction.AmountGEL < converted)
                {
                    return (message, status);
                }
                switch (toCur)
                {
                    case "USD":
                        newTransaction.AmountUSD += purchase;
                        break;
                    case "EUR":
                        newTransaction.AmountEUR += purchase;
                        break;
                    case "GEL":
                        newTransaction.AmountGEL += purchase;
                        break;
                }
                newTransaction.AmountGEL -= converted;
                break;
            case "USD":
                if (latestTransaction.AmountUSD / EXCHANGE_RATES[fromCur] < converted)
                {
                    return (message, status);
                }
                switch (toCur)
                {
                    case "GEL":
                        newTransaction.AmountGEL += purchase;
                        break;
                    case "EUR":
                        newTransaction.AmountEUR += purchase;
                        break;
                    case "USD":
                        newTransaction.AmountUSD += purchase;
                        break;
                }
                newTransaction.AmountUSD -= converted * EXCHANGE_RATES[fromCur];
                break;
            case "EUR":
                if (latestTransaction.AmountEUR / EXCHANGE_RATES[fromCur] < converted)
                {
                    return (message, status);
                }
                switch (toCur)
                {
                    case "GEL":
                        newTransaction.AmountGEL += purchase;
                        break;
                    case "USD":
                        newTransaction.AmountUSD += purchase;
                        break;
                    case "EUR":
                        newTransaction.AmountEUR += purchase;
                        break;
                }
                newTransaction.AmountEUR -= converted * EXCHANGE_RATES[fromCur];
                break;
        }

        message = $"Successfully Sold {fromCur}{converted} | Bought {toCur}{purchase}";
        addTransaction(newTransaction);
        return (message,status);
    }

    // Helper Functions

    public bool checkPin(string Pin)
    {
        return Pin == this.Pin;
    }
    
    private void addTransaction(Transaction transaction)
    {
        var filtered = Transactions.OrderByDescending(x => x.TransactionDate).ToList();
        if (Transactions.Count >= 5)
        {
            filtered.Remove(filtered.Last());
        }
        filtered.Add(transaction);
        Transactions = filtered.OrderByDescending(x=>x.TransactionDate).ToList();
    }
    

    private Transaction? getLatestTransaction()
    {
        return Transactions.OrderByDescending(x => x.TransactionDate).FirstOrDefault();
    }

    private void sortTransactions()
    {
        Transactions = Transactions.OrderByDescending(x => x.TransactionDate).ToList();
    }

    private Transaction createTransaction(string transactionType)
    {
        
        var newTransaction = new Transaction() {TransactionType = transactionType};
        var latestTransaction = getLatestTransaction();
        if (latestTransaction is not null)
        {
            newTransaction.AmountGEL = latestTransaction.AmountGEL;
            newTransaction.AmountEUR = latestTransaction.AmountEUR;
            newTransaction.AmountUSD = latestTransaction.AmountUSD;
        }

        return newTransaction;
        
    }
    


}

