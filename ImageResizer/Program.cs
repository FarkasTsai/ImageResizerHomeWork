using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #region 設備

            int coreCount = 0;
            foreach (var item in new ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            Console.WriteLine($"核心數: {coreCount}");
            Console.WriteLine($"邏輯處理器數: {Environment.ProcessorCount}");

            #endregion

            string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            string destinationPath = Path.Combine(Environment.CurrentDirectory, "output"); ;

            ImageProcess imageProcess = new ImageProcess();
            Stopwatch sw = new Stopwatch();

            #region 同步

            imageProcess.Clean(destinationPath);
            sw.Start();
            imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);
            sw.Stop();
            var syncTime = sw.ElapsedMilliseconds;
            Console.WriteLine($"同步處理花費時間: {syncTime} ms");

            #endregion

            #region 非同步

            imageProcess.Clean(destinationPath);
            sw.Restart();
            await imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0);
            sw.Stop();
            var asyncTime = sw.ElapsedMilliseconds;

            Console.WriteLine($"非同步處裡花費時間: {asyncTime} ms");
            Console.WriteLine($"提升效率: {((syncTime - asyncTime) / (double)syncTime * 100).ToString("#.#")} %");
            Console.ReadKey();

            #endregion
        }
    }
}
