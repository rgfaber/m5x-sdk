namespace M5x.Store
{
    public class ViewResponseRow<TKey, TValue>
    {
        public string Id { get; set; }
        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }
}