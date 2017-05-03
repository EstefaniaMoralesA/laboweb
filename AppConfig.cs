using System;
using System.IO;

namespace ABC
{
    public class AppConfig
    {
        public string ConfigDirectory { get; set; }

        public string WildCard => "1QQ";

        public string FullDirectoryPath { get; set;}

        public string SaltLogin { get; set; }

        public string SaltRequest { get; set; }

        public AppConfig(string saltLogin, string saltRequest, string configDirectory = "config")
        {
            SaltLogin = saltLogin;
            SaltRequest = saltRequest;
            ConfigDirectory = configDirectory;
            InitDirectory();
        }

        private void InitDirectory()
        {
            string path = Path.Combine(SharedContainer.AppDirectory, ConfigDirectory);
            Directory.CreateDirectory(path);
            FullDirectoryPath = path;
        }

        public void CreateConfigFile()
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(FullDirectoryPath, "config.php")))
            {
                sw.WriteLine("<?php");
                sw.Write("    ");
                sw.WriteLine($"$salt = array('login' => '{SaltLogin}','request'	=> '{SaltRequest}');");
                sw.Write("    ");
                sw.WriteLine($"include_once(\"{SharedContainer.DbInstance.DatabaseConfigFile}\");");
                sw.Write("    ");
                sw.WriteLine($"$db = new MySQL();");
                sw.Write("    ");
                sw.WriteLine($"session_start();");
                sw.WriteLine("?>");
            }
        }
    }
}