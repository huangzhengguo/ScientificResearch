using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificResearch.Models
{
    public class LineModel
    {
        public int XAxis { get; set; }
        public bool IsTurnOn { get; set; }
        public List<int> ColorPercent { get; set; }
    }
}
