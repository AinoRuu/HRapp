using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace HRapp;

public partial class PersonelPage : ContentPage
{
    //Assigning the directory for the employee info to be saved at
    string Dir = FileSystem.Current.AppDataDirectory;
    string textfilename = "employeeinfo.json";
    string fullPath;

    //Assigning the directory for the user logs to be saved at
    string Directory = FileSystem.Current.AppDataDirectory;
    string logfilename = "logs.txt";
    string fullPathLogs;

    //Initializing the observable collection to store the employment details into
    public ObservableCollection<EmployeeInfo> Employee { get; set; }

    //Initializing the list to store the Postalcode and city details into

    public Dictionary<string, string> PostalLookUp = new Dictionary<string, string>();
    public PersonelPage()
    {
        InitializeComponent();
        // Combining the employment and log filepaths
        fullPath = Path.Combine(Dir, textfilename);
        fullPathLogs = Path.Combine(Directory, logfilename);
        //Setting the name of the observable collection that will hold /holds the mployment details
        Employee = new ObservableCollection<EmployeeInfo>();
        BindingContext = this;


        //Checking if the file exists and isn't empty

        if (File.Exists(fullPath))
        {
            string jsonData = File.ReadAllText(fullPath);

            if (!string.IsNullOrEmpty(jsonData))
            {
                //Deserializing, decrypting and setting the items from the file onto the Employee observable collection

                ObservableCollection<char> decryptedFromJson = LoadFromJsonFile(fullPath);
                if (decryptedFromJson.Count > 0)
                {
                    string decryptedText = Decrypt(decryptedFromJson);
                    var employees = JsonSerializer.Deserialize<ObservableCollection<EmployeeInfo>>(decryptedText);

                    foreach (var employee in employees)
                    {
                        Employee.Add(employee);
                    }
                }
            }
            else
            {
                Employee = new ObservableCollection<EmployeeInfo>(); // Initialize an empty collection
            }
        }
        else // If the file doesn't exist
        {
            Employee = new ObservableCollection<EmployeeInfo>(); // Initialize an empty collection
        }

    }

    //resets/empties all of the data from the form
    public void Empty_Form()
    {
        FirstNameEntry.Text = "";
        NickNameEntry.Text = "";
        LastNameEntry.Text = "";
        IDEntry.Text = "";
        AddressEntry.Text = "";
        PostalCodeEntry.Text = "";
        CityEntry.Text = "";


        JobStart.Date = DateTime.Now;
        JobEnd.Date = DateTime.Now;
        JobEndEntry.Text = "Ended on:";
        JobTitleEntry.Text = "";
        UnitEntry.Text = "";
    }

    //Checks the form for inconsistencies: 
    public bool Check_Form()
    {


        //makes sure none of the entries are empty
        if (string.IsNullOrWhiteSpace(FirstNameEntry.Text))
        {
            throw new Exception(message: "Firstnames were entered incorrectly");
        }


        if (string.IsNullOrWhiteSpace(NickNameEntry.Text))
        {
            throw new Exception(message: "Lastname was entered incorrectly");
        }


         if (string.IsNullOrWhiteSpace(LastNameEntry.Text))
        {
            throw new Exception(message: "Nickname was entered incorrectly");
        }

         if (string.IsNullOrWhiteSpace(IDEntry.Text))
        {
            throw new Exception("Identity number was entered incorrectly");
        }

        //Checks that the Finnish indentity number is in the correct form
        string Sid = IDEntry.Text;

        Sid = Sid.Remove(6, 1);
        string result = Sid.Substring(0, 9);

        string Check = "0123456789ABCDEFHJKLMNPRSTUVWXY";

        int fJJ = (int.Parse(result)) % 31;

        

        if (Sid[9] != Check[fJJ])
        {
            for (int i = 0; i < Check.Length; i++)
            {
                if (fJJ < Check[i] || fJJ > Check[i])
                {
                    throw new Exception("Identity number was entered incorrectly");
                }
            }
        }

        //makes sure none of the entries are empty
        if (string.IsNullOrWhiteSpace(AddressEntry.Text))
        {
            throw new Exception(message: "Address was entered incorrectly");
        }

        //makes sure the postal code consists of numbers only
        if (!int.TryParse(PostalCodeEntry.Text, out int postcode))
    {
        throw new Exception("Postal code was not entered correctly");
    }

        //makes sure none of the entries are empty
        if (string.IsNullOrWhiteSpace(CityEntry.Text))
        {
            throw new Exception(message: "City was entered incorrectly");
        }


        if (string.IsNullOrWhiteSpace(JobTitleEntry.Text))
        {
            throw new Exception(message: "Jobtitle was entered incorrectly");

        }


        if (string.IsNullOrWhiteSpace(UnitEntry.Text))
        {
            throw new Exception(message: "Unit was entered incorrectly");
        }

        //checks that the dates make sense
        if (JobStart.Date > JobEnd.Date ) 
        {
            throw new Exception(message: "The beginning date was more current than the ending date");
        }

        //returns a boolean value of true if all of the earlier tests were passed
        return true;
    }


    private void Reset_Clicked(object sender, EventArgs e)
    {
        //resets/empties all of the data from the form
        Empty_Form();

    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        //makes sure "reset the form" and "JobEnd.DatePicker" -buttons are usable after being disabled
        Reset.IsEnabled = true;
        JobEnd.IsEnabled = true;

        //assigns the information to Employee struct
        EmployeeInfo person = new EmployeeInfo();
        person.Firstnames = FirstNameEntry.Text;
        person.Nickname = NickNameEntry.Text;
        person.Lastname = LastNameEntry.Text;
        person.ID = IDEntry.Text;
        person.Address = AddressEntry.Text;
        person.Postalcode = PostalCodeEntry.Text;
        person.City = CityEntry.Text;
        person.Beginning = JobStart.Date;
        person.End = JobEnd.Date;
        person.JobEnd = JobEndEntry.Text;
        person.JobTitle = JobTitleEntry.Text;
        person.Unit = UnitEntry.Text;

        
        try
        {
            //Checks if the information is of a new employee or an already saved one being edited

            string searchString = IDEntry.Text;

            bool containsString = false;

            foreach (EmployeeInfo item in Employee)
            {
                if (item.ID == searchString)
                {
                    //if the same ID is already in the collection, changes the containsString value to true to let the next if-statement know that the person is already in the collection 
                    containsString = true;
                    //removes the original employee data from the collection and listview to make room for the edited data
                    Employee.Remove(item);
                    break;
                }
            }

            //takes into consideration the containsString value that it gets from the former step and acts accordingly
            if (containsString == true) //if the same identity number was already in the collection..
            {
                if (File.Exists(fullPathLogs) == false)     //checks whether the file that stores the user logs doesn't exist...
                {

                    File.Create(fullPathLogs);      //and creates it if it does not exist, ...
                    using (StreamWriter sw = File.AppendText(fullPathLogs)) //opens the streamwriter, ...
                    { 
                        sw.WriteLine("{0} employee {1} edited", DateTime.Now, person.Lastname); // writes a log about an employee info being edited...
                        sw.Close(); // and closes the streamwriter
                    }
                    
                }
                else 
                {
                    using (StreamWriter sw = File.AppendText(fullPathLogs)) //opens the streamwriter, ...
                    {
                        sw.WriteLine("{0} employee {1} edited", DateTime.Now, person.Lastname); // writes a log about an employee info being edited...
                        sw.Close(); // and closes the streamwriter
                    }
                    
                }
                
            }
            else //is used if the collection doesn't have the identity number already in it or the if- statement can't be excecuted for some other reason
            {

                if (File.Exists(fullPath)) //checks if the file that stores the user logs exists...
                {
                    using (StreamWriter sw = File.AppendText(fullPathLogs)) //opens the streamwriter, ...
                    {
                        sw.WriteLine("{0} new employee {1} saved", DateTime.Now, person.Lastname); // writes a log about a new employee info being saved...
                        sw.Close(); // and closes the streamwriter
                    }
                }
                else
                {
                    File.Create(fullPathLogs);      //creates the file if it does not exist, ...
                    using (StreamWriter sw = File.AppendText(fullPathLogs)) //opens the streamwriter, ...
                    {
                        sw.WriteLine("{0} new employee {1} saved", DateTime.Now, person.Lastname); // writes a log about a new employee info being saved...
                        sw.Close(); // and closes the streamwriter
                    }
                }

            }

            //Calls the Check_Form method to check the input information and to get the 
            Check_Form();
            //Adds the info from the form to the collection
            Employee.Add(person);
            //Empties the form and makes it ready for new data
            Empty_Form();

            //serializes, encrypts and saves/writes the info assigned to employee collection into a file
            var serializedEmployees = JsonSerializer.Serialize(Employee);

            ObservableCollection<char> encryptedEmployees = Encrypt(serializedEmployees);

            SaveToJsonFile(encryptedEmployees, fullPath);

            string jsonData = File.ReadAllText(fullPath);

            //Check that the file is not empty
            if (!string.IsNullOrEmpty(jsonData))
            {
                //Deserializing, decrypting and setting the items from the file onto the Employee observable collection
                ObservableCollection<char> decryptedFromJson = LoadFromJsonFile(fullPath);
                if (decryptedFromJson.Count > 0)
                {
                    string decryptedText = Decrypt(decryptedFromJson);
                    var employees = JsonSerializer.Deserialize<ObservableCollection<EmployeeInfo>>(decryptedText);
                }
                else
                {
                    Employee = new ObservableCollection<EmployeeInfo>(); // Initialize an empty collection
                }
            }
            else // If the file is empty
            {
                Employee = new ObservableCollection<EmployeeInfo>(); // Initialize an empty collection
            }

                    Continuous.IsChecked = false;
        }
        catch (Exception ex) //catches all of the exceptions from the try part of the statement and displays the exceptions for the user
        {
            await DisplayAlert("Alert!", ex.Message, "OK");
        }
    }

    //is initialized when an item from the ListView PersonInfo is selected
    private async void PersonInfo_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        //gives different options on what to do to the selected item: delete or edit
        string result = await DisplayActionSheet("Do you want to edit or delete?", "Cancel", null, "Edit", "Delete");
        EmployeeInfo person = e.SelectedItem as EmployeeInfo;

        if (result == "Edit") //the user selects to edit the employee's information
        {
            // this makes sure "reset the form" -button can't be used, so no information is deleted by accident
            Reset.IsEnabled = false;

            // selected employee's information is assigned to the corresponding entries for editing
            FirstNameEntry.Text = person.Firstnames;
            NickNameEntry.Text = person.Nickname;
            LastNameEntry.Text = person.Lastname;
            IDEntry.Text = person.ID;
            AddressEntry.Text = person.Address;
            PostalCodeEntry.Text = person.Postalcode;
            CityEntry.Text = person.City;
            JobStart.Date = person.Beginning;
            JobEnd.Date = person.End;
            JobEndEntry.Text = person.JobEnd;
            JobTitleEntry.Text = person.JobTitle;
            UnitEntry.Text = person.Unit;


        }
        else if (result == "Delete") //the user selects to delete the employee's information
        {


            string answer = await DisplayActionSheet("Are you sure?", "Yes", "No"); //makes sure the user really wanted to delete the info and that it wasn't a missclick
            if (answer == "Yes") //the user wants to delete the data
            {

                if (File.Exists(fullPathLogs) == false) //checks if the file that stores the user logs does not exist...
                {
                    File.Create(fullPathLogs);  //creates the file if it does not exist, ...
                    using (StreamWriter sw = File.AppendText(fullPathLogs)) //opens the streamwriter, ...
                    {
                        sw.WriteLine("{0} employee {1} deleted", DateTime.Now, person.Lastname); // writes a log about an employee info being deleted...
                        sw.Close(); // and closes the streamwriter
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(fullPathLogs)) //opens the streamwriter, ...
                    {
                        sw.WriteLine("{0} employee {1} deleted", DateTime.Now, person.Lastname); // writes a log about an employee info being deleted...
                        sw.Close(); // and closes the streamwriter
                    }
                }

                // removes the employee's info from the collection
                Employee.Remove(person);

                //serializes, encrypts and writes the info from the collection into the .json file
                var serializedEmployees = JsonSerializer.Serialize(Employee);

                ObservableCollection<char> encryptedEmployees = Encrypt(serializedEmployees);

                SaveToJsonFile(encryptedEmployees, fullPath);
            }

        }
    }

    //when "return to the menu" -button is pressed, the program navigates to the "StartPage"
    private async void Return_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

   

    //Method for matching the postal code to a city and suggesting it to the user as the program is being used. Needs saved information so it won't work on the first time city and postalcode are being inputed
    private void PostalCodeEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        foreach (EmployeeInfo person in Employee)
        {
            if (PostalCodeEntry.Text.StartsWith(person.Postalcode.Substring(0,2), StringComparison.OrdinalIgnoreCase))
            {
                CityEntry.Text = person.City;
            }
        }
    }

    //By clicking the Continuous Checkbox the user can indicate that the employmentship is ongoing at the time of saving the info
    private void Continuous_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (Continuous.IsChecked == true) // while the checkbox is checked..
        {
            JobEnd.IsEnabled = false; // disables the JobEnd datepicker
            JobEnd.Date = DateTime.Today; // sets the date to the current date
            JobEndEntry.Text = "Ongoing as of:"; // adds a line about the employment being currently active
        }
        else   // while the checkbox is not checked..
        {
            JobEnd.IsEnabled = true; //enables the JobEnd datepicker
            JobEnd.Date = DateTime.Today; // sets the date to the current date by default (can be edited by the user)
            JobEndEntry.Text = "Ended on:"; // adds a line about the employment having been ended
        }  
    }
    
    // Sort the ListView items by Nickname in descending order
    private void NickA_Clicked(object sender, EventArgs e)
    {
        var sortedList = PersonInfo.ItemsSource.Cast<EmployeeInfo>().OrderBy(e => e.Nickname).ToList();
        PersonInfo.ItemsSource = sortedList;
        
    }


    // Sort the ListView items by Nickname in ascending order
    private void NickZ_Clicked(object sender, EventArgs e)
    {
        var sortedList = PersonInfo.ItemsSource.Cast<EmployeeInfo>().OrderByDescending(e => e.Nickname).ToList();
        PersonInfo.ItemsSource = sortedList;
    }

    

    // Sort the ListView items by Lastname in descending order
    private void LastA_Clicked(object sender, EventArgs e)
    {
        var List = PersonInfo.ItemsSource.Cast<EmployeeInfo>().OrderBy(e => e.Lastname).ToList();
        PersonInfo.ItemsSource = List;
    }

    // Sort the ListView items by Lastname in ascending order
    private void LastB_Clicked(object sender, EventArgs e)
    {
        
        var List = PersonInfo.ItemsSource.Cast<EmployeeInfo>().OrderByDescending(e => e.Lastname).ToList();
        PersonInfo.ItemsSource = List;
    }
 
    // Sort the ListView items by Jobtitle in descending order
    private void TitleA_Clicked(object sender, EventArgs e)
    {
        var List = PersonInfo.ItemsSource.Cast<EmployeeInfo>().OrderBy(e => e.JobTitle).ToList();
        PersonInfo.ItemsSource = List;
    }

    // Sort the ListView items by Jobtitle in ascending order
    private void TitZ_Clicked(object sender, EventArgs e)
    {

        var List = PersonInfo.ItemsSource.Cast<EmployeeInfo>().OrderByDescending(e => e.JobTitle).ToList();
        PersonInfo.ItemsSource = List;
    }


    //A method for encrypting the data
    static ObservableCollection<char> Encrypt(string text)
    {
        ObservableCollection<char> encryptedText = new ObservableCollection<char>();

        foreach (char c in text)
        {
            char encryptedChar = c;
            if (char.IsLetterOrDigit(c))
            {
                encryptedChar = (char)(c + 1);
            }
            encryptedText.Add(encryptedChar);
        }

        return encryptedText;
    }

    // a method for decrypting data
    static string Decrypt(ObservableCollection<char> encryptedText)
    {
        StringBuilder decryptedText = new StringBuilder();

        foreach (char c in encryptedText)
        {
            char decryptedChar = c;
            if (char.IsLetterOrDigit(c))
            {
                decryptedChar = (char)(c - 1);
            }
            decryptedText.Append(decryptedChar);
        }

        return decryptedText.ToString();
    }
    static void SaveToJsonFile(ObservableCollection<char> data, string fullPath)
    {
        // Serialize the ObservableCollection to JSON
        string jsonData = JsonSerializer.Serialize(data);

        // Write the JSON data to a file
        File.WriteAllText(fullPath, jsonData);
    }

    // loads needed information from the json file
    static ObservableCollection<char> LoadFromJsonFile(string fullPath)
    {
        if (File.Exists(fullPath) && !string.IsNullOrEmpty(fullPath))
        {
            string jsonData = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<ObservableCollection<char>>(jsonData);
        }
        else
        {
            throw new FileNotFoundException("File not found", fullPath);
        }
    }

}

