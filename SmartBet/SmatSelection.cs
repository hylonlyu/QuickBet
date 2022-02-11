using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EatZD
{
    public partial class SmatSelection : UserControl
    {
       public SelectionExpress se;
        public SmatSelection()
        {
            InitializeComponent();
            Init();
            se = new SelectionExpress();
        }

        private void Init()
        {
            lblExpress.Text = "";
            ResetBtnColor();
            AddBtnEvent();
        }

        private void AddBtnEvent()
        {
            foreach (Control c in tblHorses.Controls)
            {
                if (c is Button)
                {
                    c.MouseDown += button_MouseDown;
                }
            }
        }
        private void ResetBtnColor()
        {
            foreach (Control c in tblHorses.Controls)
            {
                if (c is Button)
                {
                    c.BackColor = Control.DefaultBackColor;
                }
            }
        }

        private void RightClick(Button btn)
        {
            string horse = btn.Text.Trim();
            se.RightAction(horse);
            SetBtnColor();
            lblExpress.Text = se.GetExpresstion();
        }

        private void LeftClick(Button btn)
        {
            string horse = btn.Text.Trim();
            se.LeftAction(horse);
            SetBtnColor();
            lblExpress.Text = se.GetExpresstion();
        }

        private void SetBtnColor()
        {
            ResetBtnColor();
            foreach (Control c in tblHorses.Controls)
            {
                if (c is Button)
                {
                    Button btn = c as Button;
                    string horse = btn.Text.Trim();
                    if (se.Main.Equals(horse))
                    {
                        btn.BackColor = Color.Red;
                    }
                    if (se.LstHorse.Contains(horse))
                    {
                        btn.BackColor = Color.Green;
                    }
                }
            }
        }

        private void button_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            if(e.Button == MouseButtons.Left)
            {
                LeftClick(btn);
            }
            if(e.Button == MouseButtons.Right)
            {
                RightClick(btn);
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            foreach (Control c in tblHorses.Controls)
            {
                if (c is Button)
                {
                    LeftClick(c as Button);
                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            se = new SelectionExpress();
            SetBtnColor();
            lblExpress.Text = se.GetExpresstion();
        }
    }
}
