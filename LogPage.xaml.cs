

namespace HRapp;

public partial class LogPage : ContentPage
{
    //Assigning the directory for the user logs to be saved at
    string Directory = FileSystem.Current.AppDataDirectory;
    string logfilename = "logs.txt";
    string fullPathLogs;


    public LogPage()
    {
        InitializeComponent();
        // Combining the log filepath
        fullPathLogs = Path.Combine(Directory, logfilename);



        if (File.Exists(fullPathLogs)) // if the file exists
        {
            // Open the text file using a stream reader.
            using StreamReader streamReader = new StreamReader(fullPathLogs);
            {
                // Read the stream as a string, and write the string to the editor.
                LogViewer.Text = streamReader.ReadToEnd();
                streamReader.Close(); //close the streamreader
            }
        }
        else //in other cases like when the file doesn't exist
        {
            // Create the file and write an initial message
            using (StreamWriter writer = File.CreateText(fullPathLogs))
            {
                writer.WriteLine("Welcome to view the records boss!");
            }

            // Read the newly created file
            using (StreamReader streamReader = new StreamReader(fullPathLogs))
            {
                // Read the stream as a string, and write the string to the editor.
                LogViewer.Text = streamReader.ReadToEnd();
                streamReader.Close(); //close the streamreader
            }
        }
    }

    //return to the startpage view
    private async void Return_2_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}