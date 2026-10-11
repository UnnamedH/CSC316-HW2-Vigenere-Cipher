using System.Text;

namespace CSC316_HW_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>Reads the key, the two permutation orders, the round count and the mode from the form.</summary>
        private CipherSettings ReadSettings() => new()
        {
            Key = keyBox.Text,
            ColumnOrder = VigenereEngine.ParseOrder(columnsBox.Text, "column order"),
            RowOrder = VigenereEngine.ParseOrder(rowsBox.Text, "row order"),
            Rounds = (int)roundsBox.Value,
            CaseSensitive = caseCheck.Checked,
        };

        private void RunBtn_Click(object sender, EventArgs e)
        {
            bool encrypting = encryptRadio.Checked;

            try
            {
                CipherSettings settings = ReadSettings();
                List<Step> log = new();

                string result = encrypting
                    ? VigenereEngine.Encrypt(inputBox.Text, settings, log)
                    : VigenereEngine.Decrypt(inputBox.Text, settings, log);

                outputBox.Text = result;
                stepsBox.Text = FormatLog(log);
                ShowStatus(encrypting
                    ? "Encrypted. Use Copy output, then switch to Decrypt and paste it back."
                    : "Decrypted.", success: true);
            }
            catch (ArgumentException error)
            {
                // Every problem with the input (bad key, bad order, unsupported character,
                // damaged ciphertext) arrives here with a message written for the user.
                outputBox.Clear();
                stepsBox.Clear();
                ShowStatus(error.Message, success: false);
            }
        }

        private void CopyBtn_Click(object sender, EventArgs e)
        {
            if (outputBox.Text.Length == 0)
            {
                ShowStatus("There is nothing to copy yet.", success: false);
                return;
            }

            Clipboard.SetText(outputBox.Text);
            ShowStatus("Output copied to the clipboard.", success: true);
        }

        private void SelfTestBtn_Click(object sender, EventArgs e)
        {
            outputBox.Clear();
            stepsBox.Text = VigenereEngine.RunSelfTest();
            ShowStatus("Self test finished. The result of every case is listed below.", success: true);
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            inputBox.Clear();
            outputBox.Clear();
            stepsBox.Clear();
            statusLabel.Text = "";
            inputBox.Focus();
        }

        private void Mode_CheckedChanged(object sender, EventArgs e)
        {
            bool encrypting = encryptRadio.Checked;
            runBtn.Text = encrypting ? "Encrypt" : "Decrypt";
            inputLabel.Text = encrypting ? "Plaintext" : "Ciphertext";
            outputLabel.Text = encrypting ? "Ciphertext" : "Plaintext";
        }

        /// <summary>Switches between classic (A-Z, mod 26) and case-sensitive (A-Z and a-z, mod 52) mode.</summary>
        private void CaseCheck_CheckedChanged(object sender, EventArgs e)
        {
            hintLabel.Text = caseCheck.Checked
                ? "letters A-Z and a-z, case kept (mod 52); spaces, digits and punctuation are removed"
                : "letters only, turned into capitals A-Z (mod 26); spaces, digits and punctuation are removed";
            outputBox.Clear();
            stepsBox.Clear();
            statusLabel.Text = "";
        }

        private void ShowStatus(string message, bool success)
        {
            statusLabel.ForeColor = success ? Color.Green : Color.Red;
            statusLabel.Text = message;
        }

        /// <summary>Lays the step list out as one labelled, indented block per step.</summary>
        private static string FormatLog(List<Step> log)
        {
            StringBuilder text = new();

            foreach (Step step in log)
            {
                text.AppendLine(step.Label);
                foreach (string line in step.Value.Split('\n'))
                    text.AppendLine("    " + line.TrimEnd('\r'));
                text.AppendLine();
            }

            return text.ToString();
        }
    }
}
