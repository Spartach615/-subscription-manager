using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SubscriptionManager
{
    public class Form1 : Form
    {
        private Panel mainContainer;

        private Panel loginPanel;
        private Panel registerPanel;
        private Panel userPanel;
        private Panel adminPanel;

        private TextBox txtLoginEmail, txtLoginPassword;
        private GradientButton btnLogin, btnGoToRegister;
        private Label lblLoginTitle;

        private TextBox txtRegName, txtRegEmail, txtRegPassword;
        private GradientButton btnRegister, btnBackToLogin;
        private Label lblRegTitle;

        private Label lblUserBalance;
        private GradientButton btnAddBalance, btnUserLogout, btnRefreshUser;
        private FlowLayoutPanel flowAvailableSubs;
        private FlowLayoutPanel flowMySubs;
        private int currentUserId;

        private TabControl adminTabControl;
        private DataGridView dgvUsers, dgvTariffs, dgvUserSubscriptions;
        private ComboBox cmbUserForSubs;
        private GradientButton btnChangeBalance, btnChangeRole, btnRefreshUsers;
        private GradientButton btnAddTariff, btnEditPrice, btnRefreshTariffs;
        private GradientButton adminLogout;
        private int adminId;

        public Form1()
        {
            this.Text = "Менеджер подписок";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1100, 700);
            this.MinimumSize = new Size(1100, 700);
            this.MaximumSize = new Size(1100, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);

            mainContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            this.Controls.Add(mainContainer);

            CreateLoginPanel();
            CreateRegisterPanel();
            CreateUserPanel();
            CreateAdminPanel();

            ShowLoginPanel();

            this.Resize += (s, e) =>
            {
                if (loginPanel.Parent == mainContainer) CenterPanel(loginPanel);
                if (registerPanel.Parent == mainContainer) CenterPanel(registerPanel);
            };
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x00020000;
                return cp;
            }
        }

        private void ClearContainer() => mainContainer.Controls.Clear();

        private void ShowLoginPanel()
        {
            ClearContainer();
            mainContainer.Controls.Add(loginPanel);
            CenterPanel(loginPanel);
            txtLoginEmail.Text = "";
            txtLoginPassword.Text = "";
            txtLoginEmail.Focus();
        }

        private void ShowRegisterPanel()
        {
            ClearContainer();
            mainContainer.Controls.Add(registerPanel);
            CenterPanel(registerPanel);
            txtRegName.Text = "";
            txtRegEmail.Text = "";
            txtRegPassword.Text = "";
            txtRegName.Focus();
        }

        private void ShowUserPanel(int userId)
        {
            currentUserId = userId;
            LoadUserData();
            ClearContainer();
            userPanel.Dock = DockStyle.Fill;
            mainContainer.Controls.Add(userPanel);
        }

        private void ShowAdminPanel(int adminId)
        {
            this.adminId = adminId;
            ClearContainer();
            adminPanel.Dock = DockStyle.Fill;
            mainContainer.Controls.Add(adminPanel);
            LoadAdminUsers();
            LoadAdminTariffs();
            LoadAdminUserCombo();
        }

        private void CreateLoginPanel()
        {
            loginPanel = new RoundedPanel
            {
                Size = new Size(420, 530),
                BackColor = Color.White,
                Anchor = AnchorStyles.None,
                CornerRadius = 20
            };

            Label lblIcon = new Label
            {
                Text = "🔐",
                Font = new Font("Segoe UI", 40, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 70,
                Width = 90,
                ForeColor = Color.FromArgb(52, 152, 219)
            };

            lblLoginTitle = new Label
            {
                Text = "Вход в систему",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(0, 0, 0, 10)
            };

            Label lblSubtitle = new Label
            {
                Text = "Управляйте своими подписками легко",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(149, 165, 166),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 25,
                Padding = new Padding(0, 0, 0, 20)
            };

            Panel inputPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 160,
                Padding = new Padding(40, 10, 40, 0)
            };

            Panel emailPanel = CreateInputField("✉️", "Email", 0);
            txtLoginEmail = (TextBox)emailPanel.Controls[2];
            Panel emailLine = (Panel)emailPanel.Controls[3];

            Panel passPanel = CreateInputField("🔒", "Пароль", 70);
            txtLoginPassword = (TextBox)passPanel.Controls[2];
            txtLoginPassword.PasswordChar = '•';
            Panel passLine = (Panel)passPanel.Controls[3];

            inputPanel.Controls.Add(emailPanel);
            inputPanel.Controls.Add(passPanel);

            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(0, 20, 0, 0)
            };

            btnLogin = new GradientButton
            {
                Text = "Войти",
                Size = new Size(340, 42),
                Location = new Point(40, 0),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                StartColor = Color.FromArgb(52, 152, 219),
                EndColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                CornerRadius = 10,
                Cursor = Cursors.Hand
            };
            btnLogin.Click += BtnLogin_Click;
            buttonPanel.Controls.Add(btnLogin);

            Panel linkPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(0, 15, 0, 0)
            };

            int lblWidth = 105, btnWidth = 115, spacing = 10;
            int totalLinkWidth = lblWidth + spacing + btnWidth;
            int linkLeft = (420 - totalLinkWidth) / 2;

            Label lblNoAccount = new Label
            {
                Text = "Нет аккаунта?",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(linkLeft, 5),
                Size = new Size(lblWidth, 23),
                TextAlign = ContentAlignment.MiddleRight
            };

            btnGoToRegister = new GradientButton
            {
                Text = "Регистрация",
                Location = new Point(linkLeft + lblWidth + spacing, 0),
                Size = new Size(btnWidth, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                StartColor = Color.FromArgb(46, 204, 113),
                EndColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };
            btnGoToRegister.Click += (s, e) => ShowRegisterPanel();

            linkPanel.Controls.Add(lblNoAccount);
            linkPanel.Controls.Add(btnGoToRegister);

            loginPanel.Controls.Clear();
            loginPanel.Controls.AddRange(new Control[]
            {
                lblIcon, lblLoginTitle, lblSubtitle, inputPanel, buttonPanel, linkPanel
            });

            txtLoginEmail.Enter += (s, e) => emailLine.BackColor = Color.FromArgb(52, 152, 219);
            txtLoginEmail.Leave += (s, e) => emailLine.BackColor = Color.FromArgb(189, 195, 199);
            txtLoginPassword.Enter += (s, e) => passLine.BackColor = Color.FromArgb(52, 152, 219);
            txtLoginPassword.Leave += (s, e) => passLine.BackColor = Color.FromArgb(189, 195, 199);

            txtLoginPassword.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                    BtnLogin_Click(s, e);
            };
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtLoginEmail.Text.Trim();
            string pass = txtLoginPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Пожалуйста, введите email и пароль", "Ошибка входа",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (userId, role) = DatabaseHelper.AuthenticateUser(email, pass);

            if (userId == -1)
            {
                MessageBox.Show("Неверный email или пароль", "Ошибка входа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (role == "admin")
                ShowAdminPanel(userId);
            else
                ShowUserPanel(userId);
        }

        private void CreateRegisterPanel()
        {
            registerPanel = new RoundedPanel
            {
                Size = new Size(420, 500),
                BackColor = Color.White,
                Anchor = AnchorStyles.None,
                CornerRadius = 20
            };

            Label lblIcon = new Label
            {
                Text = "✨",
                Font = new Font("Segoe UI", 40, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 70,
                ForeColor = Color.FromArgb(46, 204, 113)
            };

            lblRegTitle = new Label
            {
                Text = "Создание аккаунта",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(0, 0, 0, 10)
            };

            Panel inputPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 230,
                Padding = new Padding(40, 10, 40, 0)
            };

            Panel namePanel = CreateInputField("👤", "Имя", 0);
            txtRegName = (TextBox)namePanel.Controls[2];
            Panel nameLine = (Panel)namePanel.Controls[3];

            Panel emailPanel = CreateInputField("✉️", "Email", 70);
            txtRegEmail = (TextBox)emailPanel.Controls[2];
            Panel emailLine = (Panel)emailPanel.Controls[3];

            Panel passPanel = CreateInputField("🔒", "Пароль", 140);
            txtRegPassword = (TextBox)passPanel.Controls[2];
            txtRegPassword.PasswordChar = '•';
            Panel passLine = (Panel)passPanel.Controls[3];

            inputPanel.Controls.AddRange(new Control[] { namePanel, emailPanel, passPanel });

            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(0, 20, 0, 0)
            };

            btnRegister = new GradientButton
            {
                Text = "Зарегистрироваться",
                Size = new Size(340, 42),
                Location = new Point(40, 0),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                StartColor = Color.FromArgb(46, 204, 113),
                EndColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                CornerRadius = 10,
                Cursor = Cursors.Hand
            };
            btnRegister.Click += BtnRegister_Click;
            buttonPanel.Controls.Add(btnRegister);

            Panel linkPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(0, 15, 0, 0)
            };

            int lblWidth = 120, btnWidth = 115, spacing = 10;
            int totalLinkWidth = lblWidth + spacing + btnWidth;
            int linkLeft = (420 - totalLinkWidth) / 2;

            Label lblHaveAccount = new Label
            {
                Text = "Уже есть аккаунт?",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(linkLeft, 5),
                Size = new Size(lblWidth, 23),
                TextAlign = ContentAlignment.MiddleRight
            };

            btnBackToLogin = new GradientButton
            {
                Text = "← Войти",
                Location = new Point(linkLeft + lblWidth + spacing, 0),
                Size = new Size(btnWidth, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                StartColor = Color.FromArgb(108, 117, 125),
                EndColor = Color.FromArgb(88, 97, 105),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };
            btnBackToLogin.Click += (s, e) => ShowLoginPanel();

            linkPanel.Controls.Add(lblHaveAccount);
            linkPanel.Controls.Add(btnBackToLogin);

            registerPanel.Controls.AddRange(new Control[]
            {
                lblIcon, lblRegTitle, inputPanel, buttonPanel, linkPanel
            });

            txtRegName.Enter += (s, e) => nameLine.BackColor = Color.FromArgb(46, 204, 113);
            txtRegName.Leave += (s, e) => nameLine.BackColor = Color.FromArgb(189, 195, 199);
            txtRegEmail.Enter += (s, e) => emailLine.BackColor = Color.FromArgb(46, 204, 113);
            txtRegEmail.Leave += (s, e) => emailLine.BackColor = Color.FromArgb(189, 195, 199);
            txtRegPassword.Enter += (s, e) => passLine.BackColor = Color.FromArgb(46, 204, 113);
            txtRegPassword.Leave += (s, e) => passLine.BackColor = Color.FromArgb(189, 195, 199);
        }

        private Panel CreateInputField(string icon, string placeholder, int yPosition)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, yPosition),
                Size = new Size(340, 65)
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 14),
                Location = new Point(0, 15),
                Size = new Size(30, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblPlaceholder = new Label
            {
                Text = placeholder,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(35, 0),
                Size = new Size(305, 18)
            };

            TextBox textBox = new TextBox
            {
                Location = new Point(35, 20),
                Size = new Size(305, 30),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            Panel line = new Panel
            {
                Location = new Point(35, 52),
                Size = new Size(305, 2),
                BackColor = Color.FromArgb(189, 195, 199)
            };

            panel.Controls.AddRange(new Control[] { lblIcon, lblPlaceholder, textBox, line });
            return panel;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string name = txtRegName.Text.Trim();
            string email = txtRegEmail.Text.Trim();
            string pass = txtRegPassword.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка регистрации",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DatabaseHelper.RegisterUser(name, email, pass))
            {
                MessageBox.Show("Регистрация прошла успешно!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowLoginPanel();
            }
            else
            {
                MessageBox.Show("Пользователь с таким email уже существует", "Ошибка регистрации",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateUserPanel()
        {
            userPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(20)
            };

            RoundedPanel topPanel = new RoundedPanel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White,
                CornerRadius = 15,
                BorderWidth = 0,
                Padding = new Padding(20)
            };

            lblUserBalance = new Label
            {
                Text = "Баланс: 0,00 ₽",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            btnAddBalance = new GradientButton
            {
                Text = "💰 Пополнить",
                Size = new Size(130, 38),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(52, 152, 219),
                EndColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };
            btnAddBalance.Click += BtnAddBalance_Click;

            btnUserLogout = new GradientButton
            {
                Text = "🚪 Выйти",
                Size = new Size(100, 38),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(231, 76, 60),
                EndColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };
            btnUserLogout.Click += (s, e) => ShowLoginPanel();

            topPanel.Controls.Add(lblUserBalance);
            topPanel.Controls.Add(btnAddBalance);
            topPanel.Controls.Add(btnUserLogout);

            topPanel.Resize += (s, e) =>
            {
                btnAddBalance.Location = new Point(topPanel.Width - 250, 16);
                btnUserLogout.Location = new Point(topPanel.Width - 120, 16);
            };

            Label lblAvailable = new Label
            {
                Text = "Доступные подписки",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 90),
                AutoSize = true
            };

            flowAvailableSubs = new FlowLayoutPanel
            {
                Location = new Point(20, 125),
                Size = new Size(userPanel.Width - 40, 260),
                AutoScroll = true,
                WrapContents = true,
                Padding = new Padding(5),
                BackColor = Color.Transparent
            };

            Label lblMy = new Label
            {
                Text = "Мои подписки",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 400),
                AutoSize = true
            };

            flowMySubs = new FlowLayoutPanel
            {
                Location = new Point(20, 435),
                Size = new Size(userPanel.Width - 40, 180),
                AutoScroll = true,
                WrapContents = true,
                Padding = new Padding(5),
                BackColor = Color.Transparent
            };

            btnRefreshUser = new GradientButton
            {
                Text = "🔄 Обновить",
                Size = new Size(110, 35),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(108, 117, 125),
                EndColor = Color.FromArgb(88, 97, 105),
                ForeColor = Color.White,
                CornerRadius = 8,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Cursor = Cursors.Hand
            };
            btnRefreshUser.Click += (s, e) => LoadUserData();

            userPanel.Controls.Add(topPanel);
            userPanel.Controls.Add(lblAvailable);
            userPanel.Controls.Add(flowAvailableSubs);
            userPanel.Controls.Add(lblMy);
            userPanel.Controls.Add(flowMySubs);
            userPanel.Controls.Add(btnRefreshUser);

            userPanel.Resize += (s, e) =>
            {
                flowAvailableSubs.Width = userPanel.Width - 40;
                flowMySubs.Width = userPanel.Width - 40;
                btnRefreshUser.Location = new Point(20, userPanel.Height - 55);
            };
        }

        private void LoadUserData()
        {
            if (currentUserId == 0) return;

            decimal balance = DatabaseHelper.GetUserBalance(currentUserId);
            lblUserBalance.Text = $"Баланс: {balance:N2} ₽";

            flowAvailableSubs.Controls.Clear();
            DataTable allSubs = DatabaseHelper.GetPodpiski();

            foreach (DataRow row in allSubs.Rows)
            {
                if (!Convert.ToBoolean(row["is_active"])) continue;

                int id = Convert.ToInt32(row["id"]);
                string name = row["name"].ToString();
                string desc = row["description"]?.ToString() ?? "Описание отсутствует";
                decimal price = Convert.ToDecimal(row["price"]);
                int duration = Convert.ToInt32(row["duration_days"]);

                var card = new SubscriptionCard(name, desc, price, duration, id);
                card.OnBuy += (subId) => { BuySubscription(subId, price, duration); };
                flowAvailableSubs.Controls.Add(card);
            }

            flowMySubs.Controls.Clear();
            DataTable mySubs = DatabaseHelper.GetUserSubscriptions(currentUserId);

            if (mySubs.Rows.Count == 0)
            {
                Label noSubs = new Label
                {
                    Text = "У вас пока нет активных подписок",
                    Font = new Font("Segoe UI", 9.75f),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(10, 10)
                };
                flowMySubs.Controls.Add(noSubs);
            }
            else
            {
                foreach (DataRow row in mySubs.Rows)
                {
                    string name = row["name"].ToString();
                    DateTime start = Convert.ToDateTime(row["start_date"]);
                    DateTime end = Convert.ToDateTime(row["end_date"]);
                    string status = row["status"].ToString();
                    decimal pricePaid = Convert.ToDecimal(row["price_paid"]);
                    bool isActive = status == "active" && end > DateTime.Now;

                    var card = new SubscriptionStatusCard(name, start, end, pricePaid, isActive);
                    flowMySubs.Controls.Add(card);
                }
            }
        }

        private void BuySubscription(int podpiskaId, decimal price, int durationDays)
        {
            var activeSub = DatabaseHelper.GetActiveSubscriptionForUser(currentUserId, podpiskaId);

            string input = Prompt.ShowDialog(
                $"На какой срок вы хотите приобрести подписку?\n\n" +
                $"Стоимость: {price:N2} ₽ за {durationDays} дн.\n\n" +
                $"Введите количество периодов (1, 2, 3...):",
                "Покупка подписки",
                "1");

            if (string.IsNullOrEmpty(input))
                return;

            if (!int.TryParse(input, out int periods) || periods <= 0)
            {
                MessageBox.Show("Введите корректное количество периодов", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totalPrice = price * periods;
            int totalDays = durationDays * periods;

            DateTime newEndDate;
            if (activeSub != null)
            {
                newEndDate = activeSub.EndDate > DateTime.Now
                    ? activeSub.EndDate.AddDays(totalDays)
                    : DateTime.Now.AddDays(totalDays);
            }
            else
            {
                newEndDate = DateTime.Now.AddDays(totalDays);
            }

            if (DatabaseHelper.GetUserBalance(currentUserId) < totalPrice)
            {
                MessageBox.Show($"Недостаточно средств на балансе.\nНеобходимо: {totalPrice:N2} ₽",
                    "Недостаточно средств", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string message = activeSub != null
                ? $"Продлить подписку на {totalDays} дн. ({periods} период(ов))?\n\nСумма: {totalPrice:N2} ₽\nНовая дата окончания: {newEndDate:dd.MM.yyyy}"
                : $"Приобрести подписку на {totalDays} дн. ({periods} период(ов))?\n\nСумма: {totalPrice:N2} ₽\nДата окончания: {newEndDate:dd.MM.yyyy}";

            DialogResult result = MessageBox.Show(message, "Подтверждение покупки",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool success;
                if (activeSub != null)
                    success = DatabaseHelper.ExtendSubscription(currentUserId, podpiskaId, totalDays, totalPrice);
                else
                    success = DatabaseHelper.PurchaseSubscription(currentUserId, podpiskaId, totalDays, totalPrice);

                if (success)
                {
                    MessageBox.Show("Подписка успешно оформлена!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUserData();
                }
                else
                {
                    MessageBox.Show("Произошла ошибка при оформлении подписки", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnAddBalance_Click(object sender, EventArgs e)
        {
            string input = Prompt.ShowDialog("Введите сумму пополнения (₽):", "Пополнение баланса", "100");

            if (decimal.TryParse(input, out decimal amount) && amount > 0)
            {
                DatabaseHelper.AddToBalance(currentUserId, amount);
                LoadUserData();
                MessageBox.Show($"Баланс пополнен на {amount:N2} ₽", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Введите корректную сумму", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CreateAdminPanel()
        {
            adminPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10)
            };

            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                Padding = new Padding(0, 5, 10, 0)
            };

            adminLogout = new GradientButton
            {
                Text = "🚪 Выйти",
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(231, 76, 60),
                EndColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            adminLogout.Click += (s, e) => ShowLoginPanel();

            topBar.Controls.Add(adminLogout);
            topBar.Resize += (s, e) =>
            {
                adminLogout.Location = new Point(topBar.Width - 110, 5);
            };

            adminTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.75f),
                Margin = new Padding(0, 5, 0, 0)
            };

            adminPanel.Controls.Add(adminTabControl);
            adminPanel.Controls.Add(topBar);

            TabPage tpUsers = new TabPage("👥 Пользователи");

            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true
            };
            StyleDataGridView(dgvUsers);

            btnChangeBalance = new GradientButton
            {
                Text = "💰 Изменить баланс",
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(52, 152, 219),
                EndColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };

            btnChangeRole = new GradientButton
            {
                Text = "🔄 Сменить роль",
                Size = new Size(140, 35),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(241, 196, 15),
                EndColor = Color.FromArgb(243, 156, 18),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };

            btnRefreshUsers = new GradientButton
            {
                Text = "🔄 Обновить",
                Size = new Size(110, 35),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(108, 117, 125),
                EndColor = Color.FromArgb(88, 97, 105),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };

            Panel pnlUsers = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                Padding = new Padding(10, 10, 10, 10)
            };
            pnlUsers.Controls.AddRange(new Control[] { btnChangeBalance, btnChangeRole, btnRefreshUsers });

            btnChangeBalance.Location = new Point(10, 10);
            btnChangeRole.Location = new Point(170, 10);
            btnRefreshUsers.Location = new Point(320, 10);

            tpUsers.Controls.Add(dgvUsers);
            tpUsers.Controls.Add(pnlUsers);

            btnChangeBalance.Click += BtnChangeBalance_Click;
            btnChangeRole.Click += BtnChangeRole_Click;
            btnRefreshUsers.Click += (s, e) => LoadAdminUsers();

            TabPage tpTariffs = new TabPage("📋 Тарифы");

            dgvTariffs = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true
            };
            StyleDataGridView(dgvTariffs);

            btnAddTariff = new GradientButton
            {
                Text = "➕ Добавить тариф",
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(46, 204, 113),
                EndColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };

            btnEditPrice = new GradientButton
            {
                Text = "💰 Изменить цену",
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(241, 196, 15),
                EndColor = Color.FromArgb(243, 156, 18),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };

            btnRefreshTariffs = new GradientButton
            {
                Text = "🔄 Обновить",
                Size = new Size(110, 35),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(108, 117, 125),
                EndColor = Color.FromArgb(88, 97, 105),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };

            Panel pnlTariffs = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                Padding = new Padding(10, 10, 10, 10)
            };
            pnlTariffs.Controls.AddRange(new Control[] { btnAddTariff, btnEditPrice, btnRefreshTariffs });

            btnAddTariff.Location = new Point(10, 10);
            btnEditPrice.Location = new Point(170, 10);
            btnRefreshTariffs.Location = new Point(330, 10);

            tpTariffs.Controls.Add(dgvTariffs);
            tpTariffs.Controls.Add(pnlTariffs);

            btnAddTariff.Click += BtnAddTariff_Click;
            btnEditPrice.Click += BtnEditPrice_Click;
            btnRefreshTariffs.Click += (s, e) => LoadAdminTariffs();

            TabPage tpUserSubs = new TabPage("📊 Подписки пользователей");

            Label lblUser = new Label
            {
                Text = "Пользователь:",
                Location = new Point(15, 15),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold)
            };

            cmbUserForSubs = new ComboBox
            {
                Location = new Point(120, 12),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.75f)
            };

            dgvUserSubscriptions = new DataGridView
            {
                Location = new Point(15, 50),
                Size = new Size(990, 500),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                ReadOnly = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            StyleDataGridView(dgvUserSubscriptions);

            tpUserSubs.Controls.Add(lblUser);
            tpUserSubs.Controls.Add(cmbUserForSubs);
            tpUserSubs.Controls.Add(dgvUserSubscriptions);
            cmbUserForSubs.SelectedIndexChanged += CmbUserForSubs_SelectedIndexChanged;

            adminTabControl.TabPages.Add(tpUsers);
            adminTabControl.TabPages.Add(tpTariffs);
            adminTabControl.TabPages.Add(tpUserSubs);
        }

        private void LoadAdminUsers()
        {
            DataTable users = DatabaseHelper.GetAllUsersWithBalance();
            dgvUsers.DataSource = users;

            if (dgvUsers.Columns.Contains("password_hash"))
                dgvUsers.Columns["password_hash"].Visible = false;

            ApplyRussianHeadersUsers();
        }

        private void LoadAdminTariffs()
        {
            DataTable tariffs = DatabaseHelper.GetPodpiski();
            dgvTariffs.DataSource = tariffs;

            if (dgvTariffs.Columns.Contains("id"))
                dgvTariffs.Columns["id"].Visible = false;

            ApplyRussianHeadersTariffs();
        }

        private void LoadAdminUserCombo()
        {
            DataTable users = DatabaseHelper.GetAllUsersWithBalance();
            cmbUserForSubs.DisplayMember = "name";
            cmbUserForSubs.ValueMember = "id";
            cmbUserForSubs.DataSource = users;
        }

        private void CmbUserForSubs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUserForSubs.SelectedValue != null &&
                int.TryParse(cmbUserForSubs.SelectedValue.ToString(), out int uid))
            {
                DataTable subs = DatabaseHelper.GetUserSubscriptions(uid);
                dgvUserSubscriptions.DataSource = subs;
                ApplyRussianHeadersUserSubs();
            }
        }

        private void BtnChangeBalance_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null)
            {
                MessageBox.Show("Пожалуйста, выберите пользователя", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int uid = Convert.ToInt32(dgvUsers.CurrentRow.Cells["id"].Value);
            string userName = dgvUsers.CurrentRow.Cells["name"].Value.ToString();
            decimal curBalance = Convert.ToDecimal(dgvUsers.CurrentRow.Cells["balance"].Value);

            string input = Prompt.ShowDialog(
                $"Пользователь: {userName}\nТекущий баланс: {curBalance:N2} ₽\n\nВведите новый баланс:",
                "Изменение баланса",
                curBalance.ToString("F2"));

            if (decimal.TryParse(input, out decimal newBalance) && newBalance >= 0)
            {
                DatabaseHelper.UpdateUserBalance(uid, newBalance);
                LoadAdminUsers();
                LoadAdminUserCombo();
                MessageBox.Show($"Баланс пользователя {userName} изменен на {newBalance:N2} ₽",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Введите корректное значение баланса", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnChangeRole_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null)
            {
                MessageBox.Show("Пожалуйста, выберите пользователя", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int uid = Convert.ToInt32(dgvUsers.CurrentRow.Cells["id"].Value);
            string userName = dgvUsers.CurrentRow.Cells["name"].Value.ToString();
            string curRole = dgvUsers.CurrentRow.Cells["role"].Value.ToString();
            string newRole = curRole == "admin" ? "user" : "admin";

            if (uid == adminId && newRole == "user")
            {
                MessageBox.Show("Нельзя изменить свою собственную роль!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Изменить роль пользователя {userName} с \"{curRole}\" на \"{newRole}\"?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                DatabaseHelper.ChangeUserRole(uid, newRole);
                LoadAdminUsers();
                LoadAdminUserCombo();
            }
        }

        private void BtnAddTariff_Click(object sender, EventArgs e)
        {
            Form dialog = new Form
            {
                Text = "Новый тариф",
                Size = new Size(400, 370),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Font = new Font("Segoe UI", 9.75f)
            };

            Label lblName = new Label { Text = "Название:", Location = new Point(20, 20), Size = new Size(100, 23) };
            TextBox txtName = new TextBox { Location = new Point(130, 18), Size = new Size(230, 23) };

            Label lblDesc = new Label { Text = "Описание:", Location = new Point(20, 60), Size = new Size(100, 23) };
            TextBox txtDesc = new TextBox { Location = new Point(130, 58), Size = new Size(230, 23) };

            Label lblPrice = new Label { Text = "Цена (₽):", Location = new Point(20, 100), Size = new Size(100, 23) };
            TextBox txtPrice = new TextBox { Location = new Point(130, 98), Size = new Size(100, 23) };

            Label lblDur = new Label { Text = "Длительность (дней):", Location = new Point(20, 140), Size = new Size(100, 23) };
            TextBox txtDuration = new TextBox { Location = new Point(130, 138), Size = new Size(100, 23), Text = "30" };

            Label lblTrial = new Label { Text = "Пробных дней:", Location = new Point(20, 180), Size = new Size(100, 23) };
            TextBox txtTrial = new TextBox { Location = new Point(130, 178), Size = new Size(100, 23), Text = "0" };

            CheckBox chkAuto = new CheckBox
            {
                Text = "Автопродление по умолчанию",
                Location = new Point(130, 220),
                Checked = true,
                Size = new Size(200, 23)
            };

            GradientButton btnSave = new GradientButton
            {
                Text = "Сохранить",
                Location = new Point(130, 270),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                StartColor = Color.FromArgb(46, 204, 113),
                EndColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };

            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    MessageBox.Show("Введите название тарифа", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
                {
                    MessageBox.Show("Введите корректную цену", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtDuration.Text, out int dur) || dur <= 0)
                {
                    MessageBox.Show("Введите корректную длительность", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int trial = int.TryParse(txtTrial.Text, out int t) ? t : 0;

                DatabaseHelper.AddNewSubscription(
                    txtName.Text.Trim(),
                    txtDesc.Text.Trim(),
                    price,
                    dur,
                    trial,
                    chkAuto.Checked);

                LoadAdminTariffs();
                dialog.Close();
            };

            dialog.Controls.AddRange(new Control[]
            {
                lblName, txtName, lblDesc, txtDesc, lblPrice, txtPrice,
                lblDur, txtDuration, lblTrial, txtTrial, chkAuto, btnSave
            });

            dialog.ShowDialog();
        }

        private void BtnEditPrice_Click(object sender, EventArgs e)
        {
            if (dgvTariffs.CurrentRow == null)
            {
                MessageBox.Show("Пожалуйста, выберите тариф", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(dgvTariffs.CurrentRow.Cells["id"].Value);
            string name = dgvTariffs.CurrentRow.Cells["name"].Value.ToString();
            decimal currentPrice = Convert.ToDecimal(dgvTariffs.CurrentRow.Cells["price"].Value);

            string input = Prompt.ShowDialog(
                $"Тариф: {name}\nТекущая цена: {currentPrice:N2} ₽\n\nВведите новую цену:",
                "Изменение цены",
                currentPrice.ToString("F2"));

            if (decimal.TryParse(input, out decimal newPrice) && newPrice >= 0)
            {
                DatabaseHelper.UpdateSubscriptionPrice(id, newPrice);
                LoadAdminTariffs();
                MessageBox.Show($"Цена тарифа \"{name}\" изменена на {newPrice:N2} ₽",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Введите корректную цену", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CenterPanel(Panel panel)
        {
            if (mainContainer != null && panel.Parent == mainContainer)
                panel.Location = new Point(
                    (mainContainer.Width - panel.Width) / 2,
                    (mainContainer.Height - panel.Height) / 2);
        }

        private void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(230, 230, 230);
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowTemplate.Height = 35;
            dgv.Font = new Font("Segoe UI", 9.75f);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.RowsDefaultCellStyle.BackColor = Color.White;
            dgv.RowsDefaultCellStyle.ForeColor = Color.FromArgb(50, 50, 50);
            dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgv.RowsDefaultCellStyle.SelectionForeColor = Color.White;
            dgv.RowsDefaultCellStyle.Padding = new Padding(5, 0, 5, 0);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
        }

        private void ApplyRussianHeadersUsers()
        {
            if (dgvUsers.Columns.Contains("id")) dgvUsers.Columns["id"].HeaderText = "ID";
            if (dgvUsers.Columns.Contains("name")) dgvUsers.Columns["name"].HeaderText = "Имя пользователя";
            if (dgvUsers.Columns.Contains("email")) dgvUsers.Columns["email"].HeaderText = "Email";
            if (dgvUsers.Columns.Contains("role")) dgvUsers.Columns["role"].HeaderText = "Роль";
            if (dgvUsers.Columns.Contains("balance")) dgvUsers.Columns["balance"].HeaderText = "Баланс (₽)";
            if (dgvUsers.Columns.Contains("is_active")) dgvUsers.Columns["is_active"].HeaderText = "Активен";
            if (dgvUsers.Columns.Contains("password_hash"))
            {
                dgvUsers.Columns["password_hash"].Visible = false;
                dgvUsers.Columns["password_hash"].HeaderText = "Хеш пароля";
            }
            if (dgvUsers.Columns.Contains("created_at")) dgvUsers.Columns["created_at"].HeaderText = "Дата регистрации";
        }

        private void ApplyRussianHeadersTariffs()
        {
            if (dgvTariffs.Columns.Contains("id")) dgvTariffs.Columns["id"].HeaderText = "ID";
            if (dgvTariffs.Columns.Contains("name")) dgvTariffs.Columns["name"].HeaderText = "Название";
            if (dgvTariffs.Columns.Contains("description")) dgvTariffs.Columns["description"].HeaderText = "Описание";
            if (dgvTariffs.Columns.Contains("price")) dgvTariffs.Columns["price"].HeaderText = "Цена (₽)";
            if (dgvTariffs.Columns.Contains("duration_days")) dgvTariffs.Columns["duration_days"].HeaderText = "Длительность (дни)";
            if (dgvTariffs.Columns.Contains("trial_days")) dgvTariffs.Columns["trial_days"].HeaderText = "Пробный период (дни)";
            if (dgvTariffs.Columns.Contains("auto_renewal_default")) dgvTariffs.Columns["auto_renewal_default"].HeaderText = "Автопродление";
            if (dgvTariffs.Columns.Contains("is_active")) dgvTariffs.Columns["is_active"].HeaderText = "Активен";
        }

        private void ApplyRussianHeadersUserSubs()
        {
            if (dgvUserSubscriptions.Columns.Contains("id")) dgvUserSubscriptions.Columns["id"].HeaderText = "ID подписки";
            if (dgvUserSubscriptions.Columns.Contains("name")) dgvUserSubscriptions.Columns["name"].HeaderText = "Название подписки";
            if (dgvUserSubscriptions.Columns.Contains("start_date")) dgvUserSubscriptions.Columns["start_date"].HeaderText = "Дата начала";
            if (dgvUserSubscriptions.Columns.Contains("end_date")) dgvUserSubscriptions.Columns["end_date"].HeaderText = "Дата окончания";
            if (dgvUserSubscriptions.Columns.Contains("status")) dgvUserSubscriptions.Columns["status"].HeaderText = "Статус";
            if (dgvUserSubscriptions.Columns.Contains("price_paid")) dgvUserSubscriptions.Columns["price_paid"].HeaderText = "Оплачено (₽)";
            if (dgvUserSubscriptions.Columns.Contains("auto_renewal")) dgvUserSubscriptions.Columns["auto_renewal"].HeaderText = "Автопродление";
        }

        private void DrawShadow(Panel panel, Graphics g, int offset = 3)
        {
            using (var path = GetRoundedRectanglePath(panel.ClientRectangle, 15))
            using (var brush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                g.TranslateTransform(offset, offset);
                g.FillPath(brush, path);
                g.TranslateTransform(-offset, -offset);
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius - 1, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius - 1, rect.Bottom - radius - 1, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static class Prompt
        {
            public static string ShowDialog(string text, string caption, string defaultValue = "")
            {
                Form prompt = new Form()
                {
                    Width = 420,
                    Height = 180,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterParent,
                    Font = new Font("Segoe UI", 9.75f)
                };

                Label textLabel = new Label()
                {
                    Left = 15,
                    Top = 15,
                    Text = text,
                    Width = 380,
                    Font = new Font("Segoe UI", 9.75f)
                };

                TextBox textBox = new TextBox()
                {
                    Left = 15,
                    Top = 60,
                    Width = 380,
                    Text = defaultValue,
                    Font = new Font("Segoe UI", 9.75f)
                };

                Button confirmation = new Button()
                {
                    Text = "OK",
                    Left = 290,
                    Width = 100,
                    Top = 100,
                    DialogResult = DialogResult.OK,
                    Font = new Font("Segoe UI", 9.75f)
                };

                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }
    }

    public class GradientButton : Button
    {
        public Color StartColor { get; set; } = Color.FromArgb(52, 152, 219);
        public Color EndColor { get; set; } = Color.FromArgb(41, 128, 185);
        public int CornerRadius { get; set; } = 8;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
            path.AddArc(Width - CornerRadius - 1, 0, CornerRadius, CornerRadius, 270, 90);
            path.AddArc(Width - CornerRadius - 1, Height - CornerRadius - 1, CornerRadius, CornerRadius, 0, 90);
            path.AddArc(0, Height - CornerRadius - 1, CornerRadius, CornerRadius, 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);

            using (LinearGradientBrush brush = new LinearGradientBrush(
                ClientRectangle, StartColor, EndColor, 90f))
            {
                e.Graphics.FillPath(brush, path);
            }

            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle,
                ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    public class RoundedPanel : Panel
    {
        public int CornerRadius { get; set; } = 15;
        public int BorderWidth { get; set; } = 0;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicsPath path = new GraphicsPath();
            int radius = CornerRadius;

            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(Width - radius - 1, 0, radius, radius, 270, 90);
            path.AddArc(Width - radius - 1, Height - radius - 1, radius, radius, 0, 90);
            path.AddArc(0, Height - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);

            if (BorderWidth > 0)
            {
                using (Pen pen = new Pen(Color.FromArgb(220, 220, 220), BorderWidth))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            base.OnPaint(e);
        }
    }

    public class SubscriptionCard : Panel
    {
        public int SubscriptionId { get; set; }
        public event Action<int> OnBuy;

        public SubscriptionCard(string name, string description, decimal price, int durationDays, int id)
        {
            SubscriptionId = id;
            this.Size = new Size(250, 200);
            this.BackColor = Color.White;
            this.Padding = new Padding(15);
            this.Margin = new Padding(10);
            this.Cursor = Cursors.Hand;

            this.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRect(this.ClientRectangle, 12))
                {
                    this.Region = new Region(path);
                    using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                        e.Graphics.DrawPath(pen, path);
                }
            };

            Label lblName = new Label
            {
                Text = name,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(0, 5, 0, 0)
            };

            Label lblDesc = new Label
            {
                Text = string.IsNullOrEmpty(description) ? "Описание отсутствует" : description,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Dock = DockStyle.Top,
                Height = 45
            };

            Label lblPrice = new Label
            {
                Text = $"{price:N2} ₽ / {durationDays} дн.",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Dock = DockStyle.Top,
                Height = 25
            };

            GradientButton btnBuy = new GradientButton
            {
                Text = "Приобрести",
                Dock = DockStyle.Bottom,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                StartColor = Color.FromArgb(52, 152, 219),
                EndColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                CornerRadius = 8,
                Cursor = Cursors.Hand
            };
            btnBuy.Click += (s, e) => OnBuy?.Invoke(SubscriptionId);

            this.Controls.Add(btnBuy);
            this.Controls.Add(lblPrice);
            this.Controls.Add(lblDesc);
            this.Controls.Add(lblName);
        }

        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius - 1, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius - 1, rect.Bottom - radius - 1, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class SubscriptionStatusCard : Panel
    {
        public SubscriptionStatusCard(string name, DateTime start, DateTime end, decimal price, bool isActive)
        {
            this.Size = new Size(250, 180);
            this.BackColor = Color.White;
            this.Padding = new Padding(15);
            this.Margin = new Padding(10);

            this.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRect(this.ClientRectangle, 12))
                {
                    this.Region = new Region(path);
                    using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                        e.Graphics.DrawPath(pen, path);
                }
            };

            Label lblName = new Label
            {
                Text = name,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(0, 5, 0, 0)
            };

            Label lblPeriod = new Label
            {
                Text = $"{start:dd.MM.yyyy} – {end:dd.MM.yyyy}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Dock = DockStyle.Top,
                Height = 25
            };

            Label lblPrice = new Label
            {
                Text = $"{price:N2} ₽",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(108, 117, 125),
                Dock = DockStyle.Top,
                Height = 25
            };

            Panel statusPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(0, 5, 0, 0)
            };

            string statusText = isActive ? "● Активна" : "● Неактивна";
            Color statusColor = isActive ? Color.FromArgb(46, 204, 113) : Color.FromArgb(231, 76, 60);

            Label lblStatus = new Label
            {
                Text = statusText,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = statusColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };

            statusPanel.Controls.Add(lblStatus);

            this.Controls.Add(statusPanel);
            this.Controls.Add(lblPrice);
            this.Controls.Add(lblPeriod);
            this.Controls.Add(lblName);
        }

        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius - 1, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius - 1, rect.Bottom - radius - 1, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}