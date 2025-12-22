using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.IO;

namespace TicTacToeGUI
{

public class Form1 : Form
{
    private Button[] buttons = new Button[9];
    private char currentPlayer = 'X';
    private Panel? boardPanel;
    private Image? boardBackground;
    private Button? btnChangeBackground;

    public Form1()
    {
        // small debug marker: write that constructor started
        try { System.IO.File.AppendAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "run.log"), "Form1 constructor start\n"); } catch { }
        this.Text = "🎮 Морски шах 🎮";
        this.ClientSize = new Size(640, 560);
        this.BackColor = Color.FromArgb(64, 64, 64); // Средно тъмно сив фон
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.MaximizeBox = true;
    CreateBoard();
    CreateChangeBgButton();
    // Ensure form is centered and comes to front when shown
    this.StartPosition = FormStartPosition.CenterScreen;
    this.Shown += Form1_Shown;
    this.Load += Form1_Load;
    this.Activated += Form1_Activated;
    this.FormClosing += Form1_FormClosing;
    this.FormClosed += Form1_FormClosed;
    }

    private void Form1_Load(object? sender, EventArgs e)
    {
        try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "run.log"), $"Form1 Load at {DateTime.Now}\n"); } catch { }
    }

    private void Form1_Activated(object? sender, EventArgs e)
    {
        try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "run.log"), $"Form1 Activated at {DateTime.Now}\n"); } catch { }
    }

    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "run.log"), $"Form1 FormClosing at {DateTime.Now} Reason={e.CloseReason}\n"); } catch { }
    }

    private void Form1_FormClosed(object? sender, FormClosedEventArgs e)
    {
        try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "run.log"), $"Form1 FormClosed at {DateTime.Now}\n"); } catch { }
    }

    private async void Form1_Shown(object? sender, EventArgs e)
    {
        try { System.IO.File.AppendAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "run.log"), "Form1 shown\n"); } catch { }
        // Force the window to the front briefly (TopMost) then return to normal
        try
        {
            this.TopMost = true;
            this.BringToFront();
            this.Activate();
            await Task.Delay(250);
            this.TopMost = false;
            this.BringToFront();
            this.Activate();
        }
        catch { }
        // If developer sets this env var, show a blocking dialog so users can confirm the window is visible
        try
        {
            if (Environment.GetEnvironmentVariable("TTT_SHOW_STARTUP_DIALOG") == "1")
            {
                MessageBox.Show("TicTacToe started (diagnostic): the window should be visible. Click OK to continue.", "Startup Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "run.log"), $"Startup dialog shown at {DateTime.Now}\n"); } catch { }
            }
        }
        catch { }
    }

    private void CreateBoard()
    {
        // Create a panel to hold the board so we can set a background image behind the buttons
        boardPanel = new Panel();
        boardPanel.Location = new Point(25, 60);
        // Slightly taller panel and more horizontal margin for nicer framing
        boardPanel.Size = new Size(600, 500);
        boardPanel.BackColor = Color.Transparent;
        boardPanel.BackgroundImageLayout = ImageLayout.Stretch;
        // Background will be set only when user chooses an image
        this.Controls.Add(boardPanel);

        for (int i = 0; i < 9; i++)
        {
            buttons[i] = new Button();
            // Reduce button size so background is more visible and all buttons fit comfortably
            buttons[i].Size = new Size(140, 140);
            // Position relative to panel so the panel background is visible around/behind buttons
            buttons[i].Location = new Point((i % 3) * 180 + 30, (i / 3) * 160 + 30);
            buttons[i].Font = new Font("Arial Black", 48, FontStyle.Bold);
            buttons[i].FlatStyle = FlatStyle.Flat;
            buttons[i].BackColor = Color.FromArgb(45, 45, 45); // По-светъл от фона
            buttons[i].ForeColor = Color.White; // Започваме с бял текст
            buttons[i].FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 75);
            buttons[i].FlatAppearance.BorderColor = Color.White;
            buttons[i].FlatAppearance.BorderSize = 1;
            buttons[i].Click += ButtonClick;
            boardPanel.Controls.Add(buttons[i]);
        }
    }

    private void CreateChangeBgButton()
    {
        btnChangeBackground = new Button();
        btnChangeBackground.Text = "Промени фон";
        btnChangeBackground.Size = new Size(120, 32);
        btnChangeBackground.Location = new Point(this.ClientSize.Width - btnChangeBackground.Width - 20, 18);
        btnChangeBackground.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnChangeBackground.Click += BtnChangeBackground_Click;
        this.Controls.Add(btnChangeBackground);
    }

    private void BtnChangeBackground_Click(object? sender, EventArgs e)
    {
        using (OpenFileDialog ofd = new OpenFileDialog())
        {
            ofd.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            ofd.Title = "Изберете снимка за фон (висока резолюция)";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Load image via stream to avoid file locks (Image.FromFile can lock)
                    using (var fs = File.OpenRead(ofd.FileName))
                    using (var img = Image.FromStream(fs))
                    {
                        // Create a copy we own
                        Bitmap newImage = new Bitmap(img);

                        // Dispose old image if exists
                        if (boardBackground != null)
                        {
                            try { boardBackground.Dispose(); } catch { }
                        }

                        // Apply new image to our board background and panel only
                        boardBackground = newImage;
                        if (boardPanel != null)
                        {
                            boardPanel.BackgroundImage = boardBackground;
                            boardPanel.BackgroundImageLayout = ImageLayout.Stretch;
                        }

                        // Resize form to match image dimensions (with padding for the Change BG button and margins)
                        if (newImage != null && newImage.Width > 0 && newImage.Height > 0)
                        {
                            int newWidth = newImage.Width + 20;  // left/right padding
                            int newHeight = newImage.Height + 70;  // top padding (button) + bottom
                            this.ClientSize = new Size(newWidth, newHeight);

                            // Also resize the panel to fill the new form (minus button area)
                            if (boardPanel != null)
                            {
                                boardPanel.Location = new Point(10, 50);
                                boardPanel.Size = new Size(newWidth - 20, newHeight - 60);
                            }
                        }

                        // Try to save to assets folder for persistence (optional, don't fail if it doesn't work)
                        try
                        {
                            string assetsDir = Path.Combine(AppContext.BaseDirectory, "Assets");
                            if (!Directory.Exists(assetsDir)) Directory.CreateDirectory(assetsDir);
                            string dest = Path.Combine(assetsDir, "beach.jpg");
                            // Save a copy as JPEG; wrap in try/catch as it may fail on some formats
                            boardBackground.Save(dest, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        catch (Exception exSave)
                        {
                            try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "error.log"), "Failed saving beach.jpg: " + exSave.ToString() + "\n"); } catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    try { File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "error.log"), "BtnChangeBackground error: " + ex.ToString() + "\n"); } catch { }
                    MessageBox.Show("Грешка при зареждане: " + ex.GetType().Name + " - " + ex.Message, "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void SetBoardBackground()
    {
        // If the user placed an image at Assets\beach.jpg (copied to output), prefer it.
        try
        {
            string external = Path.Combine(Application.StartupPath, "Assets", "beach.jpg");
            if (File.Exists(external))
            {
                try
                {
                    // Load into memory via stream then copy to a new Bitmap to avoid file locks and GDI+ issues
                    using (var fs = File.OpenRead(external))
                    using (var img = Image.FromStream(fs))
                    {
                        if (boardBackground != null) { try { boardBackground.Dispose(); } catch { } }
                        boardBackground = new Bitmap(img);
                        // assign to panel when it exists; avoid assigning same instance to both Form and Panel
                        // panel assignment in CreateBoard will pick this up if panel isn't created yet
                    }
                    return;
                }
                catch (Exception exLoad)
                {
                    try { File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "error.log"), "Error loading Assets\\beach.jpg: " + exLoad.ToString()); } catch { }
                    // fall through to generated background
                }
            }
        }
        catch { /* ignore and fall back to generated background */ }

        // Generate a simple beach background bitmap programmatically (sea + sand + sky)
    Bitmap bmp = CreateBeachBitmap(600, 500);
        // store in field; the panel's BackgroundImage will be set when the panel is created
        boardBackground = bmp;
    }

    private Bitmap CreateBeachBitmap(int width, int height)
    {
        Bitmap bmp = new Bitmap(width, height);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // Sky gradient
            using (var skyBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, width, height/2), Color.FromArgb(135, 206, 235), Color.FromArgb(135, 206, 250), 90f))
            {
                g.FillRectangle(skyBrush, 0, 0, width, height/2);
            }

            // Sea gradient
            using (var seaBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, height/4, width, height/2), Color.FromArgb(25, 147, 206), Color.FromArgb(0, 105, 148), 90f))
            {
                g.FillRectangle(seaBrush, 0, height/4, width, height/2);
            }

            // Sand gradient
            using (var sandBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, height*3/4, width, height/4), Color.FromArgb(245, 222, 179), Color.FromArgb(238, 214, 175), 90f))
            {
                g.FillRectangle(sandBrush, 0, height*3/4, width, height/4);
            }

            // Simple sun (warm yellow/orange) with a soft glow
            using (var glowBrush = new SolidBrush(Color.FromArgb(80, 255, 200, 50)))
            {
                g.FillEllipse(glowBrush, width - 150, 10, 140, 140);
            }
            using (var sunBrush = new SolidBrush(Color.FromArgb(255, 210, 60)))
            {
                g.FillEllipse(sunBrush, width - 120, 30, 100, 100);
            }

            // A few subtle waves
            using (var pen = new Pen(Color.FromArgb(120, Color.White), 2))
            {
                for (int i = 0; i < 4; i++)
                {
                    int y = height/3 + i * 18;
                    g.DrawBezier(pen, 0, y, width/4, y - 8, width*3/4, y + 8, width, y);
                }
            }
        }
        return bmp;
    }

    private void ButtonClick(object? sender, EventArgs e)
    {
        Button? btn = sender as Button;
        if (btn != null && btn.Text == "")
        {
            btn.Text = currentPlayer.ToString();
            if (currentPlayer == 'X')
            {
                btn.ForeColor = Color.FromArgb(255, 50, 50); // Ярко червено
                btn.BackColor = Color.FromArgb(60, 30, 30); // Тъмно червен фон
            }
            else
            {
                btn.ForeColor = Color.FromArgb(50, 255, 50); // Ярко зелено
                btn.BackColor = Color.FromArgb(30, 60, 30); // Тъмно зелен фон
            }
            btn.Font = new Font("Arial Black", 52, FontStyle.Bold);
            btn.Enabled = false;

            if (CheckWin())
            {
                MessageBox.Show($"🎉 Браво! Играч {currentPlayer} печели! 🎉", 
                    "Победа!", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
                ResetBoard();
            }
            else if (IsDraw())
            {
                MessageBox.Show("🤝 Равенство! Добра игра! 🤝", 
                    "Край на играта", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
                ResetBoard();
            }
            else
            {
                currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
            }
        }
    }

    private bool CheckWin()
    {
        int[][] winCombos = new int[][]
        {
            new int[] {0,1,2}, new int[] {3,4,5}, new int[] {6,7,8},
            new int[] {0,3,6}, new int[] {1,4,7}, new int[] {2,5,8},
            new int[] {0,4,8}, new int[] {2,4,6}
        };

        foreach (var combo in winCombos)
        {
            if (buttons[combo[0]].Text == currentPlayer.ToString() &&
                buttons[combo[1]].Text == currentPlayer.ToString() &&
                buttons[combo[2]].Text == currentPlayer.ToString())
                return true;
        }
        return false;
    }

    private bool IsDraw()
    {
        foreach (var btn in buttons)
            if (btn.Text == "") return false;
        return true;
    }

    private async void ResetBoard()
    {
        await Task.Delay(500); // Малко забавяне преди рестарта
        foreach (var btn in buttons)
        {
            btn.Text = "";
            btn.Enabled = true;
            btn.BackColor = Color.FromArgb(45, 45, 45);
            btn.ForeColor = Color.White;
            await Task.Delay(50); // Плавна анимация при изчистване
        }
        currentPlayer = 'X';
    }
}
}
