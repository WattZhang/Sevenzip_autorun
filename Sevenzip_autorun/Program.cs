using SevenZip;

internal class Program
{
    const int MBsize = 1024 * 1024;
    private static void Main(string[] args)
    {
        SevenZip.SevenZipBase.SetLibraryPath(@"D:\7-Zip\7z.dll");
        string sourcedir;//= @"L:\游戏\解压用缓冲区\7ziptest";
        Console.Write("输入解压目录:");
        sourcedir = Console.ReadLine();
        if (!Path.Exists(sourcedir))
            Console.Write("目录不存在，中断");
        else
        {
            //拉取完整任务列表
            var l = ShowFiles(sourcedir);
            int number = GetNumber();
            //鲁棒性检查
            if (number > 0 & number <= l.Length)
            {
                //单层解压
                MustFinish(l[number - 1]);
            }
            else
            {
                Console.WriteLine("未存在的文件序号");
                number = GetNumber();
            }
            Console.WriteLine("Mission Complete!");
        }
    }

    /// <summary>
    /// 获取指定目录下的压缩文件
    /// </summary>
    /// <param name="inpath">目录名</param>
    /// <returns></returns>
    public static List<string> GetZipFiles(string inpath)
    {
        List<string> outlist = new List<string>();
        var zipfilesExtensions = new string[] { ".zip", ".rar", ".7z", ".jpg" };
        foreach (var pathone in Directory.GetFiles(inpath,"",SearchOption.AllDirectories))
        {
            foreach (var ext in zipfilesExtensions)
            {
                if (pathone.EndsWith(ext))
                {
                    outlist.Add(pathone);
                }
            }
        }
        ;
        return outlist;
    }

    /// <summary>
    /// 拉取完整任务列表
    /// </summary>
    /// <param name="inpath"></param>
    /// <returns></returns>
    public static string[] ShowFiles(string inpath)
    {
        int step = 1;
        var l = GetZipFiles(inpath).ToArray();
        foreach (var file in l)
        {
            Console.WriteLine(step++.ToString() + ": " + file);
        }
        return l;
    }

    public static bool IsUnpackFinish(string inpath) 
    {
        //fix 20250218 搜索文件子目录
        if (Directory.GetFiles(inpath, "", SearchOption.AllDirectories).Length == GetZipFiles(inpath).Count)
            return false;
        else
            return true;
    }

    /// <summary>
    /// 至死方休解压模式
    /// </summary>
    /// <param name="inpath"></param>
    /// <returns>解压目录</returns>
    public static string MustFinish(string inpath) 
    {
        var f = Path.GetFileNameWithoutExtension(inpath);
        Console.WriteLine(f);
        var target = Path.Join(Path.GetDirectoryName(inpath), f);
        try
        {
            Console.WriteLine("正在解压 " + inpath);
            Unzip(inpath, target);
            Console.WriteLine("完成 输出到" + target);
        }
        catch (Exception ex) 
        {
            Console.WriteLine("Error code:1");
        }
        if (IsUnpackFinish(target))
        {
            //Todo:解压文件移动 游戏和最后一层解压文件 清理中间文件 一定要确认！！
            return target;
        }
        else
        {
            //Todo:自动文件定位
            return MustFinish(GetZipFiles(target).ToArray()[0]);
        }
        ;
    }
    public static void Unzip(string inpath, string outpath)
    {
        SevenZipExtractor extractor = new SevenZipExtractor(inpath);
        extractor.ExtractArchive(outpath);
    }

    public static int GetNumber() 
    {
        //解压序号
        Console.Write("输入解压序号:");
        int uppack = 0;
        try
        {
            uppack = int.Parse(Console.ReadLine());
            return uppack;
        }
        catch (Exception e)
        {
            Console.WriteLine("输入错误,重新输入");
            return GetNumber();
        }
    }

    /// <summary>
    /// 获取列表中的大于指定大小的文件
    /// </summary>
    /// <param name="inpath"></param>
    /// <param name="size">1MB 为 1024 * 1024</param>
    /// <returns></returns>
    public static List<string> Bigthan(List<string> inpath, int size)
    {
        List<string> outlist = new List<string>();
        foreach (var file in inpath)
        {
            if (new FileInfo(file).Length > size)
            {
                outlist.Add(file);
            }
        }
        return null;
    }
}