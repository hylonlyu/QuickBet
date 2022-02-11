using System;
using System.Windows.Forms;

namespace EatZD
{
    public partial class HeadfoodCtrl : UserControl
    {
        public string Horse
        {
            get
            {
                return lblHorse.Text;
            }
            set
            {
                lblHorse.Text = value;
            }
        }
        public bool Head
        {
            get
            {
                return chkHead.Checked;
            }
            set
            {
                chkHead.Checked = value;
            }
        }

        public bool Foot
        {
            get
            {
                return chkFoot.Checked;
            }
            set
            {
                chkFoot.Checked = value;
            }
        }
        public HeadfoodCtrl()
        {
            InitializeComponent();
        }

        private void chkHead_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHead.Checked)
            {
                chkFoot.Checked = false;
            }
        }

        private void chkFoot_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFoot.Checked)
            {
                chkHead.Checked = false;
            }
        }
        public void ClearSelection()
        {
            chkHead.Checked = false;
            chkFoot.Checked = false;
        }
    }
}
