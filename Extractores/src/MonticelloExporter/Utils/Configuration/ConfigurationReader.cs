using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class ConfigurationReader
    {
        public static Configuration Read(string filePath)
        {
            var configValues = new Dictionary<string, string>();

            if (!File.Exists(filePath))
            {
                return null;
            }
            
            // Read the file and display it line by line.
            var file = new StreamReader(filePath);
            string line;

            while ((line = file.ReadLine()) != null)
            {
                if (line[0] != '#')
                {
                    var aux = line.Split('=');
                    configValues.Add(aux[0], aux[1]);
                }
            }
            file.Close();

            var configuration = new Configuration
            {
                DatabaseName = configValues["databaseName"],
                User = configValues["user"],
                Password = configValues["password"]
            };

            return configuration;
        }
    }
}
