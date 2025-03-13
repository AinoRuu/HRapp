
namespace HRapp;

public partial class StartPage : ContentPage
{
    //Assigning the directory for the user logs to be saved at
    string Directory = FileSystem.Current.AppDataDirectory;
    string textfilename = "logs.txt";
    string fullPath;


    private readonly MainPage _mainPage;

    public StartPage(MainPage mainPage)
    {
        InitializeComponent();
        _mainPage = mainPage;
        // Combining the log filepath
        fullPath = Path.Combine(Directory, textfilename);


    }

    //when the new button is clicked, opens a new personelpage
    private async void New_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PersonelPage());
    }

    //when the log button is clicked
    private async void Log_Clicked(object sender, EventArgs e)
    {

        User user = _mainPage.DataFromMainPage(); //, retrieves userdata from the login/mainpage

        if (user.UserName == "admin12") // checks that the loggedin user is indeed the admin12 
        {
            await Navigation.PushAsync(new LogPage()); //and in that case lets them into the logpage to view the records
        }
        else // if the user is not admin12
        {
            await DisplayAlert("Access denied", "You do not have access, login as the main user to see the records", "OK"); //denies the access and gives a popup about it 
        }
    }


    //when logout button is clicked
    private async void Logout_Clicked(object sender, EventArgs e)
    {
        MainPage main = new MainPage();
        await Navigation.PushAsync(main); //takes the user to a new login/mainpage


        if (File.Exists(fullPath) == false) //if the logfile does not exist
        {
            File.Create(fullPath); //creates the file
            using (StreamWriter sw = File.AppendText(fullPath)) //opens streamwriter 
            {
                sw.WriteLine("{0} the user logged out", DateTime.Now); //writes the logtext into the file
                sw.Close(); //and closes streamwriter
            }
        }
        else//if the logfile exists
        {
            using (StreamWriter sw = File.AppendText(fullPath))//opens streamwriter 
            {
                sw.WriteLine("{0} the user logged out", DateTime.Now); //writes the logtext into the file
                sw.Close(); //and closes streamwriter
            }
        }

    }


}