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
    public partial class BetMoney : UserControl
    {
        public int Money
        {
            get
            {
                int ret = 0;
                string tmoney = txtMoney.Text.Trim();
                if (string.IsNullOrEmpty(tmoney))
                {
                    foreach(var c in this.Controls)
                    {
                        if(c is RadioButton)
                        {
                            RadioButton rb = c as RadioButton;
                            if(rb.Checked)
                            {
                                tmoney = rb.Text.Trim();
                                break;
                            }
                        }
                    }
                }
                int.TryParse(tmoney, out ret);
                return ret;
            }
        }
        public BetMoney()
        {
            InitializeComponent();
        }
    }
}
