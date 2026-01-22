//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2024, all rights reserved.
//
using System.Collections.Generic;

namespace Boagaphish.Analytics
{
    public class SplitInfo
    {
        public int SplitColumn { get; set; }
        public double SplitValue { get; set; }
        public List<int> LessRows { get; set; }
        public List<int> GreaterRows { get; set; }
    }
}
