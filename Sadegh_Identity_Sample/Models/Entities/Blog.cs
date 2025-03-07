using Sadegh_Identity_Sample.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadegh_Identity_Sample.Models.Entities
{
    public class Blog
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
