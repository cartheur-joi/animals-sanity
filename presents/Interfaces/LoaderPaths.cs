//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2025, all rights reserved.
//
using System;
using System.IO;

namespace Cartheur.Presents
{
    /// <summary>
    /// Sets the paths for the files given the configuration, either debug or release.
    /// </summary>
    public class LoaderPaths
    {
        /// <summary>
        /// The active configuration for the application runtume.
        /// </summary>
        public static string ActiveRuntime;
        /// <summary>
        /// Initializes a new instance of the <see cref="LoaderPaths"/> class with a build configuration.
        /// </summary>
        /// <param name="configuration">The active runtime configuration.</param>
        public LoaderPaths(string configuration)
        {
            if (configuration == "Debug")
                ActiveRuntime = SharedFunctions.PathDebugFolder;
            if (configuration == "Release")
                ActiveRuntime = SharedFunctions.PathReleaseFolder;
        }
        /// <summary>
        /// Gets the path to training data.
        /// </summary>
        public string PathToTrainingData
        {
            get
            {
                var path = Path.Combine(ActiveRuntime, SharedFunctions.PathDebugFolder + @"/datasets");
                return new Uri(path).LocalPath;
            }
        }
        /// <summary>
        /// Gets the path to the language processing model files.
        /// </summary>
        public string PathToLanguageModel
        {
            get
            {
                return "";//Path.Combine(ActiveRuntime, SharedFunctions.ThisAeon.GlobalSettings.GrabSetting("languagemodeldirectory"));
            }
}
        /// <summary>
        /// Sets the save path relative to the application.
        /// </summary>
        public static string SavePath
        {
            get
            {
                var path = Path.Combine(ActiveRuntime, SharedFunctions.PathDebugFolder + @"/recordings");
                return new Uri(path).LocalPath;
            }
        }
    }
}
