using System.Text;
using System.Text.Json;

namespace Final_Exam_1;

class Program
{
    static void Main(string[] args)
    {

        Logger logger = new Logger()
            {FilePath = "./logs.log"};

        var accounts = getBankAccounts(logger,"./BankAccounts.json");

        /*var accounts = new List<BankAccount>();
        var account1 = new BankAccount()
        {
            FirstName = "Luka", LastName = "Varsimashvili",
            CardDetails = new CardDetail(){CardNumber = "123456789101112", CardValidDate = "01/28", CVC = "000",},
            Pin = "1234"
        };
        var account2 = new BankAccount()
        {
            FirstName = "Saba", LastName = "Machavariani",
            CardDetails = new CardDetail(){CardNumber = "378936521489", CardValidDate = "05/29", CVC = "383",},
            Pin = "5678"
        };

        accounts.Add(account1);
        accounts.Add(account2);*/
        startApp(accounts, logger);

    }
    

    static void startApp(List<BankAccount>? bankAccounts, Logger logger)
    {
        logger.Log("App Started");
        Console.WriteLine("Welcome To ATM, Please Log In To Your Account! ");
        
        var (loggedIn, myAccount) = login(bankAccounts, logger);
        logger.Log($"Successful LogIn (CardNumber: {myAccount.CardDetails.CardNumber}, First Name: {myAccount.FirstName}, Last Name: {myAccount.LastName})");
        Console.WriteLine($"Welcome {myAccount.FirstName} {myAccount.LastName}");
        while (loggedIn)
        {
            //1. Check Deposit
            //2. Get Amount
            //3. Get Last 5 Transactions
            //4. Add Amount
            //5. Change PIN
            //6. Change Currency
            //0. Exit Application
            
            Console.WriteLine("Please Choose Operation (1,2,3,4,5,6)\n1. Check Deposit\n2. Get Amount\n3. Get Last 5 Transactions\n4. Add Amount\n5. Change PIN\n6. Change Currency\n0. Exit Application");
            if (int.TryParse(Console.ReadLine(), out var operation))
            {
                switch (operation)
                {
                    case 0:
                        loggedIn = false;
                        logger.Log("Exiting Application");
                        break;
                    case 1:
                        var latestTransaction = myAccount.checkDeposit();
                        if (latestTransaction is not null)
                        {
                            logger.Log($"Checked Deposit For ({myAccount.FirstName} {myAccount.LastName})");
                            Console.WriteLine($"Current Amount Is:\nGEL: {latestTransaction.AmountGEL}\nEUR: {latestTransaction.AmountEUR}\nUSD: {latestTransaction.AmountUSD}");
                        }
                        else
                        {
                            Console.WriteLine("No Transactions Found!");
                            logger.Log($"Checked Deposit For ({myAccount.FirstName} {myAccount.LastName}) No Transactions Found!");
                        }

                        break;
                    case 2:
                        logger.Log($"Trying To Withdraw Money ({myAccount.FirstName} {myAccount.LastName})");
                        Console.WriteLine("Please Enter Currency (1,2,3): \n1. GEL\n2. USD\n3. EUR");
                        if (int.TryParse(Console.ReadLine(), out var currency) && currency >= 1 && currency <= 3)
                        {
                            Console.WriteLine("Please Type Amount To Withdraw: ");
                            if (decimal.TryParse(Console.ReadLine(), out var amount))
                            {
                                var (message, status) = myAccount.getAmount(currency, amount);
                                if (!status)
                                {
                                    logger.Log($"Trying To Withdraw Money | {message} ({myAccount.FirstName} {myAccount.LastName})");
                                    Console.WriteLine($"Couldn't Withdraw Money | {message}");
                                }
                                else
                                {
                                    logger.Log($"Money Withdrawed {message}");
                                    Console.WriteLine($"{message}");
                                }
                            }
                            else
                            {
                                logger.Log($"Trying To Withdraw Money | Invalid Amount Input ({myAccount.FirstName} {myAccount.LastName})");
                                Console.WriteLine("Invalid Amount Input");
                            }
                        }
                        else
                        {
                            logger.Log($"Trying To Withdraw Money | Invalid Currency Input ({myAccount.FirstName} {myAccount.LastName})");
                            Console.WriteLine("Invalid Currency Input");
                        }

                        break;
                    case 3:
                        Console.WriteLine("Your Last 5 Operation: ");
                        var transactions = myAccount.getLastFiveTransactions();
                        foreach (var transaction in transactions)
                        {
                            Console.WriteLine($"{transaction.TransactionDate} | {transaction.TransactionType}\n----------------------\nGEL: {transaction.AmountGEL}\nUSD: {transaction.AmountUSD}\nEUR: {transaction.AmountEUR}\n----------------------\n");
                        }
                        
                        break;
                    case 4:
                        logger.Log($"Trying To Deposit Money ({myAccount.FirstName} {myAccount.LastName})");
                        Console.WriteLine("Please Enter Currency (1,2,3): \n1. GEL\n2. USD\n3. EUR");
                        if (int.TryParse(Console.ReadLine(), out currency) && currency >= 1 && currency <= 3)
                        {
                            Console.WriteLine("Please Type Amount To Deposit: ");
                            if (decimal.TryParse(Console.ReadLine(), out var amount))
                            {
                                var (message, status) = myAccount.addAmount(currency, amount);
                                if (!status)
                                {
                                    logger.Log($"Trying To Deposit Money | {message} ({myAccount.FirstName} {myAccount.LastName})");
                                    Console.WriteLine($"Couldn't Deposit Money | {message}");
                                }
                                else
                                {
                                    logger.Log($"Money Deposited {message}");
                                    Console.WriteLine($"{message}");
                                }
                            }
                            else
                            {
                                logger.Log($"Trying To Deposit Money | Invalid Amount Input ({myAccount.FirstName} {myAccount.LastName})");
                                Console.WriteLine("Invalid Amount Input");
                            }
                        }
                        else
                        {
                            logger.Log($"Trying To Deposit Money | Invalid Currency Input ({myAccount.FirstName} {myAccount.LastName})");
                            Console.WriteLine("Invalid Currency Input");
                        }
                        break;
                    case 5:
                        Console.WriteLine("To Change PIN Code, Please Provide Old PIN: ");
                        var oldPin = Console.ReadLine();
                        Console.WriteLine("Please Provide New PIN Code: ");
                        var newPin = Console.ReadLine();
                        if (myAccount.changePin(oldPin, newPin))
                        {
                            logger.Log($"PIN Code Changed Successfully For (CardNumber: {myAccount.CardDetails.CardNumber}, First Name: {myAccount.FirstName}, Last Name: {myAccount.LastName})");
                            Console.WriteLine("PIN Code Changed Successfully");
                        }
                        else
                        {
                            logger.Log($"Unsuccessful PIN Code Change Attempt For (CardNumber: {myAccount.CardDetails.CardNumber}, First Name: {myAccount.FirstName}, Last Name: {myAccount.LastName})");
                            Console.WriteLine("Incorrect Old PIN");
                        }
                        break;
                    case 6:
                        Console.WriteLine("Please Enter Currency You Want To Sell (1,2,3): \n1. GEL\n2. USD\n3. EUR");
                        var input1 = Console.ReadLine();
                        Console.WriteLine("Please Enter Currency You Want To Purchase (1,2,3): \n1. GEL\n2. USD\n3. EUR");
                        var input2 = Console.ReadLine();
                        if (int.TryParse(input1, out var currencyFrom) && currencyFrom >= 1 && currencyFrom <= 3 && int.TryParse(input2,out var currencyTo) && currencyTo >= 1 && currencyTo<=3)
                        {
                            Console.WriteLine("Please Type Amount To Purchase");
                            if (decimal.TryParse(Console.ReadLine(), out var purchase))
                            {
                                var (message, status) = myAccount.exchangeCurrency(currencyFrom,currencyTo, purchase);
                                if (!status)
                                {
                                    logger.Log($"Trying To Exchange Money | {message} ({myAccount.FirstName} {myAccount.LastName})");
                                    Console.WriteLine($"Couldn't Exchange Money | {message}");
                                }
                                else
                                {
                                    logger.Log($"Money Exchanged {message}");
                                    Console.WriteLine($"{message}");
                                }
                            }
                            else
                            {
                                logger.Log($"Trying To Exchange Money | Invalid Amount Input ({myAccount.FirstName} {myAccount.LastName})");
                                Console.WriteLine("Invalid Amount Input");
                            }
                        }
                        break;
                }
                saveJson(logger, bankAccounts);
            }
            else
            {
                Console.WriteLine("Please Type Correct Operation");
            }
        }

        Console.WriteLine("Thanks For Using This ATM, Hope To See You Soon, Goodbye!");
        
    }

    static (bool loggedIn, BankAccount myAccount) login(List<BankAccount>? bankAccounts, Logger logger)
    {
        var loggedIn = false;
        BankAccount myAccount = new BankAccount(){FirstName = "",LastName = "",
            CardDetails = new CardDetail(){CardNumber = "",CardValidDate = "",CVC = ""}, 
            Pin = ""};
        
        while (!loggedIn)
        {
            Console.WriteLine("Please Enter Card Number (1234567891011121): ");
            var cardNumber = Console.ReadLine();
            Console.WriteLine("Please Enter Card Valid Date (07/28): ");
            var cardValidDate = Console.ReadLine();
            Console.WriteLine("Please Enter Card CVC (000): ");
            var CVC = Console.ReadLine();

            if (bankAccounts is null)
            {
                Console.WriteLine("Invalid Bank Details");
                continue;
            }

            var filtered = bankAccounts.Where(x => x.CardDetails.ValidateCardNumber(cardNumber) &&
                                                   x.CardDetails.ValidateCardValidDate(cardValidDate) &&
                                                   x.CardDetails.ValidateCVC(CVC)).ToList();
            
            
            if (filtered.Count == 1)
            {
                Console.WriteLine("To Access Your Account Please Provide Your Pin: ");
                var pin = Console.ReadLine();

                if (filtered[0].checkPin(pin))
                {
                    myAccount = filtered[0];
                    loggedIn = true;
                    
                }
                else
                {
                    logger.Log($"Invalid PIN Attempt (CardNumber: {cardNumber}, CardValidDate: {cardValidDate}, CVC: {CVC})");
                    Console.WriteLine("Invalid Pin, Please Try Again!");
                }


            }
            else
            {
                logger.Log($"Unsuccessful LogIn Entry (CardNumber: {cardNumber}, CardValidDate: {cardValidDate}, CVC: {CVC})");
                Console.WriteLine("Invalid Bank Details");
            }

        }
        
        return (loggedIn, myAccount);
        
    }

    static void saveJson(Logger logger,  List<BankAccount> bankAccounts, string filePath = "./BankAccounts.json")
    {
        logger.Log($"Saving JSON File ({filePath})");
        
        var options = new JsonSerializerOptions();
        options.WriteIndented = true;
        
        var json = JsonSerializer.Serialize<List<BankAccount>>(bankAccounts, options);
        File.WriteAllText(filePath,json);
        
    }
    static List<BankAccount>? getBankAccounts(Logger logger, string filePath = "./BankAccounts.json")
    {
        if (!File.Exists(filePath))
        {
            logger.Log($"File Does Not Exist, Creating File  {filePath}");
            File.Create(filePath).Close();
        }

        using (StreamReader r = new StreamReader(filePath))
        {
            
            logger.Log($"Reading JSON File ({filePath})");
            
            List<BankAccount> accounts = null;
            var json = r.ReadToEnd();
            try
            {
                accounts = JsonSerializer.Deserialize<List<BankAccount>>(json);
                logger.Log($"JSON Successfully Deserialized ({filePath})");
            }
            catch
            {
                logger.Log($"JSON Deserialization Failed, No Data Found! ({filePath})");
                
            }
            return accounts;
            
        }
        
        
    }


}