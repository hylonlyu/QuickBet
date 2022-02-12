namespace EatZD
{
    partial class HeadfoodCtrl
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
            this.lblHorse = new System.Windows.Forms.Label();
            this.chkHead = new System.Windows.Forms.CheckBox();
            this.chkFoot = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblHorse
            // 
            this.lblHorse.AutoSize = true;
            this.lblHorse.Location = new System.Drawing.Point(1, 0);
            this.lblHorse.Name = "lblHorse";
            this.lblHorse.Size = new System.Drawing.Size(11, 12);
            this.lblHorse.TabIndex = 0;
            this.lblHorse.Text = "1";
            // 
            // chkHead
            // 
            this.chkHead.AutoSize = true;
            this.chkHead.Location = new System.Drawing.Point(0, 15);
            this.chkHead.Name = "chkHead";
            this.chkHead.Size = new System.Drawing.Size(15, 14);
            this.chkHead.TabIndex = 1;
            this.chkHead.UseVisualStyleBackColor = true;
            this.chkHead.CheckedChanged += new System.EventHandler(this.chkHead_CheckedChanged);
            // 
            // chkFoot
            // 
            this.chkFoot.AutoSize = true;
            this.chkFoot.Location = new System.Drawing.Point(0, 38);
            this.chkFoot.Name = "chkFoot";
            this.chkFoot.Size = new System.Drawing.Size(15, 14);
            this.chkFoot.TabIndex = 2;
            this.chkFoot.UseVisualStyleBackColor = true;
            this.chkFoot.CheckedChanged += new System.EventHandler(this.chkFoot_CheckedChanged);
            // 
            // HeadfoodCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkFoot);
            this.Controls.Add(this.chkHead);
            this.Controls.Add(this.lblHorse);
            this.Name = "HeadfoodCtrl";
            this.Size = new System.Drawing.Size(15, 58);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHorse;
        private System.Windows.Forms.CheckBox chkHead;
        private System.Windows.Forms.CheckBox chkFoot;
    }
}
