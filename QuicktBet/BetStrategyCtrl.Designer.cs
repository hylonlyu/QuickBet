namespace EatZD
{
    partial class BetStrategyCtrl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblExpress = new System.Windows.Forms.Label();
            this.lblMoney = new System.Windows.Forms.Label();
            this.lblYjpc = new System.Windows.Forms.Label();
            this.lblCllose = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblExpress
            // 
            this.lblExpress.AutoSize = true;
            this.lblExpress.Location = new System.Drawing.Point(13, 10);
            this.lblExpress.Name = "lblExpress";
            this.lblExpress.Size = new System.Drawing.Size(55, 15);
            this.lblExpress.TabIndex = 0;
            this.lblExpress.Text = "label1";
            // 
            // lblMoney
            // 
            this.lblMoney.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMoney.AutoSize = true;
            this.lblMoney.Location = new System.Drawing.Point(13, 35);
            this.lblMoney.Name = "lblMoney";
            this.lblMoney.Size = new System.Drawing.Size(55, 15);
            this.lblMoney.TabIndex = 1;
            this.lblMoney.Text = "label2";
            // 
            // lblYjpc
            // 
            this.lblYjpc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblYjpc.AutoSize = true;
            this.lblYjpc.Location = new System.Drawing.Point(209, 35);
            this.lblYjpc.Name = "lblYjpc";
            this.lblYjpc.Size = new System.Drawing.Size(55, 15);
            this.lblYjpc.TabIndex = 2;
            this.lblYjpc.Text = "label3";
            // 
            // lblCllose
            // 
            this.lblCllose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCllose.AutoSize = true;
            this.lblCllose.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCllose.ForeColor = System.Drawing.Color.Red;
            this.lblCllose.Location = new System.Drawing.Point(236, 6);
            this.lblCllose.Name = "lblCllose";
            this.lblCllose.Size = new System.Drawing.Size(26, 25);
            this.lblCllose.TabIndex = 3;
            this.lblCllose.Text = "X";
            this.lblCllose.Click += new System.EventHandler(this.lblCllose_Click);
            // 
            // BetStrategyCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.lblCllose);
            this.Controls.Add(this.lblYjpc);
            this.Controls.Add(this.lblMoney);
            this.Controls.Add(this.lblExpress);
            this.Name = "BetStrategyCtrl";
            this.Size = new System.Drawing.Size(278, 59);
            this.Click += new System.EventHandler(this.BetStrategyCtrl_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExpress;
        private System.Windows.Forms.Label lblMoney;
        private System.Windows.Forms.Label lblYjpc;
        private System.Windows.Forms.Label lblCllose;
    }
}
