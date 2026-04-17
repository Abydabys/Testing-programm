namespace tt.UI
{
    public partial class TestingForm : Form
    {
        private readonly ServiceContainer _serviceContainer;
        private readonly Models.User _currentUser;
        private readonly Models.TestAttempt _testAttempt;
        private List<Models.Question> _questions;
        private int _currentQuestionIndex = 0;

        public TestingForm(Models.User user, Models.TestAttempt testAttempt)
        {
            InitializeComponent();
            _serviceContainer = new ServiceContainer();
            _currentUser = user;
            _testAttempt = testAttempt;
        }

        private void InitializeComponent()
        {
            // TODO: Initialize UI components
            this.Text = "Test Taking";
            this.Width = 1000;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void TestingForm_Load(object sender, EventArgs e)
        {
            // TODO: Load test questions
            // _questions = (await _serviceContainer.QuestionService.GetQuestionsByTestIdAsync(_testAttempt.TestId)).ToList();
            // DisplayQuestion(_currentQuestionIndex);
        }

        private void DisplayQuestion(int index)
        {
            if (index < 0 || index >= _questions.Count)
                return;

            var question = _questions[index];
            
            // TODO: Display question
            // 1. Output question text
            // 2. If there is an image, display it
            // 3. Depending on question type, display RadioButton or CheckBox
            // 4. Update progress indicator
        }

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            // TODO: Implement logic for moving to next question
            // 1. Save answer to current question
            // 2. Move to next question
        }

        private async void BtnPrevious_Click(object sender, EventArgs e)
        {
            // TODO: Implement logic for moving to previous question
        }

        private async void BtnFinish_Click(object sender, EventArgs e)
        {
            // TODO: Implement logic for completing test
            // 1. Complete TestAttempt
            // 2. Calculate results
            // 3. Display results to user
        }
    }
}
