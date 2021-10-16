namespace M5x.Store.Tests
{
    public class ItemBase : DomainBase
    {
        /// <summary>
        ///     Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder { get; set; }


        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public virtual TypeBase Category { get; set; }


        public string CategoryId { get; set; }
    }
}