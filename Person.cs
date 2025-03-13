namespace HRapp
{
    // set up the information that will be collected and saved from the employee
    public class EmployeeInfo
    {

        public string Firstnames { get; set; }
        public string Lastname { get; set; }
        public string Nickname { get; set; }
        public string ID { get; set; }
        public string Address { get; set; }
        public string Postalcode { get; set; }
        public string City { get; set; }
        public DateTime Beginning { get; set; }
        public DateTime End { get; set; }
        public string JobEnd { get; set; }
        public string JobTitle { get; set; }
        public string Unit { get; set; }

    }

    //set up a struct for the user, so the info can be accessed on other pages as well.
    public struct User
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}
