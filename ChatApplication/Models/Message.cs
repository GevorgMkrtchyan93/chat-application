using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ChatApplication.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime When { get; set; }

        public string UserId { get; set; }

        public virtual AppUser Sender { get; set; }

        public Message()
        {
            When = DateTime.UtcNow;
        }
    }
}
