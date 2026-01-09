using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EatZD
{
    public partial class QuickSelection : UserControl
    {
        private HashSet<int> heads = new HashSet<int>();
        private HashSet<int> feet = new HashSet<int>();

        public HashSet<int> Heads => heads;
        public HashSet<int> Feet => feet;

        public QuickSelection()
        {
            InitializeComponent();

            // 为所有数字按钮注册鼠标点击事件
            foreach (Control ctrl in tblHorses.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("button") && btn != btnAll && btn != btnDel)
                {
                    btn.MouseDown += NumberButton_MouseDown;
                }
            }

            btnAll.Click += BtnAll_Click;
            btnDel.Click += BtnDel_Click;

            UpdateDisplay();
        }

        private void BtnAll_Click(object sender, EventArgs e)
        {
            feet.Clear();
            heads.Clear();
            for (int i = 1; i <= 20; i++)
            {
                heads.Add(i);
            }

            foreach (Control ctrl in tblHorses.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("button") && btn != btnAll && btn != btnDel)
                {
                    btn.BackColor = Color.Green;
                }
            }

            UpdateDisplay();
        }

        private void BtnDel_Click(object sender, EventArgs e)
        {
            heads.Clear();
            feet.Clear();

            foreach (Control ctrl in tblHorses.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("button") && btn != btnAll && btn != btnDel)
                {
                    btn.BackColor = SystemColors.Control;
                }
            }

            UpdateDisplay();
        }

        private void NumberButton_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            int number = int.Parse(btn.Text);

            if (e.Button == MouseButtons.Left)
            {
                if (heads.Contains(number))
                {
                    heads.Remove(number);
                    btn.BackColor = SystemColors.Control;
                }
                else
                {
                    feet.Remove(number);
                    heads.Add(number);
                    btn.BackColor = Color.Green;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (feet.Contains(number))
                {
                    feet.Remove(number);
                    btn.BackColor = SystemColors.Control;
                }
                else
                {
                    heads.Remove(number);
                    feet.Add(number);
                    btn.BackColor = Color.Red;
                }
            }

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            var headStr = "头: " + string.Join(" ", heads.OrderBy(x => x));
            var footStr = "脚: " + string.Join(" ", feet.OrderBy(x => x));
            lblExpress.Text = headStr + Environment.NewLine + footStr;
        }

        public List<string> GetQExpression()
        {
            if (heads.Count == 0 || feet.Count == 0)
            {
                return new List<string>();
            }

            var result = new List<string>();
            var sortedHeads = heads.OrderBy(x => x);
            var sortedFeet = feet.OrderBy(x => x);
            var feetStr = string.Join("_", sortedFeet);

            foreach (var head in sortedHeads)
            {
                result.Add($"{head}_{feetStr}");
            }

            return result;
        }

        public string GetWPExpression()
        {
            return string.Join(",", heads.OrderBy(x => x));
        }
    }
}
