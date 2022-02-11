using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatZD
{
    public class CalStrategy
    {
        private string Match;
        private string Race;
        private string Qtype;
        DbHelperSQL SqlHelper = new DbHelperSQL();
        public CalStrategy()
        {

        }
        public CalStrategy(string match, string race, string qtype)
        {
            Match = match;
            Race = race;
            Qtype = qtype;
        }

        public  DataTable GetHistoryMatch()
        {
            string sql = $"select distinct(match) from [qpOdds]";
            DataSet ds= SqlHelper.Query(sql);
            return ds.Tables[0];
        }

        public DataTable GetHistoryMinute()
        {
            string sql = $"select distinct(minute) from[qpOdds] where match = '{Match}' and race = {Race} order by minute";
            DataSet ds = SqlHelper.Query(sql);
            return ds.Tables[0];
        }
        public DataTable GetCompareDetail(List<int> lstHead, List<int> lstFoot, double ypc, int dt, int dt1=-1)
        {
            DataTable dtRet = CreateDetailTable();
            DataTable dtDt = GetOdds(dt);
            if(dt1==-1)
            {
                dt1 = GetLatestTime();
            }
            DataTable dtDt1 = GetOdds(dt1);
            List<Tuple<int, int>> lstHorse = GetHorseList(lstHead, lstFoot);
            foreach (var item in lstHorse)
            {
                double odds = GetOdds(item.Item1, item.Item2, dtDt);
                double odds1 = GetOdds(item.Item1, item.Item2, dtDt1);
                if (odds * odds1 != 0)
                {
                    int piao = (int)(ypc / odds);
                    int piao1 = (int)(ypc / odds1);
                    int gap = (int)((1-piao1*1.0/piao) *100);
                    DataRow dr = dtRet.NewRow();
                    dr["组合"] = $"{item.Item1}-{item.Item2}";
                    dr["DT"] = odds;
                    dr["DT1"] = odds1;
                    dr["派彩"] = ypc;
                    dr["票"] = piao;
                    dr["票1"] = piao1;
                    dr["相差"] = gap;
                    dtRet.Rows.Add(dr);
                }
            }
            dtRet.DefaultView.Sort = "相差 desc";
            return dtRet.DefaultView.ToTable();
        }

        public string[,] GetOddsArray()
        {
            string[,] arr = new string[21, 21];
            int dt1 = GetLatestTime();
            DataTable dtDt1 = GetOdds(dt1);
            for (int i = 1; i <= 20; i++)
            {
                for (int j = 1; j <= 20; j++)
                {
                    arr[i, j] = "0";
                    if (i != j)
                    {
                        double odds1 = GetOdds(i, j, dtDt1);
                        arr[i, j] = odds1.ToString();
                    }
                }
            }
            return arr;
        }

        private double ToTopPei(double pei)
        {
            double p = pei;
            if (Qtype == "Q")
            {
                if (pei > 70)
                {
                    p = 70;
                }
            }
            if (Qtype == "QP")
            {
                if (pei > 40)
                {
                    p = 40;
                }
            }
            return p;
        }
        private int GetLatestTime()
        {
            int dt1 = 0;
            string sql = $"select min(minute) from [qpOdds] where match = '{Match}' and race = {Race}";

            object obj = SqlHelper.GetSingle(sql);
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out dt1);
            }
            return dt1;
        }
        private DataTable CreateDetailTable()
        {
            DataTable dtRet = new DataTable();
            dtRet.Columns.Add("组合");
            dtRet.Columns.Add("DT");
            dtRet.Columns.Add("DT1");
            dtRet.Columns.Add("派彩");
            dtRet.Columns.Add("票");
            dtRet.Columns.Add("票1");
            dtRet.Columns.Add("相差",typeof(double));
            return dtRet;
        }

        /// <summary>
        /// 获取dt时间的qp赔率
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable GetOdds(int minute)
        {
            DataTable dtRet = new DataTable();
            string sql = $"Select * From qpOdds where match='{Match}' and race ={Race} and minute ={minute} and qtype='{Qtype}'";
            DataSet ds = SqlHelper.Query(sql);
            dtRet = ds.Tables[0];
            return dtRet;
        }

        /// <summary>
        /// 得到用户已经选择的马号所有组合
        /// </summary>
        /// <param name="lstHead"></param>
        /// <param name="lstFoot"></param>
        /// <returns></returns>
        private List<Tuple<int,int>> GetHorseList(List<int> lstHead, List<int> lstFoot)
        {
            List<Tuple<int, int>> lstRet = new List<Tuple<int, int>>();
            //头拖脚
            foreach(var head in lstHead)
            {
                foreach(var foot in lstFoot)
                {
                    int h1 = head;
                    int h2 = foot;
                    Swap(ref h1,ref h2);
                    lstRet.Add(new Tuple<int, int>(h1,h2));
                }
            }
            //关互拖
            for(int i=0;i<lstHead.Count;i++)
            {
                for(int j=i+1;j<lstHead.Count;j++)
                {
                    int h1 = lstHead[i];
                    int h2 = lstHead[j];
                    Swap(ref h1, ref h2);
                    lstRet.Add(new Tuple<int, int>(h1, h2));
                }
            }
            return lstRet;
        }

        private double GetOdds(int h1,int h2,DataTable dt)
        {
            double ret = 0;
            DataRow []drs =  dt.Select($"horse1 ={h1} and horse2 ={h2}");
            if(drs.Length>0)
            {
                double.TryParse(drs[0]["odds"].ToString(), out ret);
                //ret = ToTopPei(ret);
            }
            return ret;
        }

        private void Swap(ref int h1 ,ref int h2)
        {
            if(h1>h2)
            {
                int tmp = h1;
                h1 = h2;
                h2 = tmp;
            }
        }
    }
}
