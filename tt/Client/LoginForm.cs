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
            this.Text = "Registration";
            this.Width = 400;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen;

            txtUsername = new TextBox { Left = 100, Top = 40, Width = 200 };
            txtPassword = new TextBox { Left = 100, Top = 80, Width = 200 };
            txtPassword.UseSystemPasswordChar = true;

            txtFullName = new TextBox { Left = 100, Top = 120, Width = 200 };

            btnRegister = new Button
            {
                Text = "Register",
                Left = 100,
                Top = 160,
                Width = 200
            };

            lblError = new Label
            {
                Left = 100,
                Top = 200,
                Width = 250,
                ForeColor = Color.Red,
                Visible = false
            };

            this.Controls.Add(txtUsername);
            this.Controls.Add(txtPassword);
            this.Controls.Add(txtFullName);
            this.Controls.Add(btnRegister);
            this.Controls.Add(lblError);

            btnRegister.Click += BtnRegister_Click;
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ShowError("Username is required");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Password is required");
                return;
            }

            if (string.IsNullOrEmpty(fullName))
            {
                ShowError("Full name is required");
                return;
            }

            try
            {
                var success = await _serviceContainer
                    .AuthenticationService
                    .RegisterAsync(username, password, fullName);

                if (!success)
                {
                    ShowError("Registration failed");
                    return;
                }

                RegisteredUsername = username;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {
                ShowError("Error during registration");
            }
        }

        private void ShowError(string msg)
        {
            lblError.Text = msg;
            lblError.Visible = true;
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
                MessageBox.Show("Cannot connect to server. Is the server running?");
                Application.Exit();
            }
        }

        private void InitializeComponent()
        {
            this.Text = "System Login";
            this.Width = 400;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            txtUsername = new TextBox { Left = 100, Top = 50, Width = 200 };
            txtPassword = new TextBox { Left = 100, Top = 90, Width = 200 };
            txtPassword.UseSystemPasswordChar = true;

            btnLogin = new Button { Text = "Login", Left = 100, Top = 130, Width = 90 };
            btnRegister = new Button { Text = "Register", Left = 210, Top = 130, Width = 90 };

            lblError = new Label
            {
                Left = 100,
                Top = 170,
                Width = 250,
                ForeColor = Color.Red,
                Visible = false
            };

            this.Controls.Add(txtUsername);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
            this.Controls.Add(lblError);

            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;

            this.AcceptButton = btnLogin;
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username))
            {
                ShowError("Username is required");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Password is required");
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
                    ShowError("Invalid username or password");
                    return;
                }

                new TestSelectionForm(user).Show();
                this.Close();
            }
            catch
            {
                ShowError("An unexpected error occurred. Please try again.");
            }
            finally
            {
                ToggleControls(true);
                Cursor = Cursors.Default;
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
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
                MessageBox.Show("Failed to open registration: " + ex.Message);
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
