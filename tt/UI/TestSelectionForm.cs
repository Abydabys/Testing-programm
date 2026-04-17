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
            // TODO: Инициализация UI компонентов
            this.Text = "Доступные тесты";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void TestSelectionForm_Load(object sender, EventArgs e)
        {
            // TODO: Загрузить список доступных тестов
            // var tests = await _serviceContainer.TestService.GetAllPublishedTestsAsync();
            // Отобразить в DataGridView или ListBox
        }

        private async void BtnStartTest_Click(object sender, EventArgs e)
        {
            // TODO: Реализовать логику начала теста
            // 1. Проверить количество оставшихся попыток
            // 2. Создать TestAttempt
            // 3. Открыть форму тестирования
        }
    }
}
