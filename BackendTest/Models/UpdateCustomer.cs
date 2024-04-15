using System.ComponentModel.DataAnnotations;

namespace BackendTest.Models
{
    public class UpdateCustomer
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }

    }
}
