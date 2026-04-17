namespace tt.UI
{
    public partial class LoginForm : Form
    {
        private readonly ServiceContainer _serviceContainer;

        public LoginForm()
        {
            InitializeComponent();
            _serviceContainer = new ServiceContainer();
        }

        private void InitializeComponent()
        {
            // TODO: Initialize UI components in designer
            this.Text = "System Login";
            this.Width = 400;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            // TODO: Implement login logic
            // string username = txtUsername.Text;
            // string password = txtPassword.Text;
            // var user = await _serviceContainer.AuthenticationService.LoginAsync(username, password);
            // if (user != null)
            // {
            //     // Open main form
            // }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // TODO: Open registration form
        }
    }
}
