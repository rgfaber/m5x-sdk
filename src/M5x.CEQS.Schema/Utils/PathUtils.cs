using System;
using System.Diagnostics;

namespace M5x.CEQS.Schema.Utils
{
    /// <summary>
    ///     Static class containing a number of methods returning special windows paths.
    /// </summary>
    public static class PathUtils
    {
        /// <summary>
        ///     Starts the Process that is associated with fileName's extension.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void RunFile(string fileName)
        {
            var p = new Process {StartInfo = {FileName = fileName}};
            p.Start();
        }


        /// <summary>
        ///     Returns the Applications Data Directory
        /// </summary>
        /// <returns>Applidation Data Directory</returns>
        public static string ApplicationDataDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        /// <summary>
        ///     Returns the Common Application Data Directory.
        /// </summary>
        /// <returns>Common Application Data Directory</returns>
        public static string CommonApplicationDataDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        }

        /// <summary>
        ///     Returns the Common Program Files Directory
        /// </summary>
        /// <returns>Common Program Files Directory</returns>
        public static string CommonProgramFilesDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
        }

        /// <summary>
        ///     Returns the Cookie Directory
        /// </summary>
        /// <returns>Cookie Directory</returns>
        public static string CookiesDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
        }

        /// <summary>
        ///     Returns the Desktop Directory
        /// </summary>
        /// <returns>Desktop Directory</returns>
        public static string DesktopDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        /// <summary>
        ///     Returns the Desktop Directory
        /// </summary>
        /// <returns>Desktop Directory</returns>
        public static string DesktopDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        /// <summary>
        ///     Returns the My Favorites Directory
        /// </summary>
        /// <returns>My Favorites Directory</returns>
        public static string MyFavoritesDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
        }

        /// <summary>
        ///     Histories the dir.
        /// </summary>
        /// <returns></returns>
        public static string HistoryDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.History);
        }

        /// <summary>
        ///     Internets the cache dir.
        /// </summary>
        /// <returns></returns>
        public static string InternetCacheDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
        }

        /// <summary>
        ///     Locals the application data dir.
        /// </summary>
        /// <returns></returns>
        public static string LocalApplicationDataDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        /// <summary>
        ///     Mies the pictures dir.
        /// </summary>
        /// <returns></returns>
        public static string MyPicturesDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }

        /// <summary>
        ///     Mies the computer dir.
        /// </summary>
        /// <returns></returns>
        public static string MyComputerDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
        }

        /// <summary>
        ///     Mies the documents dir.
        /// </summary>
        /// <returns></returns>
        public static string MyDocumentsDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /// <summary>
        ///     Mies the music dir.
        /// </summary>
        /// <returns></returns>
        public static string MyMusicDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        }

        /// <summary>
        ///     Personals the dir.
        /// </summary>
        /// <returns></returns>
        public static string PersonalDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        /// <summary>
        ///     Programs the files dir.
        /// </summary>
        /// <returns></returns>
        public static string ProgramFilesDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        /// <summary>
        ///     Programses the dir.
        /// </summary>
        /// <returns></returns>
        public static string ProgramsDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        }

        /// <summary>
        ///     Recents the dir.
        /// </summary>
        /// <returns></returns>
        public static string RecentDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Recent);
        }

        /// <summary>
        ///     Sends to dir.
        /// </summary>
        /// <returns></returns>
        public static string SendToDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
        }

        /// <summary>
        ///     Starts the menu dir.
        /// </summary>
        /// <returns></returns>
        public static string StartMenuDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        }

        /// <summary>
        ///     Startups the dir.
        /// </summary>
        /// <returns></returns>
        public static string StartupDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        }

        /// <summary>
        ///     Systems the dir.
        /// </summary>
        /// <returns></returns>
        public static string SystemDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.System);
        }

        /// <summary>
        ///     Templateses the dir.
        /// </summary>
        /// <returns></returns>
        public static string TemplatesDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Templates);
        }
    }
}