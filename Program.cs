using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Accord;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using Accord.Statistics.Filters;
using Boagaphish.Analytics;
using Cartheur.Presents;

class Program
{
    static Recorder VoiceRecorder { get; set; }
    static int RecordingDuration { get; set; }
    static LoaderPaths Configuration;
    static string TrainingDataFiles { get; set; }
    static string FileName = "recorded";
    static bool UseFile { get; set; }
    static DetectEmotion DetectEmotion { get; set; }
    static string OutputDetectionFileName = "recorded";

    static async Task Main()
    {
        Console.WriteLine("in here");
        Configuration = new LoaderPaths("Debug");
        // Create an instance of the Classifier
        var classifier = new EmotionClassification.Classifier();
        classifier.LoadData(Configuration.PathToTrainingData);
        var path = Path.Combine(LoaderPaths.ActiveRuntime, LoaderPaths.SavePath, FileName + ".wav");
        Console.WriteLine(Terminal.StartBashProcess("arecord -vv -r 16000 -c 1 -f S16_LE -d 1 " + path));
        // Set the recording duration.
        RecordingDuration = 1000;
        if (!UseFile)
        {
            VoiceRecorder = new Recorder();
            await VoiceRecorder.Record(ReturnRecordingFilePath("recorded"), RecordingDuration);
            Console.WriteLine("Started recording...");
            VoiceRecorder.RecordingFinished += RecordingEvent;
        }
        Console.WriteLine("Getting the file for analysis...");
        string audioFilePath = ReturnRecordingFilePath("recorded");
        // Send over for the file for analysis and return the approximated emotion.
        try
        {
            DetectEmotion = new DetectEmotion(ReturnRecordingFilePath(OutputDetectionFileName), "linux");
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }

        //Thread.Sleep(2000);
        // Load the data

        // Train the model
        classifier.TrainModel();
        // Predict the emotion
        var emotion = classifier.PredictEmotion(FileName);

        // Display the classification result
        Console.WriteLine($"The predicted emotion is: {emotion}");
        await Task.CompletedTask;
    }
    static string ReturnRecordingFilePath(string filename)
    {
        return Path.Combine(LoaderPaths.ActiveRuntime, LoaderPaths.SavePath, filename + ".wav");
    }
    static void RecordingEvent(object source, EventArgs e)
    {
        Console.WriteLine("Recording has completed.");
    }
    /// <summary>
    /// //// Read the Excel worksheet into a DataTable
            ////DataTable table = new ExcelReader("examples.xls").GetWorksheet("Classification - Yin Yang");
            //FileStream stream = File.Open("examples.xls", FileMode.Open, FileAccess.Read);

            ////1. Reading from a binary Excel file ('97-2003 format; *.xls)
            //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            //DataSet dataset = excelReader.AsDataSet();
            ////DataTable table = dataset.Tables[0];

            //// Convert the DataTable to input and output vectors
            //double[][] inputs = table.ToArray<double>("X", "Y");
            //int[] outputs = table.Columns["G"].ToArray<int>();


            //// In our problem, we have 2 classes (samples can be either
            //// positive or negative), and 2 continuous-valued inputs.
            //DecisionTree tree = new DecisionTree(
            //                inputs: new List<DecisionVariable>
            //                    {
            //            DecisionVariable.Continuous("X"),
            //            DecisionVariable.Continuous("Y")
            //                    },
            //                classes: 2);

            //C45Learning teacher = new C45Learning(tree);

            //// The C4.5 algorithm expects the class labels to
            //// range from 0 to k, so we convert -1 to be zero:
            //outputs = outputs.Apply(x => x < 0 ? 0 : x);

            //double error = teacher.Run(inputs, outputs);

            //// Classify the samples using the model
            //int[] answers = inputs.Apply(tree.Decide);

            //// Plot the results
            //ScatterplotBox.Show("Expected results", inputs, outputs);
            //ScatterplotBox.Show("Decision Tree results", inputs, answers).Hold();
    /// </summary>
    static void Example() {
            //LINK: http://accord-framework.net/docs/html/T_Accord_MachineLearning_DecisionTrees_Learning_ID3Learning.htm
            //LINK: http://accord-framework.net/docs/html/T_Accord_MachineLearning_DecisionTrees_DecisionTree.htm

            DataTable data = new DataTable("Mitchell's Tennis Example");

            data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D2", "Sunny", "Hot", "High", "Strong", "No");
            data.Rows.Add("D3", "Overcast", "Hot", "High", "Weak", "Yes");
            data.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D6", "Rain", "Cool", "Normal", "Strong", "No");
            data.Rows.Add("D7", "Overcast", "Cool", "Normal", "Strong", "Yes");
            data.Rows.Add("D8", "Sunny", "Mild", "High", "Weak", "No");
            data.Rows.Add("D9", "Sunny", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D10", "Rain", "Mild", "Normal", "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", "Mild", "Normal", "Strong", "Yes");
            data.Rows.Add("D12", "Overcast", "Mild", "High", "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D14", "Rain", "Mild", "High", "Strong", "No");

            // Create a new codification codebook to 
            // convert strings into integer symbols
            Codification codebook = new Codification(data,
              "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            // Translate our training data into integer symbols using our codebook:
            DataTable symbols = codebook.Apply(data);
            int[][] inputs = symbols.ToArray<int>("Outlook", "Temperature", "Humidity", "Wind");
            int[] outputs = symbols.ToArray<int>("PlayTennis");

            // Gather information about decision variables
            DecisionVariable[] attributes =
            {
              new DecisionVariable("Outlook",     3), // 3 possible values (Sunny, overcast, rain)
              new DecisionVariable("Temperature", 3), // 3 possible values (Hot, mild, cool)  
              new DecisionVariable("Humidity",    2), // 2 possible values (High, normal)    
              new DecisionVariable("Wind",        2)  // 2 possible values (Weak, strong) 
            };

            int classCount = 2; // 2 possible output values for playing tennis: yes or no

            //Create the decision tree using the attributes and classes
            Accord.MachineLearning.DecisionTrees.DecisionTree tree = new Accord.MachineLearning.DecisionTrees.DecisionTree(attributes, classCount);

            //Create a new instance of the ID3 algorithm
            ID3Learning id3learning = new ID3Learning(tree);

            // Learn the training instances!
            id3learning.Run(inputs, outputs);

            //Suggest you read the example in the guide carefully. At the very end of the procedure they generate the expression tree with var expression = tree.ToExpression(); and compile it:
            var expression = tree.ToExpression();
            var func = expression.Compile();
            //The result is a delegate that you can simply execute to get a decision for a given input.In the example, you could do something like
            bool willPlayTennis = func(new double[] { 1.0, 1.0, 1.0, 1.0 }) == 1;

            int[] query = codebook.Translate("Sunny", "Hot", "High", "Strong");
            int output = tree.Decide(query);
            string answer = codebook.Translate("PlayTennis", output);

            Console.WriteLine(answer);
            Console.ReadLine();

            //RESULT:
            //In the above example, answer will be "No".
        }
}