using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public int Id_User { get; set; }

        [Required(ErrorMessage = "Сообщение не может быть пустым")]
        [StringLength(500, ErrorMessage = "Сообщение не может превышать 500 символов")]
        public string MessageText { get; set; } = string.Empty;

        public DateTime MessageDate { get; set; } = DateTime.Now;
    }
}
