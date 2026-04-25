// ============================================================================
// TestSelectionForm.cs
// ============================================================================
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

        public TestSelectionForm(Models.User user)
        {
            InitializeComponent();
            try
            {
                _serviceContainer = new NetworkServiceContainer();
            }
            catch
            {
                MessageBox.Show("Cannot connect to server.");
                this.Close();
            }
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
            btnStartTest.Location = new Point(0, 0);
            btnStartTest.Name = "btnStartTest";
            btnStartTest.Size = new Size(75, 23);
            btnStartTest.TabIndex = 0;
            btnStartTest.Click += BtnStartTest_Click;
            // 
            // lblWelcome
            // 
            lblWelcome.Location = new Point(0, 0);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(100, 23);
            lblWelcome.TabIndex = 0;
            // 
            // TestSelectionForm
            // 
            ClientSize = new Size(784, 561);
            Controls.Add(dataGridView);
            Name = "TestSelectionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Available Tests";
            Load += TestSelectionForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
        }

        private async void TestSelectionForm_Load(object sender, EventArgs e)
        {
            Label lblWelcome = this.Controls.Find("lblWelcome", true).FirstOrDefault() as Label;
            lblWelcome.Text = $"Welcome, {_currentUser.FullName}!";
            var tests = await _serviceContainer.TestService.GetAllPublishedTestsAsync();

        }

        private async void BtnStartTest_Click(object sender, EventArgs e)
        {
            DataGridView dgvTests = this.Controls.Find("dgvTests", true).FirstOrDefault() as DataGridView;
            if (dgvTests.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a test first.");
                return;
            }

            int testId = (int)dgvTests.SelectedRows[0].Cells["Id"].Value;
            _serviceContainer.TestAttemptService.CanUserAttemptTestAsync(_currentUser.Id, testId).ContinueWith(canAttemptTask =>
            {
                if (!canAttemptTask.Result)
                {
                    MessageBox.Show("You have used all available attempts.");
                    return;
                }

                _serviceContainer.TestAttemptService.StartTestAsync(_currentUser.Id, testId).ContinueWith(startTestTask =>
                {
                    var attempt = startTestTask.Result;
                    if (attempt == null)
                    {
                        MessageBox.Show("Failed to start the test.");
                        return;
                    }

                    TestingForm testingForm = new TestingForm(_currentUser, attempt);
                    testingForm.Show();
                    this.Close();
                });
            });


        }
    }
}

// ============================================================================
// TestingForm.cs
// ============================================================================
namespace tt.UI
{
    public partial class TestingForm : Form
    {
        private readonly Client.NetworkServiceContainer _serviceContainer;
        private readonly Models.User _currentUser;
        private readonly Models.TestAttempt _testAttempt;
        private List<Models.Question> _questions;
        private int _currentQuestionIndex = 0;

        public TestingForm(Models.User user, Models.TestAttempt testAttempt)
        {
            InitializeComponent();
            try
            {
                _serviceContainer = new Client.NetworkServiceContainer("127.0.0.1", 9000);
            }
            catch
            {
                MessageBox.Show("Cannot connect to server.");
                this.Close();
                return;
            }

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
            lblQuestion = new Label();
            picQuestion = new PictureBox();
            pnlAnswers = new Panel();
            lblProgress = new Label();
            btnPrevious = new Button();
            btnNext = new Button();
            btnFinish = new Button();
            Controls.Add(lblQuestion);
            Controls.Add(picQuestion);
            Controls.Add(pnlAnswers);
            Controls.Add(lblProgress);
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

                DisplayQuestion(0);
            }
            catch
            {
                MessageBox.Show("Failed to load questions.");
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

// ============================================================================
// ResultsForm.cs
// ============================================================================
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
            Button btnClose = new Button();
            btnClose.Click += BtnClose_Click;
            Load += ResultsForm_Load;
            Controls.Add(btnClose);
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

            Label lbl = new Label
            {
                Left = 20,
                Top = 50,
                Width = 500,
                Text = $"Score: {_testAttempt.Score}/{_testAttempt.MaxScore}\n" +
                       $"Percentage: {percent:F2}%\n" +
                       $"Grade: {grade}"
            };

            this.Controls.Add(lbl);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
                this.Close();
        }
    }
}
