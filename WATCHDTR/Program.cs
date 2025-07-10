// See https://aka.ms/new-console-template for more information
using WATCHDTR;

Console.WriteLine("Watch DTR");
while(true)
{
    Thread.Sleep(2000);
    FileProcessing fp = new FileProcessing();

    var path = AppDomain.CurrentDomain.BaseDirectory;

    string CSVDirectory = Path.Combine(path, "DTR");
    string BackupPath = Path.Combine(path, "BACKUP");

    Console.WriteLine(fp.CheckFolderCSV(CSVDirectory,BackupPath));
}
