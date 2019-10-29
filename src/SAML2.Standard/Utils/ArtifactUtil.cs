using System;
using System.Security.Cryptography;
using System.Text;

namespace SAML2.Utils
{
    /// <summary>
    /// Contains functions to generate and parse artifacts, as defined in "Bindings for the OASIS 
    /// Security Assertion Markup Language (SAML) v. 2.0" specification.
    /// </summary>
    public class ArtifactUtil
    {
        /// <summary>
        /// Argument length error format.
        /// </summary>
        private const string ArgumentLengthErrorFmt = "Unexpected length of byte[] parameter: {0}. Should be {1}";

        /// <summary>
        /// Artifact length.
        /// </summary>
        private const int ArtifactLength = 44;

        /// <summary>
        /// Length of message handle
        /// </summary>
        private const int MessageHandleLength = 20;

        /// <summary>
        /// Length of source id
        /// </summary>
        private const int SourceIdLength = 20;

        /// <summary>
        /// Creates the artifact.
        /// </summary>
        /// <param name="typeCodeValue">The type code value.</param>
        /// <param name="endpointIndexValue">The endpoint index value.</param>
        /// <param name="sourceIdHash">The source id hash.</param>
        /// <param name="messageHandle">The message handle.</param>
        /// <returns>A Base64 encoded string containing the artifact</returns>
        public static string CreateArtifact(short typeCodeValue, short endpointIndexValue, byte[] sourceIdHash, byte[] messageHandle)
        {
            if (sourceIdHash.Length != SourceIdLength)
            {
                throw new ArgumentException(string.Format(ArgumentLengthErrorFmt, sourceIdHash.Length, SourceIdLength), "sourceIdHash");
            }

            if (messageHandle.Length != MessageHandleLength)
            {
                throw new ArgumentException(string.Format(ArgumentLengthErrorFmt, messageHandle.Length, MessageHandleLength), "messageHandle");
            }

            var typeCode = new byte[2];
            typeCode[0] = (byte)(typeCodeValue >> 8);
            typeCode[1] = (byte)typeCodeValue;
            
            var endpointIndex = new byte[2];
            endpointIndex[0] = (byte)(endpointIndexValue >> 8);
            endpointIndex[1] = (byte)endpointIndexValue;

            var result = new byte[2 + 2 + SourceIdLength + MessageHandleLength];

            typeCode.CopyTo(result, 0);
            endpointIndex.CopyTo(result, 2);
            sourceIdHash.CopyTo(result, 4);
            messageHandle.CopyTo(result, 4 + SourceIdLength);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Generates the message handle.
        /// </summary>
        /// <returns>The message handle.</returns>
        public static byte[] GenerateMessageHandle()
        {
            var rng = RandomNumberGenerator.Create();

            var messageHandle = new byte[MessageHandleLength];
            rng.GetNonZeroBytes(messageHandle);

            return messageHandle;
        }

        /// <summary>
        /// Generates the source id hash.
        /// </summary>
        /// <param name="sourceIdUrl">The source id URL.</param>
        /// <returns>The source id hash.</returns>
        public static byte[] GenerateSourceIdHash(string sourceIdUrl)
        {
            var sha = SHA1Managed.Create();
            var sourceId = sha.ComputeHash(Encoding.ASCII.GetBytes(sourceIdUrl));

            return sourceId;
        }

        /// <summary>
        /// Parses the artifact.
        /// </summary>
        /// <param name="artifact">The artifact.</param>
        /// <param name="typeCodeValue">The type code value.</param>
        /// <param name="endpointIndex">Index of the endpoint.</param>
        /// <param name="sourceIdHash">The source id hash.</param>
        /// <param name="messageHandle">The message handle.</param>
        public static void ParseArtifact(string artifact, ref short typeCodeValue, ref short endpointIndex, ref byte[] sourceIdHash, ref byte[] messageHandle)
        {
            if (sourceIdHash.Length != SourceIdLength)
            {
                throw new ArgumentException(string.Format(ArgumentLengthErrorFmt, sourceIdHash.Length, SourceIdLength), "sourceIdHash");
            }

            if (messageHandle.Length != MessageHandleLength)
            {
                throw new ArgumentException(string.Format(ArgumentLengthErrorFmt, messageHandle.Length, MessageHandleLength), "messageHandle");
            }

            var bytes = Convert.FromBase64String(artifact);
            if (bytes.Length != ArtifactLength)
            {
                throw new ArgumentException("Unexpected artifact length", "artifact");
            }

            typeCodeValue = (short)(bytes[0] << 8 | bytes[1]);
            endpointIndex = (short)(bytes[2] << 8 | bytes[3]);

            var index = 4;
            for (var i = 0; i < SourceIdLength; i++)
            {
                sourceIdHash[i] = bytes[i + index];
            }

            index += SourceIdLength;
            for (var i = 0; i < MessageHandleLength; i++)
            {
                messageHandle[i] = bytes[i + index];
            }
        }

        /// <summary>
        /// Tries to parse artifact.
        /// </summary>
        /// <param name="artifact">The artifact.</param>
        /// <param name="typeCodeValue">The type code value.</param>
        /// <param name="endpointIndex">Index of the endpoint.</param>
        /// <param name="sourceIdHash">The source id hash.</param>
        /// <param name="messageHandle">The message handle.</param>
        /// <returns>True of parsing was successful, else false.</returns>
        public static bool TryParseArtifact(string artifact, ref short typeCodeValue, ref short endpointIndex, ref byte[] sourceIdHash, ref byte[] messageHandle)
        {
            try
            {
                ParseArtifact(artifact, ref typeCodeValue, ref endpointIndex, ref sourceIdHash, ref messageHandle);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieves the endpoint index from an artifact
        /// </summary>
        /// <param name="artifact">The artifact.</param>
        /// <returns>The endpoint index.</returns>
        public static ushort GetEndpointIndex(string artifact)
        {
            short parsedTypeCode = -1;
            short parsedEndpointIndex = -1;
            var parsedSourceIdHash = new byte[20];
            var parsedMessageHandle = new byte[20];

            if (TryParseArtifact(artifact, ref parsedTypeCode, ref parsedEndpointIndex, ref parsedSourceIdHash, ref parsedMessageHandle))
            {
                return (ushort)parsedEndpointIndex;   
            }

            throw new ArgumentException("Malformed artifact", "artifact");
        }
    }
}
