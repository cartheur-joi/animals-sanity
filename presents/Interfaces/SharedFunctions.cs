//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2025, all rights reserved.
//
using System;
using System.Reflection;
using System.IO;

namespace Cartheur.Presents
{
    /// <summary>
    /// A static class containing commonly-used (shared) functions.
    /// </summary>
    public static class SharedFunctions
    {
        /// <summary>
        /// The application version information.
        /// </summary>
        public static string ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        /// <summary>
        /// The path to the debug folder.
        /// </summary>
        public static string PathDebugFolder = Environment.CurrentDirectory + @"";
        /// <summary>
        /// The path to the release folder.
        /// </summary>
        public static string PathReleaseFolder = Path.Combine(Environment.CurrentDirectory, @"");
    }
}
