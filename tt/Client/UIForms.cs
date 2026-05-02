// TestSelectionForm.cs

using tt.Client;
using tt.Models;

namespace tt.UI
{
    public partial class TestSelectionForm : Form
    {
        private readonly NetworkServiceContainer _serviceContainer;
        private DataGridView dataGridView;
        private Button btnStartTest;
        private Label lblWelcome;
        private readonly Models.User _currentUser;

        public TestSelectionForm(Models.User user, NetworkServiceContainer serviceContainer)
        {
            InitializeComponent();
            _serviceContainer = serviceContainer;
            _currentUser = user;
        }

        private void InitializeComponent()
        {
            dataGridView = new DataGridView();
            btnStartTest = new Button();
            lblWelcome = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.Location = new Point(251, 153);
            dataGridView.Name = "dataGridView";
            dataGridView.Size = new Size(240, 150);
            dataGridView.TabIndex = 0;
            // 
            // btnStartTest
            // 
            btnStartTest.Location = new Point(300, 400);
            btnStartTest.Name = "btnStartTest";
            btnStartTest.Size = new Size(75, 23);
            btnStartTest.TabIndex = 0;
            btnStartTest.Click += BtnStartTest_Click;
            btnStartTest.Text = "Start Test";
            // 
            // lblWelcome
            // 
            lblWelcome.Location = new Point(300, 50);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(100, 23);
            lblWelcome.TabIndex = 0;
            // 
            // TestSelectionForm
            // 
            ClientSize = new Size(784, 561);
            Controls.Add(dataGridView);
            Controls.Add(btnStartTest);
            Controls.Add(lblWelcome);
            Name = "TestSelectionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Available Tests";
            Load += TestSelectionForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
        }

        private async void TestSelectionForm_Load(object sender, EventArgs e)
        {
            try
            {
                lblWelcome.Text = $"Welcome, {_currentUser.FullName}!";
                var tests = await _serviceContainer.TestService.GetAllPublishedTestsAsync();
                dataGridView.DataSource = tests.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load tests: {ex.Message}", "Error");
                this.Close();
            }
        }

        private async void BtnStartTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a test first.");
                    return;
                }

                int testId = (int)dataGridView.SelectedRows[0].Cells["Id"].Value;

                bool canAttempt = await _serviceContainer.TestAttemptService
                    .CanUserAttemptTestAsync(_currentUser.Id, testId);

                MessageBox.Show($"CanAttempt: {canAttempt}, UserId: {_currentUser.Id}, TestId: {testId}");

                if (!canAttempt)
                {
                    MessageBox.Show("You have used all available attempts.");
                    return;
                }

                var attempt = await _serviceContainer.TestAttemptService
                    .StartTestAsync(_currentUser.Id, testId);

                MessageBox.Show($"Attempt: {(attempt == null ? "NULL" : attempt.Id.ToString())}");

                if (attempt == null)
                {
                    MessageBox.Show("Failed to start the test.");
                    return;
                }

                var testingForm = new TestingForm(_currentUser, attempt, _serviceContainer);
                this.Hide();
                testingForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}\n\n{ex.StackTrace}");
            }
        }
    }
}

// TestingForm.cs

namespace tt.UI
{
    public partial class TestingForm : Form
    {
        private readonly Client.NetworkServiceContainer _serviceContainer;
        private readonly Models.User _currentUser;
        private readonly Models.TestAttempt _testAttempt;
        private List<Models.Question> _questions;
        private int _currentQuestionIndex = 0;

        public TestingForm(Models.User user, Models.TestAttempt testAttempt, Client.NetworkServiceContainer serviceContainer)
        {
            InitializeComponent();
            _serviceContainer = serviceContainer;
            _currentUser = user;
            _testAttempt = testAttempt;
        }

        private Label lblQuestion;
        private Panel pnlAnswers;
        private PictureBox picQuestion;
        private Label lblProgress;
        private Button btnNext;
        private Button btnPrevious;
        private Button btnFinish;

        private void InitializeComponent()
        {
            Text = "Test Taking";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;

            lblProgress = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(940, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            lblQuestion = new Label
            {
                Location = new Point(20, 55),
                Size = new Size(940, 60),
                Font = new Font("Segoe UI", 11)
            };

            picQuestion = new PictureBox
            {
                Location = new Point(20, 125),
                Size = new Size(400, 200),
                SizeMode = PictureBoxSizeMode.Zoom,
                Visible = false
            };

            pnlAnswers = new Panel
            {
                Location = new Point(20, 135),
                Size = new Size(940, 400),
                AutoScroll = true
            };

            btnPrevious = new Button
            {
                Text = "Previous",
                Location = new Point(20, 610),
                Size = new Size(100, 35)
            };

            btnNext = new Button
            {
                Text = "Next",
                Location = new Point(130, 610),
                Size = new Size(100, 35)
            };

            btnFinish = new Button
            {
                Text = "Finish Test",
                Location = new Point(860, 610),
                Size = new Size(110, 35)
            };

            Controls.Add(lblProgress);
            Controls.Add(lblQuestion);
            Controls.Add(picQuestion);
            Controls.Add(pnlAnswers);
            Controls.Add(btnPrevious);
            Controls.Add(btnNext);
            Controls.Add(btnFinish);

            Load += TestingForm_Load;
            btnNext.Click += BtnNext_Click;
            btnPrevious.Click += BtnPrevious_Click;
            btnFinish.Click += BtnFinish_Click;
        }

        private async void TestingForm_Load(object sender, EventArgs e)
        {
            try
            {
                var questions = await _serviceContainer
                    .QuestionService
                    .GetQuestionsByTestIdAsync(_testAttempt.TestId);
                _questions = questions.ToList();
                MessageBox.Show($"Loaded {_questions.Count} questions, TestId: {_testAttempt.TestId}");
                DisplayQuestion(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load questions: {ex.Message}");
                this.Close();
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
                using (var ms = new MemoryStream(q.ImageData))
                {
                    picQuestion.Image = Image.FromStream(ms);
                    picQuestion.Visible = true;
                }
            }
            else
            {
                picQuestion.Visible = false;
            }

            foreach (var answer in q.Answers)
            {
                Control control;

                if (q.Type == Models.QuestionType.SingleChoice)
                {
                    control = new RadioButton
                    {
                        Text = answer.Text,
                        Tag = answer.Id,
                        Top = pnlAnswers.Controls.Count * 30,
                        Width = 800
                    };
                }
                else
                {
                    control = new CheckBox
                    {
                        Text = answer.Text,
                        Tag = answer.Id,
                        Top = pnlAnswers.Controls.Count * 30,
                        Width = 800
                    };
                }

                pnlAnswers.Controls.Add(control);
            }

            lblProgress.Text = $"Question {index + 1} / {_questions.Count}";

            btnPrevious.Enabled = index > 0;
            btnNext.Enabled = index < _questions.Count - 1;
        }

        private async Task SaveAnswer()
        {
            if (_questions == null || _currentQuestionIndex >= _questions.Count) return;
            var q = _questions[_currentQuestionIndex];

            var selected = pnlAnswers.Controls
                .Cast<Control>()
                .Where(c => (c is RadioButton rb && rb.Checked) ||
                            (c is CheckBox cb && cb.Checked))
                .Select(c => (int)c.Tag)
                .ToList();

            if (!selected.Any()) return;

            if (q.Type == Models.QuestionType.SingleChoice)
            {
                await _serviceContainer.TestAttemptService
                    .SubmitAnswerAsync(_testAttempt.Id, q.Id, selected.First());
            }
            else
            {
                await _serviceContainer.TestAttemptService
                    .SubmitMultipleAnswersAsync(_testAttempt.Id, q.Id, selected);
            }
        }

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            await SaveAnswer();
            _currentQuestionIndex++;
            DisplayQuestion(_currentQuestionIndex);
        }

        private async void BtnPrevious_Click(object sender, EventArgs e)
        {
            _currentQuestionIndex--;
            DisplayQuestion(_currentQuestionIndex);
        }

        private async void BtnFinish_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
            "Are you sure you want to finish?",
            "Confirm",
            MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes) return;

            try
            {
                await SaveAnswer();

                var completed = await _serviceContainer
                    .TestAttemptService
                    .CompleteTestAsync(_testAttempt.Id);

                new ResultsForm(completed).ShowDialog();
                this.Close();
            }
            catch
            {
                MessageBox.Show("Failed to complete test.");
            }
        }
    }
}

// ResultsForm.cs

namespace tt.UI
{
    public partial class ResultsForm : Form
    {
        private readonly Models.TestAttempt _testAttempt;

        public ResultsForm(Models.TestAttempt testAttempt)
        {
            InitializeComponent();
            _testAttempt = testAttempt;
        }

        private void InitializeComponent()
        {
            Text = "Test Results";
            Width = 600;
            Height = 500;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(225, 380),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 10)
            };
            btnClose.Click += BtnClose_Click;
            Controls.Add(btnClose);
            Load += ResultsForm_Load;
        }

        private void ResultsForm_Load(object sender, EventArgs e)
        {
            DisplayResults();
        }

        private void DisplayResults()
        {
            double percent = _testAttempt.Percentage;
            string grade =
                percent >= 90 ? "A" :
                percent >= 75 ? "B" :
                percent >= 60 ? "C" :
                percent >= 50 ? "D" : "F";

            Label lblTitle = new Label
            {
                Text = "Test Results",
                Location = new Point(20, 30),
                Size = new Size(540, 40),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblScore = new Label
            {
                Text = $"Score:  {_testAttempt.Score} / {_testAttempt.MaxScore}",
                Location = new Point(20, 110),
                Size = new Size(540, 35),
                Font = new Font("Segoe UI", 13),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblPercent = new Label
            {
                Text = $"Percentage:  {percent:F2}%",
                Location = new Point(20, 160),
                Size = new Size(540, 35),
                Font = new Font("Segoe UI", 13),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblGrade = new Label
            {
                Text = $"Grade:  {grade}",
                Location = new Point(20, 210),
                Size = new Size(540, 60),
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = grade == "A" ? Color.Green :
                            grade == "B" ? Color.DarkGreen :
                            grade == "C" ? Color.DarkOrange :
                            grade == "D" ? Color.Orange : Color.Red,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblMessage = new Label
            {
                Text = percent >= 60 ? "Congratulations, you passed!" : "Better luck next time!",
                Location = new Point(20, 290),
                Size = new Size(540, 35),
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = percent >= 60 ? Color.DarkGreen : Color.Red,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.Add(lblTitle);
            Controls.Add(lblScore);
            Controls.Add(lblPercent);
            Controls.Add(lblGrade);
            Controls.Add(lblMessage);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
