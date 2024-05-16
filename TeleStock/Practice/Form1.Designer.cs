namespace Practice
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            startButton = new Button();
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // startButton
            // 
            startButton.Location = new Point(0, 0);
            startButton.Name = "startButton";
            startButton.Size = new Size(94, 29);
            startButton.TabIndex = 0;
            startButton.Text = "start";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += Start_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(454, 411);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(334, 27);
            textBox1.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox1);
            Controls.Add(startButton);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button startButton;
        private TextBox textBox1;
    }
}
