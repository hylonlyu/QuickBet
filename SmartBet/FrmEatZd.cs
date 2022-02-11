using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EatZD
{
    public partial class FrmEatZd : Form
    {
        private FrmWebbrowser frmBrowser;
        /// <summary>
        /// 已经开的比赛场次
        /// </summary>
        List<int> lstOpenedRace = new List<int>();
        List<Spider> lstSpider = new List<Spider>();
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        ZdConfig Config = new ZdConfig();
        private LoginStatus IsLogin = LoginStatus.LOGOUT;
        private ZdStrategy cCmemberInstance;
        public ZdStrategy CCmemberInstance
        {
            get
            {
                if (cCmemberInstance == null)
                {
                    cCmemberInstance = new ZdStrategy();
                }
                return cCmemberInstance;
            }
            set
            {
                cCmemberInstance = value;
            }
        }

        public bool bCheckOnline
        {
            get;
            set;
        }
        private int CurrentErrorTimes = 0;
        private readonly int ErrorTimesLimit = 3;
        UserInfo uInfo = new UserInfo();
        public FrmEatZd()
        {
            InitializeComponent();
            CCmemberInstance.OnLoginOk += CCmemberInstance_OnLoginOk;
            CCmemberInstance.OnLoginFail += CCmemberInstance_OnLoginFail;
            CCmemberInstance.OnLogout += CCmemberInstance_OnLogout;
            CCmemberInstance.OnBetOk += CCmemberInstance_OnBetOk;
            CCmemberInstance.ShowMsg += CCmemberInstance_ShowMsg;
            CCmemberInstance.OnNewTickt += CCmemberInstance_OnNewTickt;
        }

        private void CCmemberInstance_OnNewTickt(RaceInfoEnity enity)
        {
            DisplayBetInfo(enity);
        }

        private void CCmemberInstance_ShowMsg(string str)
        {
            ShowInfoMsg(str);
        }

        private void CCmemberInstance_OnBetOk(BettedItem item)
        {
            ShowInfoMsg(item.ToString());
            ShowBetResult(item);
        }

        private void CCmemberInstance_OnLogout()
        {
            ShowInfoMsg("账号退出");
            SetBtnStartEnable(false);
            IsLogin = LoginStatus.LOGOUT;

            Thread.Sleep(5000);
            ShowInfoMsg("账号退出，重新登陆");
            new Thread(new ParameterizedThreadStart(DoLogin)).Start(uInfo);
        }

        private void CCmemberInstance_OnLoginFail()
        {
            SetBtnStartEnable(false);
            IsLogin = LoginStatus.LOGOUT;
            Thread.Sleep(5000);

            ShowInfoMsg("登陆失败，重新登陆");
            new Thread(new ParameterizedThreadStart(DoLogin)).Start(uInfo);
        }

        private void CCmemberInstance_OnLoginOk()
        {
            ShowInfoMsg("登陆成功");
            SetBtnLoginText("退出");
            SetBtnStartEnable(true);
            SetBtnEnable(btnWeb, true);
            IsLogin = LoginStatus.LOGIN;
        }

        private void ShowInfoMsg(string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(ShowInfoMsg), str);
            }
            else
            {
                string time = DateTime.Now.ToString("HH:mm:ss:ffff");
                lstInfo.Items.Add($"{time} ##  {str}");
                lstInfo.SelectedIndex = lstInfo.Items.Count - 1;
            }

        }
        private void SetBtnLoginText(string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetBtnLoginText), str);
            }
            else
            {
                this.btnLogin.Text = str;
            }
        }

        private void SetBtnStartEnable(bool bEnalbe)
        {
            SetBtnEnable(btnStart, bEnalbe);
        }

        private void SetBtnEnable(Button btn, bool bEnalbe)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<Button,bool>(SetBtnEnable),btn, bEnalbe);
            }
            else
            {
                btn.Enabled = bEnalbe;
            }
        }
        private string GetRace()
        {
            string rc = "1";
            foreach(var c in gpRace.Controls)
            {
                if(c is RadioButton)
                {
                    RadioButton rb = c as RadioButton;
                    if(rb.Checked)
                    {
                        rc = rb.Text.Trim();
                        break;
                    }
                }
            }
            return rc;
        }
        private void SetRace(string rc)
        {
            foreach (var c in gpRace.Controls)
            {
                if (c is RadioButton)
                {
                    RadioButton rb = c as RadioButton;
                    if (rb.Text.Trim().Equals(rc))
                    {
                        rb.Checked = true;
                    }
                }
            }
        }
        #region Config
        private void SaveConfig()
        {
            if (!string.IsNullOrEmpty(cobMatch.SelectedText.Trim()))
            {
                Config.MatchCombol = $"{cobMatch.SelectedText.Trim()}|{cobMatch.SelectedValue.ToString().Trim()}";
            }
            if (cobMatch.SelectedValue != null)
            {
                Config.MatchUrl = cobMatch.SelectedValue.ToString();
            }
            Config.SiteUrl = txtUrl.Text.Trim();
            Config.Accout = txtAccount.Text.Trim();
            Config.Pwd = txtPwd.Text.Trim();
            Config.Pin = txtPin.Text.Trim();
            Config.Race = GetRace();

            Config.XMZK =Util.Text2Double(QbetSetting.XMZK.Text.Trim());
            Config.BXMPS = QbetSetting.BXMPS.Checked;
            Config.XMPSMIN = Util.Text2Int(QbetSetting.XMPSMIN.Text.Trim());
            Config.XMPSMAX = Util.Text2Int(QbetSetting.XMPSMAX.Text.Trim());
            Config.PLMIN = Util.Text2Double(QbetSetting.PLMIN.Text.Trim());
            Config.PLMAX = Util.Text2Double(QbetSetting.PLMAX.Text.Trim());
            Config.XDJX = Util.Text2Double(QbetSetting.XDJX.Text.Trim());
            Config.XDZK = Util.Text2Double(QbetSetting.XDZK.Text.Trim());
            Config.GDPS = Util.Text2Int(QbetSetting.GDPS.Text.Trim());
            Config.BGDPS = QbetSetting.BGDPS.Checked;

            Config.PL11 = Util.Text2Double(QbetSetting.PL11.Text.Trim());
            Config.PL12 = Util.Text2Double(QbetSetting.PL12.Text.Trim());
            Config.PL21 = Util.Text2Double(QbetSetting.PL21.Text.Trim());
            Config.PL22 = Util.Text2Double(QbetSetting.PL22.Text.Trim());
            Config.PL31 = Util.Text2Double(QbetSetting.PL31.Text.Trim());
            Config.PL32 = Util.Text2Double(QbetSetting.PL32.Text.Trim());
            Config.PL41 = Util.Text2Double(QbetSetting.PL41.Text.Trim());
            Config.PL42 = Util.Text2Double(QbetSetting.PL42.Text.Trim());
            Config.PL51 = Util.Text2Double(QbetSetting.PL51.Text.Trim());
            Config.PL52 = Util.Text2Double(QbetSetting.PL52.Text.Trim());
            Config.BU1 = Util.Text2Double(QbetSetting.BU1.Text.Trim());
            Config.BU2 = Util.Text2Double(QbetSetting.BU2.Text.Trim());
            Config.BU3 = Util.Text2Double(QbetSetting.BU3.Text.Trim());
            Config.BU4 = Util.Text2Double(QbetSetting.BU4.Text.Trim());
            Config.BU5 = Util.Text2Double(QbetSetting.BU5.Text.Trim());

            Config.XMZK2 = Util.Text2Double(QpbetSetting.XMZK.Text.Trim());
            Config.BXMPS2 = QpbetSetting.BXMPS.Checked;
            Config.XMPSMIN2 = Util.Text2Int(QpbetSetting.XMPSMIN.Text.Trim());
            Config.XMPSMAX2 = Util.Text2Int(QpbetSetting.XMPSMAX.Text.Trim());
            Config.PLMIN2 = Util.Text2Double(QpbetSetting.PLMIN.Text.Trim());
            Config.PLMAX2 = Util.Text2Double(QpbetSetting.PLMAX.Text.Trim());
            Config.XDJX2 = Util.Text2Double(QpbetSetting.XDJX.Text.Trim());
            Config.XDZK2 = Util.Text2Double(QpbetSetting.XDZK.Text.Trim());
            Config.GDPS2 = Util.Text2Int(QpbetSetting.GDPS.Text.Trim());
            Config.BGDPS2 = QpbetSetting.BGDPS.Checked;

            Config.PL112 = Util.Text2Double(QpbetSetting.PL11.Text.Trim());
            Config.PL122 = Util.Text2Double(QpbetSetting.PL12.Text.Trim());
            Config.PL212 = Util.Text2Double(QpbetSetting.PL21.Text.Trim());
            Config.PL222 = Util.Text2Double(QpbetSetting.PL22.Text.Trim());
            Config.PL312 = Util.Text2Double(QpbetSetting.PL31.Text.Trim());
            Config.PL322 = Util.Text2Double(QpbetSetting.PL32.Text.Trim());
            Config.PL412 = Util.Text2Double(QpbetSetting.PL41.Text.Trim());
            Config.PL422 = Util.Text2Double(QpbetSetting.PL42.Text.Trim());
            Config.PL512 = Util.Text2Double(QpbetSetting.PL51.Text.Trim());
            Config.PL522 = Util.Text2Double(QpbetSetting.PL52.Text.Trim());
            Config.BU12 = Util.Text2Double(QpbetSetting.BU1.Text.Trim());
            Config.BU22 = Util.Text2Double(QpbetSetting.BU2.Text.Trim());
            Config.BU32 = Util.Text2Double(QpbetSetting.BU3.Text.Trim());
            Config.BU42 = Util.Text2Double(QpbetSetting.BU4.Text.Trim());
            Config.BU52 = Util.Text2Double(QpbetSetting.BU5.Text.Trim());
            string file = string.Format(@"setting\{0}.jpg", "FrmEatZd");

            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Config);
            }
        }

        private void LoadConfig()
        {
            string file = string.Format(@"setting\{0}.jpg", "FrmEatZd");
            if (File.Exists(file))
            {
                try
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        Config = bf.Deserialize(fs) as ZdConfig;
                    }
                }
                catch (Exception ex)
                {
                    Config = new ZdConfig();
                }

            }
            else
            {
                Config = new ZdConfig();
            }
            SetConfig();
        }

        private void SetConfig()
        {
            try
            {
                txtUrl.Text = Config.SiteUrl;
                txtAccount.Text = Config.Accout;
                txtPwd.Text = Config.Pwd;
                txtPin.Text = Config.Pin;
                //cobRace.Text = Config.Race;
                SetRace(Config.Race);
     
                QbetSetting.XMZK.Text = Config.XMZK.ToString();
                QbetSetting.BXMPS.Checked = Config.BXMPS;
                QbetSetting.XMPSMIN.Text = Config.XMPSMIN.ToString();
                QbetSetting.XMPSMAX.Text = Config.XMPSMAX.ToString();
                QbetSetting.PLMIN.Text = Config.PLMIN.ToString();
                QbetSetting.PLMAX.Text = Config.PLMAX.ToString();
                QbetSetting.XDJX.Text = Config.XDJX.ToString();
                QbetSetting.XDZK.Text = Config.XDZK.ToString();
                QbetSetting.GDPS.Text = Config.GDPS.ToString();
                QbetSetting.BGDPS.Checked = Config.BGDPS;

                QbetSetting.PL11.Text = Config.PL11.ToString();
                QbetSetting.PL12.Text = Config.PL12.ToString();
                QbetSetting.PL21.Text = Config.PL21.ToString();
                QbetSetting.PL22.Text = Config.PL22.ToString();
                QbetSetting.PL31.Text = Config.PL31.ToString();
                QbetSetting.PL32.Text = Config.PL32.ToString();
                QbetSetting.PL41.Text = Config.PL41.ToString();
                QbetSetting.PL42.Text = Config.PL42.ToString();
                QbetSetting.PL51.Text = Config.PL51.ToString();
                QbetSetting.PL52.Text = Config.PL52.ToString();
                QbetSetting.BU1.Text = Config.BU1.ToString();
                QbetSetting.BU2.Text = Config.BU2.ToString();
                QbetSetting.BU3.Text = Config.BU3.ToString();
                QbetSetting.BU4.Text = Config.BU4.ToString();
                QbetSetting.BU5.Text = Config.BU5.ToString();

                QpbetSetting.XMZK.Text = Config.XMZK2.ToString();
                QpbetSetting.BXMPS.Checked = Config.BXMPS2;
                QpbetSetting.XMPSMIN.Text = Config.XMPSMIN2.ToString();
                QpbetSetting.XMPSMAX.Text = Config.XMPSMAX2.ToString();
                QpbetSetting.PLMIN.Text = Config.PLMIN2.ToString();
                QpbetSetting.PLMAX.Text = Config.PLMAX2.ToString();
                QpbetSetting.XDJX.Text = Config.XDJX2.ToString();
                QpbetSetting.XDZK.Text = Config.XDZK2.ToString();
                QpbetSetting.GDPS.Text = Config.GDPS2.ToString();
                QpbetSetting.BGDPS.Checked = Config.BGDPS2;

                QpbetSetting.PL11.Text = Config.PL112.ToString();
                QpbetSetting.PL12.Text = Config.PL122.ToString();
                QpbetSetting.PL21.Text = Config.PL212.ToString();
                QpbetSetting.PL22.Text = Config.PL222.ToString();
                QpbetSetting.PL31.Text = Config.PL312.ToString();
                QpbetSetting.PL32.Text = Config.PL322.ToString();
                QpbetSetting.PL41.Text = Config.PL412.ToString();
                QpbetSetting.PL42.Text = Config.PL422.ToString();
                QpbetSetting.PL51.Text = Config.PL512.ToString();
                QpbetSetting.PL52.Text = Config.PL522.ToString();
                QpbetSetting.BU1.Text = Config.BU12.ToString();
                QpbetSetting.BU2.Text = Config.BU22.ToString();
                QpbetSetting.BU3.Text = Config.BU32.ToString();
                QpbetSetting.BU4.Text = Config.BU42.ToString();
                QpbetSetting.BU5.Text = Config.BU52.ToString();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        private void FrmEatZd_Load(object sender, EventArgs e)
        {
            LoadConfig();
            viewHistoryQ.ViewEventHandler += ViewHistoryQ_ViewEventHandler;
            viewHistoryPq.ViewEventHandler += ViewHistoryPq_ViewEventHandler;
        }

        private void ViewHistoryPq_ViewEventHandler()
        {
            ViewQp();
        }

        private void ViewHistoryQ_ViewEventHandler()
        {
            ViewQ();
        }

        private void InitGrid(DataGridView dgvCtrl)
        {
            Dictionary<string, string> col = new Dictionary<string, string>();
            col.Add("snakehead", "会员");
            col.Add("date", "比赛日期");
            col.Add("playtype", "比赛类型");
            col.Add("country", "场地");
            col.Add("location", "赛事");
            col.Add("odds", "赔率");
            col.Add("race", "场");
            col.Add("horse", "马");
            col.Add("win", "W");
            col.Add("place", "P");
            col.Add("zhe", "%");
            col.Add("lwin", "W极");
            col.Add("lplace", "P极");
            col.Add("classType", "类型");
            col.Add("bettype", "下注");
            col.Add("time", "时间");
            col.Add("key", "key");
            col.Add("Add", "Add");
            col.Add("Guid", "Guid");

            foreach (KeyValuePair<string, string> kv in col)
            {
                dgvCtrl.Columns.Add(kv.Key, kv.Value);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (IsLogin != LoginStatus.DOINGLOGIN)
            {
                if (btnLogin.Text.Equals("登陆"))
                {
                    
                    uInfo.CUserName = txtAccount.Text.Trim();
                    uInfo.CPassword = txtPwd.Text.Trim();
                    uInfo.CPin = txtPin.Text.Trim();
                    uInfo.CUrl = txtUrl.Text.Trim();
                    new Thread(new ParameterizedThreadStart(DoLogin)).Start(uInfo);

                }
                if (btnLogin.Text.Equals("退出"))
                {
                    IsLogin = LoginStatus.LOGOUT;
                    SetBtnLoginText("登陆");
                }
            }
       
        }
        private void DoLogin(object obj)
        {
            if (IsLogin == LoginStatus.LOGOUT)
            {
                UserInfo uInfo = obj as UserInfo;
                IsLogin = LoginStatus.DOINGLOGIN;
                ShowInfoMsg("正在登陆");
                Hashtable ht = CCmemberInstance.DoLogin(uInfo.CUrl, uInfo);
                if (ht != null)
                {
                    BindMatchList(ht[4] as DataTable);
                }
            }
        }

        private void BindMatchList(DataTable dtMatch)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<DataTable>(BindMatchList), dtMatch);
            }
            else
            {
                try
                {
                    //加上上次的赛事
                    if (!string.IsNullOrEmpty(Config.MatchCombol))
                    {
                        string[] tmp = Config.MatchCombol.Split("|".ToCharArray());
                        if (tmp.Length > 1)
                        {
                            DataRow dr = dtMatch.NewRow();
                            dr["tip"] = tmp[0];
                            dr["url"] = tmp[1];
                            dtMatch.Rows.InsertAt(dr, 0);
                        }
                    }

                    //只打香港的比赛
                    string[] FilterMatch = new string[] { "香港", "马来西亚", "新加坡" };
                    DataTable TempDT = dtMatch.Clone();
                    foreach (DataRow dr in dtMatch.Rows)
                    {
                        foreach (var item in FilterMatch)
                        {
                            if (dr["tip"].ToString().StartsWith(item))
                            {
                                TempDT.ImportRow(dr);
                                break;
                            }
                        }
                    }
                    //cobMatch.DataSource = TempDT;

                    cobMatch.DataSource = dtMatch;
                    cobMatch.DisplayMember = "tip";
                    cobMatch.ValueMember = "url";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "开始")
            {
                btnStart.Text = "停止";
                SaveConfig();
                CCmemberInstance.Config = Config;
                
                CCmemberInstance.Start();
                GetOpenedRace();
                GetAllData();
            }
            else if (btnStart.Text == "停止")
            {
                btnStart.Text = "开始";
                CCmemberInstance.Stop();
                StopGetData();
            }
        }

        private void ShowBetResult(BettedItem item)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<BettedItem>(ShowBetResult), item);
            }
            else
            {
                string bettime = item.BetTime.ToLongTimeString();
                string match = "";
                string[] tmp = cobMatch.Text.Split("_".ToCharArray());
                if (tmp.Length > 2)
                {
                    match = $"{tmp[0]}_{tmp[1]}";
                }

                string race = item.Race;
                string horse = item.Horse;
                string win = item.PlayType == PlayType.Q ? item.DBetCount.ToString() : "";
                string place = item.PlayType == PlayType.QP ? item.DBetCount.ToString() : "";
                string zhe = item.Zhe.ToString();
                string lwin = item.PlayType == PlayType.Q ? item.Lim.ToString() : "";
                string lplace = item.PlayType == PlayType.QP ? item.Lim.ToString() : "";
                string bettype = item.BetType == BetType.EAT ? "吃" : "赌";
                string odds = item.Odds.ToString();
                string total = item.TotalCount.ToString();
                string result = item.Result ? "成功" : "失败";
                string reason = item.Reason;

                dgvBetResult.Rows.Insert(0, new object[] { bettime, match, race, horse, win, place, zhe, lwin, lplace, bettype, odds, total, result, reason });
            }
        }

        private void btnWeb_Click(object sender, EventArgs e)
        {
            FrmWebbrowser frmMatch = new FrmWebbrowser();
            frmMatch.Url = $"http://{CCmemberInstance.DoMain}/playerhk.jsp";
            frmMatch.CC = CCmemberInstance.cc;
            frmMatch.Show();
        }

        private void SetCookie(string Url, CookieContainer CC)
        {
            Uri uri = new Uri(Url);
            string cDomain = uri.Host;
            CookieContainer container = CC;
            CookieCollection cc = container.GetCookies(new Uri(Url));
            foreach (Cookie c in cc)
            {
                InternetSetCookie("http://" + cDomain, c.Name.ToString(), c.Value.ToString());
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void timerShowData_Tick(object sender, EventArgs e)
        {
            if(chkAuto.Checked)
            {
                GetandDisplayRace();
            }
        }

        /// <summary>
        /// 获取当前比赛的场次，并在cobrace中显示
        /// </summary>
        private void GetandDisplayRace()
        {
            string url = cobMatch.SelectedValue.ToString();
            BackgroundWorker bw = new BackgroundWorker();
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerAsync(url);
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string url = e.Argument as string;
            List<int> res = CCmemberInstance.GetOpenedRace(url);
            e.Result = res;
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lstOpenedRace = e.Result as List<int>;
            if (lstOpenedRace.Count > 0)
            {
                cobRace.DataSource = lstOpenedRace;
                //cobRace.Text = lstOpenedRace[0].ToString();
                //Config.Race = lstOpenedRace[0].ToString();
                CCmemberInstance.Config = Config;
                ShowInfoMsg($"获取了{lstOpenedRace.Count}场，首场{lstOpenedRace[0]}");
            }
        }

        private void btnTrade_Click(object sender, EventArgs e)
        {
            FrmWebbrowser frmBrowser = new FrmWebbrowser();
            frmBrowser.Url = $"http://{CCmemberInstance.DoMain}/new_history_live.jsp";
            frmBrowser.CC = CCmemberInstance.cc;
            frmBrowser.Show();
        }

        private void DisplayBetInfo(RaceInfoEnity htBetInfo)
        {

        }

        #region 注册相关
        private void DoInitRegInfo()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(this.bw_DoInitRegInfo);
            worker.ProgressChanged += new ProgressChangedEventHandler(this.bw_DoInitRegInfoChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_DoInitRegInfoCompleted);
            Hashtable argument = new Hashtable();
            worker.RunWorkerAsync(argument);
        }

        private void bw_DoInitRegInfo(object sender, DoWorkEventArgs e)
        {
            string machineCode = Security.GetMachineCode();
            if (machineCode == null)
            {
                MessageBox.Show("系統內部錯誤，錯誤代碼：100001");
                Application.Exit();
            }
            else
            {
                RegResult regStatus = Security.GetOnlineStatus();
                Security.PostUnRegUserData(machineCode, this.GetUsers());
                e.Result = regStatus;
            }
        }

        private void bw_DoInitRegInfoChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void bw_DoInitRegInfoCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RegResult result = (RegResult)e.Result;
            if (result != null && result.GetResult())
            {
                btnLogin.Enabled = true;
                btnStart.Enabled = true;
                this.Text = $"帳號到期: {result.GetExpiredTime().ToString()}";
                CurrentErrorTimes = 0;
            }
            else if (result != null && !result.GetResult())
            {
                MessageBox.Show(result.GetMsg());
                Environment.Exit(0);
            }
            else
            {
                //检测容错的次数
                CurrentErrorTimes++;
                if (CurrentErrorTimes >= ErrorTimesLimit)
                {
                    Environment.Exit(0);
                }
            }
        }

        string GetUsers()
        {
            return "";
        }
        #endregion

        private void timeReg_Tick(object sender, EventArgs e)
        {
            if (bCheckOnline)
            {
                this.DoInitRegInfo();
            }
        }

        private void FrmEatZd_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnStart2_Click(object sender, EventArgs e)
        {
            //if (btnStart2.Text == "开始")
            //{
            //    btnStart2.Text = "停止";
            //    SaveConfig();
            //    CCmemberInstance.Strategy = 2;
            //    CCmemberInstance.Config = Config;
            //    CCmemberInstance.Start();
            //    timerShowData.Enabled = true;
            //}
            //else if (btnStart2.Text == "停止")
            //{
            //    btnStart2.Text = "开始";
            //    CCmemberInstance.Stop();
            //    timerShowData.Enabled = false;
            //}
        }

        private void btnStopCp_Click(object sender, EventArgs e)
        {
        }

        private void btnCal_Click(object sender, EventArgs e)
        {
            if(chkQ.Checked)
            {
                string[,] odds = null;
                if(!chkHistory.Checked)
                {
                    odds = CCmemberInstance.qOdds;
                }
                else
                {
                    CalStrategy cs = new CalStrategy(cobMatch.Text.Trim(), GetRace(), "Q");
                    odds = cs.GetOddsArray();
                }
                QbetComposeCtrl1.CalcStrategy(smatSelection.se,PlayType.Q,betMoney.Money, odds);
            }
            if(chkQP.Checked)
            {
                string[,] odds = null;
                if (!chkHistory.Checked)
                {
                    odds = CCmemberInstance.qpOdds;
                }
                else
                {
                    CalStrategy cs = new CalStrategy(cobMatch.Text.Trim(), GetRace(), "QP");
                    odds = cs.GetOddsArray();
                }
                QPbetComposeCtrl2.CalcStrategy(smatSelection.se, PlayType.QP, betMoney.Money, odds);
            }
        }

        private void btnSaveCal_Click(object sender, EventArgs e)
        {
            if (chkQ.Checked)
            {
                QbetComposeCtrl1.AddStrategy();
            }
            if (chkQP.Checked)
            {
                QPbetComposeCtrl2.AddStrategy();
            }
        }

        private void btnBet_Click(object sender, EventArgs e)
        {
            if(chkBetQ.Checked)
            {
                CCmemberInstance.DoBetQ(QbetComposeCtrl1.LstStrategy);
            }
            if(chkBetQP.Checked)
            {
                CCmemberInstance.DoBetQP(QPbetComposeCtrl2.LstStrategy);
            }
        }

        private void rc12_CheckedChanged(object sender, EventArgs e)
        {
            QbetComposeCtrl1.ClearStrategy();
            QPbetComposeCtrl2.ClearStrategy();
        }

        private void GetOpenedRace()
        {
            string url = cobMatch.SelectedValue.ToString();
            var Res = Task<List<int>>.Run(() => CCmemberInstance.GetOpenedRace(url));
            //lstOpenedRace = CCmemberInstance.GetOpenedRace(url);
            lstOpenedRace = Res.Result;
            if (lstOpenedRace.Count > 0)
            {
                ShowInfoMsg($"获取了{lstOpenedRace.Count}场，首场{lstOpenedRace[0]}");
            }
        }

        private void GetAllData()
        {
            foreach (var race in lstOpenedRace)
            {
                Spider spider = new Spider(Config.MatchUrl, race);
                spider.CCmemberInstance = CCmemberInstance;
                spider.CurrentRace = lstOpenedRace[0];
                spider.CurrentMatch = cobMatch.Text;
                lstSpider.Add(spider);
                Task.Run(() => spider.Start());
            }
        }

        private void StopGetData()
        {
            foreach (Spider s in lstSpider)
            {
                s.Stop();
            }
        }

        private void btnViewQ_Click(object sender, EventArgs e)
        {
            ViewQ();

        }

        private void ViewQ()
        {
            viewHistoryQ.Match = cobMatch.Text.Trim();
            viewHistoryQ.Race = GetRace();
            viewHistoryQ.Qtype = "Q";
            if(!(string.IsNullOrEmpty(viewHistoryQ.Match) ||string.IsNullOrEmpty(viewHistoryQ.Race)))
            {
                if (chkHistory.Checked)
                {
                    viewHistoryQ.ShowHistory();
                }
                else
                {
                    viewHistoryQ.ShowNow();
                }
            }
           
        }

        private void chkHistory_CheckedChanged(object sender, EventArgs e)
        {
            if(chkHistory.Checked)
            {
                BindHistoryMatch();
            }
        }

        private void BindHistoryMatch()
        {
            CalStrategy cs = new CalStrategy();
            cobMatch.DataSource = cs.GetHistoryMatch();
            cobMatch.DisplayMember = "match";
            cobMatch.ValueMember = "match";
        }

        private void btnViewPq_Click(object sender, EventArgs e)
        {
            ViewQp();
        }

        private void ViewQp()
        {
            viewHistoryPq.Match = cobMatch.Text.Trim();
            viewHistoryPq.Race = GetRace();
            viewHistoryPq.Qtype = "QP";
            if (!(string.IsNullOrEmpty(viewHistoryPq.Match) || string.IsNullOrEmpty(viewHistoryPq.Race)))
            {
                if (chkHistory.Checked)
                {
                    viewHistoryPq.ShowHistory();
                }
                else
                {
                    viewHistoryPq.ShowNow();
                }
            }
        }

        private void cobMatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetMinuteDatasource();
        }

        private void SetMinuteDatasource()
        {
            string match = cobMatch.Text;
            string race = GetRace();

            CalStrategy cs = new CalStrategy(match, race, "Q");
            DataTable dtMinute = cs.GetHistoryMinute();
            if (dtMinute != null && dtMinute.Rows.Count > 0)
            {
                viewHistoryQ.SetMinuteDatasource(dtMinute);
                viewHistoryPq.SetMinuteDatasource(dtMinute);
            }
        }

        private void timerMinuteDs_Tick(object sender, EventArgs e)
        {
            SetMinuteDatasource();
            if (!chkHistory.Checked)
            {
                ViewQ();
                ViewQp();
            }
        }
    }
}
