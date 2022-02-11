using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EatZD
{
    class Spider
    {
        public Dictionary<string, Tuple<double, double>> dicQData = new Dictionary<string, Tuple<double, double>>();
        public Dictionary<string, Tuple<double, double>> dicQPData = new Dictionary<string, Tuple<double, double>>();
        public string[,] qpOdds;
        public string[,] qOdds;
        public Dictionary<string, WPOdds> dicWPOdds = new Dictionary<string, WPOdds>();
        public Dictionary<string, double> dicWPEatData = new Dictionary<string, double>();

        private CCMember cCmemberInstance;
        public CCMember CCmemberInstance
        {
            get
            {
                return cCmemberInstance;
            }
            set
            {
                cCmemberInstance = value;
                //cCmemberInstance.Config = new ZdConfig { MatchUrl = this.MatchUrl, Race = this.Race.ToString() };
            }
        }
        //表明读取数据的线程是否为停止
        private bool bStop = false;

        public string CurrentMatch
        {
            get;
            set;
        }
        public string MatchUrl
        {
            get;
            set;
        }

        public int Race
        {
            get;
            set;
        }


        public int CurrentRace
        {
            get;
            set;
        }

        int zerocounter = 0;
        public int _LastTime;
        public int LastTime
        {
            get
            {
                return _LastTime;
            }
        }
        DbHelperSQL SqlHelper = new DbHelperSQL();
        public Spider(string url, int race)
        {
            MatchUrl = url;
            Race = race;
        }

        public Spider(string url, int race, CCMember cm)
        {
            MatchUrl = url;
            Race = race;
            cCmemberInstance = cm;
            //cm.Config = new ZdConfig { MatchUrl = this.MatchUrl, Race = this.Race.ToString() };
            cm.Config = new ZdConfig { MatchUrl = this.MatchUrl };

        }

        private void GetQPOdds()
        {
            Dictionary<string, string[,]> dicData = CCmemberInstance.GetQPOddsByRace(Race.ToString());
            if (dicData != null)
            {
                qpOdds = dicData["QP"];
                qOdds = dicData["Q"];
            }
        }


        public int GetRaceLastTime()
        {
            return CCmemberInstance.GetRaceLastTime(Race.ToString());
        }
        private void GetAllData()
        {

            while (!bStop)
            {
                _LastTime = GetRaceLastTime();

                
                //时间在30分钟内，5S读一次
                if (_LastTime < 30)
                {
                    Thread.Sleep(5000);
                }
                else
                {
                    Thread.Sleep(20000);
                }
                string timetag = "";

                if (_LastTime <= 30)
                {
                    GetQPOdds();
                    //timetag = "30分内";
                    #region
                    timetag = $"{_LastTime}分";
                    if (_LastTime == 0)
                    {
                        timetag = "0秒";
                        zerocounter++;
                        if (zerocounter == 6)
                        {
                            timetag = "30秒";
                        }
                        if (zerocounter == 8)
                        {
                            timetag = "20秒";
                        }
                        if (zerocounter == 10)
                        {
                            timetag = "10秒";
                        }
                    }
                    #endregion

                }
                SaveData(timetag);
                System.Diagnostics.Debug.WriteLine("GetAllData");
            }
        }

        public void Start()
        {
            bStop = false;
            new Thread(GetAllData).Start();
        }

        private void SaveData(string timetag)
        {
            SaveqpOdds(timetag);
            SaveqOdds(timetag);
        }

        private void SaveqpOdds(string timetag)
        {
            string sql = "";
            List<string> arrayList = new List<string>();
            sql = $"delete From qpOdds where match='{CurrentMatch}' and race={Race} and qtype='QP'  and timetag='{timetag}'";
            arrayList.Add(sql);
            for (int i = 1; i <= 20; i++)
            {
                for (int j = 1; j <= 20; j++)
                {
                    if (qpOdds != null)
                    {
                        if (double.TryParse(qpOdds[i, j], out double odds))
                        {
                            sql = $"insert into qpOdds (match,race,minute,horse1,horse2,odds,qtype,timetag) " +
                                              $"values('{CurrentMatch}',{Race},{_LastTime},{i},{j},{odds},'QP','{timetag}')";
                            arrayList.Add(sql);
                        }
                    }
                }
            }
            SqlHelper.ExecuteSqlTran(arrayList);
        }
        private void SaveqOdds(string timetag)
        {
            string sql = "";
            List<string> arrayList = new List<string>();
            sql = $"delete  From qpOdds where match='{CurrentMatch}' and race={Race} and qtype='Q'  and timetag='{timetag}'";
            arrayList.Add(sql);
            for (int i = 1; i <= 20; i++)
            {
                for (int j = 1; j <= 20; j++)
                {
                    if (qOdds != null)
                    {
                        if (double.TryParse(qOdds[i, j], out double odds))
                        {
                            sql = $"insert into qpOdds (match,race,minute,horse1,horse2,odds,qtype,timetag) " +
                     $"values('{CurrentMatch}',{Race},{_LastTime},'{i}',{j},{odds},'Q','{timetag}')";
                            arrayList.Add(sql);
                        }
                    }
                }
            }
            SqlHelper.ExecuteSqlTran(arrayList);
        }
        public void Stop()
        {
            bStop = true;
        }
    }
}
