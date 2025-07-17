using System.Collections.Generic;

namespace Online_Book_Store.Utility
{
    public class SD
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Employer = "Employer";
        public const string Company = "Company";
        public const string Customer = "Customer";
        public const string Admins = "Admins";
        public const string Workers = "Workers";
        public static readonly List<string> AllRoles = new List<string>()
        {
            SuperAdmin,
            Admin,
            Employer,
            Company,
            Customer
        };
    }
}
