namespace tt.UI
{
    public partial class ResultsForm : Form
    {
        private readonly ServiceContainer _serviceContainer;
        private readonly Models.TestAttempt _testAttempt;

        public ResultsForm(Models.TestAttempt testAttempt)
        {
            InitializeComponent();
            _serviceContainer = new ServiceContainer();
            _testAttempt = testAttempt;
        }

        private void InitializeComponent()
        {
            // TODO: Initialize UI components
            this.Text = "Test Results";
            this.Width = 600;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void ResultsForm_Load(object sender, EventArgs e)
        {
            DisplayResults();
        }

        private void DisplayResults()
        {
            // TODO: Display results
            // 1. Total score and maximum score
            // 2. Percentage of correct answers
            // 3. Grade (A, B, C, D, F)
            // 4. Time taken
            // 5. Detailed review of answers
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
