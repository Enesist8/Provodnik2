using System.Diagnostics;
using System.Text;
namespace ConsoleApp1
{
    class FilePanel
    {
        List<FileSystemInfo> fs = new List<FileSystemInfo>();
        int top;
        int left;
        int height = 18;
        int width = 120;
        string path;
        int AIndex = 0;
        int FIndex = 0;
        int Amount = 16;
        public bool discs1;
        bool active;
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                DirectoryInfo di = new DirectoryInfo(value);
                if (di.Exists)
                {
                    path = value;
                }
            }
        }
        public FilePanel()
        {
            SetDiscs();
        }
        public FileSystemInfo GetActiveObject()
        {
            if (fs != null && fs.Count != 0)
            {
                return fs[AIndex];
            }
            throw new Exception();
        }
        public void Key(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    Up();
                    break;
                case ConsoleKey.DownArrow:
                    Down();
                    break;
            }
        }
        private void Down()
        {
            if (AIndex >= FIndex + Amount - 1)
            {
                FIndex += 1;
                if (FIndex + Amount >= fs.Count)
                {
                    FIndex = fs.Count - Amount;
                }
                AIndex = FIndex + Amount - 1;
                UpdateContent(false);
            }
            else
            {
                if (AIndex >= fs.Count - 1)
                {
                    return;
                }
                DeactivateObject(AIndex);
                AIndex++;
                ActivateObject(AIndex);
            }
        }
        private void Up()
        {
            if (AIndex <= FIndex)
            {
                FIndex -= 1;
                if (FIndex < 0)
                {
                    FIndex = 0;
                }
                AIndex = FIndex;
                UpdateContent(false);
            }
            else
            {
                DeactivateObject(AIndex);
                AIndex--;
                ActivateObject(AIndex);
            }
        }
        public void SetLists()
        {
            if (fs.Count != 0)
            {
                fs.Clear();
            }
            discs1 = false;
            DirectoryInfo levelUpDirectory = null;
            fs.Add(levelUpDirectory);
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                fs.Add(di);
            }
            string[] files = Directory.GetFiles(this.path);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                fs.Add(fi);
            }
        }
        public void SetDiscs()
        {
            if (fs.Count != 0)
            {
                fs.Clear();
            }
            discs1 = true;
            DriveInfo[] discs = DriveInfo.GetDrives();
            foreach (DriveInfo disc in discs)
            {
                if (disc.IsReady)
                {
                    DirectoryInfo di = new DirectoryInfo(disc.Name);
                    fs.Add(di);
                }
            }
        }
        public void borders()
        {
            Clear();
            StringBuilder caption = new StringBuilder();
            FileManager.PositionString(caption.ToString(), left + width / 2 - caption.ToString().Length / 2, top);
            PrintContent();
        }
        public void Clear()
        {
            for (int i = 0; i < height; i++)
            {
                string space = new String(' ', width);
                Console.SetCursorPosition(left, top + i);
                Console.Write(space);
            }
        }
        private void PrintContent()
        {
            if (fs.Count == 0)
            {
                return;
            }
            int count = 0;
            int lastElement = FIndex + Amount;
            if (lastElement > fs.Count)
            {
                lastElement = fs.Count;
            }
            if (AIndex >= fs.Count)
            {
                AIndex = 0;
            }
            for (int i = FIndex; i < lastElement; i++)
            {
                Console.SetCursorPosition(left + 1, top + count + 1);
                if (i == AIndex && active == true)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                PrintObject(i);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                count++;
            }
        }
        private void PrintObject(int index)
        {
            int currentCursorTopPosition = Console.CursorTop;
            int currentCursorLeftPosition = Console.CursorLeft;
            if (!discs1 && index == 0)
            {
                Console.Write("...");
                return;
            }
            Console.Write("{0}", fs[index].Name);
            Console.SetCursorPosition(currentCursorLeftPosition + width / 2, currentCursorTopPosition);
            if (fs[index] is DirectoryInfo)
            {
                Console.Write("{0}", ((DirectoryInfo)fs[index]).LastWriteTime);
            }
            else
            {
                Console.Write("{0}", ((FileInfo)fs[index]).Length);
            }
        }
        public void Panel()
        {
            FIndex = 0;
            AIndex = 0;
            borders();
        }
        public void UpdateContent(bool updateList)
        {
            if (updateList)
            {
                SetLists();
            }
            Clear();
            PrintContent();
        }
        private void ActivateObject(int index)
        {
            int offsetY = AIndex - FIndex;
            Console.SetCursorPosition(left + 1, top + offsetY + 1);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            PrintObject(index);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private void DeactivateObject(int index)
        {
            int offsetY = AIndex - FIndex;
            Console.SetCursorPosition(left + 1, top + offsetY + 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            PrintObject(index);
        }
    }
}
namespace ConsoleApp1
{
    public delegate void Key(ConsoleKeyInfo key);
    class FileManager
    {
        public event Key KeyPress;
        FilePanel filePanel = new FilePanel();
        public FileManager()
        {
            KeyPress += filePanel.Key;
            filePanel.borders();
            Console.SetCursorPosition(1, 18);
            Console.WriteLine("F6 Создать файл F7 Создать директорию F8 Удаление(директории/файла) ESC выход из программы");
        }
        public void Explorer()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userKey = Console.ReadKey(true);
                    switch (userKey.Key)
                    {
                        case ConsoleKey.Enter:
                            MovingInFM();
                            break;
                        case ConsoleKey.F6:
                            CreateFile();
                            break;
                        case ConsoleKey.F7:
                            CreateDirectory();
                            break;
                        case ConsoleKey.F8:
                            Delete();
                            break;
                        case ConsoleKey.DownArrow:
                            KeyPress(userKey);
                            break;
                        case ConsoleKey.UpArrow:
                            KeyPress(userKey);
                            break;
                        case ConsoleKey.Escape:
                            return;
                    }
                }
            }
        }
        private string NewFileName(string message)
        {
            string name;
            Console.CursorVisible = true;
            do
            {
                PositionString(new String(' ', Console.WindowWidth), 0, Console.WindowHeight - 2);
                PositionString(message, 0, Console.WindowHeight - 2);
                name = Console.ReadLine();
            } while (name.Length == 0);
            Console.CursorVisible = false;
            PositionString(new String(' ', Console.WindowWidth), 0, Console.WindowHeight - 2);
            return name;
        }
        private void NameError(string error)
        {
            Console.SetCursorPosition(1, 20);
            Console.Write(error);
            Thread.Sleep(5000);
            Console.SetCursorPosition(1, 20);
            Console.Write("                                     ");
        }
        private void Delete()
        {
            if (filePanel.discs1)
            {
                NameError("Здесь нельзя ничего удалить");
                return;
            }
            FileSystemInfo fileObject = filePanel.GetActiveObject();

            if (fileObject is DirectoryInfo)
            {
                Directory.Delete(Convert.ToString(fileObject), true);
            }
            else
            {
                File.Delete(Convert.ToString(fileObject));
            }
            NewPannels();
            return;
        }
        private void CreateFile()
        {
            if (filePanel.discs1)
            {
                NameError("Здесь нельзя создать файл");
                return;
            }
            string destPath = filePanel.Path;
            string FileName = NewFileName("Укажите имя файла вместе с форматом\nВведите имя файла: ");
            string FileFullName = Path.Combine(destPath, FileName);
            if (!File.Exists(FileFullName))
            {
                File.Create(FileFullName);
            }
            else
            {
                NameError("Файл с таким именем уже сущетсвует");
            }
            NewPannels();
        }
        private void CreateDirectory()
        {
            if (filePanel.discs1)
            {
                NameError("Здесь нельзя создать директорию");
                return;
            }
            string destPath = filePanel.Path;
            string dirName = NewFileName("Введите имя директории: ");
            string dirFullName = Path.Combine(destPath, dirName);
            DirectoryInfo dir = new DirectoryInfo(dirFullName);
            if (!dir.Exists)
            {
                dir.Create();
            }
            else
            {
                NameError("Директория с таким именем уже сущетсвует");
            }
            NewPannels();
        }
        public static void PositionString(string str, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(str);
        }
        private void NewPannels()
        {
            if (!filePanel.discs1)
            {
                filePanel.UpdateContent(true);
            }
            else if (filePanel == null)
            {
                return;
            }
        }
        private void MovingInFM()
        {
            FileSystemInfo fsInfo = filePanel.GetActiveObject();
            if (fsInfo != null)
            {
                if (fsInfo is DirectoryInfo)
                {
                    Directory.GetDirectories(fsInfo.FullName);
                    filePanel.Path = fsInfo.FullName;
                    filePanel.SetLists();
                    filePanel.Panel();
                }
                else
                {
                    Process.Start(new ProcessStartInfo(((FileInfo)fsInfo).FullName) { UseShellExecute = true });
                }
            }
            else
            {
                string currentPath = filePanel.Path;
                DirectoryInfo currentDirectory = new DirectoryInfo(currentPath);
                DirectoryInfo upLevelDirectory = currentDirectory.Parent;
                if (upLevelDirectory != null)
                {
                    filePanel.Path = upLevelDirectory.FullName;
                    filePanel.SetLists();
                    filePanel.Panel();
                }
                else
                {
                    filePanel.SetDiscs();
                    filePanel.Panel();
                }
            }
        }
    }
}