using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

namespace GitErDone
{
    // This class tracks what music is playing (on the user's YT) based on window titles
    class MusicTracker
    {
        // Music changelog file path
        public static string musicChangeLogDump = ProjectTracker.changeLogDump;

        #region Chrome stuff
        public static string lastYTTitle = "";
        public static void startYTTracker()
        {
            // Create timer
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Enabled = true;

            // Link timer
            timer.Tick += new EventHandler(checkYTTitle);

            // Log startup
            File.AppendAllLines(musicChangeLogDump, new string[]{
                "---",
                "Starting up",
                "Timestamp: " + DateTime.Now.ToString()});
        }
        public static void checkYTTitle(object sender, EventArgs e)
        {
            // Get current YT title
            string currentYTTitle = getYTTitle();

            // Compare it with stored YT title
            if (currentYTTitle != lastYTTitle && !string.IsNullOrWhiteSpace(currentYTTitle))
            {
                // Log title change
                File.AppendAllLines(musicChangeLogDump, new string[]{
                                "---",
                                "New song: " + currentYTTitle,
                                "Timestamp: " + DateTime.Now.ToString()});

                // Update stored YT title
                lastYTTitle = currentYTTitle;
            }
        }
        public static string getYTTitle()
        {
            // Get Chrome processes
            Process[] chromeProcs = Process.GetProcessesByName("chrome");

            // Check their main windows
            for (int i = 0; i < chromeProcs.Length; i++)
            {
                Process chromeProc = chromeProcs[i];

                string title = chromeProc.MainWindowTitle;
                if (title.EndsWith("YouTube - Google Chrome"))
                {
                    title = Regex.Replace(title.Remove(title.Length - 26), @"^\W+", "");
                    return title;
                }
            }

            // Nothing found
            return string.Empty;
        }
        #endregion
    }
}
