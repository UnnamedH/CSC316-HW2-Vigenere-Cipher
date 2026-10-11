# Vigenère Cipher with Row and Column Permutations

CSC316 — Homework #2 · hkelmayan (20231264)

A Windows Forms application (C#, .NET 8) that encrypts and decrypts text with a Vigenère
substitution followed by a column permutation and a row permutation, chained over as many rounds as
you choose. Every intermediate result is shown, and the ciphertext can be copied straight back into
the program to get the original letters back.

![Encryption](assets/encrypt.png)

## How to compile and run

**Visual Studio 2022** (with the *.NET desktop development* workload)

1. Open `CSC316-HW#2.sln`.
2. Press **F5**.

**Command line** (Windows, .NET 8 SDK)

```
dotnet run
```

## Using the program

1. Choose **Encrypt** or **Decrypt**.
2. Type the plaintext, or paste the ciphertext.
3. Enter the **secret key** (letters only), the **column order**, the **row order** and the number of
   **rounds**. Leave **Case-sensitive mode** unticked for the classic cipher from the lecture, or tick
   it to keep capitals and small letters apart (see *Two modes* below).
4. Press **Encrypt** / **Decrypt**. The output box shows the result and the *Intermediate results* box
   lists every step.
5. **Copy output** puts the result on the clipboard. Switch to Decrypt, paste it in and press
   Decrypt with the same settings, including the same mode.

**Self test** checks the Vigenère step against the textbook and lecture examples and the column step
against the two read-outs from the lecture slides. It then runs six encrypt → decrypt cases in each
mode and reports PASS or FAIL for each one.

## Files

| File | Contents |
|---|---|
| `VigenereEngine.cs` | The whole cipher: substitution, permutations, validation, self test |
| `Form1.cs` | Button handlers and formatting of the step list |
| `Form1.Designer.cs` | The window layout |
| `Program.cs` | Entry point |
| `assets/` | Screenshots of the program running |

## How the cipher works

**Two modes.** A tick box switches between them. Both remove spaces, digits and punctuation
before encrypting, as in the examples shown in class.

| | Classic (default) | Case-sensitive |
|---|---|---|
| Alphabet | `A`–`Z`, numbered 0 to 25 | `A`–`Z` then `a`–`z`, numbered 0 to 51 |
| Small letters | turned into capitals | kept, as separate letters |
| Modulus *N* | 26 | 52 |
| Key | read in capitals, so `orange` = `ORANGE` | case matters, so `orange` ≠ `ORANGE` |
| `AttackAtDawn` + key `Orange` | `OKTNIOOKDNCR` (as in the lecture) | `OkTNIOOkdNcR` |

**Substitution.** Standard Vigenère over the selected alphabet. Letter *i* of the text is shifted by
the number of letter *i mod key length* of the key, so the key repeats along the text:

```
encrypt : C[i] = ( P[i] + K[i mod m] ) mod N
decrypt : P[i] = ( C[i] - K[i mod m] + N ) mod N        N = 26 (classic) or 52 (case-sensitive)
```

**Grid.** The column order sets the number of columns: `3 1 4 2` gives 4 columns. The text is
written into the grid row by row, so the number of rows depends on the length of the message. The
last row may be shorter. It is not padded, so the ciphertext always has exactly as many letters as
the plaintext, in keeping with Shannon's principle that the enciphered text should be no larger than
the original message.

**Column permutation.** This follows the single permutation from the lecture. After the text is
written into the grid row by row, it is read out down the columns, taking the columns in the order
given. With the order `3 4 1 5 2`, column 3 is read first, then column 4, 1, 5 and 2:

```
  1   2   3   4   5
  W   E   A   R   E
  D   I   S   C   O        order 1 2 3 4 5 :  WDVFT EIELO ASREN RCEEC EODAE
  V   E   R   E   D        order 3 4 1 5 2 :  ASREN RCEEC WDVFT EODAE EIELO
  F   L   E   E   A
  T   O   N   C   E
```

**Row permutation.** The second permutation of the double permutation from the lecture. The result
of the column step is written row by row into the same number of columns again, and the rows are
reordered in groups. The row order `2 3 1` makes groups of 3 rows, and the new group is old row 2,
then 3, then 1. The grid is then read row by row.

**Rounds.** One round is substitution → column permutation → row permutation. With the round count
set to 2, that sequence runs twice on the result of the first round.

## How the permutations are structured and reversed

**Columns.** Encryption walks the columns in key order and copies each column top to bottom into the
output. Decryption walks the same cells in the same order and writes each letter back into its cell.
Both directions are the same loop with the source and target swapped:

```csharp
foreach (int column in settings.ColumnOrder)
    for (int cell = column - 1; cell < text.Length; cell += cols)
    {
        if (invert) result[cell] = text[next];   // decrypt: put the letter back in its cell
        else        result[next] = text[cell];   // encrypt: read the column out
        next++;
    }
```

When the last row is short, the first columns are one letter longer than the others. The decryptor
knows this from the length of the text, so it cuts the ciphertext back into columns of the right
lengths without needing any padding.

**Rows.** `PermuteRows` moves whole rows inside each group and undoes them the same way, with the
source and target row swapped. In the last group, which may have fewer rows, only its complete rows
are reordered, using the row order restricted to the rows that exist (`2 3 1` on 2 rows becomes
`2 1`). A short last row keeps its place.

The shape of the grid depends only on the number of letters and the two orders, so the decryptor
rebuilds exactly the same grid and every move is undone exactly. Decryption runs the rounds
backwards, and inside each round it undoes the three steps in the opposite order:

```
encryption : substitute -> read columns   -> permute rows        (round 1, round 2, ...)
decryption : undo rows  -> refill columns -> undo substitution   (..., round 2, round 1)
```

## Ciphertext format

The ciphertext is just the encrypted letters, with the same length as the letters of the plaintext.
In classic mode it is all capitals. It contains letters only, so copying and pasting it cannot break
it. Any spaces or line breaks picked up while copying are ignored, and in classic mode a ciphertext
pasted in small letters still decrypts correctly.

## Input rules

- **Key**: letters only, at least one.
- **Column order / row order**: every number from 1 to *n* exactly once, separated by spaces or
  commas, with at least two numbers each.
- **Rounds**: 1 to 10.
- **Mode**: the same mode must be used to encrypt and to decrypt.
- **Text**: anything. Only the letters are encrypted.

Invalid input is reported in red under the buttons, for example:

| Input | Message |
|---|---|
| column order `1 2 2` | The column order repeats the number 2. |
| column order `1 2 4` | The column order must use every number from 1 to 3 exactly once. |
| column order `x 1` | "x" in the column order is not a number. |
| key `ab1` | The key may only contain letters, so '1' cannot be used. |

## Test cases

### 1. The lecture example

Classic mode, key `A` (a shift of 0, so the substitution changes nothing), column order `3 4 1 5 2`, row order
`1 2` (rows unchanged), 1 round. This isolates the column step, and the result matches the lecture
slide exactly:

```
Mode
    Classic: 26 letters A-Z, Vigenere mod 26 (small letters become capitals)
Letters only, in capitals, everything else removed (25 letters)
    WEAREDISCOVEREDFLEEATONCE
Round 1 - written into 5 columns, row by row
      1   2   3   4   5
      W   E   A   R   E
      D   I   S   C   O
      V   E   R   E   D
      F   L   E   E   A
      T   O   N   C   E
Round 1 - columns read top to bottom in the order [3 4 1 5 2]
    ASREN RCEEC WDVFT EODAE EIELO
Ciphertext, 25 letters, the same length as the plaintext letters
    ASRENRCEECWDVFTEODAEEIELO
```

Decrypting `ASRENRCEECWDVFTEODAEEIELO` with the same settings returns `WEAREDISCOVEREDFLEEATONCE`.

### 2. End to end with spaces, a short last row and two rounds (the program's default settings)

Classic mode, key `CipherKey`, column order `3 1 4 2`, row order `2 3 1`, 2 rounds:

```
plaintext  : Meet me at the NDU library at five
letters    : MEETMEATTHENDULIBRARYATFIVE   (27 letters: 4 columns, 7 rows, last row 3 letters)
ciphertext : LEXEURJQRUUQRGGOFIMWCZVJTZL   (27 letters)
decrypted  : MEETMEATTHENDULIBRARYATFIVE
```

Intermediate results:

```
Round 1 - Vigenere substitution   OMTAQVKXRJMCKYCSFPCZNHXWSZC
Round 1 - written into 4 columns, row by row
      1   2   3   4
      O   M   T   A
      Q   V   K   X
      R   J   M   C
      K   Y   C   S
      F   P   C   Z
      N   H   X   W
      S   Z   C
Round 1 - columns read [3 1 4 2]  TKMCCXC OQRKFNS AXCSZW MVJYPHZ
Round 1 - row permutation         CXCOQRKFTKMCCSZWMVJYNSAXPHZ
Round 2 - Vigenere substitution   EFRVUIUJRMURJWQGQTLGCZEOZLX
Round 2 - columns read [3 1 4 2]  RUUQLEX EURJQCZ VJRGGO FIMWTZL
Round 2 - row permutation         LEXEURJQRUUQRGGOFIMWCZVJTZL
```

Columns 1–3 hold 7 letters and column 4 holds 6, because the last row has only 3 letters.

The same settings in case-sensitive mode keep the case of every letter:

```
letters    : MeetmeattheNDUlibraryatfive
ciphertext : lEXeuRjqRuuQRggOFimWczvjTZl
```

### 3. Every setting is needed

Decrypting the classic ciphertext from test 2 with one setting changed gives scrambled text:

| Changed setting | Result |
|---|---|
| key `WrongKey` | `MDHNISRSCGSQKJAOPMUZQKKKTSP` |
| column order `1 3 4 2` | `UZZRGEGTLMJUVUTILPDKIALFFXU` |
| row order `1 3 2` | `LEZGMENLTNONNJIIGYRINAGBISR` |
| 1 round instead of 2 | `CXCOQRKFTKMCCSZWMVJYNSAXPHZ` |
| case-sensitive mode ticked | `mEetMeAtTHEnduliBRArYAtFive` |

### 4. Self test

The **Self test** button checks:

1. the classic Vigenère step against the textbook vector and the lecture's AttackAtDawn / Orange
   example;
2. the column step against both read-outs shown in the lecture;
3. six encrypt → decrypt cases in each mode, covering several key lengths, grid sizes and round
   counts, mixed case, a single letter, text that fills its grid exactly, short last rows and text
   with spaces.

Each round trip passes only if decryption returns the letters exactly and the ciphertext is no
longer than the plaintext:

```
PASS  classic Vigenere, "ATTACKATDAWN" with key "LEMON"      LXFOPVEFRNHR
PASS  classic Vigenere, "AttackAtDawn" with key "Orange"     OKTNIOOKDNCR
PASS  lecture example, column order [1 2 3 4 5]              WDVFT EIELO ASREN RCEEC EODAE
PASS  lecture example, column order [3 4 1 5 2]              ASREN RCEEC WDVFT EODAE EIELO
PASS  classic, key "LEMON", columns [3 1 4 2], rows [2 1], 1 round(s)
PASS  classic, key "CipherKey", columns [2 3 1], rows [3 1 2], 2 round(s)
PASS  classic, key "CSC", columns [4 2 5 1 3], rows [2 3 1], 3 round(s)
PASS  classic, key "zz", columns [2 1], rows [2 1], 1 round(s)
PASS  classic, key "k", columns [3 1 2], rows [2 1], 1 round(s)
PASS  classic, key "KeyKEY", columns [2 1 3], rows [1 3 2], 2 round(s)
PASS  case-sensitive, key "LEMON", columns [3 1 4 2], rows [2 1], 1 round(s)
  ... the same five other cases in case-sensitive mode ...
16 of 16 tests passed
```
