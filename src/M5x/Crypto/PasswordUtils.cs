// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-17-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-17-2013
// ***********************************************************************
// <copyright file="PasswordUtils.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Security.Cryptography;

namespace M5x.Crypto
{
    /// <summary>
    ///     This class can generate random passwords, which do not include ambiguous characters,
    ///     such as I, l, and 1. The generated password will be made of 7-bit ASCII symbols.
    ///     Every four characters will include one lower case character, one upper case character,
    ///     one number, and one special symbol (such as '%') in a random order. The password will always
    ///     start with an alpha-numeric character; it will not start with a special symbol (we do
    ///     this because some back-end systems do not like certain special characters in the first position).
    /// </summary>
    public class PasswordUtils
    {
        // Define default min and max password lengths.
        /// <summary>
        ///     The defaul t_ mi n_ passwor d_ length
        /// </summary>
        private static readonly int DefaultMinPasswordLength = 8;

        /// <summary>
        ///     The defaul t_ ma x_ passwor d_ length
        /// </summary>
        private static readonly int DefaultMaxPasswordLength = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        /// <summary>
        ///     The passwor d_ character s_ lcase
        /// </summary>
        private static readonly string PasswordCharsLcase = "abcdefgijkmnopqrstwxyz";

        /// <summary>
        ///     The passwor d_ character s_ ucase
        /// </summary>
        private static readonly string PasswordCharsUcase = "ABCDEFGHJKLMNPQRSTWXYZ";

        /// <summary>
        ///     The passwor d_ character s_ numeric
        /// </summary>
        private static readonly string PasswordCharsNumeric = "23456789";

        /// <summary>
        ///     The passwor d_ character s_ special
        /// </summary>
        private static readonly string PasswordCharsSpecial = "*$-+?_&=!%{}/";


        /// <summary>
        ///     Determines whether the specified password is valid.
        /// </summary>
        /// <param name="encoded">The encoded.</param>
        /// <param name="plainText">The plain text.</param>
        /// <returns><c>true</c> if specified password is valid; otherwise, <c>false</c>.</returns>
        public static bool IsEncodedSameAsPassword(string encoded, string plainText)
        {
            if (string.IsNullOrEmpty(encoded) && string.IsNullOrEmpty(plainText)) return true;
            if (encoded == null) return false;
            if (encoded.Length < 32) return false;
            var md5 = encoded.Substring(encoded.Length - 32); // MD5 is 32 char length.
            var saltHex = encoded.Substring(0, encoded.Length - md5.Length);
            var salt = HexUtils.GetStringFromHex(saltHex);
            return encoded.Equals(saltHex + Md5.Encode(salt + plainText), StringComparison.InvariantCultureIgnoreCase);
        }


        /// <summary>
        ///     Generates a random password.
        /// </summary>
        /// <returns>Randomly generated password.</returns>
        /// <remarks>
        ///     The length of the generated password will be determined at
        ///     random. It will be no shorter than the minimum default and
        ///     no longer than maximum default.
        /// </remarks>
        public static string Generate()
        {
            return Generate(DefaultMinPasswordLength, DefaultMaxPasswordLength, true);
        }

        /// <summary>
        ///     Generates a random password of the exact length.
        /// </summary>
        /// <param name="length">Exact password length.</param>
        /// <returns>Randomly generated password.</returns>
        public static string Generate(int length)
        {
            return Generate(length, length, true);
        }

        /// <summary>
        ///     Generates a random password.
        /// </summary>
        /// <param name="useSpecialsChars">Include specials characters in the generated password</param>
        /// <returns>Randomly generated password.</returns>
        /// <remarks>
        ///     The length of the generated password will be determined at
        ///     random. It will be no shorter than the minimum default and
        ///     no longer than maximum default.
        /// </remarks>
        public static string Generate(bool useSpecialsChars)
        {
            return Generate(DefaultMinPasswordLength, DefaultMaxPasswordLength, useSpecialsChars);
        }

        /// <summary>
        ///     Generates a random password of the exact length.
        /// </summary>
        /// <param name="length">Exact password length.</param>
        /// <param name="useSpecialsChars">Include specials characters in the generated password</param>
        /// <returns>Randomly generated password.</returns>
        public static string Generate(int length, bool useSpecialsChars)
        {
            return Generate(length, length, useSpecialsChars);
        }

        /// <summary>
        ///     Generates a random password.
        /// </summary>
        /// <param name="minLength">Minimum password length.</param>
        /// <param name="maxLength">Maximum password length.</param>
        /// <param name="useSpecialsChars">Include specials characters in the generated password</param>
        /// <returns>Randomly generated password.</returns>
        /// <remarks>
        ///     The length of the generated password will be determined at
        ///     random and it will fall with the range determined by the
        ///     function parameters.
        /// </remarks>
        public static string Generate(int minLength, int maxLength, bool useSpecialsChars)
        {
            char[] password = null; // This array will hold password characters.
            char[][] charGroups; // Create a local array containing supported password characters grouped by types.

            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength) return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            if (useSpecialsChars)
                charGroups = new[]
                {
                    PasswordCharsLcase.ToCharArray(),
                    PasswordCharsUcase.ToCharArray(),
                    PasswordCharsNumeric.ToCharArray(),
                    PasswordCharsSpecial.ToCharArray()
                };
            else
                charGroups = new[]
                {
                    PasswordCharsLcase.ToCharArray(),
                    PasswordCharsUcase.ToCharArray(),
                    PasswordCharsNumeric.ToCharArray()
                };

            // Use this array to track the number of unused characters in each
            // character group.
            var charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (var i = 0; i < charsLeftInGroup.Length; i++) charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            var leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (var i = 0; i < leftGroupsOrder.Length; i++) leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            var randomBytes = new byte[4];

            // Generate 4 random bytes.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            var seed = ((randomBytes[0] & 0x7f) << 24) |
                       (randomBytes[1] << 16) |
                       (randomBytes[2] << 8) |
                       randomBytes[3];

            // Now, this is real randomization.
            var random = new Random(seed);

            // Allocate appropriate memory for the password.
            if (minLength < maxLength)
                password = new char[random.Next(minLength, maxLength + 1)];
            else
                password = new char[minLength];

            // Index of the last non-processed group.
            var lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (var i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                int nextLeftGroupsOrderIdx; // Index which will be used to track not processed character groups.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                var nextGroupIdx =
                    leftGroupsOrder[nextLeftGroupsOrderIdx]; // Index of the next character group to be processed.

                // Get the index of the last unprocessed characters in this group.
                var lastCharIdx =
                    charsLeftInGroup[nextGroupIdx] - 1; // Index of the last non-processed character in a group.

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                int nextCharIdx; // Index of the next character to be added to password.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                {
                    charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                }
                else
                {
                    // There are more unprocessed characters left.
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        var temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] = charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }

                    // Decrement the number of unprocessed characters in this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                {
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                }
                else
                {
                    // There are more unprocessed groups left.
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        var temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] = leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }

                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }
    }
}