namespace project1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load_data_button = new System.Windows.Forms.Button();
            this.Send_data_to_database = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Load_db_file = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Load_data_button
            // 
            this.Load_data_button.Location = new System.Drawing.Point(89, 12);
            this.Load_data_button.Name = "Load_data_button";
            this.Load_data_button.Size = new System.Drawing.Size(71, 228);
            this.Load_data_button.TabIndex = 2;
            this.Load_data_button.Text = "Load_data_button";
            this.Load_data_button.UseVisualStyleBackColor = true;
            this.Load_data_button.Click += new System.EventHandler(this.Load_data_button_Click);
            // 
            // Send_data_to_database
            // 
            this.Send_data_to_database.Location = new System.Drawing.Point(166, 12);
            this.Send_data_to_database.Name = "Send_data_to_database";
            this.Send_data_to_database.Size = new System.Drawing.Size(71, 228);
            this.Send_data_to_database.TabIndex = 4;
            this.Send_data_to_database.Text = "Send_data_to_database";
            this.Send_data_to_database.UseVisualStyleBackColor = true;
            this.Send_data_to_database.Click += new System.EventHandler(this.Send_data_to_database_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Load_db_file
            // 
            this.Load_db_file.Location = new System.Drawing.Point(12, 12);
            this.Load_db_file.Name = "Load_db_file";
            this.Load_db_file.Size = new System.Drawing.Size(71, 228);
            this.Load_db_file.TabIndex = 11;
            this.Load_db_file.Text = "Load_db_file";
            this.Load_db_file.UseVisualStyleBackColor = true;
            this.Load_db_file.Click += new System.EventHandler(this.Load_db_file_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 259);
            this.Controls.Add(this.Load_db_file);
            this.Controls.Add(this.Send_data_to_database);
            this.Controls.Add(this.Load_data_button);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Load_data_button;
        private System.Windows.Forms.Button Send_data_to_database;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button Load_db_file;
    }
}

