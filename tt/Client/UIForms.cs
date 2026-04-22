// ============================================================================
// TestSelectionForm.cs
// ============================================================================
using tt.Client;

namespace tt.UI
{
    public partial class TestSelectionForm : Form
    {
        private readonly NetworkServiceContainer _serviceContainer;
        private readonly Models.User _currentUser;

        public TestSelectionForm(Models.User user)
        {
            InitializeComponent();
            // TODO: Create a new NetworkServiceContainer and store it in _serviceContainer.
            //       Wrap in try-catch; on failure show a connection error and close the form.
            // TODO: Store the user parameter in the _currentUser field.
        }

        private void InitializeComponent()
        {
            // TODO: Set this.Text to "Available Tests".
            // TODO: Set this.Width to 800 and this.Height to 600.
            // TODO: Set this.StartPosition to FormStartPosition.CenterScreen.
            // TODO: Create a DataGridView named dgvTests to display the list of available tests.
            // TODO: Configure dgvTests columns: at minimum show test Title, Description, and MaxAttempts.
            // TODO: Set dgvTests.SelectionMode to FullRowSelect and ReadOnly to true.
            // TODO: Create a Button named btnStartTest with the text "Start Test".
            // TODO: Create a Label named lblWelcome to greet the current user (e.g. "Welcome, [FullName]!").
            // TODO: Wire up the form's Load event: this.Load += TestSelectionForm_Load;
            // TODO: Wire up the start button click event: btnStartTest.Click += BtnStartTest_Click;
            // TODO: Add all controls to the form using this.Controls.Add(...).
        }

        private async void TestSelectionForm_Load(object sender, EventArgs e)
        {
            // TODO: Set lblWelcome.Text to greet the current user using _currentUser.FullName.
            // TODO: Call _serviceContainer.TestService.GetAllPublishedTestsAsync() using await.
            //       (Sends a TCP GetAllPublishedTests request to the server.)
            // TODO: Bind the returned list to dgvTests (e.g. dgvTests.DataSource = tests.ToList()).
        }

        private async void BtnStartTest_Click(object sender, EventArgs e)
        {
            // TODO: Check that a row is selected in dgvTests. If not, show "Please select a test first." and return.
            // TODO: Get the selected test's Id from the selected row in dgvTests.
            // TODO: Call _serviceContainer.TestAttemptService.CanUserAttemptTestAsync(_currentUser.Id, testId) using await.
            //       (Sends a TCP CanUserAttemptTest request to the server.)
            // TODO: If the user cannot attempt the test, show "You have used all available attempts." and return.
            // TODO: Call _serviceContainer.TestAttemptService.StartTestAsync(_currentUser.Id, testId) using await.
            //       (Sends a TCP StartTest request to the server.)
            // TODO: If the returned attempt is null, show an error "Failed to start the test." and return.
            // TODO: Create a new TestingForm, passing _currentUser and the new attempt, and call Show() on it.
            // TODO: Optionally close this form after opening the TestingForm.
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
            // TODO: Create a new NetworkServiceContainer and store it in _serviceContainer.
            //       Wrap in try-catch; on failure show a connection error and close the form.
            // TODO: Store the user parameter in the _currentUser field.
            // TODO: Store the testAttempt parameter in the _testAttempt field.
        }

        private void InitializeComponent()
        {
            // TODO: Set this.Text to "Test Taking".
            // TODO: Set this.Width to 1000 and this.Height to 700.
            // TODO: Set this.StartPosition to FormStartPosition.CenterScreen.
            // TODO: Create a Label named lblQuestion to show the question text.
            // TODO: Create a Panel named pnlAnswers to hold the answer controls (RadioButtons or CheckBoxes).
            // TODO: Create a PictureBox named picQuestion to display an optional question image.
            // TODO: Create a Label named lblProgress to show the current question number out of total (e.g. "1 / 10").
            // TODO: Create a Button named btnNext with the text "Next".
            // TODO: Create a Button named btnPrevious with the text "Previous".
            // TODO: Create a Button named btnFinish with the text "Finish Test".
            // TODO: Wire up the form's Load event: this.Load += TestingForm_Load;
            // TODO: Wire up the button click events for btnNext, btnPrevious, and btnFinish.
            // TODO: Add all controls to the form using this.Controls.Add(...).
        }

        private async void TestingForm_Load(object sender, EventArgs e)
        {
            // TODO: Call _serviceContainer.QuestionService.GetQuestionsByTestIdAsync(_testAttempt.TestId) using await.
            //       (Sends a TCP GetQuestionsByTestId request to the server.)
            // TODO: Convert the result to a List and store it in _questions.
            // TODO: Call DisplayQuestion(0) to show the first question immediately.
        }
        private void DisplayQuestion(int index)
        {
            // TODO: If index is out of range, return early.
            // TODO: Get the question at the given index from _questions.
            // TODO: Set lblQuestion.Text to the question's Text property.
            // TODO: Clear any existing controls from pnlAnswers before adding new ones.
            // TODO: If question.ImageData is not null or empty, convert it to an Image and display it in picQuestion.
            //       Otherwise hide picQuestion.
            // TODO: If the question type is SingleChoice, create a RadioButton for each answer and add to pnlAnswers.
            // TODO: If the question type is MultipleChoice, create a CheckBox for each answer and add to pnlAnswers.
            // TODO: Update lblProgress to show the current position, e.g. "Question 3 / 10".
            // TODO: Disable btnPrevious if index == 0. Disable btnNext if index == _questions.Count - 1.
        }

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            // TODO: Save the selected answer(s) for the current question by calling the appropriate
            //       _serviceContainer.TestAttemptService method (SubmitAnswerAsync or SubmitMultipleAnswersAsync).
            //       (Sends a TCP SubmitAnswer or SubmitMultipleAnswers request to the server.)
            // TODO: Increment _currentQuestionIndex by 1.
            // TODO: Call DisplayQuestion(_currentQuestionIndex).
        }

        private async void BtnPrevious_Click(object sender, EventArgs e)
        {
            // TODO: Decrement _currentQuestionIndex by 1.
            // TODO: Call DisplayQuestion(_currentQuestionIndex).
        }

        private async void BtnFinish_Click(object sender, EventArgs e)
        {
            // TODO: Ask the user to confirm with a MessageBox Yes/No dialog.
            // TODO: If the user cancels, return early.
            // TODO: Save the current question's answer(s).
            // TODO: Call _serviceContainer.TestAttemptService.CompleteTestAsync(_testAttempt.Id) using await.
            //       (Sends a TCP CompleteTest request to the server, which scores the test and saves to the DB.)
            // TODO: Store the returned completed attempt.
            // TODO: Create a new ResultsForm passing the completed attempt and call ShowDialog().
            // TODO: Close this form after the results dialog is dismissed.
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
            // TODO: Store the testAttempt parameter in the _testAttempt field.
        }

        private void InitializeComponent()
        {
            // TODO: Set this.Text to "Test Results".
            // TODO: Set this.Width to 600 and this.Height to 500.
            // TODO: Set this.StartPosition to FormStartPosition.CenterScreen.
            // TODO: Create a Button named btnClose with the text "Close".
            // TODO: Wire up the form's Load event: this.Load += ResultsForm_Load;
            // TODO: Wire up the close button click event: btnClose.Click += BtnClose_Click;
            // TODO: Add all controls to the form using this.Controls.Add(...).
        }

        private void ResultsForm_Load(object sender, EventArgs e)
        {
            // TODO: Call DisplayResults() to populate the form when it opens.
        }

        private void DisplayResults()
        {
            // TODO: Display the total score: _testAttempt.Score out of _testAttempt.MaxScore.
            // TODO: Display the percentage from _testAttempt.Percentage, formatted to 2 decimal places.
            // TODO: Calculate and display the grade letter (A/B/C/D/F) based on the percentage.
            // TODO: Calculate and display the time taken: _testAttempt.CompletedAt minus _testAttempt.StartedAt.
            // TODO: Display a detailed answer review for each question (correct / incorrect).
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            // TODO: Set this.DialogResult to DialogResult.OK.
            // TODO: Call this.Close().
        }
    }
}
