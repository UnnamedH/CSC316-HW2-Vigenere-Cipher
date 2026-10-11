using System.Windows.Forms;

namespace CSC316_HW_2
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            modeGroup = new GroupBox();
            encryptRadio = new RadioButton();
            decryptRadio = new RadioButton();
            caseCheck = new CheckBox();
            inputLabel = new Label();
            hintLabel = new Label();
            inputBox = new TextBox();
            keyLabel = new Label();
            keyBox = new TextBox();
            columnsLabel = new Label();
            columnsBox = new TextBox();
            rowsLabel = new Label();
            rowsBox = new TextBox();
            roundsLabel = new Label();
            roundsBox = new NumericUpDown();
            runBtn = new Button();
            copyBtn = new Button();
            selfTestBtn = new Button();
            clearBtn = new Button();
            statusLabel = new Label();
            outputLabel = new Label();
            outputBox = new TextBox();
            stepsLabel = new Label();
            stepsBox = new TextBox();
            modeGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)roundsBox).BeginInit();
            SuspendLayout();
            //
            // modeGroup
            //
            modeGroup.Controls.Add(encryptRadio);
            modeGroup.Controls.Add(decryptRadio);
            modeGroup.Location = new Point(30, 20);
            modeGroup.Name = "modeGroup";
            modeGroup.Size = new Size(430, 100);
            modeGroup.TabIndex = 0;
            modeGroup.TabStop = false;
            modeGroup.Text = "Mode";
            //
            // encryptRadio
            //
            encryptRadio.Checked = true;
            encryptRadio.Location = new Point(20, 45);
            encryptRadio.Name = "encryptRadio";
            encryptRadio.Size = new Size(170, 36);
            encryptRadio.TabIndex = 0;
            encryptRadio.TabStop = true;
            encryptRadio.Text = "Encrypt";
            encryptRadio.UseVisualStyleBackColor = true;
            encryptRadio.CheckedChanged += Mode_CheckedChanged;
            //
            // decryptRadio
            //
            decryptRadio.Location = new Point(220, 45);
            decryptRadio.Name = "decryptRadio";
            decryptRadio.Size = new Size(170, 36);
            decryptRadio.TabIndex = 1;
            decryptRadio.Text = "Decrypt";
            decryptRadio.UseVisualStyleBackColor = true;
            //
            // caseCheck
            //
            caseCheck.AutoSize = true;
            caseCheck.Location = new Point(500, 65);
            caseCheck.Name = "caseCheck";
            caseCheck.Size = new Size(560, 36);
            caseCheck.TabIndex = 21;
            caseCheck.Text = "Case-sensitive mode (A-Z and a-z, mod 52)";
            caseCheck.UseVisualStyleBackColor = true;
            caseCheck.CheckedChanged += CaseCheck_CheckedChanged;
            //
            // inputLabel
            //
            inputLabel.AutoSize = true;
            inputLabel.Location = new Point(30, 140);
            inputLabel.Name = "inputLabel";
            inputLabel.Size = new Size(120, 32);
            inputLabel.TabIndex = 1;
            inputLabel.Text = "Plaintext";
            // 
            // hintLabel
            // 
            hintLabel.AutoSize = true;
            hintLabel.ForeColor = SystemColors.GrayText;
            hintLabel.Location = new Point(200, 142);
            hintLabel.Name = "hintLabel";
            hintLabel.Size = new Size(700, 32);
            hintLabel.TabIndex = 20;
            hintLabel.Text = "letters only, turned into capitals A-Z (mod 26); spaces, digits and punctuation are removed";
            //
            // inputBox
            //
            inputBox.AcceptsReturn = false;
            inputBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputBox.Font = new System.Drawing.Font("Consolas", 10F);
            inputBox.Location = new Point(30, 180);
            inputBox.Multiline = true;
            inputBox.Name = "inputBox";
            inputBox.ScrollBars = ScrollBars.Vertical;
            inputBox.Size = new Size(1440, 110);
            inputBox.TabIndex = 2;
            //
            // keyLabel
            //
            keyLabel.AutoSize = true;
            keyLabel.Location = new Point(30, 310);
            keyLabel.Name = "keyLabel";
            keyLabel.Size = new Size(230, 32);
            keyLabel.TabIndex = 3;
            keyLabel.Text = "Secret key (letters)";
            //
            // keyBox
            //
            keyBox.Font = new System.Drawing.Font("Consolas", 10F);
            keyBox.Location = new Point(30, 350);
            keyBox.Name = "keyBox";
            keyBox.Size = new Size(560, 39);
            keyBox.TabIndex = 4;
            keyBox.Text = "CipherKey";
            //
            // columnsLabel
            //
            columnsLabel.AutoSize = true;
            columnsLabel.Location = new Point(620, 310);
            columnsLabel.Name = "columnsLabel";
            columnsLabel.Size = new Size(170, 32);
            columnsLabel.TabIndex = 5;
            columnsLabel.Text = "Column order";
            //
            // columnsBox
            //
            columnsBox.Font = new System.Drawing.Font("Consolas", 10F);
            columnsBox.Location = new Point(620, 350);
            columnsBox.Name = "columnsBox";
            columnsBox.Size = new Size(400, 39);
            columnsBox.TabIndex = 6;
            columnsBox.Text = "3 1 4 2";
            //
            // rowsLabel
            //
            rowsLabel.AutoSize = true;
            rowsLabel.Location = new Point(1050, 310);
            rowsLabel.Name = "rowsLabel";
            rowsLabel.Size = new Size(130, 32);
            rowsLabel.TabIndex = 7;
            rowsLabel.Text = "Row order";
            //
            // rowsBox
            //
            rowsBox.Font = new System.Drawing.Font("Consolas", 10F);
            rowsBox.Location = new Point(1050, 350);
            rowsBox.Name = "rowsBox";
            rowsBox.Size = new Size(420, 39);
            rowsBox.TabIndex = 8;
            rowsBox.Text = "2 3 1";
            //
            // roundsLabel
            //
            roundsLabel.AutoSize = true;
            roundsLabel.Location = new Point(30, 415);
            roundsLabel.Name = "roundsLabel";
            roundsLabel.Size = new Size(100, 32);
            roundsLabel.TabIndex = 9;
            roundsLabel.Text = "Rounds";
            //
            // roundsBox
            //
            roundsBox.Location = new Point(170, 410);
            roundsBox.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            roundsBox.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            roundsBox.Name = "roundsBox";
            roundsBox.Size = new Size(100, 39);
            roundsBox.TabIndex = 10;
            roundsBox.Value = new decimal(new int[] { 2, 0, 0, 0 });
            //
            // runBtn
            //
            runBtn.Location = new Point(320, 405);
            runBtn.Name = "runBtn";
            runBtn.Size = new Size(220, 50);
            runBtn.TabIndex = 11;
            runBtn.Text = "Encrypt";
            runBtn.UseVisualStyleBackColor = true;
            runBtn.Click += RunBtn_Click;
            //
            // copyBtn
            //
            copyBtn.Location = new Point(560, 405);
            copyBtn.Name = "copyBtn";
            copyBtn.Size = new Size(240, 50);
            copyBtn.TabIndex = 12;
            copyBtn.Text = "Copy output";
            copyBtn.UseVisualStyleBackColor = true;
            copyBtn.Click += CopyBtn_Click;
            //
            // selfTestBtn
            //
            selfTestBtn.Location = new Point(820, 405);
            selfTestBtn.Name = "selfTestBtn";
            selfTestBtn.Size = new Size(200, 50);
            selfTestBtn.TabIndex = 13;
            selfTestBtn.Text = "Self test";
            selfTestBtn.UseVisualStyleBackColor = true;
            selfTestBtn.Click += SelfTestBtn_Click;
            //
            // clearBtn
            //
            clearBtn.Location = new Point(1040, 405);
            clearBtn.Name = "clearBtn";
            clearBtn.Size = new Size(160, 50);
            clearBtn.TabIndex = 14;
            clearBtn.Text = "Clear";
            clearBtn.UseVisualStyleBackColor = true;
            clearBtn.Click += ClearBtn_Click;
            //
            // statusLabel
            //
            statusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            statusLabel.Location = new Point(30, 475);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(1440, 40);
            statusLabel.TabIndex = 15;
            //
            // outputLabel
            //
            outputLabel.AutoSize = true;
            outputLabel.Location = new Point(30, 525);
            outputLabel.Name = "outputLabel";
            outputLabel.Size = new Size(100, 32);
            outputLabel.TabIndex = 16;
            outputLabel.Text = "Output";
            //
            // outputBox
            //
            outputBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            outputBox.BackColor = Color.White;
            outputBox.Font = new System.Drawing.Font("Consolas", 10F);
            outputBox.Location = new Point(30, 565);
            outputBox.Multiline = true;
            outputBox.Name = "outputBox";
            outputBox.ReadOnly = true;
            outputBox.ScrollBars = ScrollBars.Vertical;
            outputBox.Size = new Size(1440, 110);
            outputBox.TabIndex = 17;
            //
            // stepsLabel
            //
            stepsLabel.AutoSize = true;
            stepsLabel.Location = new Point(30, 695);
            stepsLabel.Name = "stepsLabel";
            stepsLabel.Size = new Size(230, 32);
            stepsLabel.TabIndex = 18;
            stepsLabel.Text = "Intermediate results";
            //
            // stepsBox
            //
            stepsBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            stepsBox.BackColor = Color.White;
            stepsBox.Font = new System.Drawing.Font("Consolas", 9F);
            stepsBox.Location = new Point(30, 735);
            stepsBox.Multiline = true;
            stepsBox.Name = "stepsBox";
            stepsBox.ReadOnly = true;
            stepsBox.ScrollBars = ScrollBars.Both;
            stepsBox.Size = new Size(1440, 290);
            stepsBox.TabIndex = 19;
            stepsBox.WordWrap = false;
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1500, 1060);
            Controls.Add(stepsBox);
            Controls.Add(stepsLabel);
            Controls.Add(outputBox);
            Controls.Add(outputLabel);
            Controls.Add(statusLabel);
            Controls.Add(clearBtn);
            Controls.Add(selfTestBtn);
            Controls.Add(copyBtn);
            Controls.Add(runBtn);
            Controls.Add(roundsBox);
            Controls.Add(roundsLabel);
            Controls.Add(rowsBox);
            Controls.Add(rowsLabel);
            Controls.Add(columnsBox);
            Controls.Add(columnsLabel);
            Controls.Add(keyBox);
            Controls.Add(keyLabel);
            Controls.Add(inputBox);
            Controls.Add(hintLabel);
            Controls.Add(inputLabel);
            Controls.Add(caseCheck);
            Controls.Add(modeGroup);
            MinimumSize = new Size(1000, 800);
            Name = "Form1";
            Text = "Vigenere Cipher with Row and Column Permutations";
            modeGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)roundsBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox modeGroup;
        private RadioButton encryptRadio;
        private RadioButton decryptRadio;
        private CheckBox caseCheck;
        private Label inputLabel;
        private Label hintLabel;
        private TextBox inputBox;
        private Label keyLabel;
        private TextBox keyBox;
        private Label columnsLabel;
        private TextBox columnsBox;
        private Label rowsLabel;
        private TextBox rowsBox;
        private Label roundsLabel;
        private NumericUpDown roundsBox;
        private Button runBtn;
        private Button copyBtn;
        private Button selfTestBtn;
        private Button clearBtn;
        private Label statusLabel;
        private Label outputLabel;
        private TextBox outputBox;
        private Label stepsLabel;
        private TextBox stepsBox;
    }
}