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
            // TODO: Инициализация UI компонентов
            this.Text = "Результаты теста";
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
            // TODO: Отобразить результаты
            // 1. Общий балл и максимальный балл
            // 2. Процент правильных ответов
            // 3. Оценка (A, B, C, D, F)
            // 4. Время прохождения
            // 5. Детальный разбор ответов
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
