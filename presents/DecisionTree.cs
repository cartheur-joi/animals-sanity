//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2024, all rights reserved.
//
using System;
using System.Collections.Generic;

namespace Boagaphish.Analytics
{
    public class DecisionTree
    {
        public int NumberOfNodes { get; set; }
        public int NumberOfClasses { get; set; }
        public List<Node> Tree { get; set; }

        public DecisionTree(int numNodes, int numClasses)
        {
            NumberOfNodes = numNodes;
            NumberOfClasses = numClasses;
            Tree = new List<Node>();
            for (int i = 0; i < numNodes; ++i)
                Tree.Add(new Node());
        }

        public void BuildTree(double[][] dataX, int[] dataY)
        {
            // prep the list and the root node
            int n = dataX.Length;

            List<int> allRows = new List<int>();
            for (int i = 0; i < n; ++i)
                allRows.Add(i);

            Tree[0].Rows = new List<int>(allRows);

            for (int i = 0; i < this.NumberOfNodes; ++i)  // finish root, and do remaining
            {
                Tree[i].NodeID = i;

                SplitInfo si = GetSplitInfo(dataX, dataY, Tree[i].Rows, NumberOfClasses);  // get the split info
                Tree[i].SplitColumn = si.SplitColumn;
                Tree[i].SplitValue = si.SplitValue;

                Tree[i].ClassCounts = ComputeClassCounts(dataY, Tree[i].Rows, NumberOfClasses);
                Tree[i].PredictedClass = ArgMax(Tree[i].ClassCounts);

                int leftChild = (2 * i) + 1;
                int rightChild = (2 * i) + 2;

                if (leftChild < NumberOfNodes)
                    Tree[leftChild].Rows = new List<int>(si.LessRows);
                if (rightChild < NumberOfNodes)
                    Tree[rightChild].Rows = new List<int>(si.GreaterRows);
            } // i

        } 

        public void Show()
        {
            for (int i = 0; i < this.NumberOfNodes; ++i)
                ShowNode(i);
        }

        public void ShowNode(int nodeID)
        {
            Console.WriteLine("\n==========");
            Console.WriteLine("Node ID: " + Tree[nodeID].NodeID);
            Console.WriteLine("Node split column: " + Tree[nodeID].SplitColumn);
            Console.WriteLine("Node split value: " + Tree[nodeID].SplitValue.ToString("F2"));
            Console.Write("Node class counts: ");
            for (int c = 0; c < NumberOfClasses; ++c)
                Console.Write(Tree[nodeID].ClassCounts[c] + "  ");
            Console.WriteLine("");
            Console.WriteLine("Node predicted class: " + ArgMax(Tree[nodeID].ClassCounts));
            Console.WriteLine("==========");
        }

        public int Predict(double[] x, bool verbose)
        {
            bool vb = verbose;
            int result = -1;
            int currNodeID = 0;
            string rule = "IF (*)";  // if any column is any value . . 
            while (true)
            {
                if (Tree[currNodeID].Rows.Count == 0)  // at an empty node
                    break;

                if (vb) Console.WriteLine("\ncurr node id = " + currNodeID);

                int sc = Tree[currNodeID].SplitColumn;
                double sv = Tree[currNodeID].SplitValue;
                double v = x[sc];
                if (vb) Console.WriteLine("Comparing " + sv + " in column " + sc + " with " + v);

                if (v < sv)
                {
                    if (vb) Console.WriteLine("attempting move left");

                    currNodeID = (2 * currNodeID) + 1;
                    if (currNodeID >= Tree.Count)
                        break;
                    result = Tree[currNodeID].PredictedClass;
                    rule += " AND (column " + sc + " < " + sv + ")";
                }
                else
                {
                    if (vb) Console.WriteLine("attempting move right");
                    currNodeID = (2 * currNodeID) + 2;
                    if (currNodeID >= Tree.Count)
                        break;
                    result = Tree[currNodeID].PredictedClass;
                    rule += " AND (column " + sc + " >= " + sv + ")";
                }

                if (vb) Console.WriteLine("new node id = " + currNodeID);
            }

            if (vb) Console.WriteLine("\n" + rule);
            if (vb) Console.WriteLine("Predcited class = " + result);

            return result;
        }
        /// <summary>
        /// Checks the accuracy of the specified input data.
        /// </summary>
        /// <param name="dataX">The data x.</param>
        /// <param name="dataY">The data y.</param>
        /// <returns></returns>
        public double Accuracy(double[][] dataX, int[] dataY)
        {
            int numCorrect = 0;
            int numWrong = 0;
            for (int i = 0; i < dataX.Length; ++i)
            {
                int predicted = Predict(dataX[i], verbose: false);
                int actual = dataY[i];
                if (predicted == actual)
                    ++numCorrect;
                else
                    ++numWrong;
            }
            // Console.WriteLine("correct = " + numCorrect);
            // Console.WriteLine("wrong   = " + numWrong);
            return (numCorrect * 1.0) / (numWrong + numCorrect);
        }
        private static SplitInfo GetSplitInfo(double[][] dataX, int[] dataY, List<int> rows, int numClasses)
        {
            // given a set of parent rows, find the col and value, and less-rows and greater-rows of
            // partition that gives lowest resulting mean impurity or entropy
            int nCols = dataX[0].Length;
            SplitInfo result = new SplitInfo();

            int bestSplitCol = 0;
            double bestSplitVal = 0.0;
            double bestImpurity = double.MaxValue;
            List<int> bestLessRows = new List<int>();
            List<int> bestGreaterRows = new List<int>();  // actually >=

            foreach (int i in rows)  // traverse the specified rows of the ref data
            {
                for (int j = 0; j < nCols; ++j)
                {
                    double splitVal = dataX[i][j];  // curr value to evaluate as possible best split value
                    List<int> lessRows = new List<int>();
                    List<int> greaterRows = new List<int>();
                    foreach (int ii in rows)  // walk down curr column
                    {
                        if (dataX[ii][j] < splitVal)
                            lessRows.Add(ii);
                        else
                            greaterRows.Add(ii);
                    } // ii

                    double meanImp = MeanImpurity(dataY, lessRows, greaterRows, numClasses);

                    if (meanImp < bestImpurity)
                    {
                        bestImpurity = meanImp;
                        bestSplitCol = j;
                        bestSplitVal = splitVal;

                        bestLessRows = new List<int>(lessRows);  // could use a CopyOf() helper
                        bestGreaterRows = new List<int>(greaterRows);
                    }

                } // j
            } // i

            result.SplitColumn = bestSplitCol;
            result.SplitValue = bestSplitVal;
            result.LessRows = new List<int>(bestLessRows);
            result.GreaterRows = new List<int>(bestGreaterRows);

            return result;
        }
        private static double Impurity(int[] dataY, List<int> rows, int numClasses)
        {
            // Gini impurity
            // dataY is all Y (class) values; rows tells which ones to analyze

            if (rows.Count == 0) return 0.0;

            int[] counts = new int[numClasses];  // counts for each of the classes
            double[] probs = new double[numClasses];  // frequency each class
            for (int i = 0; i < rows.Count; ++i)
            {
                int idx = rows[i];  // pts into refY
                int c = dataY[idx];  // class
                ++counts[c];
            }

            for (int c = 0; c < numClasses; ++c)
                if (counts[c] == 0) probs[c] = 0.0;
                else probs[c] = (counts[c] * 1.0) / rows.Count;

            double sum = 0.0;
            for (int c = 0; c < numClasses; ++c)
                sum += probs[c] * probs[c];

            return 1.0 - sum;
        }
        private static double MeanImpurity(int[] dataY, List<int> rows1, List<int> rows2, int numClasses)
        {
            if (rows1.Count == 0 && rows2.Count == 0)
                return 0.0;

            double imp1 = Impurity(dataY, rows1, numClasses); // 0.0 if rows Count is 0
            double imp2 = Impurity(dataY, rows2, numClasses);
            int count1 = rows1.Count;
            int count2 = rows2.Count;
            double wt1 = (count1 * 1.0) / (count1 + count2);
            double wt2 = (count2 * 1.0) / (count1 + count2);
            double result = (wt1 * imp1) + (wt2 * imp2);
            return result;
        }
        private static int[] ComputeClassCounts(int[] dataY, List<int> rows, int numClasses)
        {
            int[] result = new int[numClasses];
            foreach (int i in rows)
            {
                int c = dataY[i];
                ++result[c];
            }
            return result;
        }
        private static int ArgMax(int[] classCounts)
        {
            int largeCt = 0;
            int largeIndx = 0;
            for (int i = 0; i < classCounts.Length; ++i)
            {
                if (classCounts[i] > largeCt)
                {
                    largeCt = classCounts[i];
                    largeIndx = i;
                }
            }
            return largeIndx;
        }
        public static List<int> CopyOf(List<int> rows)
        {
            List<int> result = new List<int>();
            foreach (int i in rows)
                result.Add(i);
            return result;
        }
    }
}
