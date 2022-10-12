namespace psktest;

partial class FormEncode
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
            this.rtbMessage = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rtbSymbols = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.formsPlot1 = new ScottPlot.FormsPlot();
            this.btnPlay = new System.Windows.Forms.Button();
            this.nudFrequency = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.cbBaudRate = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrequency)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbMessage
            // 
            this.rtbMessage.Location = new System.Drawing.Point(12, 27);
            this.rtbMessage.Name = "rtbMessage";
            this.rtbMessage.Size = new System.Drawing.Size(245, 86);
            this.rtbMessage.TabIndex = 0;
            this.rtbMessage.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Message:";
            // 
            // rtbSymbols
            // 
            this.rtbSymbols.Location = new System.Drawing.Point(263, 27);
            this.rtbSymbols.Name = "rtbSymbols";
            this.rtbSymbols.ReadOnly = true;
            this.rtbSymbols.Size = new System.Drawing.Size(245, 86);
            this.rtbSymbols.TabIndex = 2;
            this.rtbSymbols.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(263, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Symbols";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(514, 74);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 39);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // formsPlot1
            // 
            this.formsPlot1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.formsPlot1.Location = new System.Drawing.Point(12, 119);
            this.formsPlot1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.formsPlot1.Name = "formsPlot1";
            this.formsPlot1.Size = new System.Drawing.Size(775, 445);
            this.formsPlot1.TabIndex = 5;
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(619, 74);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 39);
            this.btnPlay.TabIndex = 6;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // nudFrequency
            // 
            this.nudFrequency.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudFrequency.Location = new System.Drawing.Point(514, 28);
            this.nudFrequency.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.nudFrequency.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudFrequency.Name = "nudFrequency";
            this.nudFrequency.Size = new System.Drawing.Size(75, 23);
            this.nudFrequency.TabIndex = 7;
            this.nudFrequency.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(514, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Frequency (Hz)";
            // 
            // cbBaudRate
            // 
            this.cbBaudRate.FormattingEnabled = true;
            this.cbBaudRate.Items.AddRange(new object[] {
            "31.25",
            "62.5",
            "125",
            "250"});
            this.cbBaudRate.Location = new System.Drawing.Point(619, 28);
            this.cbBaudRate.Name = "cbBaudRate";
            this.cbBaudRate.Size = new System.Drawing.Size(75, 23);
            this.cbBaudRate.TabIndex = 9;
            this.cbBaudRate.Text = "31.25";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(619, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Baud Rate";
            // 
            // FormEncode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 576);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbBaudRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudFrequency);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.formsPlot1);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rtbSymbols);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbMessage);
            this.Name = "FormEncode";
            this.Text = "PSK31 Encoder";
            ((System.ComponentModel.ISupportInitialize)(this.nudFrequency)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private RichTextBox rtbMessage;
    private Label label1;
    private RichTextBox rtbSymbols;
    private Label label2;
    private Button btnUpdate;
    private ScottPlot.FormsPlot formsPlot1;
    private Button btnPlay;
    private NumericUpDown nudFrequency;
    private Label label3;
    private ComboBox cbBaudRate;
    private Label label4;
}