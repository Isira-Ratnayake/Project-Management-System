namespace ProjectManagementSystem.Repositories
{
    public interface IRepository<T, K>
    {
        public T ReadById(K id);
        public List<T> ReadAll();
        public void Create(T entity, string actionUserId);
        public void Update(T entity, string actionUserId);
        public void Delete(T entity, string actionUserId);
    }
}
