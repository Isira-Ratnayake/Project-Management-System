using Microsoft.Data.SqlClient;
using ProjectManagementSystem.Models;

namespace ProjectManagementSystem.Repositories
{
    public class UserGroupRepository : IRepository<UserGroup, string>
    {
        private string? _connection;
        public UserGroupRepository(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        public void Create(UserGroup entity, string actionUserId)
        {
            throw new NotImplementedException();
        }

        public void Delete(UserGroup entity, string actionUserId)
        {
            throw new NotImplementedException();
        }

        public List<UserGroup> ReadAll()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadAllUserGroups";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            List<UserGroup> entities = new List<UserGroup>();
                            while (reader.Read())
                            {
                                entities.Add(new UserGroup(reader.GetString(0), reader.GetString(1)));
                            }
                            return entities;
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Failed to fetch data. Please try again.");
                }
            }
        }

        public UserGroup ReadById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(UserGroup entity, string actionUserId)
        {
            throw new NotImplementedException();
        }
    }
}
