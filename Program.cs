using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== おうちミッション記録ツール ===");
        Console.WriteLine();

        string childName = SelectChild();
        List<string> missions = GetMissions();
        List<MissionResult> results = AskMissions(missions);

        int completedCount = CountCompleted(results);
        string message = CreateMessage(completedCount, missions.Count);

        ShowSummary(childName, results, completedCount, missions.Count, message);
        SaveToCsv(childName, results);

        Console.WriteLine();
        Console.WriteLine("Enterキーを押すと終了します。");
        Console.ReadLine();
    }

    static string SelectChild()
    {
        while (true)
        {
            Console.WriteLine("だれの記録をしますか？");
            Console.WriteLine("1: 兄");
            Console.WriteLine("2: 弟");
            Console.Write("番号を入力してください: ");

            string? input = Console.ReadLine();

            if (input == "1")
            {
                return "兄";
            }

            if (input == "2")
            {
                return "弟";
            }

            Console.WriteLine("1 か 2 を入力してください。");
            Console.WriteLine();
        }
    }

    static List<string> GetMissions()
    {
        return new List<string>
        {
            "学校の宿題",
            "計算ドリル",
            "チャレンジタッチ",
            "バイオリンの練習",
            "本を読んだ",
            "たくさん走った",
            "ごはんをちゃんと食べた",
            "明日のしたく",
            "歯みがき",
            "おふろ"
        };
    }

    static List<MissionResult> AskMissions(List<string> missions)
    {
        var results = new List<MissionResult>();

        Console.WriteLine();
        Console.WriteLine("各項目について、やったら y、やっていなければ n を入力してください。");
        Console.WriteLine();

        foreach (string mission in missions)
        {
            bool done = AskYesNo($"{mission} はやった？ (y/n): ");
            results.Add(new MissionResult(mission, done));
        }

        return results;
    }

    static bool AskYesNo(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("y か n を入力してください。");
                continue;
            }

            string normalized = input.Trim().ToLower();

            if (normalized == "y")
            {
                return true;
            }

            if (normalized == "n")
            {
                return false;
            }

            Console.WriteLine("y か n を入力してください。");
        }
    }

    static int CountCompleted(List<MissionResult> results)
    {
        int count = 0;

        foreach (MissionResult result in results)
        {
            if (result.Done)
            {
                count++;
            }
        }

        return count;
    }

    static string CreateMessage(int completedCount, int totalCount)
    {
        if (completedCount == totalCount)
        {
            return "すごい！ぜんぶできたね！";
        }

        if (completedCount >= 8)
        {
            return "かなりがんばったね！";
        }

        if (completedCount >= 5)
        {
            return "よくがんばった！";
        }

        return "明日はもう少しがんばろう！";
    }

    static void ShowSummary(
        string childName,
        List<MissionResult> results,
        int completedCount,
        int totalCount,
        string message)
    {
        Console.WriteLine();
        Console.WriteLine("=== 今日の結果 ===");
        Console.WriteLine($"名前: {childName}");
        Console.WriteLine($"達成: {completedCount} / {totalCount}");
        Console.WriteLine();

        foreach (MissionResult result in results)
        {
            string mark = result.Done ? "〇" : "×";
            Console.WriteLine($"{mark} {result.Name}");
        }

        Console.WriteLine();
        Console.WriteLine($"メッセージ: {message}");
    }

    static void SaveToCsv(string childName, List<MissionResult> results)
    {
        string filePath = "mission_log.csv";
        bool fileExists = File.Exists(filePath);

        using var writer = new StreamWriter(filePath, append: true, Encoding.UTF8);

        if (!fileExists)
        {
            writer.WriteLine("Date,ChildName,Mission,Done");
        }

        string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        foreach (MissionResult result in results)
        {
            string doneValue = result.Done ? "1" : "0";
            writer.WriteLine($"{today},{childName},{result.Name},{doneValue}");
        }

        Console.WriteLine();
        Console.WriteLine($"CSV保存しました: {Path.GetFullPath(filePath)}");
    }
}

class MissionResult
{
    public string Name { get; }
    public bool Done { get; }

    public MissionResult(string name, bool done)
    {
        Name = name;
        Done = done;
    }
}
