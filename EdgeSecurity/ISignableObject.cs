namespace Edge.EdgeSecurity
{
    public interface ISignableObject
    {
        /// <summary>
        /// Gets data used for signing the object
        /// </summary>
        /// <returns></returns>
        byte[] DataForSigning { get; }

        /// <summary>
        /// Allows for algorithm to get or set the signature
        /// </summary>
        byte[] DataSignature { get; set; }
    }
}