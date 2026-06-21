namespace Final_Exam_1;

public class Logger
{
    public string FilePath { get; set; } = "./logs.log";
    
    
    public void Log(string Message)
    {
        File.AppendAllText(FilePath,$"{DateTime.Now} - {Message}\n");
    }
}