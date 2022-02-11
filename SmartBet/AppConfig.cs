using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EatZD
{
    [Serializable]
    public class AppConfig
    {
        public string MatchCombol;
        public string MatchUrl;
        public string SiteUrl;
        public string Race;
        public string Accout;
        public string Pwd;
        public string Pin;


    }

    [Serializable]
    public class ZdConfig : AppConfig
    {
        #region Q
        public double XMZK;
        public int XMPSMIN;
        public int XMPSMAX;
        public bool BXMPS;
        public double PLMIN;
        public double PLMAX;
        public double XDJX;
        public double XDZK;
        public int GDPS;
        public bool BGDPS;

        public double PL11;
        public double PL12;
        public double PL21;
        public double PL22;
        public double PL31;
        public double PL32;
        public double PL41;
        public double PL42;
        public double PL51;
        public double PL52;
        public double BU1;
        public double BU2;
        public double BU3;
        public double BU4;
        public double BU5;
        #endregion
        #region QP
        public double XMZK2;
        public int XMPSMIN2;
        public int XMPSMAX2;
        public bool BXMPS2;
        public double PLMIN2;
        public double PLMAX2;
        public double XDJX2;
        public double XDZK2;
        public int GDPS2;
        public bool BGDPS2;
        public double PL112;
        public double PL122;
        public double PL212;
        public double PL222;
        public double PL312;
        public double PL322;
        public double PL412;
        public double PL422;
        public double PL512;
        public double PL522;
        public double BU12;
        public double BU22;
        public double BU32;
        public double BU42;
        public double BU52;
        #endregion

    }

    public class RCConfig
    {
        public static double QEatZhe = 80;
        public static int QEatLim = 700;
        public static double QBetZhe = 100;
        public static int QBetLim = 700;

        public static double QPEatZhe = 80;
        public static int QPEatLim = 400;
        public static double QPBetZhe = 100;
        public static int QPBetLim = 400;
    }
}
