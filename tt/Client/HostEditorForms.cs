using tt.Client;
using tt.Models;

namespace tt.UI
{
    public class HostEditorForm : Form
    {
        private readonly NetworkServiceContainer _svc;

        private DataGridView dgvTests;
        private Button btnNewTest;
        private Button btnEditTest;
        private Button btnDeleteTest;
        private Button btnTogglePublish;
        private Button btnManageQuestions;
        private Button btnRefresh;
        private Label  lblTitle;
        private Label  lblStatus;

        public HostEditorForm(NetworkServiceContainer svc)
        {
            _svc = svc;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text          = "Host — Test Editor";
            ClientSize    = new Size(1000, 640);
            StartPosition = FormStartPosition.CenterScreen;

            var header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(30, 80, 160) };
            lblTitle   = new Label
            {
                Text      = "Test Management",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 18, FontStyle.Bold),
                Location  = new Point(20, 10),
                AutoSize  = true
            };
            header.Controls.Add(lblTitle);

            dgvTests = new DataGridView
            {
                Location              = new Point(20, 80),
                Size                  = new Size(960, 440),
                ReadOnly              = true,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect           = false,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible     = false,
                BackgroundColor       = Color.White
            };

            int bx = 20, by = 540, bw = 148, bh = 42, gap = 10;

            btnNewTest          = MakeButton("➕  New Test",         Color.FromArgb(40, 160, 80),  new Point(bx,                    by), new Size(bw, bh));
            btnEditTest         = MakeButton("✏️  Edit Test",         Color.FromArgb(30, 120, 200), new Point(bx + (bw+gap)*1,       by), new Size(bw, bh));
            btnDeleteTest       = MakeButton("🗑️  Delete Test",       Color.FromArgb(200, 50, 50),  new Point(bx + (bw+gap)*2,       by), new Size(bw, bh));
            btnTogglePublish    = MakeButton("📢  Publish / Draft",    Color.FromArgb(200, 130, 0),  new Point(bx + (bw+gap)*3,       by), new Size(bw, bh));
            btnManageQuestions  = MakeButton("📝  Questions",          Color.FromArgb(80,  60, 180), new Point(bx + (bw+gap)*4,       by), new Size(bw, bh));
            btnRefresh          = MakeButton("🔄  Refresh",            Color.FromArgb(90,  90,  90), new Point(bx + (bw+gap)*5,       by), new Size(bw, bh));

            btnNewTest.Click         += BtnNewTest_Click;
            btnEditTest.Click        += BtnEditTest_Click;
            btnDeleteTest.Click      += BtnDeleteTest_Click;
            btnTogglePublish.Click   += BtnTogglePublish_Click;
            btnManageQuestions.Click += BtnManageQuestions_Click;
            btnRefresh.Click         += async (_, __) => await LoadTests();

            lblStatus = new Label { Location = new Point(20, 595), Size = new Size(960, 22), ForeColor = Color.DimGray };

            Controls.AddRange(new Control[]
            {
                header, dgvTests,
                btnNewTest, btnEditTest, btnDeleteTest,
                btnTogglePublish, btnManageQuestions, btnRefresh,
                lblStatus
            });

            Load += async (_, __) => await LoadTests();
        }

        private static Button MakeButton(string text, Color back, Point loc, Size size)
        {
            return new Button
            {
                Text      = text,
                Location  = loc,
                Size      = size,
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private async Task LoadTests()
        {
            try
            {
                lblStatus.Text = "Loading…";
                var tests = await _svc.TestEditorService.GetAllTestsAsync();

                dgvTests.DataSource = tests.Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    Status       = t.IsPublished ? "✅ Published" : "📝 Draft",
                    MaxAttempts  = t.MaxAttempts == 0 ? "Unlimited" : t.MaxAttempts.ToString(),
                    Created      = t.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd")
                }).ToList();

                lblStatus.Text = $"Loaded {tests.Count} tests.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
        }

        private Test? SelectedTest()
        {
            if (dgvTests.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a test first.", "No Selection");
                return null;
            }
            int id = (int)dgvTests.SelectedRows[0].Cells["Id"].Value;
            return new Test
            {
                Id          = id,
                Title       = dgvTests.SelectedRows[0].Cells["Title"].Value?.ToString() ?? "",
                Description = dgvTests.SelectedRows[0].Cells["Description"].Value?.ToString() ?? "",
                IsPublished = dgvTests.SelectedRows[0].Cells["Status"].Value?.ToString()?.StartsWith("✅") ?? false
            };
        }

        private async void BtnNewTest_Click(object sender, EventArgs e)
        {
            using var dlg = new TestEditDialog(null);
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                var created = await _svc.TestEditorService.CreateTestAsync(dlg.ResultTest);
                if (created == null) { MessageBox.Show("Failed to create test."); return; }
                lblStatus.Text = $"Created test: {dlg.ResultTest.Title}";
                await LoadTests();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }

        private async void BtnEditTest_Click(object sender, EventArgs e)
        {
            var stub = SelectedTest();
            if (stub == null) return;

            try
            {
                var full = await _svc.TestEditorService.GetTestByIdAsync(stub.Id);
                if (full == null) { MessageBox.Show("Test not found."); return; }

                using var dlg = new TestEditDialog(full);
                if (dlg.ShowDialog() != DialogResult.OK) return;

                bool ok = await _svc.TestEditorService.UpdateTestAsync(dlg.ResultTest);
                if (!ok) { MessageBox.Show("Update failed."); return; }
                lblStatus.Text = $"Updated: {dlg.ResultTest.Title}";
                await LoadTests();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }

        private async void BtnDeleteTest_Click(object sender, EventArgs e)
        {
            var stub = SelectedTest();
            if (stub == null) return;

            if (MessageBox.Show($"Delete \"{stub.Title}\"? This is permanent.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                bool ok = await _svc.TestEditorService.DeleteTestAsync(stub.Id);
                if (!ok) { MessageBox.Show("Delete failed."); return; }
                lblStatus.Text = $"Deleted test ID {stub.Id}.";
                await LoadTests();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }

        private async void BtnTogglePublish_Click(object sender, EventArgs e)
        {
            var stub = SelectedTest();
            if (stub == null) return;

            string action = stub.IsPublished ? "unpublish" : "publish";
            if (MessageBox.Show($"Are you sure you want to {action} \"{stub.Title}\"?",
                "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                bool ok = await _svc.TestEditorService.PublishTestAsync(stub.Id);
                if (!ok) { MessageBox.Show("Operation failed."); return; }
                lblStatus.Text = $"{(stub.IsPublished ? "Unpublished" : "Published")}: {stub.Title}";
                await LoadTests();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }

        private async void BtnManageQuestions_Click(object sender, EventArgs e)
        {
            var stub = SelectedTest();
            if (stub == null) return;

            try
            {
                var full = await _svc.TestEditorService.GetTestByIdAsync(stub.Id);
                if (full == null) { MessageBox.Show("Test not found."); return; }

                using var form = new QuestionManagerForm(full, _svc);
                form.ShowDialog();
                lblStatus.Text = $"Questions saved for: {full.Title}";
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }
    }

    public class TestEditDialog : Form
    {
        private TextBox        txtTitle;
        private TextBox        txtDescription;
        private NumericUpDown  nudMaxAttempts;
        private Button         btnOK;
        private Button         btnCancel;

        public Test ResultTest { get; private set; } = new Test();

        private readonly Test? _existing;

        public TestEditDialog(Test? existing)
        {
            _existing = existing;
            InitializeComponent();
            if (existing != null) Populate(existing);
        }

        private void InitializeComponent()
        {
            Text            = _existing == null ? "New Test" : $"Edit Test: {_existing.Title}";
            ClientSize      = new Size(500, 340);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;

            AddLabel("Title:",        30,  30);
            AddLabel("Description:",  30,  75);
            AddLabel("Max Attempts (0 = unlimited):", 30, 200);

            txtTitle = new TextBox { Location = new Point(180, 27), Size = new Size(290, 23) };

            txtDescription = new TextBox
            {
                Location   = new Point(180, 72),
                Size       = new Size(290, 110),
                Multiline  = true,
                ScrollBars = ScrollBars.Vertical
            };

            nudMaxAttempts = new NumericUpDown
            {
                Location = new Point(290, 197),
                Size     = new Size(80, 23),
                Minimum  = 0,
                Maximum  = 99,
                Value    = 3
            };

            btnOK = new Button
            {
                Text     = "Save",
                Location = new Point(180, 265),
                Size     = new Size(120, 36),
                BackColor = Color.FromArgb(30, 120, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button { Text = "Cancel", Location = new Point(320, 265), Size = new Size(100, 36) };
            btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { txtTitle, txtDescription, nudMaxAttempts, btnOK, btnCancel });
        }

        private void AddLabel(string text, int x, int y)
        {
            Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true,
                Font = new Font(Font, FontStyle.Bold) });
        }

        private void Populate(Test t)
        {
            txtTitle.Text             = t.Title;
            txtDescription.Text       = t.Description ?? "";
            nudMaxAttempts.Value      = Math.Clamp(t.MaxAttempts, 0, 99);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            if (string.IsNullOrEmpty(title)) { MessageBox.Show("Title is required."); return; }

            ResultTest = new Test
            {
                Id           = _existing?.Id ?? 0,
                Title        = title,
                Description  = txtDescription.Text.Trim(),
                MaxAttempts  = (int)nudMaxAttempts.Value,
                IsPublished  = _existing?.IsPublished ?? false
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }

    public class QuestionManagerForm : Form
    {
        private readonly Test                    _test;
        private readonly NetworkServiceContainer _svc;

        private DataGridView dgvQuestions;
        private Button btnAddQuestion;
        private Button btnEditQuestion;
        private Button btnDeleteQuestion;
        private Button btnUploadImage;
        private Button btnRemoveImage;
        private Button btnClose;
        private Label  lblTitle;
        private Label  lblStatus;

        public QuestionManagerForm(Test test, NetworkServiceContainer svc)
        {
            _test = test;
            _svc  = svc;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text          = $"Questions — {_test.Title}";
            ClientSize    = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox   = false;

            lblTitle = new Label
            {
                Text     = $"Test: {_test.Title}",
                Location = new Point(20, 15),
                Font     = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize = true
            };

            dgvQuestions = new DataGridView
            {
                Location              = new Point(20, 50),
                Size                  = new Size(860, 420),
                ReadOnly              = true,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect           = false,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible     = false
            };

            int bx = 20, by = 490, bw = 135, bh = 40, gap = 8;

            btnAddQuestion    = MakeBtn("➕  Add",         Color.FromArgb(40,  160,  80), new Point(bx,                  by), bw, bh);
            btnEditQuestion   = MakeBtn("✏️  Edit",         Color.FromArgb(30,  120, 200), new Point(bx+(bw+gap),         by), bw, bh);
            btnDeleteQuestion = MakeBtn("🗑️  Delete",       Color.FromArgb(200,  50,  50), new Point(bx+(bw+gap)*2,       by), bw, bh);
            btnUploadImage    = MakeBtn("🖼️  Upload Image",  Color.FromArgb(100, 100, 100), new Point(bx+(bw+gap)*3,       by), bw+20, bh);
            btnRemoveImage    = MakeBtn("❌ Remove Image",   Color.FromArgb(150,  80,  30), new Point(bx+(bw+gap)*4+20,    by), bw+20, bh);
            btnClose          = MakeBtn("Close",            Color.FromArgb(80,   80,  80), new Point(860-120,             by), 120,  bh);

            btnAddQuestion.Click    += BtnAddQuestion_Click;
            btnEditQuestion.Click   += BtnEditQuestion_Click;
            btnDeleteQuestion.Click += BtnDeleteQuestion_Click;
            btnUploadImage.Click    += BtnUploadImage_Click;
            btnRemoveImage.Click    += BtnRemoveImage_Click;
            btnClose.Click          += (_, __) => Close();

            lblStatus = new Label { Location = new Point(20, 545), Size = new Size(860, 22), ForeColor = Color.DimGray };

            Controls.AddRange(new Control[]
            {
                lblTitle, dgvQuestions,
                btnAddQuestion, btnEditQuestion, btnDeleteQuestion,
                btnUploadImage, btnRemoveImage, btnClose,
                lblStatus
            });

            Load += async (_, __) => await LoadQuestions();
        }

        private static Button MakeBtn(string text, Color back, Point loc, int w, int h)
        {
            return new Button
            {
                Text      = text,
                Location  = loc,
                Size      = new Size(w, h),
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private async Task LoadQuestions()
        {
            try
            {
                lblStatus.Text = "Loading…";
                var questions  = await _svc.QuestionEditorService.GetQuestionsByTestIdAsync(_test.Id);

                dgvQuestions.DataSource = questions.Select((q, i) => new
                {
                    q.Id,
                    Order     = i + 1,
                    q.Text,
                    Type      = q.Type == QuestionType.SingleChoice ? "Single Choice" : "Multiple Choice",
                    q.Weight,
                    Answers   = q.Answers.Count,
                    HasImage  = q.ImageData != null && q.ImageData.Length > 0 ? "Yes" : "No"
                }).ToList();

                lblStatus.Text = $"Loaded {questions.Count} question(s).";
            }
            catch (Exception ex) { lblStatus.Text = $"Error: {ex.Message}"; }
        }

        private Question? SelectedQuestion()
        {
            if (dgvQuestions.SelectedRows.Count == 0) { MessageBox.Show("Select a question first."); return null; }
            int id = (int)dgvQuestions.SelectedRows[0].Cells["Id"].Value;
            return new Question { Id = id };
        }

        private async void BtnAddQuestion_Click(object sender, EventArgs e)
        {
            using var dlg = new QuestionEditDialog(null, _test.Id);
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                var created = await _svc.QuestionEditorService.CreateQuestionAsync(dlg.ResultQuestion);
                if (created == null) { MessageBox.Show("Failed to create question."); return; }
                lblStatus.Text = "Question added.";
                await LoadQuestions();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }

        private async void BtnEditQuestion_Click(object sender, EventArgs e)
        {
            var stub = SelectedQuestion();
            if (stub == null) return;

            try
            {
                var questions = await _svc.QuestionEditorService.GetQuestionsByTestIdAsync(_test.Id);
                var full      = questions.FirstOrDefault(q => q.Id == stub.Id);
                if (full == null) { MessageBox.Show("Question not found."); return; }

                using var dlg = new QuestionEditDialog(full, _test.Id);
                if (dlg.ShowDialog() != DialogResult.OK) return;

                bool ok = await _svc.QuestionEditorService.UpdateQuestionAsync(dlg.ResultQuestion);
                if (!ok) { MessageBox.Show("Update failed."); return; }
                lblStatus.Text = "Question updated.";
                await LoadQuestions();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }

        private async void BtnDeleteQuestion_Click(object sender, EventArgs e)
        {
            var stub = SelectedQuestion();
            if (stub == null) return;

            if (MessageBox.Show("Delete this question and all its answers?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                bool ok = await _svc.QuestionEditorService.DeleteQuestionAsync(stub.Id);
                if (!ok) { MessageBox.Show("Delete failed."); return; }
                lblStatus.Text = $"Deleted question ID {stub.Id}.";
                await LoadQuestions();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }

        private async void BtnUploadImage_Click(object sender, EventArgs e)
        {
            var stub = SelectedQuestion();
            if (stub == null) return;

            using var ofd = new OpenFileDialog
            {
                Title  = "Select Image",
                Filter = "Images (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                string ext      = Path.GetExtension(ofd.FileName).ToLower();
                string mimeType = ext switch { ".jpg" or ".jpeg" => "image/jpeg", ".gif" => "image/gif",
                                               ".bmp" => "image/bmp", _ => "image/png" };
                byte[] imageData = await File.ReadAllBytesAsync(ofd.FileName);

                bool ok = await _svc.QuestionEditorService.UploadQuestionImageAsync(stub.Id, imageData, mimeType);
                if (!ok) { MessageBox.Show("Upload failed."); return; }
                lblStatus.Text = "Image uploaded successfully.";
                await LoadQuestions();
            }
            catch (Exception ex) { MessageBox.Show($"Upload error: {ex.Message}"); }
        }

        private async void BtnRemoveImage_Click(object sender, EventArgs e)
        {
            var stub = SelectedQuestion();
            if (stub == null) return;

            if (MessageBox.Show("Remove image from this question?",
                "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                bool ok = await _svc.QuestionEditorService.UploadQuestionImageAsync(stub.Id, Array.Empty<byte>(), "");
                lblStatus.Text = ok ? "Image removed." : "Failed to remove image.";
                await LoadQuestions();
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }
    }

    public class QuestionEditDialog : Form
    {
        private readonly Question? _existing;
        private readonly int       _testId;

        private TextBox       txtQuestion;
        private ComboBox      cmbType;
        private NumericUpDown nudWeight;
        private DataGridView  dgvAnswers;
        private Button        btnAddAnswer;
        private Button        btnRemoveAnswer;
        private Button        btnOK;
        private Button        btnCancel;

        public Question ResultQuestion { get; private set; } = new Question();

        public QuestionEditDialog(Question? existing, int testId)
        {
            _existing = existing;
            _testId   = testId;
            InitializeComponent();
            if (existing != null) Populate(existing);
        }

        private void InitializeComponent()
        {
            Text            = _existing == null ? "New Question" : "Edit Question";
            ClientSize      = new Size(700, 560);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;

            AddLabel("Question Text:",    20, 20);
            txtQuestion = new TextBox
            {
                Location   = new Point(160, 17),
                Size       = new Size(510, 80),
                Multiline  = true,
                ScrollBars = ScrollBars.Vertical
            };

            AddLabel("Question Type:",    20, 110);
            cmbType = new ComboBox
            {
                Location     = new Point(160, 107),
                Size         = new Size(220, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbType.Items.AddRange(new object[] { "Single Choice", "Multiple Choice" });
            cmbType.SelectedIndex = 0;

            AddLabel("Weight (points):",  20, 143);
            nudWeight = new NumericUpDown
            {
                Location = new Point(160, 140),
                Size     = new Size(80, 23),
                Minimum  = 1,
                Maximum  = 100,
                Value    = 1
            };

            var grpAnswers = new GroupBox
            {
                Text     = "Answer Options  (check ✔ = Correct Answer)",
                Location = new Point(20, 180),
                Size     = new Size(655, 290),
                Font     = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            dgvAnswers = new DataGridView
            {
                Location              = new Point(10, 25),
                Size                  = new Size(635, 215),
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible     = false,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect
            };

            var colText = new DataGridViewTextBoxColumn
                { Name = "Text", HeaderText = "Answer Text", FillWeight = 80, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            var colCorrect = new DataGridViewCheckBoxColumn
                { Name = "IsCorrect", HeaderText = "Correct", Width = 70, FillWeight = 20 };

            dgvAnswers.Columns.AddRange(colText, colCorrect);

            btnAddAnswer    = new Button { Text = "➕ Add",    Location = new Point(10, 248), Size = new Size(100, 30), FlatStyle = FlatStyle.Flat };
            btnRemoveAnswer = new Button { Text = "🗑️ Remove",  Location = new Point(120, 248), Size = new Size(100, 30), FlatStyle = FlatStyle.Flat };

            btnAddAnswer.Click    += (_, __) =>
            {
                int idx = dgvAnswers.Rows.Add("New answer", false);
                dgvAnswers.ClearSelection();
                dgvAnswers.Rows[idx].Selected = true;
                dgvAnswers.BeginEdit(false);
            };
            btnRemoveAnswer.Click += (_, __) =>
            {
                if (dgvAnswers.SelectedRows.Count > 0)
                    dgvAnswers.Rows.Remove(dgvAnswers.SelectedRows[0]);
            };

            grpAnswers.Controls.AddRange(new Control[] { dgvAnswers, btnAddAnswer, btnRemoveAnswer });

            btnOK = new Button
            {
                Text      = "Save",
                Location  = new Point(200, 490),
                Size      = new Size(130, 38),
                BackColor = Color.FromArgb(30, 120, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button { Text = "Cancel", Location = new Point(350, 490), Size = new Size(110, 38), FlatStyle = FlatStyle.Flat };
            btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { txtQuestion, cmbType, nudWeight, grpAnswers, btnOK, btnCancel });
        }

        private void AddLabel(string text, int x, int y)
        {
            Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true,
                Font = new Font(Font, FontStyle.Bold) });
        }

        private void Populate(Question q)
        {
            txtQuestion.Text      = q.Text;
            cmbType.SelectedIndex = q.Type == QuestionType.SingleChoice ? 0 : 1;
            nudWeight.Value       = Math.Clamp(q.Weight, 1, 100);

            foreach (var ans in q.Answers)
                dgvAnswers.Rows.Add(ans.Text, ans.IsCorrect);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string text = txtQuestion.Text.Trim();
            if (string.IsNullOrEmpty(text)) { MessageBox.Show("Question text is required."); return; }
            if (dgvAnswers.Rows.Count == 0) { MessageBox.Show("Add at least one answer option."); return; }

            bool hasCorrect = dgvAnswers.Rows.Cast<DataGridViewRow>()
                .Any(r => r.Cells["IsCorrect"].Value is true);
            if (!hasCorrect) { MessageBox.Show("Mark at least one answer as correct."); return; }

            var answers = new List<Answer>();
            int order   = 0;
            foreach (DataGridViewRow row in dgvAnswers.Rows)
            {
                string ansText  = row.Cells["Text"].Value?.ToString()?.Trim() ?? "";
                bool isCorrect  = row.Cells["IsCorrect"].Value is true;
                if (string.IsNullOrEmpty(ansText)) continue;
                answers.Add(new Answer { Text = ansText, IsCorrect = isCorrect, Order = order++ });
            }

            ResultQuestion = new Question
            {
                Id     = _existing?.Id ?? 0,
                TestId = _testId,
                Text   = text,
                Type   = cmbType.SelectedIndex == 0 ? QuestionType.SingleChoice : QuestionType.MultipleChoice,
                Weight = (int)nudWeight.Value,
                Order  = _existing?.Order ?? 0,
                Answers = answers
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
