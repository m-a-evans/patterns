using System;
using System.Linq;
using System.Security.Cryptography;

namespace Patterns.Security
{
    /// <summary>
    /// Provides methods to create and validate keys with a password based key derivation function,
    /// based on RFC 2898 (https://www.rfc-editor.org/rfc/rfc2898.txt) using the SHA 256 hash algorithm.
    /// </summary>
    public class Pbkdf2DataHasher
    {
        #region Fields and Constants

        public const int SaltByteSize = 32; // 256 bits
        public const int HashByteSize = 32; // 256 bits
        public const int Pbkdf2Iterations = 80000;
        public const int IterationIndex = 0;
        public const int SaltIndex = 1;
        public const int Pbkdf2Index = 2;
        public const string HashDelimiter = ";";

        #endregion

        #region Constructors
        internal Pbkdf2DataHasher()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Compares a string's resultant hash to a reference hash
        /// </summary>
        /// <param name="data">Data that will be hashed and compared to the referenceHash</param>
        /// <param name="referenceHash">The "known correct" hash</param>
        /// <returns>True if the hashes match exactly</returns>
        public bool ValidateHash(string data, string referenceHash)
        {
            if (string.IsNullOrEmpty(data) || referenceHash == null)
            {
                return false;
            }

            string[] pieces = referenceHash.Split(new string[] { HashDelimiter }, StringSplitOptions.None);
            if (pieces.Length != 3)
            {
                throw new ArgumentException("Reference hash not valid for Pbkdf2DataHasher to use");
            }
            int iterations = int.Parse(pieces[IterationIndex]);
            byte[] salt = Convert.FromBase64String(pieces[SaltIndex]);
            byte[] hashBytes = Convert.FromBase64String(pieces[Pbkdf2Index]);

            bool result = BytesEquals(hashBytes, GetPbkdf2Bytes(data, salt, iterations, hashBytes.Length));

            return result;
        }

        /// <summary>
        /// Creates a hash of a string
        /// </summary>
        /// <param name="data">The data to hash</param>
        /// <returns>A string of the following format: "<# of iterations>;<salt>;<hash>" </returns>
        public string HashString(string data)
        {
            var cryptoProvider = RandomNumberGenerator.Create();
            var salt = new byte[SaltByteSize];
            cryptoProvider.GetBytes(salt);

            byte[] hash = GetPbkdf2Bytes(data, salt, Pbkdf2Iterations, HashByteSize);
            return Pbkdf2Iterations + HashDelimiter +
                Convert.ToBase64String(salt) + HashDelimiter +
                Convert.ToBase64String(hash);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Compares two arrays of bytes for equality
        /// </summary>
        /// <param name="bytes1">First array of bytes</param>
        /// <param name="bytes2">Second array of bytes</param>
        /// <returns>True if their bytes match exactly</returns>
        private bool BytesEquals(byte[] bytes1, byte[] bytes2)
        {
            return Enumerable.SequenceEqual(bytes1, bytes2);
        }

        /// <summary>
        /// Gets the PBKDF2 key (as an array of bytes) of a password.
        /// </summary>
        /// <param name="password">The password to get the key from</param>
        /// <param name="salt">The salt used to randomize the result. This value should be cryptographically (strongly) random</param>
        /// <param name="iterations">The number of iterations to perform the algorithm</param>
        /// <param name="outputBytesLength">The desired number of bytes of the resultant key</param>
        /// <returns>Byte array representing the key derived from the password</returns>
        private byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytesLength)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(outputBytesLength);
        }

        #endregion
    }
}
