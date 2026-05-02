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
        private Label lblUsername;
        private Label lblPassword;
        private Label lblFullname;

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
            lblUsername = new Label();
            lblPassword = new Label();
            lblFullname = new Label();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(142, 47);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(160, 23);
            txtUsername.TabIndex = 0;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(142, 76);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(160, 23);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtFullName
            // 
            txtFullName.Location = new Point(142, 105);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(160, 23);
            txtFullName.TabIndex = 2;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(142, 134);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(160, 35);
            btnRegister.TabIndex = 3;
            btnRegister.Click += BtnRegister_Click;
            btnRegister.Text = "Register";
            //
            //Labels
            //
            lblUsername.Location = new Point(47, 50);
            lblUsername.Text = "Username";
            lblPassword.Location = new Point(47, 80);
            lblPassword.Text = "Password";
            lblFullname.Location = new Point(47, 110);
            lblFullname.Text = "Full Name";
            lblUsername.Font = new Font(lblUsername.Font, FontStyle.Bold);
            lblPassword.Font = new Font(lblPassword.Font, FontStyle.Bold);
            lblFullname.Font = new Font(lblFullname.Font, FontStyle.Bold);
            Controls.Add(lblUsername);
            Controls.Add(lblPassword);
            Controls.Add(lblFullname);
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
                ShowError("Enter username");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Enter password");
                return;
            }

            if (string.IsNullOrEmpty(fullName))
            {
                ShowError("Enter fullname");
                return;
            }

            try
            {
                var success = await _serviceContainer
                    .AuthenticationService
                    .RegisterAsync(username, password, fullName);

                if (!success)
                {
                    ShowError("Registration failed.");
                    return;
                }

                RegisteredUsername = username;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error while registration {ex}");
            }
        }

        private void ShowError(string msg)
        {
            MessageBox.Show($"{msg}", "Error");
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
        private Label lblLogin;
        private Label lblPassword;
        private Label lblAddress;

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
            txtServerAddress.Location = new Point(140, 105);
            txtPassword.UseSystemPasswordChar = true;
            btnLogin = new Button();
            btnRegister = new Button();
            rbHost.Location = new Point(140, 18);
            rbHost.Text = "Host";
            rbClient.Location = new Point(250, 18);
            rbClient.Text = "Client";
            rbClient.Checked = true;
            txtServerAddress.Width = 160;
            txtUsername.Width = 160;
            txtPassword.Width = 160;
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
            btnLogin.Location = new Point(140, 134);
            btnLogin.Height = 35;
            btnLogin.Width = 160;
            btnLogin.Text = "Login";
            btnRegister.Location = new Point(140, 175);
            btnRegister.Height = 35;
            btnRegister.Width = 160;
            btnRegister.Text = "Register";
            lblLogin = new Label();
            lblPassword = new Label();
            lblAddress = new Label();
            lblLogin.Location = new Point(47, 50);
            lblLogin.Text = "Username";
            lblPassword.Location = new Point(47, 80);
            lblPassword.Text = "Password";
            lblAddress.Location = new Point(47, 110);
            lblAddress.Text = "Server Address";
            Controls.Add(lblLogin);
            Controls.Add(lblPassword);
            Controls.Add(lblAddress);
            lblLogin.Font = new Font(lblLogin.Font, FontStyle.Bold);
            lblPassword.Font = new Font(lblPassword.Font, FontStyle.Bold);
            lblAddress.Font = new Font(lblAddress.Font, FontStyle.Bold);
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
                ShowError("Enter username");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Enter password");
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
                    ShowError("Wrong input, try again");
                    return;
                }

                var selectionForm = new TestSelectionForm(user, _serviceContainer!);
                this.Hide();
                selectionForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Unknown error. Try again {ex}");
            }
            finally
            {
                ToggleControls(true);
                Cursor = Cursors.Default;
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show($"{message}", "Error");
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

