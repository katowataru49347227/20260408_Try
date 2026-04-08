using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    private const string CsvFilePath = "mission_log.csv";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        while (true)
        {
            ShowTitle();
            ShowMenu();

            string choice = ReadMenuChoice();

            if (choice == "1")
            {
                RunMissionInput();
            }
            else if (choice == "2")
            {
                ShowWeeklySummary();
            }
            else if (choice == "3")
            {
                ShowRanking();
            }
            else if (choice == "0")
            {
                Console.WriteLine("終了します。");
                break;
            }
            else
            {
                Console.WriteLine("メニュー番号を選んでください。");
            }

            Console.WriteLine();
            Console.WriteLine("Enterキーでメニューに戻ります。");
            Console.ReadLine();
            Console.Clear();
        }
    }

    static void ShowTitle()
    {
        Console.WriteLine("====================================");
        Console.WriteLine("   おうちミッション記録ツール");
        Console.WriteLine("====================================");
        Console.WriteLine();
    }

    static void ShowMenu()
    {
        Console.WriteLine("1. 今日のミッションを入力する");
        Console.WriteLine("2. 今週の集計を見る");
        Console.WriteLine("3. かける vs たく のランキングを見る");
        Console.WriteLine("0. 終了");
        Console.WriteLine();
    }

    static string ReadMenuChoice()
    {
        Console.Write("番号を入力してください: ");
        return (Console.ReadLine() ?? "").Trim();
    }

    static void RunMissionInput()
    {
        Console.Clear();
        ShowTitle();

        string childName = SelectChild();
        List<Mission> missions = GetMissions();
        List<MissionResult> results = AskMissions(missions);

        int completedCount = CountCompleted(results);
        int totalScore = CalculateScore(results);
        string message = CreateMessage(totalScore);

        ShowSummary(childName, results, completedCount, missions.Count, totalScore, message);
        SaveToCsv(childName, results, totalScore);
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
            bool done = AskYesNo($"{mission.Name} ({mission.Score}点) はやった？ (y/n): ");
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
        bool fileExists = File.Exists(CsvFilePath);

        using var writer = new StreamWriter(CsvFilePath, append: true, Encoding.UTF8);

        if (!fileExists)
        {
            writer.WriteLine("Date,ChildName,Mission,Score,Done,TotalScore");
        }

        string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        foreach (MissionResult result in results)
        {
            string doneValue = result.Done ? "1" : "0";
            writer.WriteLine($"{today},{childName},{result.Name},{result.Score},{doneValue},{totalScore}");
        }

        Console.WriteLine();
        Console.WriteLine($"CSV保存しました: {Path.GetFullPath(CsvFilePath)}");
    }

    static void ShowWeeklySummary()
    {
        Console.Clear();
        ShowTitle();

        List<LogRecord> records = LoadCsv();

        if (records.Count == 0)
        {
            Console.WriteLine("まだ記録がありません。");
            return;
        }

        DateTime startDate = DateTime.Today.AddDays(-6);

        var weekly = records
            .Where(r => r.Date.Date >= startDate)
            .GroupBy(r => new { r.ChildName, Day = r.Date.Date })
            .Select(g => new
            {
                ChildName = g.Key.ChildName,
                Day = g.Key.Day,
                TotalScore = g.Max(x => x.TotalScore),
                CompletedCount = g.Count(x => x.Done)
            })
            .OrderBy(x => x.Day)
            .ThenBy(x => x.ChildName)
            .ToList();

        Console.WriteLine($"今週の集計（{startDate:yyyy-MM-dd} ～ {DateTime.Today:yyyy-MM-dd}）");
        Console.WriteLine("--------------------------------------------------");

        foreach (var item in weekly)
        {
            Console.WriteLine($"{item.Day:MM/dd}  {item.ChildName}  点数: {item.TotalScore}  達成数: {item.CompletedCount}");
        }

        Console.WriteLine();
        Console.WriteLine("子どもごとの今週合計");
        Console.WriteLine("----------------------");

        var childTotals = weekly
            .GroupBy(x => x.ChildName)
            .Select(g => new
            {
                ChildName = g.Key,
                TotalScore = g.Sum(x => x.TotalScore)
            })
            .OrderByDescending(x => x.TotalScore);

        foreach (var child in childTotals)
        {
            Console.WriteLine($"{child.ChildName}: {child.TotalScore} 点");
        }
    }

    static void ShowRanking()
    {
        Console.Clear();
        ShowTitle();

        List<LogRecord> records = LoadCsv();

        if (records.Count == 0)
        {
            Console.WriteLine("まだ記録がありません。");
            return;
        }

        var ranking = records
            .GroupBy(r => new { r.ChildName, Day = r.Date.Date })
            .Select(g => new
            {
                ChildName = g.Key.ChildName,
                Day = g.Key.Day,
                TotalScore = g.Max(x => x.TotalScore)
            })
            .GroupBy(x => x.ChildName)
            .Select(g => new
            {
                ChildName = g.Key,
                TotalScore = g.Sum(x => x.TotalScore),
                Days = g.Count()
            })
            .OrderByDescending(x => x.TotalScore)
            .ToList();

        Console.WriteLine("ランキング");
        Console.WriteLine("----------");

        for (int i = 0; i < ranking.Count; i++)
        {
            var item = ranking[i];
            Console.WriteLine($"{i + 1}位: {item.ChildName}  合計点: {item.TotalScore}  記録日数: {item.Days}");
        }

        if (ranking.Count >= 2)
        {
            Console.WriteLine();
            int diff = ranking[0].TotalScore - ranking[1].TotalScore;
            Console.WriteLine($"差は {diff} 点です。");
        }
    }

    static List<LogRecord> LoadCsv()
    {
        var results = new List<LogRecord>();

        if (!File.Exists(CsvFilePath))
        {
            return results;
        }

        string[] lines = File.ReadAllLines(CsvFilePath, Encoding.UTF8);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split(',');

            if (parts.Length < 6)
            {
                continue;
            }

            if (!DateTime.TryParse(parts[0], out DateTime date))
            {
                continue;
            }

            string childName = parts[1];
            string mission = parts[2];

            if (!int.TryParse(parts[3], out int score))
            {
                continue;
            }

            bool done = parts[4] == "1";

            if (!int.TryParse(parts[5], out int totalScore))
            {
                continue;
            }

            results.Add(new LogRecord(date, childName, mission, score, done, totalScore));
        }

        return results;
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

class LogRecord
{
    public DateTime Date { get; }
    public string ChildName { get; }
    public string Mission { get; }
    public int Score { get; }
    public bool Done { get; }
    public int TotalScore { get; }

    public LogRecord(DateTime date, string childName, string mission, int score, bool done, int totalScore)
    {
        Date = date;
        ChildName = childName;
        Mission = mission;
        Score = score;
        Done = done;
        TotalScore = totalScore;
    }
}