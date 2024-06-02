using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// -----------------------------------------------------------------------------
using utils = Edam.Serialization;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace Edam.Data.AssetUseCases
{

    public class AssetUseCaseLogItem
    {
        public string ProjectName { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Support for top recent (default 20) Use Case Log preferences and details.
    /// </summary>
    public class AssetUseCaseLog
    {
        public static int MaxLogSize = 20;
        public const string USE_CASES_FOLDER = "UseCases";

        public string LogId { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        public List<AssetUseCaseLogItem> Items { get; set; } =
           new List<AssetUseCaseLogItem>();

        [JsonIgnore]
        public AssetUseCaseLogItem LastVisited
        {
            get
            {
                if (Items.Count > 0)
                {
                    return Items[0];
                }
                return null;
            }
        }

        public AssetUseCaseLogItem CurrentItem { get; set; } =
           new AssetUseCaseLogItem();

        /// <summary>
        /// Get a relative path for the use cases
        /// </summary>
        /// <returns>path is returned</returns>
        public static string GetUseCasesFolderName()
        {
            return USE_CASES_FOLDER + "/";
        }

        /// <summary>
        /// Test if folder exists if not create it...
        /// </summary>
        /// <returns>directory info or folder name is returned.  An execption will
        /// be generated if can't create the direcoty if it is missing</returns>
        public static object OpenFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return Directory.CreateDirectory(folderPath);
            }
            return folderPath;
        }

        /// <summary>
        /// Read Asset Use Case Log from a file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static AssetUseCaseLog FromFile(string filePath)
        {
            string path = GetUseCasesFolderName() + filePath;
            if (!File.Exists(path))
            {
                return new AssetUseCaseLog();
            }
            string jsonText = File.ReadAllText(path);
            return utils.JsonSerializer.Deserialize<AssetUseCaseLog>(jsonText);
        }

        public AssetUseCaseLogItem Find(string projectName)
        {
            var l = Items.Find((x) => x.ProjectName == projectName);
            if (l == null)
            {
                return null;
            }
            return l;
        }

        /// <summary>
        /// Save log details into a file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="log"></param>
        public static void ToFile(string folderPath, string filePath,
           AssetUseCaseLog log)
        {
            OpenFolder(folderPath);

            // make sure there is a corresponding log item in the log
            var litem = log.Find(log.CurrentItem.ProjectName);
            if (litem == null)
            {
                litem = new AssetUseCaseLogItem();
                litem.ProjectName = log.CurrentItem.ProjectName;
                log.Items.Insert(0, litem);

                if (log.Items.Count > MaxLogSize)
                {
                    log.Items.RemoveAt(log.Items.Count - 1);
                }
            }

            string jsonText = utils.JsonSerializer.Serialize(log);
            File.WriteAllText(filePath, jsonText);
        }

    }

}
