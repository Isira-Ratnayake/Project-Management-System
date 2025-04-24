namespace ProjectManagementSystem.Repositories
{
    public interface IRepository<T, K>
    {
        public T ReadById(K id);
        public List<T> ReadAll();
        public T Create(T entity);
        public T Update(T entity);
        public T Delete(T entity);
    }
}
