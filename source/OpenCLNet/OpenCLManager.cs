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
#warning Meta files need to contain BuildOptions and Defines for proper uniqueness testing

    public partial class OpenCLManager : Component
    {
        /// <summary>
        /// True if OpenCL is available on this machine
        /// </summary>
        public bool OpenCLIsAvailable { get { return OpenCL.NumberOfPlatforms > 0; } }
        /// <summary>
        /// Each element in this list is interpreted as the name of an extension.
        /// Any device that does not present this extension in its Extensions
        /// property will be filtered out during context creation.
        /// </summary>
        public List<string> RequiredExtensions = new List<string>();
        /// <summary>
        /// If true, OpenCLManager will filter out any devices that don't signal image support through the HasImageSupport property
        /// </summary>
        public bool RequireImageSupport { get; set; }
        /// <summary>
        /// If true, OpenCLManager will attempt to use stored binaries(Stored at 'BinaryPath') to avoid recompilation
        /// </summary>
        public bool AttemptUseBinaries { get; set; }
        /// <summary>
        /// The location to store and look for compiled binaries
        /// </summary>
        public string BinaryPath { get; set; }
        /// <summary>
        /// If true, OpenCLManager will attempt to compile sources(Stored at 'SourcePath') to compile programs, and possibly
        /// to store binaries(If 'AttemptUseBinarues' is true)
        /// </summary>
        public bool AttemptUseSource { get; set; }
        /// <summary>
        /// The location where sources are stored
        /// </summary>
        public string SourcePath { get; set; }
        public List<DeviceType> DeviceTypes = new List<DeviceType>();
        /// <summary>
        /// BuildOptions is passed to the OpenCL build functions that take compiler options
        /// </summary>
        public string BuildOptions { get; set; }
        /// <summary>
        /// This string is prepended verbatim to any and all sources that are compiled.
        /// It can contain any kind of useful global definitions.
        /// </summary>
        public string Defines { get; set; }
        public Platform Platform;
        public Context Context;
        /// <summary>
        /// Array of CommandQueues. Indices correspond to the devices in the Context.Devices.
        /// Simple OpenCL programs will typically just enqueue operations on CQ[0] and ignore any additional devices.
        /// </summary>
        public CommandQueue[] CQ;

        public OpenCLManager()
        {
            DefaultProperties();
            InitializeComponent();
        }

        public OpenCLManager(IContainer container)
        {
            DefaultProperties();

            container.Add(this);
            InitializeComponent();
        }

        private void DefaultProperties()
        {
            RequireImageSupport = false;
            BuildOptions = "";
            Defines = "";
            SourcePath = "OpenCL" + Path.AltDirectorySeparatorChar + "src";
            BinaryPath = "OpenCL" + Path.AltDirectorySeparatorChar + "bin";
            AttemptUseBinaries = true;
            AttemptUseSource = true;
        }

        /// <summary>
        /// Create a context containing all devices in the platform returned by OpenCL.GetPlatform(0) that satisfy the current RequireImageSupport and RequireExtensions property settings.
        /// Default command queues are made available through the CQ property
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="devices"></param>
        public void CreateDefaultContext()
        {
            CreateDefaultContext(0,DeviceType.ALL);
        }

        /// <summary>
        /// Create a context containing all devices of a given type that satisfy the current RequireImageSupport and RequireExtensions property settings.
        /// Default command queues are made available through the CQ property
        /// 
        /// </summary>
        /// <param name="deviceType"></param>
        public void CreateDefaultContext(int platformNumber, DeviceType deviceType)
        {
            if (!OpenCLIsAvailable)
                throw new OpenCLNotAvailableException();

            Platform = OpenCL.GetPlatform(platformNumber);
            var devices = from d in Platform.QueryDevices(deviceType)
                    where ((RequireImageSupport && d.ImageSupport == true) || !RequireImageSupport) && d.HasExtensions( RequiredExtensions.ToArray<string>() )
                    select d;
            IntPtr[] properties = new IntPtr[]
            {
                (IntPtr)ContextProperties.PLATFORM, Platform,
                IntPtr.Zero
            };

            if (devices.Count() == 0)
                throw new OpenCLException("CreateDefaultContext: No OpenCL devices found that matched filter criteria.");

            CreateContext(Platform, properties, devices);
        }


        /// <summary>
        /// Create a context and initialize default command queues in the CQ property
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="devices"></param>
        public void CreateContext(Platform platform, IntPtr[] contextProperties, IEnumerable<Device> devices)
        {
            CreateContext(platform, contextProperties, devices, null, IntPtr.Zero);
        }

        /// <summary>
        /// Create a context and initialize default command queues in the CQ property
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="devices"></param>
        public void CreateContext( Platform platform, IntPtr[] contextProperties, IEnumerable<Device> devices, ContextNotify notify, IntPtr userData )
        {
            if (!OpenCLIsAvailable)
                throw new OpenCLNotAvailableException();

            Platform = platform;
            Context = platform.CreateContext( contextProperties, devices.ToArray<Device>(), notify, userData );
            CQ = new CommandQueue[Context.Devices.Length];
            for (int i = 0; i < Context.Devices.Length; i++)
                CQ[i] = Context.CreateCommandQueue(Context.Devices[0]);
        }

        /// <summary>
        /// CompileSource
        /// 
        /// Attempt to create a program from source and build it. No caching is attempted.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Program CompileSource(string source)
        {
            Program program = Context.CreateProgramWithSource(Defines+System.Environment.NewLine+source);
            program.Build(Context.Devices, BuildOptions, null, IntPtr.Zero);
            return program;
        }

        /// <summary>
        /// CompileFile
        /// 
        /// Attempt to compile the file identified by fileName.
        /// If the AttemptUseBinaries property is true, the method will first check if an up-to-date precompiled binary exists.
        /// If it does, it will load the binary instead, if no binary exists, compilation will be performed and the resulting binaries saved.
        /// 
        /// If the AttemptUseBinaries property is false, only compilation will be attempted.
        /// 
        /// Caveat: If AttemptUseSource is false, no compilation will be attempted - ever.
        /// If both AttemptUseSource and AttemptUseBinaries are false this function will throw an exception.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Program CompileFile( string fileName )
        {
            string sourcePath = SourcePath + Path.DirectorySeparatorChar + fileName;
            string binaryPath = BinaryPath + Path.DirectorySeparatorChar + fileName;
            Program p;

            if (!File.Exists(sourcePath))
                throw new FileNotFoundException(sourcePath);

            if (AttemptUseBinaries && !AttemptUseSource)
            {
                byte[][] binaries = LoadAllBinaries(Context, fileName);
                ErrorCode[] status = new ErrorCode[Context.Devices.Length];
                p = Context.CreateProgramWithBinary(Context.Devices, binaries, status);
                p.Build();
                return p;
            }
            else if (!AttemptUseBinaries && AttemptUseSource)
            {
                string source = Defines+System.Environment.NewLine+File.ReadAllText(sourcePath);
                p = Context.CreateProgramWithSource(source);
                p.Build(Context.Devices, BuildOptions, null, IntPtr.Zero);
                SaveAllBinaries(Context, fileName, p.Binaries);
                return p;
            }
            else if (AttemptUseBinaries && AttemptUseSource)
            {
                try
                {
                    byte[][] binaries = LoadAllBinaries(Context, fileName);
                    ErrorCode[] status = new ErrorCode[Context.Devices.Length];
                    p = Context.CreateProgramWithBinary(Context.Devices, binaries, status);
                    p.Build();
                    return p;
                }
                catch (Exception)
                {
                    // Loading binaries failed for some reason. Attempt to compile instead.
                    string source = Defines + System.Environment.NewLine + File.ReadAllText(sourcePath);
                    p = Context.CreateProgramWithSource(source);
                    p.Build(Context.Devices, BuildOptions, null, IntPtr.Zero);
                    SaveAllBinaries(Context, fileName, p.Binaries);
                    return p;
                }
            }
            else
            {
                throw new OpenCLException("OpenCLManager has both AttemptUseBinaries and AttemptUseSource set to false, and therefore can't build Programs from files");
            }
        }

        protected void SaveDeviceBinary(Context context, string fileName, byte[][] binaries, string platformDirectoryName, Device device )
        {
            throw new NotImplementedException("SaveDeviceBinary not implemented");
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
        [XmlIgnore()]
        public Dictionary<string, string> KeyValueMap = new Dictionary<string, string>();

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
