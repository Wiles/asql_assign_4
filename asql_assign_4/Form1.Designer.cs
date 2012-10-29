namespace asql_assign_4
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.gb_source = new System.Windows.Forms.GroupBox();
            this.tb_source = new System.Windows.Forms.TextBox();
            this.gb_dest = new System.Windows.Forms.GroupBox();
            this.tb_dest = new System.Windows.Forms.TextBox();
            this.tb_todo = new System.Windows.Forms.TextBox();
            this.gb_source.SuspendLayout();
            this.gb_dest.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gb_source
            // 
            this.gb_source.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_source.Controls.Add(this.tb_source);
            this.gb_source.Location = new System.Drawing.Point(13, 43);
            this.gb_source.Name = "gb_source";
            this.gb_source.Size = new System.Drawing.Size(311, 51);
            this.gb_source.TabIndex = 1;
            this.gb_source.TabStop = false;
            this.gb_source.Text = "Source";
            // 
            // tb_source
            // 
            this.tb_source.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_source.Location = new System.Drawing.Point(7, 20);
            this.tb_source.Name = "tb_source";
            this.tb_source.Size = new System.Drawing.Size(298, 20);
            this.tb_source.TabIndex = 0;
            this.tb_source.Text = "Data Source=SAMUEL-LAPTOP;Initial Catalog=source;Integrated Security=True";
            // 
            // gb_dest
            // 
            this.gb_dest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_dest.Controls.Add(this.tb_dest);
            this.gb_dest.Location = new System.Drawing.Point(13, 100);
            this.gb_dest.Name = "gb_dest";
            this.gb_dest.Size = new System.Drawing.Size(311, 51);
            this.gb_dest.TabIndex = 2;
            this.gb_dest.TabStop = false;
            this.gb_dest.Text = "Destination";
            // 
            // tb_dest
            // 
            this.tb_dest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_dest.Location = new System.Drawing.Point(7, 20);
            this.tb_dest.Name = "tb_dest";
            this.tb_dest.Size = new System.Drawing.Size(298, 20);
            this.tb_dest.TabIndex = 0;
            this.tb_dest.Text = "Data Source=SAMUEL-LAPTOP;Initial Catalog=empty_dest;Integrated Security=True";
            // 
            // tb_todo
            // 
            this.tb_todo.Location = new System.Drawing.Point(13, 158);
            this.tb_todo.Multiline = true;
            this.tb_todo.Name = "tb_todo";
            this.tb_todo.Size = new System.Drawing.Size(311, 344);
            this.tb_todo.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 504);
            this.Controls.Add(this.tb_todo);
            this.Controls.Add(this.gb_dest);
            this.Controls.Add(this.gb_source);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.gb_source.ResumeLayout(false);
            this.gb_source.PerformLayout();
            this.gb_dest.ResumeLayout(false);
            this.gb_dest.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox gb_source;
        private System.Windows.Forms.TextBox tb_source;
        private System.Windows.Forms.GroupBox gb_dest;
        private System.Windows.Forms.TextBox tb_dest;
        private System.Windows.Forms.TextBox tb_todo;
    }
}

