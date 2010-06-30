using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OpenCLNet
{
    public partial class OpenCLManager : Component
    {
        public bool OpenCLIsAvailable { get { return OpenCL.NumberOfPlatforms > 0; } }
        public List<string> RequireExtensions = new List<string>();
        public bool RequireImageSupport { get; set; }
        public bool UseMultiplePlatforms { get; set; }
        public bool AttemptUseBinaries { get; set; }
        public string BinaryPath { get; set; }
        public bool AttemptUseSource { get; set; }
        public string SourcePath { get; set; }
        public List<DeviceType> DeviceTypes = new List<DeviceType>();

        public string BuildOptions = "";
        public Platform Platform;
        public Context Context;

        public OpenCLManager()
        {
            InitializeComponent();
        }

        public OpenCLManager(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Create a context containing all devices in the platform returned by OpenCL.GetPlatform(0)
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="devices"></param>
        public void CreateDefaultContext()
        {
            if (!OpenCLIsAvailable)
                throw new OpenCLNotAvailableException();

            Platform = OpenCL.GetPlatform(0);
            Context = Platform.CreateDefaultContext();
        }

        /// <summary>
        /// Create a context containing all devices in the given platform
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="devices"></param>
        public void CreateContext( Platform platform, IEnumerable<Device> devices )
        {
            if (!OpenCLIsAvailable)
                throw new OpenCLNotAvailableException();

            foreach (Platform p in OpenCL.GetPlatforms())
            {
                //p.QueryDevices(DeviceType DeviceType.
            }
        }

        public Program CompileSourceFromFile( string fileName )
        {
            string sourcePath = SourcePath + Path.DirectorySeparatorChar + fileName;
            string binaryPath = BinaryPath + Path.DirectorySeparatorChar + fileName;

            if (!File.Exists(sourcePath))
                throw new FileNotFoundException(sourcePath);

            try
            {
                byte[][] binaries = LoadAllBinaries(Context, fileName);
                ErrorCode[] status = new ErrorCode[Context.Devices.Length];
                Program p = Context.CreateProgramWithBinary(Context.Devices, binaries, status);
                return p;
            }
            catch (Exception) // Loading binaries failed for some reason. Attempt to compile instead.
            {
                Program p = Context.CreateProgramFromFile(sourcePath);
                p.Build(Context.Devices, BuildOptions, null, IntPtr.Zero);
                SaveAllBinaries(Context, fileName, p.Binaries);
                return p;
            }
        }
        protected void SaveDeviceBinary(Context context, string fileName, byte[][] binaries, string platformDirectoryName, Device device )
        {
        }

        protected void SaveAllBinaries(Context context, string fileName, byte[][] binaries)
        {
            string metaFileName = BinaryPath + Path.DirectorySeparatorChar + "metainfo.xml";
            string platformDirectoryName;

            XmlSerializer xml = new XmlSerializer(typeof(MetaDirectory));
            TestAndCreateDirectory(BinaryPath);
            MetaDirectory platformMeta = MetaDirectory.FromPath(BinaryPath);
            
            // Make sure a platform directory with appropriate mapping exists
            if (platformMeta.ContainsKey(Platform.Name))
            {
                platformDirectoryName = platformMeta.GetPathFromKey(Platform.Name);
                TestAndCreateDirectory(platformDirectoryName);
            }
            else
            {
                platformDirectoryName = platformMeta.CreateRandomDirectoryForKey(Platform.Name);
            }

            platformMeta.Save();

            // Set up device directory structure and save device metadata
            MetaDirectory deviceMeta = new MetaDirectory(platformMeta, Platform.Name);
            for (int i = 0; i < context.Devices.Length; i++)
            {
                Device device = context.Devices[i];
                string deviceDirectoryName;

                // Make sure a device directory with appropriate mapping exists
                if (deviceMeta.ContainsKey(device.Name))
                {
                    deviceDirectoryName = deviceMeta.GetPathFromKey(device.Name);
                    TestAndCreateDirectory(deviceDirectoryName);
                }
                else
                    deviceDirectoryName = deviceMeta.CreateRandomDirectoryForKey(device.Name);

                string binaryFileName;
                MetaDirectory sourceToBinaryMeta = new MetaDirectory(deviceMeta, device.Name);

                if (sourceToBinaryMeta.ContainsKey(fileName))
                {
                    binaryFileName = sourceToBinaryMeta.GetPathFromKey(fileName);
                    TestAndCreateFile(binaryFileName);
                }
                else
                {
                    binaryFileName = sourceToBinaryMeta.CreateRandomFileForKey(fileName);
                }
                File.WriteAllBytes(binaryFileName, binaries[i]);

                sourceToBinaryMeta.Save();
            }
            deviceMeta.Save();
        }

        private string CreateRandomFile(string path)
        {
            int tries = 0;

            while (true)
            {
                string randomFileName = path + Path.DirectorySeparatorChar + Path.GetRandomFileName();
                try
                {
                    FileStream fs = File.Open(randomFileName, FileMode.CreateNew, FileAccess.ReadWrite);
                    fs.Close();
                    return randomFileName;
                }
                catch (IOException e)
                {
                    if (tries++ > 50)
                        throw e;
                }
            }
        }

        private string CreateRandomDirectory(string path)
        {
            int tries = 0;

            while (true)
            {
                string randomFileName = path + Path.DirectorySeparatorChar + Path.GetRandomFileName();
                try
                {
                    if (!Directory.Exists(randomFileName))
                        Directory.CreateDirectory(randomFileName);
                    return randomFileName;
                }
                catch (IOException e)
                {
                    if (tries++ > 50)
                        throw e;
                }
            }
        }

        private void TestAndCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private void TestAndCreateFile(string path)
        {
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }
        }

        protected byte[][] LoadAllBinaries( Context context, string fileName )
        {
            string sourcePath = SourcePath + Path.DirectorySeparatorChar + fileName;
            DateTime sourceDateTime = File.GetLastWriteTime(sourcePath);
            byte[][] binaries = new byte[context.Devices.Length][];

            if (!Directory.Exists(BinaryPath))
                throw new DirectoryNotFoundException(BinaryPath);

            MetaDirectory platformMeta = MetaDirectory.FromPath(BinaryPath);
            for( int i=0; i<context.Devices.Length; i++ )
            {
                Device device = context.Devices[i];
                MetaDirectory deviceMeta = new MetaDirectory(platformMeta, Platform.Name);
                MetaDirectory sourceToBinaryMeta = new MetaDirectory(deviceMeta, device.Name);

                if (!sourceToBinaryMeta.ContainsKey(fileName))
                    throw new FileNotFoundException(sourceToBinaryMeta.GetPathFromKey(fileName));
                if (sourceToBinaryMeta.GetLastWriteTimeFromKey(fileName) < sourceDateTime)
                    throw new Exception("binary out of date");
                binaries[i] = File.ReadAllBytes(sourceToBinaryMeta.GetPathFromKey(fileName));
            }
            return binaries;
        }
    }

    [Serializable]
    public class MetaDirectory
    {
        public string Root { get; set; }
        public string MetaFileName { get { return Root+Path.DirectorySeparatorChar + "metainfo.xml"; } }

        public MetaDirectory()
        {
        }

        public MetaDirectory(string root)
        {
            Root = root;

            MetaDirectory md = MetaDirectory.FromPath(Root);
            if (md.KeyValueMap.Count > 0)
            {
                Root = md.Root;
                KeyValueMap = md.KeyValueMap;
            }
        }

        public MetaDirectory(MetaDirectory root, string branchKey)
        {
            Root = root.Root + Path.DirectorySeparatorChar + root.GetValue(branchKey);
            MetaDirectory md = MetaDirectory.FromPath(Root);
            if (md.KeyValueMap.Count > 0)
            {
                Root = md.Root;
                KeyValueMap = md.KeyValueMap;
            }
        }

        public static MetaDirectory FromPath(string path)
        {
            MetaDirectory md;
            string metaFileName = path + Path.DirectorySeparatorChar + "metainfo.xml";
            XmlSerializer xml = new XmlSerializer(typeof(MetaDirectory));

            if (File.Exists(metaFileName))
            {
                XmlReader xmlReader = XmlReader.Create(metaFileName);
                md = (MetaDirectory)xml.Deserialize(xmlReader);
                xmlReader.Close();
            }
            else
            {
                md = new MetaDirectory();
                md.Root = path;
            }
            return md;
        }

        public void Save()
        {
            XmlSerializer xml = new XmlSerializer(typeof(MetaDirectory));
            XmlWriter xmlWriter = XmlWriter.Create(MetaFileName);
            xml.Serialize(xmlWriter, this);
            xmlWriter.Close();
        }

        public bool Exists { get { return Directory.Exists(Root); } }
        
        public void CreateIfNotExists()
        {
            if (!Exists)
                Directory.CreateDirectory(Root);
        }

        public string GetValue(string key)
        {
            return KeyValueMap[key];
        }

        public void SetValue(string key,string value)
        {
            KeyValueMap[key] = value;
        }

        public string GetPathFromKey(string key)
        {
            return Root + Path.DirectorySeparatorChar + KeyValueMap[key];
        }

        public DateTime GetCreationTimeFromKey(string key)
        {
            return File.GetCreationTime( GetPathFromKey(key) );
        }

        public DateTime GetLastWriteTimeFromKey(string key)
        {
            return File.GetLastWriteTime(GetPathFromKey(key));
        }

        public bool ContainsKey(string key)
        {
            return KeyValueMap.ContainsKey(key);
        }

        public string CreateRandomFileForKey( string key )
        {
            string value;

            value = CreateRandomFile(Root);
            KeyValueMap[key] = Path.GetFileName(value);
            return value;
        }

        public string CreateRandomDirectoryForKey( string key )
        {
            string value;
            value = CreateRandomDirectory(Root);
            KeyValueMap[key] = Path.GetFileName(value);
            return value;
        }

        private string CreateRandomFile(string path)
        {
            int tries = 0;

            while (true)
            {
                string randomFileName = path + Path.DirectorySeparatorChar + Path.GetRandomFileName();
                try
                {
                    FileStream fs = File.Open(randomFileName, FileMode.CreateNew, FileAccess.ReadWrite);
                    fs.Close();
                    return randomFileName;
                }
                catch (IOException e)
                {
                    if (tries++ > 50)
                        throw e;
                }
            }
        }

        private string CreateRandomDirectory(string path)
        {
            int tries = 0;

            while (true)
            {
                string randomFileName = path + Path.DirectorySeparatorChar + Path.GetRandomFileName();
                try
                {
                    if (!Directory.Exists(randomFileName))
                        Directory.CreateDirectory(randomFileName);
                    return randomFileName;
                }
                catch (IOException e)
                {
                    if (tries++ > 50)
                        throw e;
                }
            }
        }

        public void TestAndCreateDirectory(string path)
        {
            string name = Root + System.IO.Path.DirectorySeparatorChar + path;
            if (!Directory.Exists(name))
                Directory.CreateDirectory(name);
        }

        private void TestAndCreateFile(string path)
        {
            string name = Root + System.IO.Path.DirectorySeparatorChar + path;
            if (!File.Exists(name))
            {
                FileStream fs = File.Create(name);
                fs.Close();
            }
        }


        [XmlIgnore()]
        public Dictionary<string, string> KeyValueMap = new Dictionary<string, string>();
        [XmlArray("KeyValueMap")]
        [XmlArrayItem("DictionaryEntry", Type = typeof(DictionaryEntry))]
        public DictionaryEntry[] _PlatformToDirectoryMap
        {
            get
            {
                //Make an array of DictionaryEntries to return
                DictionaryEntry[] ret = new DictionaryEntry[KeyValueMap.Count];
                int i = 0;
                //Iterate through PlatformToDirectoryMap to load items into the array.
                foreach (KeyValuePair<string, string> entry in KeyValueMap)
                {
                    ret[i] = new DictionaryEntry(entry.Key, entry.Value);
                    i++;
                }
                return ret;
            }
            set
            {
                KeyValueMap.Clear();
                for (int i = 0; i < value.Length; i++)
                    KeyValueMap.Add(value[i].Key, value[i].Value);
            }
        }
    }

    [Serializable]
    public class DictionaryEntry
    {
        public string Key;
        public string Value;

        public DictionaryEntry()
        {
        }

        public DictionaryEntry(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
