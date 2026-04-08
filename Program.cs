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
        List<Mission> missions = GetMissions();
        List<MissionResult> results = AskMissions(missions);

        int completedCount = CountCompleted(results);
        int totalScore = CalculateScore(results);
        string message = CreateMessage(totalScore);

        ShowSummary(childName, results, completedCount, missions.Count, totalScore, message);
        SaveToCsv(childName, results, totalScore);

        Console.WriteLine();
        Console.WriteLine("Enterキーを押すと終了します。");
        Console.ReadLine();
    }

    static string SelectChild()
    {
        while (true)
        {
            Console.WriteLine("だれの記録をしますか？");
            Console.WriteLine("1: かける");
            Console.WriteLine("2: たく");
            Console.Write("番号を入力してください: ");

            string? input = Console.ReadLine();

            if (input == "1")
            {
                return "かける";
            }

            if (input == "2")
            {
                return "たく";
            }

            Console.WriteLine("1 か 2 を入力してください。");
            Console.WriteLine();
        }
    }

    static List<Mission> GetMissions()
    {
        return new List<Mission>
        {
            new Mission("学校の宿題", 20),
            new Mission("計算ドリル", 10),
            new Mission("チャレンジタッチ", 10),
            new Mission("バイオリンの練習", 20),
            new Mission("本を読んだ", 10),
            new Mission("たくさん走った", 10),
            new Mission("ごはんをちゃんと食べた", 10),
            new Mission("明日のしたく", 10),
            new Mission("歯みがき", 5),
            new Mission("おふろ", 5)
        };
    }

    static List<MissionResult> AskMissions(List<Mission> missions)
    {
        var results = new List<MissionResult>();

        Console.WriteLine();
        Console.WriteLine("各項目について、やったら y、やっていなければ n を入力してください。");
        Console.WriteLine();

        foreach (Mission mission in missions)
        {
            bool done = AskYesNo($"{mission.Name} はやった？ (y/n): ");
            results.Add(new MissionResult(mission.Name, mission.Score, done));
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

    static int CalculateScore(List<MissionResult> results)
    {
        int total = 0;

        foreach (MissionResult result in results)
        {
            if (result.Done)
            {
                total += result.Score;
            }
        }

        return total;
    }

    static string CreateMessage(int totalScore)
    {
        if (totalScore >= 90)
        {
            return "すごい！最高レベル！";
        }

        if (totalScore >= 70)
        {
            return "かなりがんばったね！";
        }

        if (totalScore >= 50)
        {
            return "よくがんばった！";
        }

        return "明日はもっとできるよ！";
    }

    static void ShowSummary(
        string childName,
        List<MissionResult> results,
        int completedCount,
        int totalCount,
        int totalScore,
        string message)
    {
        Console.WriteLine();
        Console.WriteLine("=== 今日の結果 ===");
        Console.WriteLine($"名前: {childName}");
        Console.WriteLine($"達成: {completedCount} / {totalCount}");
        Console.WriteLine($"合計点: {totalScore} 点");
        Console.WriteLine();

        foreach (MissionResult result in results)
        {
            string mark = result.Done ? "〇" : "×";
            Console.WriteLine($"{mark} {result.Name} ({result.Score}点)");
        }

        Console.WriteLine();
        Console.WriteLine($"メッセージ: {message}");
    }

    static void SaveToCsv(string childName, List<MissionResult> results, int totalScore)
    {
        string filePath = "mission_log.csv";
        bool fileExists = File.Exists(filePath);

        using var writer = new StreamWriter(filePath, append: true, Encoding.UTF8);

        if (!fileExists)
        {
            writer.WriteLine("Date,ChildName,Mission,Score,Done,TotalScore");
        }

        string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        foreach (MissionResult result in results)
        {
            string doneValue = result.Done ? "1" : "0";
            writer.WriteLine($"{today},{childName},{result.Name},{result.Score},{doneValue},{totalScore}");
        }

        Console.WriteLine();
        Console.WriteLine($"CSV保存しました: {Path.GetFullPath(filePath)}");
    }
}

class Mission
{
    public string Name { get; }
    public int Score { get; }

    public Mission(string name, int score)
    {
        Name = name;
        Score = score;
    }
}

class MissionResult
{
    public string Name { get; }
    public int Score { get; }
    public bool Done { get; }

    public MissionResult(string name, int score, bool done)
    {
        Name = name;
        Score = score;
        Done = done;
    }
}