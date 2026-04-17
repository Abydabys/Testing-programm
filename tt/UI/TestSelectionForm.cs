namespace tt.UI
{
    public partial class TestSelectionForm : Form
    {
        private readonly ServiceContainer _serviceContainer;
        private readonly Models.User _currentUser;

        public TestSelectionForm(Models.User user)
        {
            InitializeComponent();
            _serviceContainer = new ServiceContainer();
            _currentUser = user;
        }

        private void InitializeComponent()
        {
            // TODO: Initialize UI components
            this.Text = "Available Tests";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void TestSelectionForm_Load(object sender, EventArgs e)
        {
            // TODO: Load list of available tests
            // var tests = await _serviceContainer.TestService.GetAllPublishedTestsAsync();
            // Display in DataGridView or ListBox
        }

        private async void BtnStartTest_Click(object sender, EventArgs e)
        {
            // TODO: Implement test start logic
            // 1. Check remaining attempts
            // 2. Create TestAttempt
            // 3. Open testing form
        }
    }
}
