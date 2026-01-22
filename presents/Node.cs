//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2024, all rights reserved.
//
using System.Collections.Generic;

namespace Boagaphish.Analytics
{
    public class Node
    {
        public int NodeID { get; set; }
        public List<int> Rows { get; set; }  // which ref data rows
        public int SplitColumn { get; set; }
        public double SplitValue { get; set; }
        public int ClassCounts { get; set; }
        public int PredictedClass { get; set; }
    }
}
