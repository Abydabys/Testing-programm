using tt.Client;

namespace tt.UI
{
    public class RegistrationForm : Form
    {
        private readonly NetworkServiceContainer _serviceContainer;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private Button btnRegister;
        private Label lblError;

        public string RegisteredUsername { get; private set; }

        public RegistrationForm(NetworkServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            txtFullName = new TextBox();
            btnRegister = new Button();
            lblError = new Label();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(142, 47);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(100, 23);
            txtUsername.TabIndex = 0;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(142, 76);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(100, 23);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtFullName
            // 
            txtFullName.Location = new Point(142, 105);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(100, 23);
            txtFullName.TabIndex = 2;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(142, 134);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(100, 23);
            btnRegister.TabIndex = 3;
            btnRegister.Click += BtnRegister_Click;
            btnRegister.Text = "Зарегистрироваться";
            // 
            // RegistrationForm
            // 
            ClientSize = new Size(384, 261);
            Controls.Add(txtUsername);
            Controls.Add(txtPassword);
            Controls.Add(txtFullName);
            Controls.Add(btnRegister);
            Controls.Add(lblError);
            Name = "RegistrationForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Registration";
            ResumeLayout(false);
            PerformLayout();
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ShowError("Введите имя пользователя");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль");
                return;
            }

            if (string.IsNullOrEmpty(fullName))
            {
                ShowError("Введите полное имя");
                return;
            }

            try
            {
                var success = await _serviceContainer
                    .AuthenticationService
                    .RegisterAsync(username, password, fullName);

                if (!success)
                {
                    ShowError("Регистрация неуспешна.");
                    return;
                }

                RegisteredUsername = username;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {
                ShowError("Ошибка при регистрации");
            }
        }

        private void ShowError(string msg)
        {
            MessageBox.Show($"{msg}", "Ошибка");
        }
    }

    public partial class LoginForm : Form
    {
        private readonly NetworkServiceContainer _serviceContainer;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Label lblError;

        public LoginForm()
        {
            InitializeComponent();
            try
            {
                _serviceContainer = new NetworkServiceContainer("127.0.0.1", 9000);
            }
            catch
            {
                MessageBox.Show("Невозможно подключиться к серверу.");
                Application.Exit();
            }
        }

        private void InitializeComponent()
        {
            Text = "System Login";
            Width = 400;
            Height = 300;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            txtUsername.Location = new Point(140, 47);
            txtPassword.Location = new Point(140, 76);
            txtPassword.UseSystemPasswordChar = true;
            btnLogin = new Button();
            btnRegister = new Button();
            Controls.Add(txtUsername);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(btnRegister);
            Controls.Add(lblError);
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;
            AcceptButton = btnLogin;
            btnLogin.Location = new Point(140, 105);
            btnLogin.Height = 23;
            btnLogin.Width = 100;
            btnLogin.Text = "Логин";
            btnRegister.Location = new Point(140, 134);
            btnRegister.Height = 23;
            btnRegister.Width = 100;
            btnRegister.Text = "Регистрация";
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username))
            {
                ShowError("Введите имя пользователя");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль");
                return;
            }

            ToggleControls(false);
            Cursor = Cursors.WaitCursor;
            lblError.Visible = false;

            try
            {
                var user = await _serviceContainer.AuthenticationService
                    .LoginAsync(username, password);

                if (user == null)
                {
                    ShowError("Неверное имя пользователя. Попробуйте еще раз.");
                    return;
                }

                new TestSelectionForm(user).Show();
                this.Close();
            }
            catch
            {
                ShowError("Произошла неизвестная ошибка. Попробуйте еще раз");
            }
            finally
            {
                ToggleControls(true);
                Cursor = Cursors.Default;
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show($"{message}", "Ошибка");
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new RegistrationForm(_serviceContainer);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    txtUsername.Text = form.RegisteredUsername;
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось открыть меню регистрации: " + ex.Message);
            }
        }

        private void ToggleControls(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnRegister.Enabled = enabled;
        }
    }
}

