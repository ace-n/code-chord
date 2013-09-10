using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DiffPlex.DiffBuilder.Model;
using DiffPlex;
using DiffPlex.Model;
using System.Text.RegularExpressions;

namespace GitErDone
{
    class ProjectTracker
    {
        // Paths
        private static string trackedDataDump = @"C:\GitErDone\DataDump"; // Directory where old versions of files are stored
        public static string changeLogDump = @"C:\GitErDone\changeLog.txt";

        // Monitored file extension (*.* to monitor all)
        private static readonly string permittedExtension = "*.cs";

        // DiffPlex stuff
        private static IDiffer differ = new Differ();

        // Dict of files to track and their corresponding fileSystemWatchers
        private static Dictionary<string, FileSystemWatcher> trackedLocations = new Dictionary<string, FileSystemWatcher>();

        #region fileSystemWatcher targeting
        public static void trackDirectory(string directory)
        {
            if (!trackedLocations.ContainsKey(directory) &&
                Directory.Exists(directory))
            {
                // Create fsw
                FileSystemWatcher fsw = new FileSystemWatcher(directory, permittedExtension);
                fsw.EnableRaisingEvents = true;
                fsw.IncludeSubdirectories = true;
                fsw.NotifyFilter = NotifyFilters.LastWrite;
                
                trackedLocations.Add(directory, fsw);
                fsw.Changed += new FileSystemEventHandler(fsw_Changed);

                // Copy file (minus drive info) to monitored directory
                List<string> files = Directory.GetFiles(directory, permittedExtension, SearchOption.AllDirectories).ToList();
                foreach (string file in files)
                {
                    string dumpPath = getDumpPath(file);
                    string dumpDirectory = Path.GetDirectoryName(dumpPath);

                    if (!Directory.Exists(dumpDirectory))
                    {
                        Directory.CreateDirectory(dumpDirectory);
                    }
                    File.Copy(file, getDumpPath(file), true);
                }
            }
        }

        public static void trackFile(string filePath)
        {
            if (!trackedLocations.ContainsKey(filePath) &&
                File.Exists(filePath))
            {
                // Create FSW
                FileSystemWatcher fsw = new FileSystemWatcher(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
                fsw.EnableRaisingEvents = true;

                trackedLocations.Add(filePath, fsw);
                fsw.Changed += new FileSystemEventHandler(fsw_Changed);

                // Copy file (minus drive info) to monitored directory
                File.Copy(filePath, getDumpPath(filePath), true);
            }
        }

        public static void untrackDirectory(string directory)
        {
            if (trackedLocations.ContainsKey(directory))
            {
                // Remove fsw
                trackedLocations[directory].Dispose();
                trackedLocations.Remove(directory);
            }
        }

        public static void untrackFile(string filePath)
        {
            if (trackedLocations.ContainsKey(filePath))
            {
                // Remove fsw
                trackedLocations[filePath].Dispose();
                trackedLocations.Remove(filePath);
            }
        }
        #endregion

        // Handle fileSystemWatcher update
        private static void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            // Make sure file/directory still exists - otherwise, remove it from the list of tracked files
            if (!File.Exists(e.FullPath))
            {
                string dumpPath = getDumpPath(e.FullPath);

                if (trackedLocations.ContainsKey(e.FullPath)) { trackedLocations.Remove(e.FullPath); }
                else if (trackedLocations.ContainsKey(dumpPath)) { trackedLocations.Remove(dumpPath); }

                // Skip this file since it no longer exists
                return;
            }

            // --- Get changes ---
            // Get old/new filepaths
            string oldText = File.ReadAllText(getDumpPath(e.FullPath));
            string newText = File.ReadAllText(e.FullPath);

            // Get diff
            object model = new SideBySideDiffModel();
            DiffResult lineDiffs = differ.CreateLineDiffs(oldText, newText, true, true);
            // If nothing actually changed, move along
            if (lineDiffs.DiffBlocks.Count == 0) { return; }

            // Calculate LOC change
            int locChanged = 0;
            for (int i = 0; i < lineDiffs.DiffBlocks.Count; i++)
            {
                locChanged += lineDiffs.PiecesOld[i].ToCharArray().Count( x => @"\n".Contains(x) );
            }

            // Calculate code quality metrics (on the entire file)
            int avgCodeQualityChange = 0;
            avgCodeQualityChange += getCodeQuality(newText) - getCodeQuality(oldText);
            avgCodeQualityChange /= lineDiffs.DiffBlocks.Count;

            // --- Log changes ---
            File.AppendAllLines(changeLogDump, new string[]{
                                "---",
                                "File changed: " + e.FullPath,
                                "Timestamp: " + DateTime.Now.ToString(),
                                "Code quality change: " + avgCodeQualityChange.ToString(),
                                "Lines changed: " + locChanged});

            // --- Store changes ---
            File.Copy(e.FullPath, getDumpPath(e.FullPath), true);
        }

        // Get dump path of file
        private static string getDumpPath(string filePath)
        {
            return trackedDataDump + filePath.Remove(0, filePath.IndexOf(@"\"));
        }

        // Get code quality - a dirt simple method for now
        public static int getCodeQuality(string code)
        {
            // Part 1: Get # of operators (C# and RegEx) per line (the more the worse)
            int operatorScore = (int)(code.ToCharArray().Count(x => @"(){}[].=+-\*|^$;".Contains(x)));

            // Part 2: Get # of comments (the more the better)
            int commentScore = Regex.Matches(code, @"/(/|\*)", RegexOptions.IgnoreCase).Count;

            // Return result
            return 80 + (commentScore*10 - operatorScore);
        }
    }
}
