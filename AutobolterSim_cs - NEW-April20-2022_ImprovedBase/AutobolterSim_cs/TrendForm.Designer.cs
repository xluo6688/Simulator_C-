namespace AutobolterSim_cs
{
    partial class TrendForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

       // private System.Windows.Forms.DataVisualization.Charting.Chart trendForm;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TrendTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // TrendTimer
            // 
            this.TrendTimer.Tick += new System.EventHandler(this.TrendTimer_Tick);
            // 
            // TrendForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1413, 605);
            this.Name = "TrendForm";
            this.Text = "TrendForm";
            this.Load += new System.EventHandler(this.TrendForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer TrendTimer;
    }
}