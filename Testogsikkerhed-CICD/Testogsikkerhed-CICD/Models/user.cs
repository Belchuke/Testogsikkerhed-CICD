using System.ComponentModel.DataAnnotations;

namespace Testogsikkerhed_CICD.Models
{
    public class user
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Password { get; set; }
    }
}
