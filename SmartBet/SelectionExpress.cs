using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatZD
{
    [Serializable]
   public class SelectionExpress
    {
        private string main = string.Empty;
        private List<string> lsthorse = new List<string>();
        public string Main
        {
            get
            {
                return main;
            }
            set
            {
                main = value;
            }
        }
        public List<string> LstHorse
        {
            get
            {
                return lsthorse;
            }
            set
            {
                lsthorse = value;
            }
        }

        //g w   r
        //w  g r
        //g r   w

        public void RightAction(string horse)
        {
            if (!(main.Equals(horse) || lsthorse.Contains(horse)))
            {
                W2R(horse);
            }
            else if (lsthorse.Contains(horse))
            {
                G2R(horse);
            }
            else if (main.Equals(horse))
            {
                R2W(horse);
            }
        }

        public void LeftAction(string horse)
        {
            if (!(main.Equals(horse) || lsthorse.Contains(horse)))
            {
                W2G(horse);
            }
            else if (lsthorse.Contains(horse))
            {
                G2W(horse);
            }
            else if (main.Equals(horse))
            {
                R2G(horse);
            }
        }
        private void W2R(string horse)
        {
            if(!string.IsNullOrEmpty(main))
            {
                lsthorse.Add(main);
            }
            main = horse;
        }
        private void W2G(string horse)
        {
            lsthorse.Add(horse);
        }

        private void G2R(string horse)
        {
            if (!string.IsNullOrEmpty(main))
            {
                lsthorse.Add(main);
            }
            lsthorse.Remove(horse);
            main = horse;
        }
        private void G2W(string horse)
        {
            lsthorse.Remove(horse);
        }

        private void R2W(string horse)
        {
            main = string.Empty;
        }
        private void R2G(string horse)
        {
            main = string.Empty;
            lsthorse.Add(horse);
        }

        private int Compare(string x, string y)
        {
            int ret = 0;
            if(int.Parse(x) >int.Parse(y))
            {
                ret = 1;
            }
            else  if (int.Parse(x) < int.Parse(y))
            {
                ret = -1;
            }
            return ret;
        }
        public string GetExpresstion()
        {
            string exp = "";
            if(!string.IsNullOrEmpty(main))
            {
                exp += $"{main}>";
            }
            lsthorse.Sort(Compare);
            foreach (var h in lsthorse)
            {
                exp += $"{h}+";
            }
            if(!string.IsNullOrEmpty(exp))
            {
                exp = exp.Substring(0, exp.Length - 1);
            }
            return exp;
        }
        public void Clear()
        {
            main = string.Empty;
            lsthorse.Clear();
        }
    }
}
