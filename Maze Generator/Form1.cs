/// Uploaded to codeproject.com
/// Ibraheem AlKialnny - Egypt
/// Nov 2011

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Maze_Generator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // preventing out of range indexes
            comboBoxGenerate.SelectedIndex = 0;
            comboBoxSolve.SelectedIndex = 0;
        }

        private Maze maze;

        /// <summary>
        /// Gets or sets a value whether the form has a maze
        /// </summary>
        private bool hasMaze;

        /// <summary>
        /// Gets or sets a value whether the current maze has a solution
        /// </summary>
        private bool hasSolution;

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            hasSolution = false;
            labelWorking.Visible = true;
            buttonGenerate.Enabled = false;
            timer1.Enabled = true;
            buttonSolve.Enabled = false;
            // the bigger level, the larger maze. 
            // Since we divide on the level, it should be smaller to get bigger size
            // therefore we evaluate 100 - value
            backgroundWorker1.RunWorkerAsync(
                new object[] {100 - (int) numericUpDownLevel.Value, false, comboBoxGenerate.SelectedIndex});
            hasMaze = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBoxDraw.Invalidate();
        }

        private void PictureBoxDraw_Paint(object sender, PaintEventArgs e)
        {
            if (maze != null)
            {
                maze.Draw(e.Graphics);
                if (hasSolution) maze.DrawPath(e.Graphics);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] args = e.Argument as object[];

            int value = (int) args[0];
            bool solving = (bool) args[1];
            if (!solving)
            {
                maze.Generate(pictureBoxDraw.Width / value, (pictureBoxDraw.Height - value) / value, (int) args[2]);
            }
            else
            {
                maze.Solve();
                hasSolution = true;
            }
            pictureBoxDraw.Invalidate();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            labelWorking.Visible = false;
            timer1.Enabled = false;
            buttonGenerate.Enabled = true;
            buttonSolve.Enabled = true;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            maze = new Maze(pictureBoxDraw.Width, pictureBoxDraw.Height);
            // Causes the Maze.Sleep to update
            numericUpDownSpeed_ValueChanged(sender, e);
            // re-draw the picture
            pictureBoxDraw.Invalidate();
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!hasMaze) return;

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "BMP Image |*.bmp";
                dlg.Title = "Chooce where to save maze";

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    using (Bitmap bitmap = new Bitmap(pictureBoxDraw.Width, pictureBoxDraw.Height))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            maze.Draw(g);
                            if (hasSolution) maze.DrawPath(g);
                            using (Font font = new Font(Font.FontFamily, 14, FontStyle.Bold))
                                g.DrawString(
                                    "Copyright (c) 2011 By Ibraheem AlKilanny",
                                    font,
                                    Brushes.Red,
                                    12,
                                    pictureBoxDraw.Height - 40);
                            bitmap.Save(dlg.FileName);
                        }
                    }
                }
            }
        }

        private void numericUpDownSpeed_ValueChanged(object sender, EventArgs e)
        {
            maze.Sleep = (100 - (int) numericUpDownSpeed.Value) * 100;
        }

        private void buttonSolve_Click(object sender, EventArgs e)
        {
            if (!hasMaze)
            {
                MessageBox.Show(
                    this,
                    "You must generate a maze first!",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }
            labelWorking.Visible = true;
            buttonGenerate.Enabled = false;
            timer1.Enabled = true;
            buttonSolve.Enabled = false;
            backgroundWorker1.RunWorkerAsync(new object[] {0, true, comboBoxSolve.SelectedIndex});
        }
    }
}