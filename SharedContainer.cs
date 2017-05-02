using System;
using System.IO;

namespace ABC
{
    public static class SharedContainer 
    {
        public static string HeaderPath => Path.Combine(AppDirectory, "header.php");

        public static string FooterPath => Path.Combine(AppDirectory, "footer.php");

        public static DatabaseConnection DbInstance { get; set; }

        public static AppConfig AppConfigInstance { get; set; }

        public static string AppDirectory => AppDomain.CurrentDomain.BaseDirectory;
    }
}