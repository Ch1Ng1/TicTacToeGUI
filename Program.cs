namespace TicTacToeGUI;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try
        {
            Application.Run(new Form1());
        }
        catch (Exception ex)
        {
            // Write exception to console for debugging (also saves to a simple file)
            try { Console.Error.WriteLine(ex.ToString()); } catch { }
            try { System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppContext.BaseDirectory, "error.log"), ex.ToString()); } catch { }
            throw;
        }
    }
}
