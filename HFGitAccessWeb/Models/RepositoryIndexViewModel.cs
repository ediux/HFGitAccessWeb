using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HFGitAccessWeb.Models
{
    public class RepositoryIndexViewModel
    {
        
        public string Id { get; set; }
        [DisplayName("Repository")]
        public string Name { get; set; }
        [DisplayName("Access URL")]
        public string URL { get; set; }
    }
}