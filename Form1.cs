using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing.Text;

namespace TicTacToeGUI
{

public class Form1 : Form
{
    private Button[] buttons = new Button[9];
    private char currentPlayer = 'X';

    public Form1()
    {
        this.Text = "🎮 Морски шах 🎮";
        this.ClientSize = new Size(450, 500);
        this.BackColor = Color.FromArgb(64, 64, 64); // Средно тъмно сив фон
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        CreateBoard();
    }

    private void CreateBoard()
    {
        for (int i = 0; i < 9; i++)
        {
            buttons[i] = new Button();
            buttons[i].Size = new Size(120, 120);
            buttons[i].Location = new Point((i % 3) * 140 + 30, (i / 3) * 140 + 30);
            buttons[i].Font = new Font("Arial Black", 52, FontStyle.Bold);
            buttons[i].FlatStyle = FlatStyle.Flat;
            buttons[i].BackColor = Color.FromArgb(45, 45, 45); // По-светъл от фона
            buttons[i].ForeColor = Color.White; // Започваме с бял текст
            buttons[i].FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 75);
            buttons[i].FlatAppearance.BorderColor = Color.White;
            buttons[i].FlatAppearance.BorderSize = 1;
            buttons[i].Click += ButtonClick;
            this.Controls.Add(buttons[i]);
        }
    }

    private void ButtonClick(object sender, EventArgs e)
    {
        Button btn = sender as Button;
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
