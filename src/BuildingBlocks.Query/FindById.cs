namespace BuildingBlocks.Query
{
    public class FindById<T>
    {
        private readonly T _id;

        public FindById(T id)
        {
            _id = id;
        }

        public T Id
        {
            get { return _id; }
        }
    }
}