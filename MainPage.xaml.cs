using System.Text.Json;

namespace HRapp
{



    public partial class MainPage : ContentPage
    {
        //Assigning the directory for the user logs to be saved in
        string Directory = FileSystem.Current.AppDataDirectory;
        string logfilename = "logs.txt";
        string fullPathLogs;

        public MainPage()
        {
            InitializeComponent();
            // Combining the log filepath
            fullPathLogs = Path.Combine(Directory, logfilename);

        }
        private User user;

        //A method to take user login information to other pages
        public User DataFromMainPage()
        {
            user.UserName = Username.Text;
            user.PassWord = Password.Text;

            return user;

        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            //Checks that the username and password match. Lets the user in if they match and gives a popup alert if they do not match or are missing or empty etc.

            if (Password.Text == Username.Text && string.IsNullOrWhiteSpace(Username.Text) == false && string.IsNullOrWhiteSpace(Password.Text) == false)
            {
                StartPage start = new(this);
                await Navigation.PushAsync(start); //shows a new startpage 

                if (File.Exists(fullPathLogs) == false) //checks if the file for the records exists and if it does not
                {
                    File.Create(fullPathLogs); // creates a new file
                    using (StreamWriter sw = File.AppendText(fullPathLogs)) //opens streamwriter 
                    {
                        sw.WriteLine("{0} {1} has logged in", DateTime.Now, Username.Text); //writes a text about the user logging in
                        sw.Close(); // and closes the streamwriter
                    }
                }
                else //otherwise, if the file exists
                {
                    using(StreamWriter sw = File.AppendText(fullPathLogs)) //opens streamwriter
                {
                        sw.WriteLine("{0} {1} has logged in", DateTime.Now, Username.Text); //writes a text about the user logging in
                        sw.Close(); // and closes the streamwriter
                    }
                }
                
                


            }
            else // is used if username and password don't match or the fields are empty etc.
            {
                await DisplayAlert("Access denied", "Username and/or password entered incorrectly", "OK"); // a popup to let the user know that the login wasn't succesful
            }

        }

        private void QuitButton_Clicked(object sender, EventArgs e)
        {
            //Shuts down the application when the button is pressed
            App.Current.Quit();

        }
    }

}
