namespace M5x.Store.Tests
{
    public class TypeBase : DomainBase
    {
        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public virtual TypeBase Category { get; set; }

        public string CategoryId { get; set; }
    }
}