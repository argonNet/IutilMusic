using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IUtilMusic.Persistence
{
    class ConfigPersister
    {
        /// <summary>
        /// Get the path for the config !
        /// </summary>
        /// <returns></returns>
        private static String getConfigFileName()
        {
            //: %AppData%\AirspaceApps\[AppName] (AppName = UtilMusic)
            return Path.Combine(
                 Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "AirspaceApps", 
                Assembly.GetEntryAssembly().GetName().Name);
        }

        /// <summary>
        /// Load the Config
        /// </summary>
        public static Config Load()
        {
            Config config = null;

            if(File.Exists(getConfigFileName()))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                using (StreamReader sr = new StreamReader(getConfigFileName()))
                {
                    config = (Config)xs.Deserialize(sr);
                }
            }
            else
            {
                config = new Config();
            }

            return config;
        }

        /// <summary>
        /// Save the config
        /// </summary>
        public static void Save(Config config)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(getConfigFileName()));

            XmlSerializer xs = new XmlSerializer(typeof(Config));
            using(StreamWriter sw = new StreamWriter(getConfigFileName()))
            {
                xs.Serialize(sw, config);
            }
        }
    }
}
