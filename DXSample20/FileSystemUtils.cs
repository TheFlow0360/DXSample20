using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace DXSample20
{
    public abstract class FileSystemDataProviderBase
    {
        public abstract string[] GetLogicalDrives();
        public abstract string[] GetDirectories(string path);
        public abstract string[] GetFiles(string path);
        public abstract string GetDirectoryName(string path);
        public abstract string GetFileName(string path);
        public abstract string GetFileSize(string path);
        internal string GetFileSize(long size)
        {
            if (size >= 1024)
                return string.Format("{0:### ### ###} KB", size / 1024);
            return string.Format("{0} Bytes", size);
        }
    }

    public class FileSystemHelper : FileSystemDataProviderBase
    {

        public override string[] GetLogicalDrives()
        {
            return Directory.GetLogicalDrives();
        }

        public override string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public override string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public override string GetDirectoryName(string path)
        {
            return new DirectoryInfo(path).Name;
        }

        public override string GetFileName(string path)
        {
            return new FileInfo(path).Name;
        }

        public override string GetFileSize(string path)
        {
            long size = new FileInfo(path).Length;
            return GetFileSize(size);
        }
    }

    public class FileSystemItem
    {
        public FileSystemItem(string name, string type, string size, string fullName)
        {
            this.Name = name;
            this.ItemType = type;
            this.Size = size;
            this.FullName = fullName;
        }
        public string Name { get; set; }
        public string ItemType { get; set; }
        public string Size { get; set; }
        public string FullName { get; set; }
    }
    public class FileSystemImages
    {
        static BitmapImage fileImage;
        public static BitmapImage FileImage
        {
            get
            {
                if (fileImage == null)
                    fileImage = LoadImage("File");
                return fileImage;
            }
        }
        static BitmapImage diskImage;
        public static BitmapImage DiskImage
        {
            get
            {
                if (diskImage == null)
                    diskImage = LoadImage("Local_Disk");
                return diskImage;
            }
        }
        static BitmapImage closedFolderImage;
        public static BitmapImage ClosedFolderImage
        {
            get
            {
                if (closedFolderImage == null)
                    closedFolderImage = LoadImage("Folder_Closed");
                return closedFolderImage;
            }
        }
        static BitmapImage openedFolderImage;
        public static BitmapImage OpenedFolderImage
        {
            get
            {
                if (openedFolderImage == null)
                    openedFolderImage = LoadImage("Folder_Opened");
                return openedFolderImage;
            }
        }
        static BitmapImage LoadImage(string imageName)
        {
            return new BitmapImage(new Uri("/DXSample20;component/Images/" + imageName + ".png", UriKind.Relative));
        }
    }

    public sealed class DataHelper : FileSystemHelper
    {
        static readonly DataHelper instanceCore = new DataHelper();
        DataHelper() { }
        public static DataHelper Instance
        {
            get { return instanceCore; }
        }
        public long GetFileNumSize(string path)
        {
            return new FileInfo(path).Length;
        }
        public long GetFolderSize(string fullName)
        {
            DirectoryInfo info = new DirectoryInfo(fullName);
            return GetFolderSize(info);
        }
        long GetFolderSize(DirectoryInfo d)
        {
            long Size = 0;
            FileInfo[] fis = { };
            try
            {
                fis = d.GetFiles();
            }
            catch
            {
            }
            if (fis.Length != 0)
            {
                foreach (FileInfo fi in fis)
                {
                    Size += fi.Length;
                }
            }
            DirectoryInfo[] dis = { };
            try
            {
                dis = d.GetDirectories();
            }
            catch
            {
            }
            if (dis.Length != 0)
            {
                foreach (DirectoryInfo di in dis)
                {
                    Size += GetFolderSize(di);
                }
            }
            return Size;
        }
    }

    public class FileSystemItemSize
    {
        const int kb = 1024;
        const int mb = 1048576;

        public const string Folder = "<Folder>";
        public const string Drive = "<Drive>";
        public const string Calculating = "Calculating";

        public string DisplaySize { get; private set; }
        public long NumSize { get; private set; }

        public event EventHandler<ItemSizeChangedEventArgs> SizeChanged;
        void OnSizeChanged()
        {
            if (SizeChanged != null) SizeChanged(this, new ItemSizeChangedEventArgs(this));
        }

        public void Change(long size)
        {
            NumSize = size;
            DisplaySize = FileSizeToString(size);
            OnSizeChanged();
        }
        public void Change(string displaySize)
        {
            DisplaySize = displaySize;
            OnSizeChanged();
        }

        public FileSystemItemSize(string displaySize)
        {
            Change(displaySize);
        }
        public FileSystemItemSize(long size)
        {
            Change(size);
        }

        public bool IsCalculated()
        {
            return (DisplaySize != FileSystemItemSize.Calculating &&
                DisplaySize != FileSystemItemSize.Drive &&
                DisplaySize != FileSystemItemSize.Folder);
        }

        string FileSizeToString(long size)
        {
            if (size > mb)
                return string.Format("{0:### ### ###} MB", size / mb);
            else if (size > kb)
                return string.Format("{0:### ### ###} KB", size / kb);
            else
                return string.Format("{0} Bytes", size);
        }
        public override string ToString()
        {
            return DisplaySize;
        }
    }

    public class ItemSizeChangedEventArgs : EventArgs
    {
        public FileSystemItemSize Size { get; set; }
        public ItemSizeChangedEventArgs(FileSystemItemSize itemSize)
        {
            Size = itemSize;
        }
    }
}