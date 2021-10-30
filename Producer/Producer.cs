using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Producer
{
    class Producer
    {


        private readonly string _storageFolder;
        public static int messageLifeTime = Properties.Settings.Default.messageLifeTime;

        public Producer( string storageFolder)
        {
            _storageFolder = storageFolder;
            //----------------------------------------------------------------------------------------------------

            if (!Directory.Exists(_storageFolder))
            {
                throw new ApplicationException("error -- Storage folder setting is empty!");
            }
        }

        internal List<string> ReadQueue()
        {
            List<string> result = Directory.GetFiles(_storageFolder, "*.req").ToList();
            List<string> result0 = result.Select(fileName => Path.GetFileNameWithoutExtension(fileName)).ToList();


            List<string> resultResponse = Directory.GetFiles(_storageFolder, "*.resp").ToList();
            List<string> resultResponse0 = resultResponse.Select(fileName => Path.GetFileNameWithoutExtension(fileName)).ToList();

            var result1 = result0.Except(resultResponse0.Where(o => result0.Contains(o))).ToList();
            return result1;
        }

        internal void CleanQueue()
        {
            DateTime dt = DateTime.Now.AddMinutes(-messageLifeTime); // minutes
            var oldfiles =  Directory.GetFiles(_storageFolder, "*.*").Where(x => new FileInfo(x).CreationTime < dt).ToList();
            if (oldfiles.Count > 0)
            {
                lock (_storageFolder)
                {
                    try
                    {
                        foreach (var item in oldfiles)
                        {
                            File.Delete(item);
                        }
                        Console.WriteLine(DateTime.Now.ToLocalTime() + " CleanQueue done -- old message were dleted.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("CleanQueue ex: " + ex.Message);
                    }
                }
            }

        }

        internal void DoResponse(List<string> fileNames)
        {
            foreach (var item in fileNames)
            {
                //1. read request
                string fn = _storageFolder + "\\"+ item + ".req";
                string data = string.Empty;
                using (StreamReader sr = new StreamReader(fn))
                {
                    data = sr.ReadToEnd();
                }
                //2. write response
                fn = _storageFolder + "\\" + item + ".resp";
                using (StreamWriter sw = new StreamWriter(fn))
                {
                    sw.WriteLine("0");
                    sw.Write(data);
                }

            }
        }
    }
}
