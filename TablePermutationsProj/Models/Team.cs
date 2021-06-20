using System;
using System.Collections.Generic;
using System.Text;

namespace TablePermutationsProj.Models
{
    class Team
    {
        public Team(string displayName)
        {
            DisplayName = displayName.PadRight(20);
        }
        public string DisplayName { get; set; }
    }
}
