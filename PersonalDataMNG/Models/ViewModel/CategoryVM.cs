using System.ComponentModel.DataAnnotations;

namespace PersonalDataMNG.Models
{
    public class CategoryVM: EntityBase
    {
        public Int64 Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string IPAddress { get; set; }
    }
}
