using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApplication.ViewModels
{
    public class RedisCacheDataModel
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime When { get; set; }
    }
}
