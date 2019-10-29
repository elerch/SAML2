using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// <para>
    /// The DSA KeyValue
    /// DSA keys and the DSA signature algorithm are specified in [DSS]. DSA public key values can have the following fields: 
    /// </para>
    /// <para>
    /// P - a prime modulus meeting the [DSS] requirements 
    /// Q - an integer in the range 2**159 &lt; Q &lt; 2**160 which is a prime divisor of P-1 
    /// G - an integer with certain properties with respect to P and Q 
    /// Y - G**X mod P (where X is part of the private key and not made public) 
    /// J - (P - 1) / Q 
    /// seed - a DSA prime generation seed 
    /// pgenCounter - a DSA prime generation counter 
    /// </para>
    /// <para>
    /// Parameter J is available for inclusion solely for efficiency as it is calculatable from P and Q. 
    /// Parameters seed and pgenCounter are used in the DSA prime number generation algorithm specified in 
    /// [DSS]. As such, they are optional but must either both be present or both be absent. This prime 
    /// generation algorithm is designed to provide assurance that a weak prime is not being used and it yields 
    /// a P and Q value. Parameters P, Q, and G can be public and common to a group of users. They might be 
    /// known from application context. As such, they are optional but P and Q must either both appear or both 
    /// be absent. If all of P, Q, seed, and pgenCounter are present, implementations are not required to check 
    /// if they are consistent and are free to use either P and Q or seed and pgenCounter. All parameters are 
    /// encoded as base64 [MIME] values.
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class DsaKeyValue
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "DSAKeyValue";

        #region Elements

        /// <summary>
        /// Gets or sets the G.
        /// </summary>
        /// <value>The G.</value>
        [XmlElement(DataType = "base64Binary")]
        public byte[] G { get; set; }
        
        /// <summary>
        /// Gets or sets the J.
        /// </summary>
        /// <value>The J.</value>
        [XmlElement(DataType = "base64Binary")]
        public byte[] J { get; set; }

        /// <summary>
        /// Gets or sets the P.
        /// </summary>
        /// <value>The P.</value>
        [XmlElement(DataType = "base64Binary")]
        public byte[] P { get; set; }

        /// <summary>
        /// Gets or sets the pgen counter.
        /// </summary>
        /// <value>The pgen counter.</value>
        [XmlElement("PgenCounter", DataType = "base64Binary")]
        public byte[] PgenCounter { get; set; }

        /// <summary>
        /// Gets or sets the Q.
        /// </summary>
        /// <value>The Q.</value>
        [XmlElement(DataType = "base64Binary")]
        public byte[] Q { get; set; }

        /// <summary>
        /// Gets or sets the seed.
        /// </summary>
        /// <value>The seed.</value>
        [XmlElement("Seed", DataType = "base64Binary")]
        public byte[] Seed { get; set; }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        [XmlElement(DataType = "base64Binary")]
        public byte[] Y { get; set; }

        #endregion
    }
}
