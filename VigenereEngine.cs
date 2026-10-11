using System.Text;

namespace CSC316_HW_2
{
    /// <summary>
    /// One line of the step-by-step log shown in the user interface.
    /// </summary>
    public record Step(string Label, string Value);

    /// <summary>
    /// All parameters of one encryption or decryption run.
    /// </summary>
    public class CipherSettings
    {
        public string Key { get; set; } = "";
        public int[] ColumnOrder { get; set; } = System.Array.Empty<int>();
        public int[] RowOrder { get; set; } = System.Array.Empty<int>();
        public int Rounds { get; set; } = 1;

        /// <summary>
        /// false (default): classic mode, the 26 capital letters, mod 26, as in the lecture.
        /// true: case-sensitive mode, the 52 letters A-Z and a-z, mod 52.
        /// </summary>
        public bool CaseSensitive { get; set; }

        public string Alphabet => CaseSensitive ? VigenereEngine.CaseSensitiveAlphabet : VigenereEngine.ClassicAlphabet;

        public int Columns => ColumnOrder.Length;
        public int Rows => RowOrder.Length;
        public int BlockSize => Rows * Columns;
    }

    /// <summary>
    /// Vigenere substitution plus row and column permutations, chained over several rounds.
    ///
    /// Alphabet    : two modes, chosen with CipherSettings.CaseSensitive.
    ///               Classic (default): the 26 letters A to Z, numbered 0 to 25, Vigenere mod 26,
    ///               exactly as in the lecture. Small letters are turned into capitals first.
    ///               Case-sensitive: the 52 letters A to Z followed by a to z, numbered 0 to 51,
    ///               Vigenere mod 52, so the case of every letter is kept.
    ///               In both modes spaces, digits and punctuation are removed before encrypting,
    ///               as in the classic cipher.
    /// Columns     : the text is written row by row into a grid with one column per entry of the
    ///               column order, then read out down the columns, in that order (lecture,
    ///               "Permutations - Single").
    /// Rows        : the result is written row by row into the same number of columns again, and
    ///               the rows are reordered in groups of one row per entry of the row order
    ///               (lecture, "Permutation Double").
    /// Round order : substitution -> column permutation -> row permutation.
    /// Decryption  : the same steps in reverse order, each one inverted.
    /// Ciphertext  : exactly as many letters as the plaintext. No padding and no header are added,
    ///               so the ciphertext is never larger than the message (Shannon's fifth principle).
    ///               Everything is a letter, so it can be copied and pasted back safely.
    /// </summary>
    public static class VigenereEngine
    {
        /// <summary>Classic alphabet: A is 0 ... Z is 25.</summary>
        public const string ClassicAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>Case-sensitive alphabet: A is 0 ... Z is 25, a is 26 ... z is 51.</summary>
        public const string CaseSensitiveAlphabet = ClassicAlphabet + "abcdefghijklmnopqrstuvwxyz";

        // ---------- alphabet helpers ----------

        /// <summary>True for the letters A-Z and a-z, the only characters the cipher keeps.</summary>
        public static bool IsLetter(char c) => CaseSensitiveAlphabet.IndexOf(c) >= 0;

        /// <summary>
        /// Drops everything that is not a letter. In classic mode the letters are also turned into
        /// capitals, because the classic alphabet only has the 26 capital letters.
        /// </summary>
        private static string KeepLetters(string text, CipherSettings settings)
        {
            StringBuilder letters = new(text.Length);
            foreach (char c in text)
                if (IsLetter(c)) letters.Append(settings.CaseSensitive ? c : char.ToUpperInvariant(c));
            return letters.ToString();
        }

        // ---------- public API ----------

        /// <summary>Encrypts plaintext and appends every intermediate result to <paramref name="log"/>.</summary>
        public static string Encrypt(string plainText, CipherSettings settings, List<Step> log)
        {
            Validate(settings);

            string letters = KeepLetters(plainText, settings);
            if (letters.Length == 0)
                throw new ArgumentException("Enter some text to encrypt. Only letters are encrypted.");

            log.Add(new Step($"Plaintext ({plainText.Length} characters)", plainText));

            log.Add(new Step("Mode", settings.CaseSensitive
                ? "Case-sensitive: 52 letters A-Z and a-z, Vigenere mod 52"
                : "Classic: 26 letters A-Z, Vigenere mod 26 (small letters become capitals)"));

            if (letters != plainText)
                log.Add(new Step(settings.CaseSensitive
                    ? $"Letters only, everything else removed ({letters.Length} letters)"
                    : $"Letters only, in capitals, everything else removed ({letters.Length} letters)", letters));

            string text = letters;
            log.Add(new Step("Grid size",
                             $"{settings.Columns} columns, {RowCount(text.Length, settings.Columns)} rows " +
                             $"for {text.Length} letters (the last row may be shorter, no padding is added)"));

            for (int round = 1; round <= settings.Rounds; round++)
            {
                text = Substitute(text, settings, encrypt: true);
                log.Add(new Step($"Round {round} - Vigenere substitution with key \"{settings.Key}\"", text));

                log.Add(new Step($"Round {round} - written into {settings.Columns} columns, row by row",
                                 GridPicture(text, settings)));

                text = PermuteColumns(text, settings, invert: false);
                log.Add(new Step($"Round {round} - columns read top to bottom in the order " +
                                 $"{Describe(settings.ColumnOrder)}", Spaced(text, RowCount(text.Length, settings.Columns), settings)));

                text = PermuteRows(text, settings, invert: false);
                log.Add(new Step($"Round {round} - row permutation {Describe(settings.RowOrder)}", text));
            }

            string cipherText = text;
            log.Add(new Step($"Ciphertext, {cipherText.Length} letters, the same length as the plaintext letters " +
                             "(copy this for decryption)", cipherText));
            return cipherText;
        }

        /// <summary>Decrypts a ciphertext produced by <see cref="Encrypt"/>.</summary>
        public static string Decrypt(string cipherText, CipherSettings settings, List<Step> log)
        {
            Validate(settings);

            // Anything that is not a letter is ignored, so a ciphertext that picked up spaces or
            // line breaks while being copied still works.
            string letters = KeepLetters(cipherText, settings);

            if (letters.Length == 0)
                throw new ArgumentException("Enter a ciphertext to decrypt.");

            string text = letters;
            log.Add(new Step($"Ciphertext ({text.Length} letters)", text));

            // The rounds are undone in reverse order, and each step is inverted.
            for (int round = settings.Rounds; round >= 1; round--)
            {
                text = PermuteRows(text, settings, invert: true);
                log.Add(new Step($"Round {round} - row permutation undone {Describe(settings.RowOrder)}", text));

                text = PermuteColumns(text, settings, invert: true);
                log.Add(new Step($"Round {round} - column read-out undone {Describe(settings.ColumnOrder)}", text));

                text = Substitute(text, settings, encrypt: false);
                log.Add(new Step($"Round {round} - Vigenere substitution undone with key \"{settings.Key}\"", text));
            }

            string plainText = text;
            log.Add(new Step("Plaintext", plainText));
            return plainText;
        }

        // ---------- substitution ----------

        /// <summary>
        /// Standard Vigenere substitution: letter i of the text is shifted by the number of letter
        /// (i mod key length) of the key, so the key repeats along the text. The alphabet, and so
        /// the modulus (26 or 52), comes from the selected mode. In classic mode the key is read
        /// in capitals, so "orange" and "ORANGE" are the same key.
        /// </summary>
        private static string Substitute(string text, CipherSettings settings, bool encrypt)
        {
            string alphabet = settings.Alphabet;
            int size = alphabet.Length;
            string key = settings.CaseSensitive ? settings.Key : settings.Key.ToUpperInvariant();
            StringBuilder result = new(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                int shift = alphabet.IndexOf(key[i % key.Length]);
                int value = alphabet.IndexOf(text[i]);
                int shifted = encrypt
                    ? (value + shift) % size
                    : (value - shift + size) % size;
                result.Append(alphabet[shifted]);
            }

            return result.ToString();
        }

        // ---------- permutations ----------
        //
        // Both permutations write the text into a grid row by row, with one column per entry of
        // the column order. Nothing is padded, so the last row may be shorter. The layout depends
        // only on the text length and the orders, so decryption rebuilds the same grid and puts
        // every letter back where it came from.

        /// <summary>
        /// Column permutation, exactly as in the lecture ("Permutations - Single"): the text is
        /// written into the grid row by row, and the ciphertext is read out down the columns, taking
        /// the columns in the order given. With the order 3 4 1 5 2, column 3 is read first, then
        /// column 4, and so on. A short last row simply makes some columns one letter shorter.
        /// Decryption knows how long each column is from the text length, so it cuts the text back
        /// into columns and writes every letter back to its cell.
        /// </summary>
        private static string PermuteColumns(string text, CipherSettings settings, bool invert)
        {
            int cols = settings.Columns;
            char[] result = new char[text.Length];
            int next = 0;

            foreach (int column in settings.ColumnOrder)
            {
                // Cells of this column, top to bottom: column-1, column-1 + cols, ...
                for (int cell = column - 1; cell < text.Length; cell += cols)
                {
                    if (invert) result[cell] = text[next];
                    else result[next] = text[cell];
                    next++;
                }
            }

            return new string(result);
        }

        /// <summary>
        /// Row permutation (lecture, "Permutation Double"): the text is written row by row into the
        /// same number of columns, and the rows are reordered in groups of RowOrder.Length rows:
        /// row i of a group is row RowOrder[i] of the old group, and the grid is read row by row.
        /// In the last group only its complete rows are reordered (with the order restricted to
        /// them), and a final short row stays in place.
        /// </summary>
        private static string PermuteRows(string text, CipherSettings settings, bool invert)
        {
            int cols = settings.Columns, block = settings.BlockSize;
            char[] result = new char[text.Length];

            for (int start = 0; start < text.Length; start += block)
            {
                int length = Math.Min(block, text.Length - start);
                int fullRows = length / cols;
                int[] order = Restrict(settings.RowOrder, fullRows);

                for (int r = 0; r < fullRows; r++)
                {
                    int sourceRow = order[r] - 1;
                    for (int c = 0; c < cols; c++)
                    {
                        int from = start + sourceRow * cols + c;
                        int to = start + r * cols + c;
                        if (invert) result[from] = text[to];
                        else result[to] = text[from];
                    }
                }

                // A short last row (fewer letters than there are columns) is not moved.
                for (int i = start + fullRows * cols; i < start + length; i++)
                    result[i] = text[i];
            }

            return new string(result);
        }

        /// <summary>
        /// Keeps only the entries of an order that are at most <paramref name="size"/>, in the same
        /// sequence. Example: order 3 1 4 2 restricted to 3 positions is 3 1 2. The result is always
        /// a valid order of 1..size, which is what lets the last, partly filled group of rows be
        /// permuted and un-permuted without padding.
        /// </summary>
        private static int[] Restrict(int[] order, int size) =>
            size >= order.Length ? order : order.Where(value => value <= size).ToArray();

        // ---------- validation ----------

        private static void Validate(CipherSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Key))
                throw new ArgumentException("Enter a secret key.");

            foreach (char c in settings.Key)
                if (!IsLetter(c))
                    throw new ArgumentException(
                        $"The key may only contain letters, so '{c}' cannot be used.");

            RequirePermutation(settings.ColumnOrder, "column order");
            RequirePermutation(settings.RowOrder, "row order");

            if (settings.Rounds < 1)
                throw new ArgumentException("The number of rounds must be at least 1.");
        }

        private static void RequirePermutation(int[] order, string what)
        {
            if (order.Length < 2)
                throw new ArgumentException($"The {what} needs at least two positions, for example 2 1.");

            bool[] seen = new bool[order.Length];
            foreach (int value in order)
            {
                if (value < 1 || value > order.Length)
                    throw new ArgumentException(
                        $"The {what} must use every number from 1 to {order.Length} exactly once.");

                if (seen[value - 1])
                    throw new ArgumentException($"The {what} repeats the number {value}.");

                seen[value - 1] = true;
            }
        }

        /// <summary>Reads an order such as "3 1 4 2" (spaces, commas or dashes) into an array.</summary>
        public static int[] ParseOrder(string text, string what)
        {
            string[] pieces = text.Split(new[] { ' ', ',', ';', '-', '\t' },
                                         StringSplitOptions.RemoveEmptyEntries);

            if (pieces.Length == 0)
                throw new ArgumentException($"Enter a {what}, for example 3 1 4 2.");

            int[] order = new int[pieces.Length];
            for (int i = 0; i < pieces.Length; i++)
                if (!int.TryParse(pieces[i], out order[i]))
                    throw new ArgumentException($"\"{pieces[i]}\" in the {what} is not a number.");

            RequirePermutation(order, what);
            return order;
        }

        // ---------- display helpers ----------

        private static string Describe(int[] order) => "[" + string.Join(" ", order) + "]";

        private static int RowCount(int length, int columns) => (length + columns - 1) / columns;

        /// <summary>
        /// Draws the text as the grid it is written into, row by row, with the column numbers on
        /// top, like the grid in the lecture.
        /// </summary>
        private static string GridPicture(string text, CipherSettings settings)
        {
            int cols = settings.Columns;
            StringBuilder picture = new();

            for (int c = 1; c <= cols; c++) picture.Append($" {c,2} ");
            for (int start = 0; start < text.Length; start += cols)
            {
                picture.AppendLine();
                for (int c = 0; c < cols && start + c < text.Length; c++)
                    picture.Append($"  {text[start + c]} ");
            }

            return picture.ToString();
        }

        /// <summary>Shows the column read-out with a space after each column, e.g. ASREN RCEEC WDVFT.</summary>
        private static string Spaced(string text, int rows, CipherSettings settings)
        {
            int cols = settings.Columns, fullCols = text.Length % cols == 0 ? cols : text.Length % cols;
            StringBuilder spaced = new();
            int next = 0;

            foreach (int column in settings.ColumnOrder)
            {
                int height = column <= fullCols ? rows : rows - 1;
                if (spaced.Length > 0) spaced.Append(' ');
                spaced.Append(text, next, height);
                next += height;
            }

            return spaced.ToString();
        }

        // ---------- self test ----------

        /// <summary>
        /// Checks classic Vigenere against the textbook and lecture examples, checks the column step
        /// against the two read-outs from the lecture slides, then encrypts and decrypts a set of
        /// test strings in both modes and reports whether each one came back unchanged.
        /// Used by the Self test button.
        /// </summary>
        public static string RunSelfTest()
        {
            (string text, string key, string columns, string rows, int rounds)[] cases =
            {
                ("ATTACKATDAWN", "LEMON", "3 1 4 2", "2 1", 1),
                ("Meet me at the NDU library at five", "CipherKey", "2 3 1", "3 1 2", 2),
                ("TheQuickBrownFoxJumpsOverTheLazyDog", "CSC", "4 2 5 1 3", "2 3 1", 3),
                ("A", "zz", "2 1", "2 1", 1),
                ("TwelveLetter", "k", "3 1 2", "2 1", 1),
                ("MixedCASEletters", "KeyKEY", "2 1 3", "1 3 2", 2),
            };

            StringBuilder report = new();
            int passed = 0, total = 0;

            // Classic Vigenere against the textbook vector and the lecture's AttackAtDawn / Orange.
            (string text, string key, string expectedCipher)[] vigenere =
            {
                ("ATTACKATDAWN", "LEMON", "LXFOPVEFRNHR"),
                ("AttackAtDawn", "Orange", "OKTNIOOKDNCR"),
            };

            foreach ((string text, string key, string expectedCipher) in vigenere)
            {
                total++;
                CipherSettings classic = new() { Key = key };
                string cipher = Substitute(KeepLetters(text, classic), classic, encrypt: true);
                bool ok = cipher == expectedCipher;
                if (ok) passed++;

                report.AppendLine($"{(ok ? "PASS" : "FAIL")}  classic Vigenere, \"{text}\" with key \"{key}\"");
                report.AppendLine($"      expected   : {expectedCipher}");
                report.AppendLine($"      ciphertext : {cipher}");
                report.AppendLine();
            }

            // The two column read-outs shown in the lecture. Key "A" shifts by 0 and row order
            // "1 2" keeps the rows in place, so only the column step changes the text.
            (string order, string expectedCipher)[] lecture =
            {
                ("1 2 3 4 5", "WDVFTEIELOASRENRCEECEODAE"),
                ("3 4 1 5 2", "ASRENRCEECWDVFTEODAEEIELO"),
            };

            foreach ((string order, string expectedCipher) in lecture)
            {
                total++;
                CipherSettings slide = new()
                {
                    Key = "A",
                    ColumnOrder = ParseOrder(order, "column order"),
                    RowOrder = ParseOrder("1 2", "row order"),
                    Rounds = 1,
                };
                string cipher = Encrypt("WE ARE DISCOVERED FLEE AT ONCE", slide, new List<Step>());
                bool ok = cipher == expectedCipher;
                if (ok) passed++;

                report.AppendLine($"{(ok ? "PASS" : "FAIL")}  lecture example, column order [{order}]");
                report.AppendLine($"      expected   : {Spaced(expectedCipher, 5, slide)}");
                report.AppendLine($"      ciphertext : {Spaced(cipher, 5, slide)}");
                report.AppendLine();
            }

            foreach (bool caseSensitive in new[] { false, true })
            foreach ((string text, string key, string columns, string rows, int rounds) in cases)
            {
                total++;
                string mode = caseSensitive ? "case-sensitive" : "classic";
                try
                {
                    CipherSettings settings = new()
                    {
                        Key = key,
                        ColumnOrder = ParseOrder(columns, "column order"),
                        RowOrder = ParseOrder(rows, "row order"),
                        Rounds = rounds,
                        CaseSensitive = caseSensitive,
                    };

                    List<Step> log = new();
                    string cipher = Encrypt(text, settings, log);
                    string back = Decrypt(cipher, settings, log);
                    string expected = KeepLetters(text, settings);

                    bool ok = back == expected && cipher.Length == expected.Length;
                    if (ok) passed++;

                    report.AppendLine($"{(ok ? "PASS" : "FAIL")}  {mode}, key \"{key}\", columns [{columns}], " +
                                      $"rows [{rows}], {rounds} round(s)");
                    report.AppendLine($"      plaintext  : {text}");
                    report.AppendLine($"      letters    : {expected}");
                    report.AppendLine($"      ciphertext : {cipher}");
                    report.AppendLine($"      decrypted  : {back}");
                    report.AppendLine();
                }
                catch (Exception error)
                {
                    report.AppendLine($"FAIL  {mode}, {text} -> {error.Message}");
                    report.AppendLine();
                }
            }

            report.AppendLine($"{passed} of {total} tests passed (Vigenere and lecture examples matched; every " +
                              "round trip returned its letters exactly, with a ciphertext no longer than the plaintext).");
            return report.ToString();
        }
    }
}