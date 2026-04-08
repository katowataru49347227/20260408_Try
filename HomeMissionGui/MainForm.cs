using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HomeMissionGui
{
    public class MainForm : Form
    {
        private readonly ComboBox _childComboBox;
        private readonly CheckedListBox _missionCheckedListBox;
        private readonly Button _calculateButton;
        private readonly Button _saveButton;
        private readonly Button _clearButton;
        private readonly Label _resultLabel;
        private readonly Label _messageLabel;

        private readonly List<Mission> _missions;

        private int _lastScore = 0;

        public MainForm()
        {
            Text = "おうちミッション記録ツール";
            Width = 520;
            Height = 620;
            StartPosition = FormStartPosition.CenterScreen;

            _missions = CreateMissions();

            var titleLabel = new Label
            {
                Text = "おうちミッション記録ツール",
                Left = 20,
                Top = 20,
                Width = 400,
                Height = 30
            };

            var childLabel = new Label
            {
                Text = "名前を選んでください",
                Left = 20,
                Top = 70,
                Width = 200,
                Height = 25
            };

            _childComboBox = new ComboBox
            {
                Left = 20,
                Top = 100,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _childComboBox.Items.Add("かける");
            _childComboBox.Items.Add("たく");
            _childComboBox.SelectedIndex = 0;

            var missionLabel = new Label
            {
                Text = "今日やったことをチェックしてください",
                Left = 20,
                Top = 145,
                Width = 300,
                Height = 25
            };

            _missionCheckedListBox = new CheckedListBox
            {
                Left = 20,
                Top = 175,
                Width = 440,
                Height = 260,
                CheckOnClick = true
            };

            foreach (var mission in _missions)
            {
                _missionCheckedListBox.Items.Add($"{mission.Name} ({mission.Score}点)");
            }

            _calculateButton = new Button
            {
                Text = "点数を計算",
                Left = 20,
                Top = 455,
                Width = 120,
                Height = 35
            };
            _calculateButton.Click += CalculateButton_Click;

            _saveButton = new Button
            {
                Text = "CSV保存",
                Left = 155,
                Top = 455,
                Width = 120,
                Height = 35
            };
            _saveButton.Click += SaveButton_Click;

            _clearButton = new Button
            {
                Text = "チェックをクリア",
                Left = 290,
                Top = 455,
                Width = 170,
                Height = 35
            };
            _clearButton.Click += ClearButton_Click;

            _resultLabel = new Label
            {
                Text = "合計点: 0 点",
                Left = 20,
                Top = 510,
                Width = 200,
                Height = 30
            };

            _messageLabel = new Label
            {
                Text = "メッセージ: まだ計算していません",
                Left = 20,
                Top = 545,
                Width = 440,
                Height = 30
            };

            Controls.Add(titleLabel);
            Controls.Add(childLabel);
            Controls.Add(_childComboBox);
            Controls.Add(missionLabel);
            Controls.Add(_missionCheckedListBox);
            Controls.Add(_calculateButton);
            Controls.Add(_saveButton);
            Controls.Add(_clearButton);
            Controls.Add(_resultLabel);
            Controls.Add(_messageLabel);
        }

        private List<Mission> CreateMissions()
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

        private void CalculateButton_Click(object? sender, EventArgs e)
        {
            int totalScore = 0;

            for (int i = 0; i < _missionCheckedListBox.Items.Count; i++)
            {
                if (_missionCheckedListBox.GetItemChecked(i))
                {
                    totalScore += _missions[i].Score;
                }
            }

            _lastScore = totalScore;
            _resultLabel.Text = $"合計点: {totalScore} 点";
            _messageLabel.Text = $"メッセージ: {CreateMessage(totalScore)}";
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            string childName = _childComboBox.SelectedItem?.ToString() ?? "未選択";
            string filePath = "mission_log.csv";
            bool fileExists = File.Exists(filePath);

            using var writer = new StreamWriter(filePath, append: true, Encoding.UTF8);

            if (!fileExists)
            {
                writer.WriteLine("Date,ChildName,Mission,Score,Done,TotalScore");
            }

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            for (int i = 0; i < _missions.Count; i++)
            {
                bool done = _missionCheckedListBox.GetItemChecked(i);
                string doneValue = done ? "1" : "0";

                writer.WriteLine(
                    $"{now},{childName},{_missions[i].Name},{_missions[i].Score},{doneValue},{_lastScore}");
            }

            MessageBox.Show(
                $"CSVに保存しました。\n{Path.GetFullPath(filePath)}",
                "保存完了",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            for (int i = 0; i < _missionCheckedListBox.Items.Count; i++)
            {
                _missionCheckedListBox.SetItemChecked(i, false);
            }

            _lastScore = 0;
            _resultLabel.Text = "合計点: 0 点";
            _messageLabel.Text = "メッセージ: まだ計算していません";
        }

        private string CreateMessage(int totalScore)
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
    }

    public class Mission
    {
        public string Name { get; }
        public int Score { get; }

        public Mission(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}