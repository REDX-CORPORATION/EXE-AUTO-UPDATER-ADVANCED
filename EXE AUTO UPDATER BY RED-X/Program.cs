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
            Console.Title = "RED-X CORPORATION | AUTO UPDATER";
            try
            {
                Console.SetWindowSize(70, 30);
                Console.SetBufferSize(70, 30);
            }
            catch { }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            ShowBanner();

            TypeWrite("[→] Initializing RED-X secure environment...", ConsoleColor.Gray, 20);
            Thread.Sleep(200);
            TypeWrite("[→] Performing integrity checks...", ConsoleColor.Gray, 20);
            Thread.Sleep(200);
            CheckNetwork();

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

            switch (choice)
            {
                case "1":
                    productName = "RED-X ELITE.exe";
                    url = "https://raw.githubusercontent.com/REDX-CORPORATION/EXE-AUTO-UPDATER/main/RED-X/ELITE/RED-X%20ELITE.exe";
                    break;
                case "2":
                    productName = "RED-X PRIME.exe";
                    url = "https://raw.githubusercontent.com/REDX-CORPORATION/EXE-AUTO-UPDATER/main/RED-X/PRIME/RED-X%20PRIME.exe";
                    break;
                case "3":
                    productName = "RED-X PREMIUM.exe";
                    url = "https://raw.githubusercontent.com/REDX-CORPORATION/EXE-AUTO-UPDATER/main/RED-X/PREMIUM/RED-X%20PREMIUM.exe";
                    break;
                default:
                    TypeWrite("[x] Invalid selection. Exiting...", ConsoleColor.Red, 10);
                    Thread.Sleep(1200);
                    return;
            }

            Console.WriteLine();
            TypeWrite($"[↓] Updating {productName}...", ConsoleColor.Cyan, 10);

            string savePath = Path.Combine(Path.GetTempPath(), productName);

            // Your GitHub PAT here
            string githubPAT = "ghp_xxxxxxxxxx";

            bool ok = DownloadWithProgress(url, savePath, productName, githubPAT);

            if (!ok)
            {
                TypeWrite("[x] Update failed.", ConsoleColor.Red, 10);
                Thread.Sleep(1200);
                return;
            }

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

            Environment.Exit(0);
        }

        static void CheckNetwork()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.google.com");
                request.Timeout = 5000;
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

        static bool DownloadWithProgress(string url, string savePath, string displayName, string githubPAT)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.UserAgent = "RED-X-Updater/1.0";
                req.Headers.Add("Authorization", "token " + githubPAT);

                using (var res = (HttpWebResponse)req.GetResponse())
                using (var rs = res.GetResponseStream())
                using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    long total = res.ContentLength;

                    Console.WriteLine();
                    TypeWrite("[i] Target: " + displayName, ConsoleColor.Gray, 5);
                    if (total > 0)
                    {
                        string sizeMb = (total / 1024d / 1024d).ToString("0.00");
                        TypeWrite("[i] Size: " + sizeMb + " MB", ConsoleColor.Gray, 5);
                    }

                    Console.WriteLine();
                    int progressRow = Console.CursorTop;
                    Console.WriteLine();

                    byte[] buffer = new byte[8192];
                    long readTotal = 0;
                    int read;
                    Stopwatch sw = Stopwatch.StartNew();

                    while ((read = rs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, read);
                        readTotal += read;
                        DrawProgressLine(progressRow, readTotal, total, sw);
                    }

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

        static void DrawProgressLine(int row, long current, long total, Stopwatch sw)
        {
            if (sw.Elapsed.TotalSeconds <= 0) sw.Restart();

            double pct = (total > 0) ? (current * 100.0 / total) : 0.0;
            double bytesPerSec = current / Math.Max(0.001, sw.Elapsed.TotalSeconds);
            string speed = (bytesPerSec >= 1024 * 1024)
                ? (bytesPerSec / 1024d / 1024d).ToString("0.00") + " MB/s"
                : (bytesPerSec / 1024d).ToString("0.0") + " KB/s";

            double remainSec = (total > 0 && bytesPerSec > 0) ? (total - current) / bytesPerSec : 0;
            string eta = (total > 0 && bytesPerSec > 0) ? FormatETA(remainSec) : "--:--";

            string currentMb = (current / 1024d / 1024d).ToString("0.00");
            string totalMb = (total > 0) ? (total / 1024d / 1024d).ToString("0.00") : "??";

            int barWidth = 28;
            int fill = (int)((pct / 100.0) * barWidth);
            fill = Math.Max(0, Math.Min(barWidth, fill));

            string bar = "[" + new string('=', fill) + "RED-X" + new string(' ', barWidth - fill) + "]";

            string line = $"INIT {pct:0}% {bar} | {speed} | ETA {eta} | {currentMb}/{totalMb} MB";

            int width = Console.WindowWidth > 0 ? Console.WindowWidth : 80;
            if (line.Length > width) line = line.Substring(0, width);
            else if (line.Length < width) line += new string(' ', width - line.Length);

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

        static string FormatETA(double seconds)
        {
            if (seconds < 0) seconds = 0;
            int s = (int)Math.Round(seconds);
            int m = s / 60; s %= 60;
            return m.ToString("00") + ":" + s.ToString("00");
        }

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

        static void TypeWrite(string message, ConsoleColor color, int delay)
        {
            Console.ForegroundColor = color;
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.ResetColor();
            Console.WriteLine();
        }

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
