using System;
using System.IO;
using System.Threading.Tasks;
using Boagaphish.Analytics;
using Cartheur.Presents;

class Program
{
    static Recorder VoiceRecorder { get; set; }
    static int RecordingDuration { get; set; }
    static LoaderPaths Configuration;
    static string TrainingDataFiles { get; set; }
    static string FileName { get; set; }
    static bool UseFile { get; set; }
    static DetectEmotion DetectEmotion { get; set; }
    static string OutputDetectionFileName = "listening_";

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
}