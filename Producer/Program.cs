using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Producer producer = new Producer(Properties.Settings.Default.StorageFolder);
            List<string> fileNames;
            int timeInterval = Properties.Settings.Default.timeInterval; //ms
            do
            {
                producer.CleanQueue();

                fileNames = producer.ReadQueue();

                if (fileNames.Count > 0) producer.DoResponse(fileNames);

                fileNames.Clear();

                // pause
                Thread.Sleep(timeInterval);
            } while (true);
        }
    }
}
