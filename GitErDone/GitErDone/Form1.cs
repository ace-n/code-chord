using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.PowerPacks;
//
namespace GitErDone
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
		
        private void Form1_Load(object sender, EventArgs e)
        {
            ProjectTracker.trackDirectory(@"C:\Users\Ace\My Code\GitErDone\GitErDone");

            // --- Youtube monitoring via Google Chrome ---
            MusicTracker.startYTTracker();

            // --- Website-ify ---
            //this.WindowState = FormWindowState.Maximized;
            btnGo_Click();
            
        }

        // Log shutdown
        private void LogShutdown(object sender, EventArgs e)
        {
            File.AppendAllLines(ProjectTracker.changeLogDump, new string[]{
                "---",
                "Shutting down",
                "Timestamp: " + DateTime.Now.ToString()});
        }

        private void btnGo_Click()
        {
            string[] lines = File.ReadAllLines(ProjectTracker.changeLogDump);

            // ===== Hacking time/total lines changed =====
            long secondsHacked = 0;
            bool systemActive = false;
            DateTime prevTimeStamp = new DateTime(2000, 1, 1);

            // === Hack time ===
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.ToLowerInvariant() == "starting up" || line.ToLowerInvariant() == "shutting down")
                {
                    // Get timestamp
                    DateTime timeStamp = DateTime.Parse(lines[++i].Remove(0, 11));

                    // Update hacked time
                    if (systemActive && line.ToLowerInvariant() == "shutting down")
                    {
                        systemActive = false;
                        if (prevTimeStamp != new DateTime(2000, 1, 1))
                        {
                            secondsHacked += (long)(timeStamp - prevTimeStamp).TotalSeconds;
                        }
                    }
                    else if (!systemActive)
                    {
                        systemActive = true;
                        prevTimeStamp = timeStamp;
                    }
                }
            }
            
            // === Coded lines ===
            long linesCoded = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.StartsWith("File changed"))
                {
                    // Get number of lines changed
                    i += 3;
                    line = lines[i];
                    linesCoded += int.Parse(Regex.Match(line, "(-|)\\d+$").Value);
                }
            }

            // ===== Most productive songs =====

            // == Get stats for songs from logs ==
            // STATS GUIDE (these are all totals for a given song across all listens of that song)
            // [0]: duration
            // [1]: saves pushed
            // [2]: lines changed
            // [3]: code quality changed
            Dictionary<string, int[]> songStats = new Dictionary<string, int[]>();

            // Loop thru songs
            systemActive = false;
            string activeSong = "";
            DateTime activeSong_start = File.GetCreationTime(ProjectTracker.changeLogDump);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Handle song changes
                if (line.StartsWith("New song"))
                {
                    // Get song name
                    string prevSong = activeSong;
                    activeSong = line.Remove(0, 10);
                    if (!songStats.ContainsKey(activeSong)) { songStats.Add(activeSong, new int[4]); }

                    // Get song listening timestamp
                    line = lines[++i];
                    DateTime prevSong_start = activeSong_start;
                    activeSong_start = DateTime.Parse(lines[i].Remove(0, 11));

                    // Update listening duration of past song
                    if (!string.IsNullOrWhiteSpace(prevSong))
                    {
                        songStats[prevSong][0] += (int)(activeSong_start - prevSong_start).TotalSeconds;
                    }

                    // Move on
                    continue;
                }

                // System start/stops
                if ((line.ToLowerInvariant() == "starting up" && !systemActive)
                    || (line.ToLowerInvariant() == "shutting down" && systemActive))
                {
                    // Update system activation status
                    systemActive = line.ToLowerInvariant() == "starting up";

                    // Clear song name
                    string prevSong = activeSong;
                    activeSong = "";

                    // Get timestamp
                    DateTime timeStamp = DateTime.Parse(lines[++i].Remove(0,11));

                    // Update listening duration of past song
                    if (!string.IsNullOrWhiteSpace(prevSong))
                    {
                        songStats[prevSong][0] += (int)(activeSong_start - timeStamp).TotalSeconds;
                    }
                }

                // Aggregate song stats
                if (line.StartsWith("File changed") && !string.IsNullOrWhiteSpace(activeSong))
                {
                    // Update saves pushed
                    songStats[activeSong][1]++;

                    // Get code quality change (for that particular save)
                    i+=2;
                    line = lines[i];
                    songStats[activeSong][3] += int.Parse(Regex.Match(line, "(-|)\\d+$").Value);

                    // Get number of lines changed
                    i++;
                    line = lines[i];
                    songStats[activeSong][2] += int.Parse(Regex.Match(line, "(-|)\\d+$").Value);
                }
            }

            // Remove 0-time songs
            songStats = songStats.Where(x => x.Value[0] != 0).ToDictionary(x => x.Key, y => y.Value);

            // == Visualize songs ==
            List<RectangleShape> rects = new List<RectangleShape>();
            int songCount = Math.Min(songStats.Count, 20);

            // Organize rectangles
            // FYI: Productivity = lines of code / min (simple and flawed metric, but who cares - it's a hackathon)
            //Dictionary<string, decimal> productivityList = new Dictionary<string, decimal>();
            Dictionary<string, decimal> sortedSongEnum = songStats
                .ToDictionary(x => x.Key, y => y.Value[2] / (decimal)y.Value[0]).
                OrderByDescending(x => x.Value).ToDictionary(x => x.Key, y => y.Value);
            
            // Get rectangle width coefficient and height
            decimal widthCoef = pnlMostProductiveSongs.Width / (decimal)sortedSongEnum.Values.Max();
            int height = (int)(pnlMostProductiveSongs.Height / songCount);

            // Add rectangles
            for (int i = 0; i < songCount; i++)
            {
                Panel rShape = new Panel();
                pnlMostProductiveSongs.Controls.Add((Control)rShape);

                rShape.BackColor = i % 2 == 0 ? Color.Orange : Color.Blue;
                rShape.Location = new Point(0, height * i);
                rShape.Size = new Size((int)(widthCoef * sortedSongEnum.Values.ToArray()[i]), height);

                // Add tooltips
                ToolTip tip = new ToolTip();
                tip.AutoPopDelay = 0;
                tip.InitialDelay = 0;
                tip.ShowAlways = true;

                // Get song name (for data access)
                string songName = sortedSongEnum.Keys.ToArray<string>()[i];
                int[] songStatValues = songStats[songName];

                // Populate tooltip
                tip.ToolTipTitle = "Song: " + songName;
                tip.SetToolTip(rShape,
                    "Time listened: " + getTimeFromSeconds((long)songStatValues[0]) + "\r" +
                    "Saves pushed: " + songStatValues[1] + "\r" +
                    "Lines changed: " + songStatValues[2] + "\r" +
                    "Code quality change: " + songStatValues[3]);
            }

            // ===== Most productive hours =====
            // List of save times
            // -- STATS ARRAY GUIDE
            // -- 0: Lines changed
            // -- 1: Quality change
            // -- 2 (saveTimeStats_perHour only): Saves pushed
            Dictionary<DateTime, int[]> saveTimeStats = new Dictionary<DateTime, int[]>();

            // == Get stats for hours from logs ==
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                
                // Aggregate time stats
                if (line.StartsWith("File changed"))
                {
                    // Get timestamp
                    line = lines[++i];
                    string timestampString = line.Remove(0, 11);
                    DateTime activeTimestamp = DateTime.Parse(timestampString);
                    if (saveTimeStats.ContainsKey(activeTimestamp)) { continue; } // Multiple-time saving of timestamps was a bug in earlier versions
                    saveTimeStats.Add(activeTimestamp, new int[2]);

                    // Get code quality change (for that particular save)
                    line = lines[++i];
                    saveTimeStats[activeTimestamp][1] += int.Parse(Regex.Match(line, "(-|)\\d+$").Value);

                    // Get number of lines changed
                    line = lines[++i];
                    saveTimeStats[activeTimestamp][0] += int.Parse(Regex.Match(line, "(-|)\\d+$").Value);
                }
            }

            // == Visualize time period stats ==
            saveTimeStats = saveTimeStats.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);

            // Aggregate save snapshots into per-time-period bunches
            Dictionary<DateTime, int[]> saveTimeStats_PerHour = new Dictionary<DateTime, int[]>();
            for (int i = 0; i < saveTimeStats.Count; i++)
            {
                // Get hour-only DateTime
                DateTime hourOnlyDateTime = saveTimeStats.Keys.ToArray()[i];
                hourOnlyDateTime = hourOnlyDateTime.Subtract(new TimeSpan(0, 0, hourOnlyDateTime.Minute, hourOnlyDateTime.Second));

                // Update productivity-time-period dictionary
                if (!saveTimeStats_PerHour.ContainsKey(hourOnlyDateTime))
                {
                    saveTimeStats_PerHour.Add(hourOnlyDateTime, new int[4]);
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 2) { continue; }
                        saveTimeStats_PerHour[hourOnlyDateTime][j] = saveTimeStats.Values.ToArray()[i][j];
                    }
                    saveTimeStats_PerHour[hourOnlyDateTime][2] += 1; // # of saves in that time period
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 2) { continue; }
                        saveTimeStats_PerHour[hourOnlyDateTime][j] += saveTimeStats.Values.ToArray()[i][j];
                    }
                    saveTimeStats_PerHour[hourOnlyDateTime][2] += 1; // # of saves in that time period
                }
            }
            widthCoef = (decimal)pnlMostProductiveHrs.Width /
                Math.Max(saveTimeStats_PerHour.Select(x => x.Value[0]).Max(), 1);

            // Add rectangles
            for (int i = 0; i < saveTimeStats_PerHour.Count; i++)
            {
                Panel rShape = new Panel();
                pnlMostProductiveHrs.Controls.Add((Control)rShape);

                int[] timePeriodStats = saveTimeStats_PerHour.Values.ToArray()[i];

                rShape.BackColor = i % 2 == 0 ? Color.Orange : Color.Blue;
                rShape.Location = new Point(0, height * i);
                rShape.Size = new Size((int)(widthCoef * timePeriodStats[0]), height);

                // Add tooltips
                ToolTip tip = new ToolTip();
                tip.AutoPopDelay = 0;
                tip.InitialDelay = 0;
                tip.ShowAlways = true;

                // Populate tooltip
                DateTime periodStart = saveTimeStats_PerHour.Keys.ToArray()[i];
                tip.ToolTipTitle = "Period: " + periodStart.ToShortTimeString() + " - " + periodStart.AddHours(1).ToShortTimeString();
                tip.SetToolTip(rShape,
                    "Saves pushed: " + timePeriodStats[2] + "\r" +
                    "Lines changed: " + timePeriodStats[0] + "\r" +
                    "Code quality change: " + timePeriodStats[1]);
            }

            // ===== Total stats =====
            lblStats.Text = "Session stats\r" +
                "Songs listened to: " + sortedSongEnum.Count + "\r" +
                "Time hacked: " + getTimeFromSeconds((long)secondsHacked) + "\r" +
                "Saves: " + saveTimeStats.Count + "\r" + 
                "Lines saved (total): " + linesCoded.ToString();
        }

        // Get time from seconds
        public string getTimeFromSeconds(long seconds)
        {
            string timeOut = "";
            List<string> timeStrings = new List<string>();
            TimeSpan span = new TimeSpan((int)(seconds / 3600),(int)((seconds / 60) % 60), (int)(seconds % 60));

            bool startAddingTimes = false;

            // Days
            startAddingTimes = span.Days > 0;
            if (startAddingTimes)
            {
                timeStrings.Add(span.Days.ToString() + " days");
            }
            
            // Hours
            startAddingTimes = startAddingTimes || span.Hours > 0;
            if (startAddingTimes)
            {
                timeStrings.Add(span.Hours.ToString() + " hours");
            }

            // Minutes
            startAddingTimes = startAddingTimes || span.Minutes > 0;
            if (startAddingTimes)
            {
                timeStrings.Add(span.Minutes.ToString() + " minutes");
            }

            // Seconds
            startAddingTimes = startAddingTimes || span.Seconds > 0;
            if (startAddingTimes)
            {
                timeStrings.Add(span.Seconds.ToString() + " seconds");
            }

            // Output result
            return String.Join(", ", timeStrings);
        }
    }
}
