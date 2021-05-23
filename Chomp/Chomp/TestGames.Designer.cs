
namespace Chomp
{
    partial class TestGames
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
            this.gameLabel = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.player1Label = new System.Windows.Forms.Label();
            this.player2Label = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gameLabel
            // 
            this.gameLabel.AutoSize = true;
            this.gameLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gameLabel.Location = new System.Drawing.Point(158, 120);
            this.gameLabel.Name = "gameLabel";
            this.gameLabel.Size = new System.Drawing.Size(509, 32);
            this.gameLabel.TabIndex = 0;
            this.gameLabel.Text = "Trwa gra obliczanie gier (zakończono 10/100)...";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(357, 84);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // player1Label
            // 
            this.player1Label.AutoSize = true;
            this.player1Label.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.player1Label.Location = new System.Drawing.Point(282, 152);
            this.player1Label.Name = "player1Label";
            this.player1Label.Size = new System.Drawing.Size(237, 32);
            this.player1Label.TabIndex = 2;
            this.player1Label.Text = "Gracz 1 wygrał 5 gier";
            // 
            // player2Label
            // 
            this.player2Label.AutoSize = true;
            this.player2Label.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.player2Label.Location = new System.Drawing.Point(282, 196);
            this.player2Label.Name = "player2Label";
            this.player2Label.Size = new System.Drawing.Size(237, 32);
            this.player2Label.TabIndex = 3;
            this.player2Label.Text = "Gracz 2 wygrał 5 gier";
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(362, 263);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 4;
            this.closeButton.Text = "Zamknij";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // TestGames
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.player2Label);
            this.Controls.Add(this.player1Label);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.gameLabel);
            this.Name = "TestGames";
            this.Text = "Tryb testowy";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label gameLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label player1Label;
        private System.Windows.Forms.Label player2Label;
        private System.Windows.Forms.Button closeButton;
    }
}