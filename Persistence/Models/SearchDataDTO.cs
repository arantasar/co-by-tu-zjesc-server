using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Models
{
    public class SearchDataDTO
    {
        public List<SearchItem> Ingredients { get; set; }
        public List<SearchItem> Categories { get; set; }
        public List<SearchItem> Diets { get; set; }
        public List<SearchItem> Size { get; set; }
    }
}
