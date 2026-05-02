using tt.Client;
using tt.Server;

namespace tt.UI
{
    public class RegistrationForm : Form
    {
        private readonly NetworkServiceContainer _serviceContainer;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private Button btnRegister;

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
            catch (Exception ex)
            {
                ShowError($"Ошибка при регистрации {ex}");
            }
        }

        private void ShowError(string msg)
        {
            MessageBox.Show($"{msg}", "Ошибка");
        }
    }

    public partial class LoginForm : Form
    {
        private NetworkServiceContainer? _serviceContainer;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtServerAddress;
        private RadioButton rbHost;
        private RadioButton rbClient;
        private Button btnLogin;
        private Button btnRegister;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "System Login";
            Width = 450;
            Height = 300;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            txtServerAddress = new TextBox();
            rbHost = new RadioButton();
            rbClient = new RadioButton();
            txtUsername.Location = new Point(140, 47);
            txtPassword.Location = new Point(140, 76);
            txtServerAddress.Location = new Point(140, 134);
            txtPassword.UseSystemPasswordChar = true;
            btnLogin = new Button();
            btnRegister = new Button();
            rbHost.Location = new Point(140, 18);
            rbHost.Text = "Host";
            rbClient.Location = new Point(270, 18);
            rbClient.Text = "Client";
            rbClient.Checked = true;
            txtServerAddress.Width = 130;
            txtServerAddress.Text = "192.168.0.6";
            //txtServerAddress.Text = "127.0.0.1";
            Controls.Add(txtUsername);
            Controls.Add(txtPassword);
            Controls.Add(txtServerAddress);
            Controls.Add(rbHost);
            Controls.Add(rbClient);
            Controls.Add(btnLogin);
            Controls.Add(btnRegister);
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;
            rbHost.CheckedChanged += ModeChanged;
            rbClient.CheckedChanged += ModeChanged;
            AcceptButton = btnLogin;
            btnLogin.Location = new Point(140, 105);
            btnLogin.Height = 23;
            btnLogin.Width = 100;
            btnLogin.Text = "Логин";
            btnRegister.Location = new Point(140, 134);
            btnRegister.Height = 23;
            btnRegister.Width = 100;
            btnRegister.Text = "Регистрация";
            btnLogin.Top = 163;
            txtServerAddress.Top = 192;
            btnRegister.Top = 221;
            ModeChanged(this, EventArgs.Empty);
        }

        private void ModeChanged(object? sender, EventArgs e)
        {
            txtServerAddress.Enabled = rbClient.Checked;
        }

        private async Task<bool> EnsureConnected()
        {
            if (_serviceContainer != null)
                return true;

            try
            {
                const int port = 9000;
                var address = rbHost.Checked ? "127.0.0.1" : txtServerAddress.Text.Trim();

                if (string.IsNullOrWhiteSpace(address))
                {
                    ShowError("Enter a server address.");
                    return false;
                }

                if (rbHost.Checked)
                    EmbeddedServerHost.EnsureStarted(port);

                await Task.Delay(500);

                _serviceContainer = await NetworkServiceContainer.CreateAsync(address, port);
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Connection error: {ex.Message}");
                return false;
            }
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
            if (!await EnsureConnected())
            {
                return;
            }

            ToggleControls(false);
            Cursor = Cursors.WaitCursor;

            try
            {
                var user = await _serviceContainer.AuthenticationService
                    .LoginAsync(username, password);

                if (user == null)
                {
                    ShowError("Неверное имя пользователя. Попробуйте еще раз.");
                    return;
                }

                var selectionForm = new TestSelectionForm(user, _serviceContainer!);
                this.Hide();
                selectionForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Произошла неизвестная ошибка. Попробуйте еще раз {ex}");
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

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            if (!await EnsureConnected())
                return;

            try
            {
                var form = new RegistrationForm(_serviceContainer!);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    txtUsername.Text = form.RegisteredUsername;
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening registration: " + ex.Message);
            }
        }

        private void ToggleControls(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnRegister.Enabled = enabled;
        }

        private static class EmbeddedServerHost
        {
            private static readonly object Sync = new object();
            private static bool _started;

            public static void EnsureStarted(int port)
            {
                lock (Sync)
                {
                    if (_started)
                    {
                        return;
                    }

                    var server = new TcpServer(port);
                    _ = Task.Run(() => server.StartAsync());
                    _started = true;
                }
            }
        }
    }
}

