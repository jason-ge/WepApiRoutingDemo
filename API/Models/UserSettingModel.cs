using System.ComponentModel.DataAnnotations;

namespace WebApiRoutingDemo.API.Models
{
    public class UserSettingModel
    {
        public UserSettingModel()
        {
        }

        [Key]
        public int UserSettingId { get; set; }

        [Required]
        public string SettingKey { get; set; }

        [Required]
        public string SettingValue { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
