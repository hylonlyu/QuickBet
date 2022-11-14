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

        public string Race
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
        public Spider(string url, string race)
        {
            MatchUrl = url;
            Race = race;
        }

        //public Spider(string url, string race, CCMember cm)
        //{
        //    MatchUrl = url;
        //    Race = race;
        //    cCmemberInstance = cm;
        //    //cm.Config = new ZdConfig { MatchUrl = this.MatchUrl, Race = this.Race.ToString() };
        //    cm.Config = new ZdConfig { MatchUrl = this.MatchUrl };

        //}

        private void GetQData()
        {
            Dictionary<string, Tuple<double, double>> dicQData2 = CCmemberInstance.GetQData(Race.ToString());
            if (dicQData2.Count > 0)
            {
                dicQData = dicQData2;
            }
        }

        private void GetQPData()
        {
            Dictionary<string, Tuple<double, double>> dicQPData2 = CCmemberInstance.GetQPData(Race.ToString());
            if (dicQPData2.Count > 0)
            {
                dicQPData = dicQPData2;
            }
        }

        private void GetQPOdds()
        {
            Dictionary<string, string[,]> dicData = CCmemberInstance.GetQPOddsByRace(Race.ToString());
            if (dicData != null)
            {
                if (!IsOddsEmpty(qpOdds))
                {
                    qpOdds = dicData["QP"];
                }
                if(!IsOddsEmpty(qOdds))
                {
                    qOdds = dicData["Q"];
                }
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
                MatchLatestTime.SetLatestTime($"{MatchUrl}-{Race}",_LastTime);


                System.Diagnostics.Debug.WriteLine($"race=============: {Race.ToString()}");
                Thread.Sleep(5000);
                if (_LastTime <= 30)
                {
                    GetQData();
                    GetQPData();
                    GetQPOdds();
                    #region
                    //timetag = $"{_LastTime}分";
                    string timetag = $"{_LastTime}";
                    #endregion
                    if (_LastTime == 0)
                    {
                        timetag = "0";
                        zerocounter++;
                        if (zerocounter == 6)
                        {
                            timetag = "-30";
                        }
                        if (zerocounter == 8)
                        {
                            timetag = "-20";
                        }
                        if (zerocounter == 10)
                        {
                            timetag = "-10";
                        }
                    }
                    SaveData(timetag);
                }
                //断线时获取到的_LastTime=9999，断线时的数据不保存
                //if (_LastTime<9990)
                //{
                //    SaveData(timetag);
                //}
                
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
            SavedicQData(timetag);
            SavedicQPData(timetag);
            SaveqpOdds(timetag);
            SaveqOdds(timetag);
        }

        private void SavedicQData(string timetag)
        {
            string sql = "";
            List<string> arrayList = new List<string>();
            sql = $"delete From dicQData where match='{CurrentMatch}' and race={Race} and qtype='Q' and  timetag='{timetag}'";
            arrayList.Add(sql);
            foreach (var item in dicQData)
            {
                sql = $"insert into dicQdata (match,race,horse,zhe,piao,minute,qtype,timetag) " +
                   $"values('{CurrentMatch}',{Race},'{item.Key}',{item.Value.Item1},{item.Value.Item2},{_LastTime},'Q','{timetag}')";
                arrayList.Add(sql);
            }
            SqlHelper.ExecuteSqlTran(arrayList);
        }
        private void SavedicQPData(string timetag)
        {
            string sql = "";
            List<string> arrayList = new List<string>();
            sql = $"delete From dicQData where match='{CurrentMatch}' and race={Race} and qtype='QP'  and timetag='{timetag}'";
            arrayList.Add(sql);
            foreach (var item in dicQPData)
            {
                sql = $"insert into dicQdata (match,race,horse,zhe,piao,minute,qtype,timetag) " +
                   $"values('{CurrentMatch}',{Race},'{item.Key}',{item.Value.Item1},{item.Value.Item2},{_LastTime},'QP','{timetag}')";
                arrayList.Add(sql);
            }
            SqlHelper.ExecuteSqlTran(arrayList);

        }

        private void SaveqpOdds(string timetag)
        {
            string sql = "";
            if(!IsOddsEmpty(qpOdds))
            {
                List<string> arrayList = new List<string>();
                sql = $"delete From qpOdds where match='{CurrentMatch}' and race={Race} and qtype='QP'  and timetag='{timetag}'";
                arrayList.Add(sql);
                for (int i = 1; i <= qpOdds.GetLength(0) - 1; i++)
                {
                    for (int j = 1; j <= qpOdds.GetLength(1) - 1; j++)
                    {
                        if (double.TryParse(qpOdds[i, j], out double odds))
                        {
                            sql = $"insert into qpOdds (match,race,minute,horse1,horse2,odds,qtype,timetag) " +
                                              $"values('{CurrentMatch}',{Race},{_LastTime},{i},{j},{odds},'QP','{timetag}')";
                            arrayList.Add(sql);
                        }
                    }
                }
                SqlHelper.ExecuteSqlTran(arrayList);
            }
        }
        private void SaveqOdds(string timetag)
        {
            string sql = "";
            if (!IsOddsEmpty(qOdds))
            {
                List<string> arrayList = new List<string>();
                sql = $"delete  From qpOdds where match='{CurrentMatch}' and race={Race} and qtype='Q'  and timetag='{timetag}'";
                arrayList.Add(sql);
                for (int i = 1; i <= qOdds.GetLength(0) - 1; i++)
                {
                    for (int j = 1; j <= qOdds.GetLength(1) - 1; j++)
                    {
                        if (double.TryParse(qOdds[i, j], out double odds))
                        {
                            sql = $"insert into qpOdds (match,race,minute,horse1,horse2,odds,qtype,timetag) " +
                     $"values('{CurrentMatch}',{Race},{_LastTime},'{i}',{j},{odds},'Q','{timetag}')";
                            arrayList.Add(sql);
                        }
                    }
                }
                SqlHelper.ExecuteSqlTran(arrayList);
            }
        }

        private bool IsOddsEmpty(string[,] odds)
        {
            bool bRet = true;
            if (odds != null)
            {
                for (int i = 1; i <= odds.GetLength(0) - 1; i++)
                {
                    for (int j = 1; j <= odds.GetLength(1) - 1; j++)
                    {
                        if (double.TryParse(qOdds[i, j], out double odd))
                        {
                            if (odd > 0)
                            {
                                bRet = false;
                                break;
                            }
                        }
                    }
                }
            }
            return bRet;
        }
        public void Stop()
        {
            bStop = true;
        }
    }
}
