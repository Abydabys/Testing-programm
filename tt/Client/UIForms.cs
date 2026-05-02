using tt.Client;
using tt.Models;

namespace tt.UI
{
    //  Test Selection Form

    public partial class TestSelectionForm : Form
    {
        private readonly NetworkServiceContainer _serviceContainer;
        private readonly Models.User             _currentUser;

        private DataGridView dgvTests;
        private Button       btnStartTest;
        private Label        lblWelcome;

        public TestSelectionForm(Models.User user, NetworkServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer;
            _currentUser      = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text          = "Available Tests";
            ClientSize    = new Size(800, 580);
            StartPosition = FormStartPosition.CenterScreen;

            lblWelcome = new Label
            {
                Location  = new Point(20, 15),
                Size      = new Size(760, 35),
                Font      = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize  = false
            };

            dgvTests = new DataGridView
            {
                Location               = new Point(20, 60),
                Size                   = new Size(760, 390),
                ReadOnly               = true,
                SelectionMode          = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect            = false,
                AllowUserToAddRows     = false,
                AllowUserToDeleteRows  = false,
                AutoSizeColumnsMode    = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnStartTest = new Button
            {
                Location  = new Point(310, 470),
                Size      = new Size(180, 40),
                Text      = "▶  Start Test",
                Font      = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnStartTest.Click += BtnStartTest_Click;

            Controls.AddRange(new Control[] { lblWelcome, dgvTests, btnStartTest });
            Load += TestSelectionForm_Load;

            ((System.ComponentModel.ISupportInitialize)dgvTests).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvTests).EndInit();
        }

        private async void TestSelectionForm_Load(object sender, EventArgs e)
        {
            try
            {
                lblWelcome.Text = $"Welcome, {_currentUser.FullName}!";
                var tests = await _serviceContainer.TestService.GetAllPublishedTestsAsync();

                dgvTests.DataSource = tests.Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.MaxAttempts
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load tests: {ex.Message}", "Error");
                Close();
            }
        }

        private async void BtnStartTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTests.SelectedRows.Count == 0) { MessageBox.Show("Please select a test first."); return; }

                int testId = (int)dgvTests.SelectedRows[0].Cells["Id"].Value;

                bool canAttempt = await _serviceContainer.TestAttemptService
                    .CanUserAttemptTestAsync(_currentUser.Id, testId);

                if (!canAttempt) { MessageBox.Show("You have used all available attempts for this test."); return; }

                var attempt = await _serviceContainer.TestAttemptService
                    .StartTestAsync(_currentUser.Id, testId);

                if (attempt == null) { MessageBox.Show("Failed to start the test."); return; }

                var testingForm = new TestingForm(_currentUser, attempt, _serviceContainer);
                Hide();
                testingForm.ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}\n\n{ex.StackTrace}");
            }
        }
    }

    //  Testing Form

    public partial class TestingForm : Form
    {
        private readonly NetworkServiceContainer _serviceContainer;
        private readonly Models.User             _currentUser;
        private readonly Models.TestAttempt      _testAttempt;

        private List<Models.Question> _questions = new();
        private int _currentQuestionIndex = 0;

        private Label      lblProgress;
        private Label      lblQuestion;
        private PictureBox picQuestion;
        private Panel      pnlAnswers;
        private Button     btnPrevious;
        private Button     btnNext;
        private Button     btnFinish;

        public TestingForm(Models.User user, Models.TestAttempt testAttempt, NetworkServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer;
            _currentUser      = user;
            _testAttempt      = testAttempt;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text          = "Test Taking";
            ClientSize    = new Size(1000, 680);
            StartPosition = FormStartPosition.CenterScreen;

            lblProgress = new Label { Location = new Point(20, 20), Size = new Size(960, 25), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            lblQuestion = new Label { Location = new Point(20, 55), Size = new Size(960, 65), Font = new Font("Segoe UI", 11), AutoEllipsis = true };

            picQuestion = new PictureBox
            {
                Location = new Point(20, 125),
                Size     = new Size(400, 200),
                SizeMode = PictureBoxSizeMode.Zoom,
                Visible  = false,
                BorderStyle = BorderStyle.FixedSingle
            };

            pnlAnswers = new Panel
            {
                Location   = new Point(20, 140),
                Size       = new Size(960, 490),
                AutoScroll = true
            };

            btnPrevious = new Button { Text = "◀ Previous", Location = new Point(20,  615), Size = new Size(120, 35) };
            btnNext     = new Button { Text = "Next ▶",     Location = new Point(150, 615), Size = new Size(120, 35) };
            btnFinish   = new Button
            {
                Text      = "Finish Test",
                Location  = new Point(860, 615),
                Size      = new Size(120, 35),
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnPrevious.Click += BtnPrevious_Click;
            btnNext.Click     += BtnNext_Click;
            btnFinish.Click   += BtnFinish_Click;
            Load += TestingForm_Load;

            Controls.AddRange(new Control[] { lblProgress, lblQuestion, picQuestion, pnlAnswers, btnPrevious, btnNext, btnFinish });
        }

        private async void TestingForm_Load(object sender, EventArgs e)
        {
            try
            {
                var questions = await _serviceContainer.QuestionService
                    .GetQuestionsByTestIdAsync(_testAttempt.TestId);
                _questions = questions.ToList();

                if (_questions.Count == 0) { MessageBox.Show("This test has no questions."); Close(); return; }
                DisplayQuestion(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load questions: {ex.Message}");
                Close();
            }
        }

        private void DisplayQuestion(int index)
        {
            if (index < 0 || index >= _questions.Count) return;

            var q = _questions[index];
            lblQuestion.Text = q.Text;
            pnlAnswers.Controls.Clear();

            if (q.ImageData != null && q.ImageData.Length > 0)
            {
                using var ms = new MemoryStream(q.ImageData);
                picQuestion.Image   = Image.FromStream(ms);
                picQuestion.Visible = true;
                pnlAnswers.Top      = picQuestion.Bottom + 10;
                pnlAnswers.Height   = btnFinish.Top - pnlAnswers.Top - 10;
            }
            else
            {
                picQuestion.Visible = false;
                pnlAnswers.Top      = 140;
                pnlAnswers.Height   = btnFinish.Top - 150;
            }

            int y = 5;
            foreach (var answer in q.Answers)
            {
                Control ctrl;
                if (q.Type == Models.QuestionType.SingleChoice)
                    ctrl = new RadioButton { Text = answer.Text, Tag = answer.Id, Location = new Point(5, y), Width = 900, AutoSize = false, Height = 30 };
                else
                    ctrl = new CheckBox   { Text = answer.Text, Tag = answer.Id, Location = new Point(5, y), Width = 900, AutoSize = false, Height = 30 };

                ctrl.Font = new Font("Segoe UI", 10);
                pnlAnswers.Controls.Add(ctrl);
                y += 36;
            }

            lblProgress.Text    = $"Question {index + 1} / {_questions.Count}";
            btnPrevious.Enabled = index > 0;
            btnNext.Enabled     = index < _questions.Count - 1;
        }

        private async Task SaveCurrentAnswer()
        {
            if (_currentQuestionIndex >= _questions.Count) return;
            var q = _questions[_currentQuestionIndex];

            var selected = pnlAnswers.Controls
                .Cast<Control>()
                .Where(c => (c is RadioButton rb && rb.Checked) || (c is CheckBox cb && cb.Checked))
                .Select(c => (int)c.Tag!)
                .ToList();

            if (!selected.Any()) return;

            if (q.Type == Models.QuestionType.SingleChoice)
                await _serviceContainer.TestAttemptService.SubmitAnswerAsync(_testAttempt.Id, q.Id, selected.First());
            else
                await _serviceContainer.TestAttemptService.SubmitMultipleAnswersAsync(_testAttempt.Id, q.Id, selected);
        }

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            await SaveCurrentAnswer();
            DisplayQuestion(++_currentQuestionIndex);
        }

        private async void BtnPrevious_Click(object sender, EventArgs e)
        {
            DisplayQuestion(--_currentQuestionIndex);
        }

        private async void BtnFinish_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to finish the test?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                await SaveCurrentAnswer();

                var completed = await _serviceContainer.TestAttemptService.CompleteTestAsync(_testAttempt.Id);
                if (completed == null) { MessageBox.Show("Failed to complete test. Please try again."); return; }

                using var rf = new ResultsForm(completed);
                var result   = rf.ShowDialog();

                if (result == DialogResult.Retry)
                {
                    var selectionForm = new TestSelectionForm(_currentUser, _serviceContainer);
                    Hide();
                    selectionForm.ShowDialog();
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error completing test: {ex.Message}");
            }
        }
    }

    //  Results Form

    public partial class ResultsForm : Form
    {
        private readonly Models.TestAttempt _attempt;

        public ResultsForm(Models.TestAttempt testAttempt)
        {
            _attempt = testAttempt;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text            = "Test Results";
            ClientSize      = new Size(600, 480);
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            Load += ResultsForm_Load;
        }

        private void ResultsForm_Load(object sender, EventArgs e)
        {
            BuildResultsUI();
        }

        private void BuildResultsUI()
        {
            double pct   = _attempt.Percentage;
            string grade = pct >= 90 ? "A" : pct >= 75 ? "B" : pct >= 60 ? "C" : pct >= 50 ? "D" : "F";
            bool   passed = pct >= 60;

            Color gradeColor = grade switch
            {
                "A" => Color.FromArgb(0, 160, 80),
                "B" => Color.FromArgb(0, 130, 60),
                "C" => Color.DarkOrange,
                "D" => Color.Orange,
                _   => Color.Crimson
            };

            var banner = new Panel { Location = new Point(0, 0), Size = new Size(600, 100),
                BackColor = passed ? Color.FromArgb(20, 160, 80) : Color.FromArgb(180, 40, 40) };

            var lblTitle = new Label
            {
                Text      = "Test Results",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 22, FontStyle.Bold),
                Location  = new Point(0, 20),
                Size      = new Size(600, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };
            banner.Controls.Add(lblTitle);

            var lblScore = new Label
            {
                Text      = $"Score:   {_attempt.Score} / {_attempt.MaxScore}",
                Location  = new Point(0, 115), Size = new Size(600, 36),
                Font      = new Font("Segoe UI", 13),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblPct = new Label
            {
                Text      = $"Percentage:   {pct:F1}%",
                Location  = new Point(0, 155), Size = new Size(600, 36),
                Font      = new Font("Segoe UI", 13),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblGrade = new Label
            {
                Text      = grade,
                Location  = new Point(0, 195), Size = new Size(600, 80),
                Font      = new Font("Segoe UI", 52, FontStyle.Bold),
                ForeColor = gradeColor,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblMsg = new Label
            {
                Text      = passed ? "🎉  Congratulations, you passed!" : "💪  Better luck next time!",
                Location  = new Point(0, 280), Size = new Size(600, 36),
                Font      = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = passed ? Color.DarkGreen : Color.Crimson,
                TextAlign = ContentAlignment.MiddleCenter
            };

            //Buttons 

            var btnAnotherTest = new Button
            {
                Text      = "Take Another Test",
                Location  = new Point(80, 370),
                Size      = new Size(180, 42),
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(30, 120, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAnotherTest.FlatAppearance.BorderSize = 0;
            btnAnotherTest.Click += (_, __) =>
            {
                DialogResult = DialogResult.Retry;   // TestingForm checks this
                Close();
            };

            var btnExit = new Button
            {
                Text      = "Exit",
                Location  = new Point(340, 370),
                Size      = new Size(180, 42),
                Font      = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            btnExit.Click += (_, __) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.AddRange(new Control[]
            {
                banner, lblScore, lblPct, lblGrade, lblMsg, btnAnotherTest, btnExit
            });
        }
    }
}
