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
    public delegate void ViewHandler();
    public partial class ViewHistory : UserControl
    {
        public string Match;
        public string Race;
        public string Qtype;

        public event ViewHandler ViewEventHandler;
        public ViewHistory()
        {
            InitializeComponent();
        }

        public void ShowNow()
        {
            ShowGrid();

        }
        public void ShowHistory()
        {
            int.TryParse(cobQt1.Text.Trim(), out int dt1);
            ShowGrid(dt1);
        }

        public void SetMinuteDatasource(DataTable dtSource)
        {
            string dt = cobQt.Text.Trim();
            string dt1 = cobQt1.Text.Trim();

            cobQt.DataSource = dtSource.Copy();
            cobQt.ValueMember = "minute";
            cobQt.DisplayMember = "minute";
            cobQt.Text = dt;

            cobQt1.DataSource = dtSource.Copy();
            cobQt1.ValueMember = "minute";
            cobQt1.DisplayMember = "minute";
            cobQt1.Text = dt1;
        }
        private void ShowGrid(int dt1 =-1)
        {
            double.TryParse(lblYjpc.Text.Trim(), out double ypc);
            int.TryParse(cobQt.Text.Trim(), out int dt);
            CalStrategy cs = new CalStrategy(Match, Race, Qtype);
            DataTable dtDetail = cs.GetCompareDetail(GetHeads(), GetFoots(), ypc, dt, dt1);
            dgvGrid.DataSource = dtDetail;
        }
        private List<int> GetHeads()
        {
            List<int> lstHead = new List<int>();
            foreach(Control c in tblHorse.Controls)
            {
                if(c is HeadfoodCtrl)
                {
                    HeadfoodCtrl hf = c as HeadfoodCtrl;
                    if(hf.Head)
                    {
                        int.TryParse(hf.Horse,out int h);
                        lstHead.Add(h);
                    }
                }
            }
            return lstHead;
        }

        private List<int> GetFoots()
        {
            List<int> lstFoot = new List<int>();
            foreach (Control c in tblHorse.Controls)
            {
                if (c is HeadfoodCtrl)
                {
                    HeadfoodCtrl hf = c as HeadfoodCtrl;
                    if (hf.Foot)
                    {
                        int.TryParse(hf.Horse, out int h);
                        lstFoot.Add(h);
                    }
                }
            }
            return lstFoot;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if(ViewEventHandler!=null)
            {
                ViewEventHandler?.Invoke();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            foreach (Control c in tblHorse.Controls)
            {
                if (c is HeadfoodCtrl)
                {
                    HeadfoodCtrl hf = c as HeadfoodCtrl;
                    hf.ClearSelection();
                }
            }
        }

        private void dgvGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //【相关】列
            if(e.ColumnIndex==6)
            {
                if(e.Value!=null)
                {
                    double.TryParse(e.Value.ToString(), out double val);
                    if (val > 0)
                    {
                        dgvGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                    }
                    else 
                    {
                        dgvGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                    }
                }
            }
        }
    }
}
