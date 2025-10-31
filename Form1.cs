using System;
using System.Drawing;
using System.Windows.Forms;

namespace TicTacToeGUI
{
    public class Form1 : Form
    {
        private Button[] buttons = new Button[9];
        private char currentPlayer = 'X';

        public Form1()
        {
            this.Text = "Морски шах";
            this.ClientSize = new Size(300, 320);
            CreateBoard();
        }

        private void CreateBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                buttons[i] = new Button();
                buttons[i].Size = new Size(80, 80);
                buttons[i].Location = new Point((i % 3) * 90 + 10, (i / 3) * 90 + 10);
                buttons[i].Font = new Font("Arial", 24);
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
                btn.Enabled = false;

                if (CheckWin())
                {
                    MessageBox.Show($"Играч {currentPlayer} печели!");
                    ResetBoard();
                }
                else if (IsDraw())
                {
                    MessageBox.Show("Равенство!");
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

        private void ResetBoard()
        {
            foreach (var btn in buttons)
            {
                btn.Text = "";
                btn.Enabled = true;
            }
            currentPlayer = 'X';
        }
    }
}
