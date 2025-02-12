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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Load_db_file = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Load_data_button
            // 
            this.Load_data_button.Location = new System.Drawing.Point(3, 78);
            this.Load_data_button.Name = "Load_data_button";
            this.Load_data_button.Size = new System.Drawing.Size(237, 60);
            this.Load_data_button.TabIndex = 2;
            this.Load_data_button.Text = "Добавить данные в базу";
            this.Load_data_button.UseVisualStyleBackColor = true;
            this.Load_data_button.Click += new System.EventHandler(this.Load_data_button_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Load_db_file
            // 
            this.Load_db_file.Location = new System.Drawing.Point(3, 12);
            this.Load_db_file.Name = "Load_db_file";
            this.Load_db_file.Size = new System.Drawing.Size(237, 60);
            this.Load_db_file.TabIndex = 11;
            this.Load_db_file.Text = "Загрузить файл базы данных";
            this.Load_db_file.UseVisualStyleBackColor = true;
            this.Load_db_file.Click += new System.EventHandler(this.Load_db_file_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(245, 150);
            this.Controls.Add(this.Load_db_file);
            this.Controls.Add(this.Load_data_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "SqliteMiniManager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Load_data_button;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button Load_db_file;
    }
}

