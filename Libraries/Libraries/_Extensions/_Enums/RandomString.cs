﻿namespace KGySoft.Libraries
{
    /// <summary>
    /// Represents a strategy for generating random strings.
    /// </summary>
    public enum RandomString
    {
        /// <summary>
        /// Represents random characters including unpaired surrogates (invalid Unicode characters).
        /// </summary>
        AnyChars,

        /// <summary>
        /// Represents random characters ensuring that the string will not contain invalid Unicode characters.
        /// </summary>
        AnyValidChars,

        /// <summary>
        /// Represents random ASCII non-control characters.
        /// </summary>
        Ascii,

        /// <summary>
        /// Represents random digit characters.
        /// </summary>
        Digits,

        /// <summary>
        /// Represents random digit characters ensuring that the first character is not zero.
        /// </summary>
        DigitsNoLeadingZeros,

        /// <summary>
        /// Represents random English letters.
        /// </summary>
        Letters,

        /// <summary>
        /// Represents random English letters and digit characters.
        /// </summary>
        LettersAndDigits,

        /// <summary>
        /// Represents random English uppercase letters.
        /// </summary>
        UpperCaseLetters,

        /// <summary>
        /// Represents random English lowercase letters.
        /// </summary>
        LowerCaseLetters,

        /// <summary>
        /// Represents random English title case letters.
        /// </summary>
        TitleCaseLetters,

        /// <summary>
        /// Represents random word-like English characters in uppercase.
        /// </summary>
        UpperCaseWord,

        /// <summary>
        /// Represents random word-like English characters in lowercase.
        /// </summary>
        LowerCaseWord,

        /// <summary>
        /// Represents random word-like English characters in title case.
        /// </summary>
        TitleCaseWord,

        /// <summary>
        /// Represents random word-like sequences with uppercase first letter and sentence end mark.
        /// </summary>
        Sentence
    }
}