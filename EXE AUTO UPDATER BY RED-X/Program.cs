//░░█ █▀█ █ █▄░█   █▀▄ █ █▀ █▀▀ █▀█ █▀█ █▀▄
//█▄█ █▄█ █ █░▀█   █▄▀ █ ▄█ █▄▄ █▄█ █▀▄ █▄▀

// Official RED-X CORPORATION Auto Updater
// GitHub / Discord integration
// https://discord.gg/f7KPc9JyeY

using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace EXE_AUTO_UPDATER_BY_RED_X
{
    class Program
    {
        static void Main()
        {
            // === Console setup ===
            Console.Title = "RED-X CORPORATION | AUTO UPDATER";
            try
            {
                Console.SetWindowSize(70, 30); // Set console size
                Console.SetBufferSize(70, 30); // Set buffer size
            }
            catch { } // Ignore if system does not allow resizing

            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            // Show ASCII art banner
            ShowBanner();

            // === Initialization sequence ===
            TypeWrite("[→] Initializing RED-X secure environment...", ConsoleColor.Gray, 20);
            Thread.Sleep(200);
            TypeWrite("[→] Performing integrity checks...", ConsoleColor.Gray, 20);
            Thread.Sleep(200);
            CheckNetwork(); // Verify internet connection

            // === Product selection menu ===
            Console.WriteLine();
            TypeWrite("[+] SELECT YOUR PRODUCT", ConsoleColor.Cyan, 10);
            Console.WriteLine();
            Console.WriteLine("[1] RED-X ELITE");
            Console.WriteLine("[2] RED-X PRIME");
            Console.WriteLine("[3] RED-X PREMIUM");
            Console.Write("\nEnter choice (1-3): ");

            string choice = Console.ReadLine();
            string productName = null;
            string url = null;

            // === Assign product based on choice ===
            switch (choice)
            {
                case "1":
                    productName = "RED-X ELITE.exe";
                    url = "https://raw.githubusercontent.com/REDX-CORPORATION/"; // TODO: Add GitHub raw link
                    break;
                case "2":
                    productName = "RED-X PRIME.exe";
                    url = "https://raw.githubusercontent.com/REDX-CORPORATION/"; // TODO: Add GitHub raw link
                    break;
                case "3":
                    productName = "RED-X PREMIUM.exe";
                    url = "https://raw.githubusercontent.com/REDX-CORPORATION/"; // TODO: Add GitHub raw link
                    break;
                default:
                    TypeWrite("[x] Invalid selection. Exiting...", ConsoleColor.Red, 10);
                    Thread.Sleep(1200);
                    return;
            }

            // === Download process ===
            Console.WriteLine();
            TypeWrite($"[↓] Updating {productName}...", ConsoleColor.Cyan, 10);

            string savePath = Path.Combine(Path.GetTempPath(), productName); // Save in temp folder

            // ⚠️ Insert your GitHub Personal Access Token here
            
//─█▀▀█ ░█▀▀▄ ░█▀▀▄ 　 ░█▀▀█ ▀█▀ ▀▀█▀▀ ░█─░█ ░█─░█ ░█▀▀█ 　 ░█▀▀█ ─█▀▀█ ▀▀█▀▀ 　 ░█─░█ ░█▀▀▀ ░█▀▀█ ░█▀▀▀ 
//░█▄▄█ ░█─░█ ░█─░█ 　 ░█─▄▄ ░█─ ─░█── ░█▀▀█ ░█─░█ ░█▀▀▄ 　 ░█▄▄█ ░█▄▄█ ─░█── 　 ░█▀▀█ ░█▀▀▀ ░█▄▄▀ ░█▀▀▀ 
//░█─░█ ░█▄▄▀ ░█▄▄▀ 　 ░█▄▄█ ▄█▄ ─░█── ░█─░█ ─▀▄▄▀ ░█▄▄█ 　 ░█─── ░█─░█ ─░█── 　 ░█─░█ ░█▄▄▄ ░█─░█ ░█▄▄▄
           
            string githubPAT = "YOUR_GITHUB_PERSONAL_ACCESS_TOKEN";

            // Download with progress tracking
            bool ok = DownloadWithProgress(url, savePath, productName, githubPAT);

            if (!ok)
            {
                TypeWrite("[x] Update failed.", ConsoleColor.Red, 10);
                Thread.Sleep(1200);
                return;
            }

            // === Launch the updated program ===
            TypeWrite($"[✔] Update complete. Launching {productName}...", ConsoleColor.Green, 10);
            Thread.Sleep(700);

            try
            {
                Process.Start(new ProcessStartInfo(savePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                TypeWrite("Launch error: " + ex.Message, ConsoleColor.Red, 10);
                Thread.Sleep(1200);
            }

            Environment.Exit(0); // Exit updater
        }

        // === Network check ===
        static void CheckNetwork()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.google.com");
                request.Timeout = 5000; // 5s timeout
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        TypeWrite("[→] Network handshake: CONNECTED", ConsoleColor.Green, 20);
                    else
                        TypeWrite("[→] Network handshake: FAILED", ConsoleColor.Red, 20);
                }
            }
            catch
            {
                TypeWrite("[→] Network handshake: FAILED", ConsoleColor.Red, 20);
            }
        }

        // === File download with progress bar ===
        static bool DownloadWithProgress(string url, string savePath, string displayName, string githubPAT)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.UserAgent = "RED-X-Updater/1.0";
                req.Headers.Add("Authorization", "token " + githubPAT); // GitHub PAT authorization

                using (var res = (HttpWebResponse)req.GetResponse())
                using (var rs = res.GetResponseStream())
                using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    long total = res.ContentLength; // File size

                    Console.WriteLine();
                    TypeWrite("[i] Target: " + displayName, ConsoleColor.Gray, 5);
                    if (total > 0)
                    {
                        string sizeMb = (total / 1024d / 1024d).ToString("0.00");
                        TypeWrite("[i] Size: " + sizeMb + " MB", ConsoleColor.Gray, 5);
                    }

                    Console.WriteLine();
                    int progressRow = Console.CursorTop; // Row to show progress
                    Console.WriteLine();

                    byte[] buffer = new byte[8192];
                    long readTotal = 0;
                    int read;
                    Stopwatch sw = Stopwatch.StartNew();

                    // === Read data chunks and update progress ===
                    while ((read = rs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, read);
                        readTotal += read;
                        DrawProgressLine(progressRow, readTotal, total, sw);
                    }

                    // Final progress line
                    DrawProgressLine(progressRow, total > 0 ? total : readTotal, total, sw);
                    sw.Stop();
                }

                return true;
            }
            catch (Exception ex)
            {
                TypeWrite("[x] Error: " + ex.Message, ConsoleColor.Red, 5);
                return false;
            }
        }

        // === Draw download progress bar ===
        static void DrawProgressLine(int row, long current, long total, Stopwatch sw)
        {
            if (sw.Elapsed.TotalSeconds <= 0) sw.Restart();

            double pct = (total > 0) ? (current * 100.0 / total) : 0.0;
            double bytesPerSec = current / Math.Max(0.001, sw.Elapsed.TotalSeconds);

            // Format speed display
            string speed = (bytesPerSec >= 1024 * 1024)
                ? (bytesPerSec / 1024d / 1024d).ToString("0.00") + " MB/s"
                : (bytesPerSec / 1024d).ToString("0.0") + " KB/s";

            // Estimate time remaining
            double remainSec = (total > 0 && bytesPerSec > 0) ? (total - current) / bytesPerSec : 0;
            string eta = (total > 0 && bytesPerSec > 0) ? FormatETA(remainSec) : "--:--";

            // Format MB progress
            string currentMb = (current / 1024d / 1024d).ToString("0.00");
            string totalMb = (total > 0) ? (total / 1024d / 1024d).ToString("0.00") : "??";

            // Progress bar visual
            int barWidth = 28;
            int fill = (int)((pct / 100.0) * barWidth);
            fill = Math.Max(0, Math.Min(barWidth, fill));

            string bar = "[" + new string('=', fill) + "RED-X" + new string(' ', barWidth - fill) + "]";

            string line = $"INIT {pct:0}% {bar} | {speed} | ETA {eta} | {currentMb}/{totalMb} MB";

            // Console width handling
            int width = Console.WindowWidth > 0 ? Console.WindowWidth : 80;
            if (line.Length > width) line = line.Substring(0, width);
            else if (line.Length < width) line += new string(' ', width - line.Length);

            // Write line at correct row
            try
            {
                Console.SetCursorPosition(0, row);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(line);
                Console.ResetColor();
            }
            catch { }

            try { Console.SetCursorPosition(0, row + 1); } catch { }
        }

        // === Format time (ETA) into MM:SS ===
        static string FormatETA(double seconds)
        {
            if (seconds < 0) seconds = 0;
            int s = (int)Math.Round(seconds);
            int m = s / 60; s %= 60;
            return m.ToString("00") + ":" + s.ToString("00");
        }

        // === Print ASCII banner ===
        static void ShowBanner()
        {
            string[] banner = new string[]
            {
"██████╗░███████╗██████╗░░░░░░░██╗░░██╗",
"██╔══██╗██╔════╝██╔══██╗░░░░░░╚██╗██╔╝",
"██████╔╝█████╗░░██║░░██║█████╗░╚███╔╝░",
"██╔══██╗██╔══╝░░██║░░██║╚════╝░██╔██╗░",
"██║░░██║███████╗██████╔╝░░░░░░██╔╝╚██╗",
"╚═╝░░╚═╝╚══════╝╚═════╝░░░░░░░╚═╝░░╚═╝"
            };

            foreach (var line in banner)
                CenterWrite(line, ConsoleColor.Red);

            CenterWrite("[AUTO UPDATER - A TOOL FROM RED-X CORPORATION]", ConsoleColor.Gray);
            Console.WriteLine();
        }

        // === Animated typing effect ===
        static void TypeWrite(string message, ConsoleColor color, int delay)
        {
            Console.ForegroundColor = color;
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(delay); // Delay between chars
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        // === Centered text printing ===
        static void CenterWrite(string text, ConsoleColor color)
        {
            int width = Console.WindowWidth > 0 ? Console.WindowWidth : 80;
            int pad = Math.Max(0, (width - text.Length) / 2);
            Console.ForegroundColor = color;
            Console.WriteLine(new string(' ', pad) + text);
            Console.ResetColor();
        }
    }
}
