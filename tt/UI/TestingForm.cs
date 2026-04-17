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
            // TODO: Инициализация UI компонентов
            this.Text = "Прохождение теста";
            this.Width = 1000;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void TestingForm_Load(object sender, EventArgs e)
        {
            // TODO: Загрузить вопросы теста
            // _questions = (await _serviceContainer.QuestionService.GetQuestionsByTestIdAsync(_testAttempt.TestId)).ToList();
            // DisplayQuestion(_currentQuestionIndex);
        }

        private void DisplayQuestion(int index)
        {
            if (index < 0 || index >= _questions.Count)
                return;

            var question = _questions[index];
            
            // TODO: Отобразить вопрос
            // 1. Вывести текст вопроса
            // 2. Если есть изображение, отобразить его
            // 3. В зависимости от типа вопроса, отобразить RadioButton или CheckBox
            // 4. Обновить индикатор прогресса
        }

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            // TODO: Реализовать логику перехода к следующему вопросу
            // 1. Сохранить ответ на текущий вопрос
            // 2. Перейти к следующему вопросу
        }

        private async void BtnPrevious_Click(object sender, EventArgs e)
        {
            // TODO: Реализовать логику возврата к предыдущему вопросу
        }

        private async void BtnFinish_Click(object sender, EventArgs e)
        {
            // TODO: Реализовать логику завершения теста
            // 1. Завершить TestAttempt
            // 2. Вычислить результаты
            // 3. Отобразить результаты пользователю
        }
    }
}
