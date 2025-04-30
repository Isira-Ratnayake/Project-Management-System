using Microsoft.Data.SqlClient;
using ProjectManagementSystem.Models;

namespace ProjectManagementSystem.Repositories
{
    public class UserRepository : IRepository<User, string>
    {
        private string? _connection;
        public UserRepository(IConfiguration configuration) {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }
        public void Create(User entity, string actionUserId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "CreateUser";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@UserEmail", System.Data.SqlDbType.VarChar).Value = entity.UserEmail;
                        sqlCommand.Parameters.Add("@UserPassword", System.Data.SqlDbType.VarChar).Value = entity.UserPassword;
                        sqlCommand.Parameters.Add("@UserFullname", System.Data.SqlDbType.VarChar).Value = entity.UserFullname;
                        sqlCommand.Parameters.Add("@UserGroupId", System.Data.SqlDbType.VarChar).Value = entity.UserGroup.UserGroupId;
                        sqlCommand.Parameters.Add("@ActionUserId", System.Data.SqlDbType.VarChar).Value = actionUserId;
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("User creation failed. Please try again.");
                }
            }
        }

        public void Delete(User entity, string actionUserId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "DeleteUser";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@UserId", System.Data.SqlDbType.VarChar).Value = entity.UserId;
                        sqlCommand.Parameters.Add("@CurrentBatchNo", System.Data.SqlDbType.VarChar).Value = entity.CurrentBatchNo;
                        sqlCommand.Parameters.Add("@ActionUserId", System.Data.SqlDbType.VarChar).Value = actionUserId;
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("User deleting failed. Please try again.");
                }
            }
        }

        public List<User> ReadAll()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadAllUsers";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            List<User> entities = new List<User>();
                            while (reader.Read())
                            {
                                entities.Add(new User(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), new UserGroup(reader.GetString(4), reader.GetString(5)), reader.GetString(6), reader.GetString(7)));
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

        public User ReadById(string id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadUserById";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@UserId", System.Data.SqlDbType.VarChar).Value = id;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader()) {
                            if (reader.Read()) {
                                return new User(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), new UserGroup(reader.GetString(4), reader.GetString(5)), reader.GetString(6), reader.GetString(7));
                            }
                            throw new Exception("User not found.");
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Failed to fetch data. Please try again.");
                }
            }
        }

        public User ReadByEmail(string email)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadUserByEmail";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@UserEmail", System.Data.SqlDbType.VarChar).Value = email;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), new UserGroup(reader.GetString(4), reader.GetString(5)), reader.GetString(6), reader.GetString(7));
                            }
                            throw new Exception("User not found.");
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Failed to fetch data. Please try again.");
                }
            }
        }

        public void Update(User entity, string actionUserId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "UpdateUser";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@UserEmail", System.Data.SqlDbType.VarChar).Value = entity.UserEmail;
                        sqlCommand.Parameters.Add("@UserFullname", System.Data.SqlDbType.VarChar).Value = entity.UserFullname;
                        sqlCommand.Parameters.Add("@UserGroupId", System.Data.SqlDbType.VarChar).Value = entity.UserGroup.UserGroupId;
                        sqlCommand.Parameters.Add("@UserId", System.Data.SqlDbType.VarChar).Value = entity.UserId;
                        sqlCommand.Parameters.Add("@CurrentBatchNo", System.Data.SqlDbType.VarChar).Value = entity.CurrentBatchNo;
                        sqlCommand.Parameters.Add("@ActionUserId", System.Data.SqlDbType.VarChar).Value = actionUserId;
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("User updating failed. Please try again.");
                }
            }
        }

        public Audit<User> ReadAuditByCurrentBatchNo(string currentBatchNo)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadUserShadowByCurrentBatchNo";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@CurrentBatchNo", System.Data.SqlDbType.VarChar).Value = currentBatchNo;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Audit<User>(reader.GetString(7), reader.GetDateTime(8), reader.GetString(9), new User(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), new UserGroup(reader.GetString(4)), reader.GetString(5), reader.GetString(6)));
                            }
                            throw new Exception("Audit not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to fetch data. Please try again.");
                }
            }
        }
    }
}
