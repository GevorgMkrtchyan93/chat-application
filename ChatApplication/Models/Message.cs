using System;
<<<<<<< HEAD
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
=======
>>>>>>> 6bc8348db9b4805d2e387f7e5b56dde68ac1cd7a

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
