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
            txtUsername.Location = new Point(142, 105);
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
            txtFullName.Location = new Point(142, 47);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(100, 23);
            txtFullName.TabIndex = 2;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(154, 134);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(75, 23);
            btnRegister.TabIndex = 3;
            btnRegister.Click += BtnRegister_Click;
            // 
            // lblError
            // 
            lblError.Location = new Point(0, 0);
            lblError.Name = "lblError";
            lblError.Size = new Size(100, 23);
            lblError.TabIndex = 4;
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
            Text = "System Login";
            Width = 400;
            Height = 300;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            txtPassword.UseSystemPasswordChar = true;
            btnLogin = new Button();
            btnRegister = new Button();
            lblError = new Label();
            Controls.Add(txtUsername);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(btnRegister);
            Controls.Add(lblError);
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;
            AcceptButton = btnLogin;
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
