namespace SatisfactorySaveEditor.Data;
public class AppSettings
{
    public int Id { get; set; }
    public IList<string> LastSaves { get; set; }
    public double WindowWidth { get; set; }
    public double WindowHeight { get; set; }
    public double WindowLeft { get; set; }
    public double WindowTop { get; set; }
    public bool AutoUpdate { get; set; }
    public bool AutoBackup { get; set; }
}
