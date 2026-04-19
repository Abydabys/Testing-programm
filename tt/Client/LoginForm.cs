using tt.Client;

namespace tt.UI
{
    public partial class LoginForm : Form
    {
        // CHANGED from ServiceContainer to NetworkServiceContainer.
        // The form no longer touches the database directly — all calls
        // go over TCP to the server through NetworkServiceContainer.
        private readonly NetworkServiceContainer _serviceContainer;

        public LoginForm()
        {
            InitializeComponent();
            // TODO: Create a new NetworkServiceContainer instance (using default address "127.0.0.1" and port 9000)
            //       and store it in _serviceContainer.
            // TODO: Wrap the constructor call in a try-catch.
            //       If the server is not reachable, show a MessageBox "Cannot connect to server. Is the server running?"
            //       and then call Application.Exit() to close the application.
        }

        private void InitializeComponent()
        {
            // TODO: Create a TextBox control named txtUsername for entering the username.
            // TODO: Create a TextBox control named txtPassword for entering the password.
            // TODO: Set txtPassword.UseSystemPasswordChar = true to hide the password input.
            // TODO: Create a Button control named btnLogin with the text "Login".
            // TODO: Create a Button control named btnRegister with the text "Register".
            // TODO: Create a Label control named lblError for displaying error messages.
            // TODO: Set lblError.ForeColor to Color.Red.
            // TODO: Set lblError.Visible = false so it is hidden by default.
            // TODO: Set this.Text (the form title bar) to "System Login".
            // TODO: Set this.Width to 400 and this.Height to 300.
            // TODO: Set this.StartPosition to FormStartPosition.CenterScreen.
            // TODO: Set this.FormBorderStyle to FormBorderStyle.FixedDialog.
            // TODO: Set this.MaximizeBox = false and this.MinimizeBox = false.
            // TODO: Add all controls to the form using this.Controls.Add(...).
            // TODO: Wire the login button click event: btnLogin.Click += BtnLogin_Click;
            // TODO: Wire the register button click event: btnRegister.Click += BtnRegister_Click;
            // TODO: Set this.AcceptButton = btnLogin so pressing Enter triggers login.
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            // TODO: Read the username from txtUsername.Text and trim whitespace.
            // TODO: Read the password from txtPassword.Text (do NOT trim passwords).
            // TODO: If the username is null or empty, show an error "Username is required" and return early.
            // TODO: If the password is null or empty, show an error "Password is required" and return early.
            // TODO: Disable txtUsername, txtPassword, btnLogin, and btnRegister.
            // TODO: Change the cursor to Cursors.WaitCursor to indicate loading.
            // TODO: Hide lblError in case it was showing a previous error.
            // TODO: Call _serviceContainer.AuthenticationService.LoginAsync(username, password) using await.
            //       (This now sends a TCP request to the server instead of querying the DB directly.)
            // TODO: Store the returned user in a variable.
            // TODO: If user is null, display "Invalid username or password" in lblError and set lblError.Visible = true.
            // TODO: If user is not null, create a new TestSelectionForm passing the user, call Show() on it, and call this.Close().
            // TODO: Wrap the authentication call in a try-catch block.
            // TODO: In the catch block, show a generic error message "An unexpected error occurred. Please try again."
            // TODO: In the finally block, re-enable txtUsername, txtPassword, btnLogin, and btnRegister.
            // TODO: In the finally block, restore the cursor to Cursors.Default.
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // TODO: Create a new RegistrationForm instance, passing any required services.
            // TODO: Open the registration form as a modal dialog using ShowDialog().
            // TODO: After the dialog closes, check the DialogResult to see if registration succeeded.
            // TODO: If successful, pre-fill txtUsername with the newly registered username.
            // TODO: Optionally set focus to txtPassword so the user can log in immediately.
            // TODO: Wrap the block in a try-catch and show an error message if the form fails to open.
        }
    }
}
