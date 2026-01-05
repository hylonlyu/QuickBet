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
    public partial class BetComposeCtrl : UserControl
    {
        private BetStrategyCtrl BetStrategy;
        public string Caption
        {
            set
            {
                lblCaption.Text = value;
            }
        }

        public List<BetStrategyCtrl> LstStrategy
        {
            get
            {
                List<BetStrategyCtrl> _lststrategy = new List<BetStrategyCtrl>();
                foreach (var c in flStrategy.Controls)
                {
                    if (c is BetStrategyCtrl)
                    {
                        BetStrategyCtrl bs = c as BetStrategyCtrl;
                        _lststrategy.Add(bs);
                    }
                }
                return _lststrategy;
            }
        }
        public BetComposeCtrl()
        {
            InitializeComponent();
        }

        public void CalcStrategy(SelectionExpress se, PlayType pt, int money,string [,] odds)
        {
            BetStrategy = new BetStrategyCtrl();
            BetStrategy.Se =Util.CloneObject(se) as SelectionExpress;
            BetStrategy.Qplaytype = pt;
            BetStrategy.Money = money;
            BetStrategy.Odds = odds;
            BetStrategy.ParentControl = this;
            //显示在表格

            ShowDetail(BetStrategy.GetDtDetail(out double zbl, out double tzzs, out double yjpc));
           
            lblZb.Text = zbl.ToString();
            lblTz.Text = tzzs.ToString();
            lblPc.Text = yjpc.ToString();
        }

        public void AddStrategy()
        {
            //在界面中显示
            flStrategy.Controls.Add(BetStrategy);
        }

        public void RemoveStrategy(string guid)
        {
            foreach (var c in flStrategy.Controls)
            {
                if (c is BetStrategyCtrl)
                {
                    BetStrategyCtrl bs = c as BetStrategyCtrl;
                    if (bs.GUID.Equals(guid))
                    {
                        flStrategy.Controls.Remove(bs);
                        break;
                    }
                }
            }
        }

        public void ClearStrategy()
        {
            foreach (var c in flStrategy.Controls)
            {
                if (c is BetStrategyCtrl)
                {
                    BetStrategyCtrl bs = c as BetStrategyCtrl;
                    flStrategy.Controls.Remove(bs);
                }
            }
        }

        public void ShowDetail(DataTable dt)
        {
            dgvDetail.DataSource = dt;
        }
    }
}
