using System.ComponentModel.DataAnnotations;

namespace ComputerStore.MVC.Models
{
    public class StoreBranch
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chi nhánh")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }

        public string PhoneNumber { get; set; }
        public bool IsMainBranch { get; set; }
        public bool IsActive { get; set; } = true;
    }
}