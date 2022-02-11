using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace EatZD
{
    public class ZdStrategy:CCMember
    {
        private Thread WorkThread;
        private Dictionary<string, int> dicBetWinPiao = new Dictionary<string, int>();
        private HashSet<string> hsEatWin = new HashSet<string>();
        private HashSet<string> hsBetFail = new HashSet<string>();
        //RaceInfoEnity oldEnity = null;
        Dictionary<string, RaceInfoEnity> dicRaceinfo = new Dictionary<string, RaceInfoEnity>();
        Dictionary<string, WPOdds> dicWPOddsData = new Dictionary<string, WPOdds>();

        public Dictionary<string, Tuple<double, double>> dicQData = new Dictionary<string, Tuple<double, double>>();
        public Dictionary<string, Tuple<double, double>> dicQPData = new Dictionary<string, Tuple<double, double>>();
        public string[,] qpOdds;
        public string[,] qOdds;
        public override void Start()
        {
            if (WorkThread != null && WorkThread.IsAlive == true)
            {
                WorkThread.Abort();
            }
            WorkThread = new Thread(DoWork);
            WorkThread.IsBackground = true;
            WorkThread.Start();
            base.Start();
        }

        public override void Stop()
        {
            if (WorkThread != null && WorkThread.IsAlive == true)
            {
                WorkThread.Abort();
            }
            base.Stop();
        }

        private void DoWork()
        {
            while(true)
            {
                GetQPOdds();
                GetQData();
                GetQPData();
                Thread.Sleep(1000);
            }
        }


        private new void GetQData()
        {
            Dictionary<string, Tuple<double, double>> dicQData2 = GetQData(Config.Race);
            if (dicQData2.Count > 0)
            {
                dicQData = dicQData2;
            }
        }

        private new void GetQPData()
        {
            Dictionary<string, Tuple<double, double>> dicQPData2 = GetQPData(Config.Race);
            if (dicQPData2.Count > 0)
            {
                dicQPData = dicQPData2;
            }
        }

        private new void  GetQPOdds()
        {
            Dictionary<string, string[,]> dicData = GetQPOddsByRace(Config.Race);
            if (dicData != null)
            {
                qpOdds = dicData["QP"];
                qOdds = dicData["Q"];
            }
        }

        private void GetHorses(string hss, out int h1, out int h2)
        {
            string tmp = hss.Replace("(", "").Replace(")", "");
            string[] tmps = tmp.Split("-".ToCharArray());
            if (tmps.Length > 1)
            {
                int.TryParse(tmps[0], out int horse1);
                int.TryParse(tmps[1], out int horse2);
                h1 = horse1;
                h2 = horse2;
            }
            else
            {
                h1 = 0;
                h2 = 0;
            }

        }

        private double GetQOdds(string hss)
        {
            double pl = 0;
            try
            {
                GetHorses(hss, out int Horse1, out int Horse2);
                string qpei = "";
                if (qOdds != null)
                {
                    qpei = qOdds[Horse1, Horse2];
                }
                double.TryParse(qpei, out double pei);
                pl = pei >70?70:pei;
            }
            catch (Exception ex)
            {

            }
            return pl;
        }

        private double GetQpOdds(string hss)
        {
            double pl = 0;
            try
            {
                GetHorses(hss, out int Horse1, out int Horse2);
                string qpei = "";
                if (qpOdds != null)
                {
                    qpei = qpOdds[Horse1, Horse2];
                }
                double.TryParse(qpei, out double pei);
                pl = pei>40?40:pei;
            }
            catch (Exception ex)
            {

            }
            return pl;
        }

        private double GetBetQpPiao(double odds, double yjpc)
        {
            double piao = yjpc / odds;
            if (Config.BGDPS2)
            {
                piao = Config.GDPS2;
            }
            else
            {
                //判断是符合哪一档比例
                if (odds >= Config.PL112 && odds <= Config.PL122)
                {
                    piao = piao * Config.BU12 * 0.01;
                }
                else if (odds >= Config.PL212 && odds <= Config.PL222)
                {
                    piao = piao * Config.BU22 * 0.01;
                }
                else if (odds >= Config.PL312 && odds <= Config.PL322)
                {
                    piao = piao * Config.BU32 * 0.01;
                }
                else if (odds >= Config.PL412 && odds <= Config.PL422)
                {
                    piao = piao * Config.BU42 * 0.01;
                }
                else if (odds >= Config.PL512 && odds <= Config.PL522)
                {
                    piao = piao * Config.BU52 * 0.01;
                }
                else
                {
                    piao = -1;
                }
            }
            return piao;
        }
        private void DoBetQp(string horse, double odds, double yjpc)
        {
            double piao = GetBetQpPiao(odds, yjpc);
            if (piao > 0)
            {
                RaceInfoItem raceitem = new RaceInfoItem();
                raceitem.Url = Config.MatchUrl;
                raceitem.Race = Config.Race;
                raceitem.Horse = horse;

                raceitem.Zhe = Config.XDZK2;
                raceitem.Win = Util.Morethan10(piao);
                raceitem.Place = raceitem.Win;
                raceitem.LWin = (int)Config.XDJX2;
                raceitem.LPlace = (int)Config.XDJX2;
                raceitem.Date = GetNow();
                raceitem.Playtype = PlayType.QP;
                raceitem.Bettype = BetType.BET;
                bool bRet = QiPiaoGuaQ(raceitem, out BetResultInfo info);


                BettedItem bitem = new BettedItem();
                bitem.BetTime = DateTime.Now;
                bitem.Race = raceitem.Race;
                GetHorses(raceitem.Horse, out int h1, out int h2);
                bitem.Horse1 = h1.ToString();
                bitem.Horse2 = h2.ToString();
                //bitem.Horse = raceitem.Horse;
                bitem.DBetCount = (int)raceitem.Win;
                bitem.Zhe = raceitem.Zhe;
                bitem.Lim = raceitem.LWin;
                bitem.PlayType = raceitem.Playtype;
                bitem.BetType = raceitem.Bettype;
                bitem.Odds = odds;
                bitem.TotalCount = (int)raceitem.Win;
                bitem.Result = bRet;
                bitem.Reason = info.StrAnswer;
                SendBetOkEvent(bitem);
            }
        }
        private double GetBetQPiao(double odds, double yjpc)
        {
            double piao = yjpc / odds;
            if(Config.BGDPS)
            {
                piao = Config.GDPS;
            }
            else
            {
                //判断是符合哪一档比例
                if(odds >= Config.PL11 && odds <= Config.PL12)
                {
                    piao = piao * Config.BU1*0.01;
                }
                else if (odds >= Config.PL21 && odds <= Config.PL22)
                {
                    piao = piao * Config.BU2 * 0.01;
                }
                else if (odds >= Config.PL31 && odds <= Config.PL32)
                {
                    piao = piao * Config.BU3 * 0.01;
                }
                else if (odds >= Config.PL41 && odds <= Config.PL42)
                {
                    piao = piao * Config.BU4 * 0.01;
                }
                else if (odds >= Config.PL51 && odds <= Config.PL52)
                {
                    piao = piao * Config.BU5 * 0.01;
                }
                else
                {
                    piao = -1;
                }
            }
            return piao;
        }
        private void DoBetQ(string horse,double odds,double yjpc)
        {
            double piao = GetBetQPiao(odds,yjpc);
            if(piao >0)
            {
                RaceInfoItem raceitem = new RaceInfoItem();
                raceitem.Url = Config.MatchUrl;
                raceitem.Race = Config.Race;
                raceitem.Horse = horse;

                raceitem.Zhe = Config.XDZK;
                raceitem.Win = Util.Morethan10(piao);
                raceitem.Place = raceitem.Win;
                raceitem.LWin = (int)Config.XDJX;
                raceitem.LPlace = (int)Config.XDJX;
                raceitem.Date = GetNow();
                raceitem.Playtype = PlayType.Q;
                raceitem.Bettype = BetType.BET;
                bool bRet = QiPiaoGuaQ(raceitem, out BetResultInfo info);

                
                BettedItem bitem = new BettedItem();
                bitem.BetTime = DateTime.Now;
                bitem.Race = raceitem.Race;
                GetHorses(raceitem.Horse, out int h1, out int h2);
                bitem.Horse1 = h1.ToString();
                bitem.Horse2 = h2.ToString();
                //bitem.Horse = raceitem.Horse;
                bitem.DBetCount = (int)raceitem.Win;
                bitem.Zhe = raceitem.Zhe;
                bitem.Lim = raceitem.LWin;
                bitem.PlayType = raceitem.Playtype;
                bitem.BetType = raceitem.Bettype;
                bitem.Odds = odds;
                bitem.TotalCount = (int)raceitem.Win;
                bitem.Result = bRet;
                bitem.Reason = info.StrAnswer;
                SendBetOkEvent(bitem);
            }
        }
        public void DoBetQ(List<BetStrategyCtrl> LstStrategy)
        {
            foreach(var item in LstStrategy)
            {
              DataTable dtDetail =  item.GetDtDetail(out _,out _,out double yjpc);
                //过滤
                foreach(DataRow item2 in dtDetail.Rows)
                {
                    string horse = item2["投注组合"].ToString();
                    double odds = GetQOdds(horse);
                    if(odds !=0 && odds >= Config.PLMIN && odds <= Config.PLMAX)
                    {
                        if (dicQData.ContainsKey(horse))
                        {
                            Tuple<double, double> data = dicQData[horse];
                            double zhe = data.Item1;
                            double piao = data.Item2;
                            if(zhe >= Config.XMZK)
                            {
                                if(Config.BXMPS)
                                {
                                    if(piao >=Config.XMPSMIN && piao<= Config.XMPSMAX)
                                    {
                                        //符合过滤条件再打单
                                        DoBetQ(horse, odds, yjpc);
                                    }
                                }else
                                {
                                    DoBetQ(horse, odds, yjpc);
                                }
                            }
                        }
                    }
                   
                }
                //修正单数


            }
        }

        public void DoBetQP(List<BetStrategyCtrl> LstStrategy)
        {
            foreach (var item in LstStrategy)
            {
                DataTable dtDetail = item.GetDtDetail(out _, out _, out double yjpc);
                //过滤
                foreach (DataRow item2 in dtDetail.Rows)
                {
                    string horse = item2["投注组合"].ToString();
                    double odds = GetQpOdds(horse);
                    if (odds != 0 && odds >= Config.PLMIN2 && odds <= Config.PLMAX2)
                    {
                        if (dicQPData.ContainsKey(horse))
                        {
                            Tuple<double, double> data = dicQPData[horse];
                            double zhe = data.Item1;
                            double piao = data.Item2;
                            if (zhe >= Config.XMZK2)
                            {
                                if (Config.BXMPS2)
                                {
                                    if (piao >= Config.XMPSMIN2 && piao <= Config.XMPSMAX2)
                                    {
                                        //符合过滤条件再打单
                                        DoBetQp(horse, odds, yjpc);
                                    }
                                }
                                else
                                {
                                    DoBetQp(horse, odds, yjpc);
                                }
                            }
                        }
                    }

                }
                //修正单数


            }
        }
    }
}
