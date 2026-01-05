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
    public delegate void ShowGridHandler(List<string> horses,string qtype);
    public partial class ViewHistory : UserControl
    {
        public string Match;
        public string Race;
        public string Qtype;

        public event ViewHandler ViewEventHandler;
        public event ShowGridHandler OnShowGrid;
        public ViewHistory()
        {
            InitializeComponent();
        }

        public void ShowNow()
        {
            if(!chkQt1.Checked)
            {
                ShowGrid();
            }
            else
            {
                int.TryParse(cobQt1.Text.Trim(), out int dt1);
                ShowGrid(dt1);
            }
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
            cobQt.Text = string.IsNullOrEmpty(dt)?"":dt;

            cobQt1.DataSource = dtSource.Copy();
            cobQt1.ValueMember = "minute";
            cobQt1.DisplayMember = "minute";
            cobQt1.Text = string.IsNullOrEmpty(dt1) ? "" : dt1;
        }
        private void ShowGrid(int dt1 =-1)
        {
            double.TryParse(lblYjpc.Text.Trim(), out double ypc);
            int.TryParse(cobQt.Text.Trim(), out int dt);
            CalStrategy cs = new CalStrategy(Match, Race, Qtype);
            DataTable dtDetail = cs.GetCompareDetail(GetHeads(), GetFoots(), ypc, dt, dt1);
            DataTable dtSource = AddFilter(dtDetail);
            dgvGrid.DataSource = dtSource;
            List<string> lstHorse = GetSendHorse(dtSource);
            OnShowGrid?.Invoke(lstHorse, Qtype);
            SetZhe1Color();
        }

        private List<string> GetSendHorse(DataTable dtSource)
        {
            List<string> lstRet = new List<string>();
            foreach(DataRow dr in dtSource.Rows)
            {
                lstRet.Add(dr["组合"].ToString());
            }
            return lstRet;
        }
        private DataTable AddFilter(DataTable dtDetail)
        {
            DataTable dtRet = new DataTable();
            dtRet = dtDetail.Clone();
            DataRow[] drs = dtDetail.Select(GetDt1Range(),GetSort());
            foreach(DataRow dr in drs)
            {
                dtRet.Rows.Add(dr.ItemArray);
            }
            return dtRet;
        }

        private string GetDt1Range()
        {
            //DT1
            double.TryParse(txtMin.Text.Trim(), out double min);
            double.TryParse(txtMax.Text.Trim(),out double max);
            string strRet = $"DT1 >={min} and DT1<={max}";
            //折1
            double.TryParse(txtZhe.Text.Trim(),out double zhe1);
            strRet += $" and 折1>={zhe1}";
            return strRet;
        }
        private string GetSort()
        {
            string strRet="";
            if(radCha.Checked)
            {
                if (radAsc.Checked)
                {
                    strRet = "相差 asc";
                }
                else
                {
                    strRet = "相差 desc";
                }
            }
            else if (radZhe.Checked)
            {
                if (radAsc.Checked)
                {
                    strRet = "折1 asc";
                }
                else
                {
                    strRet = "折1 desc";
                }
            }

            return strRet;
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
            if(e.ColumnIndex==7)
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

        private void btnAll_Click(object sender, EventArgs e)
        {
            foreach (Control c in tblHorse.Controls)
            {
                if (c is HeadfoodCtrl)
                {
                    HeadfoodCtrl hf = c as HeadfoodCtrl;
                    hf.SelectHead();
                }
            }
        }

        private void SetZhe1Color()
        {
            DataTable dt = dgvGrid.DataSource as DataTable;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    double.TryParse(dr["折"].ToString(), out double zhe);
                    double.TryParse(dr["折1"].ToString(), out double zhe1);
                    if (zhe1 >zhe)
                    {
                        SetCellColor(i, 4, GetZhe1Color(zhe1));
                    }
                    else
                    {
                        SetCellColor(i, 4, dgvGrid.DefaultCellStyle.BackColor);
                    }
                }
            }

        }
        private Color GetZhe1Color(double zhe)
        {
            Color Ret = dgvGrid.DefaultCellStyle.BackColor;
            if(zhe>=82 && zhe<=84)
            {
                Ret = Color.FromArgb(0,178,247);
            }
            else if(zhe>=85 && zhe<=89)
            {
                Ret = Color.FromArgb(249, 165, 160);
            }
            else if(zhe>=90 && zhe<=93)
            {
                Ret = Color.FromArgb(253, 154, 3);
            }
            else if (zhe >= 94 && zhe <= 100)
            {
                Ret = Color.FromArgb(255, 0, 255);
            }

            return Ret;
        }
        private void SetCellColor(int row, int col, Color c)
        {
            try
            {
                dgvGrid.Rows[row].Cells[col].Style.BackColor = c;
            }
            catch (Exception ex)
            {

            }

        }

        private void btnFoot_Click(object sender, EventArgs e)
        {
            foreach (Control c in tblHorse.Controls)
            {
                if (c is HeadfoodCtrl)
                {
                    HeadfoodCtrl hf = c as HeadfoodCtrl;
                    hf.SelectFoot();
                }
            }
        }
    }
}
