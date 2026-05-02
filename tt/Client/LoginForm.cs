using tt.Client;
using tt.Server;

namespace tt.UI
{
    //  Registration Form
    public class RegistrationForm : Form
    {
        private readonly NetworkServiceContainer _serviceContainer;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private Button  btnRegister;
        private Label   lblUsername;
        private Label   lblPassword;
        private Label   lblFullname;

        public string RegisteredUsername { get; private set; }

        public RegistrationForm(NetworkServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text             = "Registration";
            Width            = 400;
            Height           = 280;
            StartPosition    = FormStartPosition.CenterScreen;
            FormBorderStyle  = FormBorderStyle.FixedDialog;
            MaximizeBox      = false;

            txtUsername  = new TextBox { Location = new Point(160, 50),  Size = new Size(180, 23) };
            txtPassword  = new TextBox { Location = new Point(160, 82),  Size = new Size(180, 23), UseSystemPasswordChar = true };
            txtFullName  = new TextBox { Location = new Point(160, 114), Size = new Size(180, 23) };

            btnRegister  = new Button  { Location = new Point(160, 150), Size = new Size(180, 35), Text = "Register" };
            btnRegister.Click += BtnRegister_Click;

            lblUsername  = new Label { Location = new Point(30, 53),  Text = "Username",  Font = new Font(Font, FontStyle.Bold), AutoSize = true };
            lblPassword  = new Label { Location = new Point(30, 85),  Text = "Password",  Font = new Font(Font, FontStyle.Bold), AutoSize = true };
            lblFullname  = new Label { Location = new Point(30, 117), Text = "Full Name", Font = new Font(Font, FontStyle.Bold), AutoSize = true };

            Controls.AddRange(new Control[] { lblUsername, lblPassword, lblFullname,
                                              txtUsername, txtPassword, txtFullName, btnRegister });
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();

            if (string.IsNullOrEmpty(username)) { ShowError("Enter username"); return; }
            if (string.IsNullOrEmpty(password)) { ShowError("Enter password"); return; }
            if (string.IsNullOrEmpty(fullName)) { ShowError("Enter full name"); return; }

            try
            {
                bool ok = await _serviceContainer.AuthenticationService
                    .RegisterAsync(username, password, fullName);

                if (!ok) { ShowError("Username already taken."); return; }

                RegisteredUsername = username;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ShowError($"Registration error: {ex.Message}");
            }
        }

        private void ShowError(string msg) => MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    //  Login Form

    public partial class LoginForm : Form
    {
        private NetworkServiceContainer? _serviceContainer;

        private TextBox     txtUsername;
        private TextBox     txtPassword;
        private TextBox     txtServerAddress;
        private RadioButton rbHost;
        private RadioButton rbClient;
        private Button      btnLogin;
        private Button      btnRegister;
        private Label       lblUsername;
        private Label       lblPassword;
        private Label       lblAddress;
        private Label       lblMode;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text            = "Testing System — Login";
            Width           = 460;
            Height          = 340;
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;

            // Mode selection
            lblMode  = new Label { Location = new Point(30, 20), Text = "Mode:", Font = new Font(Font, FontStyle.Bold), AutoSize = true };
            rbHost   = new RadioButton { Location = new Point(150, 17), Text = "Host (start server)", Width = 160 };
            rbClient = new RadioButton { Location = new Point(310, 17), Text = "Client",              Width = 90,  Checked = true };

            // Fields
            lblUsername      = new Label   { Location = new Point(30,  60), Text = "Username",       Font = new Font(Font, FontStyle.Bold), AutoSize = true };
            lblPassword      = new Label   { Location = new Point(30,  92), Text = "Password",       Font = new Font(Font, FontStyle.Bold), AutoSize = true };
            lblAddress       = new Label   { Location = new Point(30, 124), Text = "Server Address", Font = new Font(Font, FontStyle.Bold), AutoSize = true };

            txtUsername      = new TextBox { Location = new Point(160,  57), Size = new Size(200, 23) };
            txtPassword      = new TextBox { Location = new Point(160,  89), Size = new Size(200, 23), UseSystemPasswordChar = true };
            txtServerAddress = new TextBox { Location = new Point(160, 121), Size = new Size(200, 23), Text = "127.0.0.1" };

            // Buttons
            btnLogin    = new Button { Location = new Point(160, 165), Size = new Size(200, 38), Text = "Login",    Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnRegister = new Button { Location = new Point(160, 212), Size = new Size(200, 35), Text = "Register", Font = new Font("Segoe UI", 9) };

            btnLogin.Click    += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;
            rbHost.CheckedChanged  += ModeChanged;
            rbClient.CheckedChanged += ModeChanged;
            AcceptButton = btnLogin;

            Controls.AddRange(new Control[]
            {
                lblMode, rbHost, rbClient,
                lblUsername, lblPassword, lblAddress,
                txtUsername, txtPassword, txtServerAddress,
                btnLogin, btnRegister
            });

            ModeChanged(this, EventArgs.Empty);
        }

        private void ModeChanged(object? sender, EventArgs e)
        {
            txtServerAddress.Enabled = rbClient.Checked;
            if (rbHost.Checked)
                txtServerAddress.Text = "127.0.0.1 (auto)";
        }

        //Connection

        private async Task<bool> EnsureConnected()
        {
            if (_serviceContainer != null) return true;

            try
            {
                const int port = 9000;
                string address = rbHost.Checked ? "127.0.0.1" : txtServerAddress.Text.Trim();

                if (string.IsNullOrWhiteSpace(address))
                {
                    ShowError("Enter a server address.");
                    return false;
                }

                if (rbHost.Checked)
                    EmbeddedServerHost.EnsureStarted(port);

                await Task.Delay(600); // give server a moment to start

                _serviceContainer = await NetworkServiceContainer.CreateAsync(address, port);
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Connection error: {ex.Message}");
                return false;
            }
        }

        //Login

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username)) { ShowError("Enter username"); return; }
            if (string.IsNullOrEmpty(password)) { ShowError("Enter password"); return; }
            if (!await EnsureConnected()) return;

            ToggleControls(false);
            Cursor = Cursors.WaitCursor;

            try
            {
                var user = await _serviceContainer!.AuthenticationService.LoginAsync(username, password);

                if (user == null)
                {
                    ShowError("Invalid username or password.");
                    return;
                }

                Hide();

                if (rbHost.Checked)
                {
                    var editorForm = new HostEditorForm(_serviceContainer!);
                    editorForm.ShowDialog();
                }
                else
                {
                    var selectionForm = new TestSelectionForm(user, _serviceContainer!);
                    selectionForm.ShowDialog();
                }

                Close();
            }
            catch (Exception ex)
            {
                ShowError($"Unexpected error: {ex.Message}");
            }
            finally
            {
                ToggleControls(true);
                Cursor = Cursors.Default;
            }
        }

        //Register

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            if (!await EnsureConnected()) return;

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

        private void ShowError(string message) =>
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void ToggleControls(bool enabled)
        {
            txtUsername.Enabled    = enabled;
            txtPassword.Enabled    = enabled;
            btnLogin.Enabled       = enabled;
            btnRegister.Enabled    = enabled;
        }

        //server launcher

        private static class EmbeddedServerHost
        {
            private static readonly object _lock = new();
            private static bool _started;

            public static void EnsureStarted(int port)
            {
                lock (_lock)
                {
                    if (_started) return;
                    var server = new TcpServer(port);
                    _ = Task.Run(() => server.StartAsync());
                    _started = true;
                }
            }
        }
    }
}
