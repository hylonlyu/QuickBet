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
   
    public partial class BetStrategyCtrl : UserControl
    {

        private SelectionExpress _se;
        public SelectionExpress Se
        {
            get
            {
                return _se;
            }
            set
            {
                _se = value;
                lblExpress.Text = _se.GetExpresstion();
            }
        }

        private int _money;
        public int Money
        {
            get
            {
                return _money;
            }
            set
            {
                _money = value;
                lblMoney.Text = _money.ToString();
            }
        }
        private int _yjpc;
        public int Yjpc
        {
            get
            {
                return _yjpc;
            }
            set
            {
                _yjpc = value;
                lblYjpc.Text = _yjpc.ToString();
            }
        }
        public PlayType Qplaytype
        {
            get;
            set;
        }
        public string[,] Odds
        {
            get;
            set;
        }

        public string GUID
        {
            get;
            set;
        }
        public BetComposeCtrl ParentControl;
        public BetStrategyCtrl()
        {
            InitializeComponent();
            GUID = Guid.NewGuid().ToString();
        }


        public DataTable GetDtDetail(out double zbl, out double tzzs, out double yjpc)
        {
            zbl = GetZb();
            tzzs = 0;
            yjpc = GetYjpc(Money, zbl);
            //取整数
            yjpc = (int)yjpc;
            Yjpc = (int)yjpc;
            DataTable dtDetail = CreateDetailTable();
            //循环得到对马号的投注数
            foreach (var item in GetHorses())
            {
                string horses = $"{item.Item1}-{item.Item2}";
                double odds = GetOdds(item.Item1, item.Item2);
                double zs = yjpc / odds;
                zs = (int)zs;
                tzzs += zs;
                DataRow dr = dtDetail.NewRow();
                dr["投组"] = horses;
                dr["赔率"] = odds;
                dr["投总"] = zs;
                dr["预派"] = yjpc;
                dtDetail.Rows.Add(dr);
            }
            zbl = Math.Round(zbl, 5);
            tzzs = (int)tzzs;
 
            return dtDetail;
        }

        private DataTable CreateDetailTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("投组");
            dt.Columns.Add("赔率");
            dt.Columns.Add("投总");
            dt.Columns.Add("预派");
            return dt;
        }
        /// <summary>
        /// 计算值博的总和
        /// </summary>
        /// <returns></returns>
        private double GetZb()
        {
            double zbl = 0;
            foreach (var item in GetHorses())
            {
                //得到赔率
                double pei = GetOdds(item.Item1, item.Item2);
                //取赔率倒数
                if (pei != 0)
                {
                    double tmp = 1.0 / pei;
                    //计算值博
                    zbl += tmp;
                }
                else
                {
                    throw new Exception("赔率不能为 0");
                }
            }
            return zbl;
        }
        /// <summary>
        ///计算预派
        /// </summary>
        /// <returns></returns>
        private double GetYjpc(int money,double zb)
        {
            double yjpc = 0;
            if(zb!=0)
            {
                yjpc = money / zb;
            }
            else
            {
                throw new Exception("值博不能为 0");
            }
            return yjpc;
        }
        private double GetOdds(int h1,int h2)
        {
            double p = 0;
            if(h1>h2)
            {
                int tmp = h1;
                h1 = h2;
                h2 = tmp;
            }
            if(Odds!=null)
            {
                string pei = Odds[h1, h2];
                double.TryParse(pei, out p);
            }
            p = ToTopPei(p);
            return p;
        }
        private List<Tuple<int,int>> GetHorses()
        {
            List<Tuple<int, int>> lstHorses = new List<Tuple<int, int>>();
            if (!string.IsNullOrEmpty(_se.Main))
            {
                int.TryParse(_se.Main,out int h1);
                foreach(var item in _se.LstHorse)
                {
                    int.TryParse(item,out int h2);
                    int tmp1 = h1;
                    int tmp2 = h2;
                    SwapHorse(ref tmp1, ref tmp2);
                    lstHorses.Add(new Tuple<int, int>(tmp1, tmp2));
                }
            }
            else
            {
                for(int i=0;i<_se.LstHorse.Count;i++)
                {
                    int.TryParse(_se.LstHorse[i], out int h1);
                    for(int j=i+1;j<_se.LstHorse.Count; j++)
                    {
                        int.TryParse(_se.LstHorse[j], out int h2);
                        if (h1 != h2)
                        {
                            int tmp1 = h1;
                            int tmp2 = h2;
                            SwapHorse(ref tmp1, ref tmp2);
                            lstHorses.Add(new Tuple<int, int>(tmp1, tmp2));
                        }
                    }
                }
            }
            return lstHorses;
        }

        /// <summary>
        /// 交换马号，保证小号在前
        /// </summary>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        private void SwapHorse(ref int h1,ref int h2)
        {
            if(h1>h2)
            {
                int tmp = h1;
                h1 = h2;
                h2 = tmp;
            }
        }

        private void lblCllose_Click(object sender, EventArgs e)
        {
            ParentControl.RemoveStrategy(GUID);
        }

        private void BetStrategyCtrl_Click(object sender, EventArgs e)
        {
            ParentControl.ShowDetail(GetDtDetail(out _, out _, out _));
        }
        private double ToTopPei(double pei)
        {
            double p = pei;
            if(Qplaytype == PlayType.Q)
            {
                if(pei >70)
                {
                    p = 70;
                }
            }
            if(Qplaytype == PlayType.QP)
            {
                if (pei > 40)
                {
                    p = 40;
                }
            }
            return p;
        }
    }
}
