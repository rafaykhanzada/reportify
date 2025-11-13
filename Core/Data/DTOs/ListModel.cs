namespace Core.Data.DTOs
{
    public class ListModel<T> where T : class
    {
        public ListModel(IEnumerable<T> list, int count)
        {
            List = list;
            Count = count;
        }

        public IEnumerable<T> List { get; set; }
        public int Count { get; set; }
    }
}
