
using Microsoft.Data.SqlClient;
using ProjectManagementSystem.Models;
using System.Data;
using Task = ProjectManagementSystem.Models.Task;
using TaskStatus = ProjectManagementSystem.Models.TaskStatus;

namespace ProjectManagementSystem.Repositories
{
    public class TaskRepository : IRepository<Task, string>
    {
        private string? _connection;
        public TaskRepository(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        public void Create(Task entity, string actionUserId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "CreateTask";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        if (entity.Users == null || entity.Users.Count == 0) {
                            throw new Exception("Users list null or empty.");
                        }
                        DataTable userIdList = new DataTable("UserIdList");
                        userIdList.Columns.Add("UserId", typeof(string));
                        foreach (User user in entity.Users) {
                            userIdList.Rows.Add(user.UserId);
                        }
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@TaskTitle", System.Data.SqlDbType.VarChar).Value = entity.TaskTitle;
                        sqlCommand.Parameters.Add("@TaskDescription", System.Data.SqlDbType.VarChar).Value = entity.TaskDescription;
                        sqlCommand.Parameters.Add("@StartDate", System.Data.SqlDbType.VarChar).Value = entity.StartDate;
                        sqlCommand.Parameters.Add("@EndDate", System.Data.SqlDbType.VarChar).Value = entity.EndDate;
                        sqlCommand.Parameters.Add("@ProjectId", System.Data.SqlDbType.VarChar).Value = entity.Project.ProjectId;
                        sqlCommand.Parameters.Add("@Users", System.Data.SqlDbType.Structured).Value = userIdList;
                        sqlCommand.Parameters.Add("@ActionUserId", System.Data.SqlDbType.VarChar).Value = actionUserId;
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Task creation failed. Please try again.");
                }
            }
        }

        public void Delete(Task entity, string actionUserId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "DeleteTask";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@TaskId", System.Data.SqlDbType.VarChar).Value = entity.TaskId;
                        sqlCommand.Parameters.Add("@CurrentBatchNo", System.Data.SqlDbType.VarChar).Value = entity.CurrentBatchNo;
                        sqlCommand.Parameters.Add("@ActionUserId", System.Data.SqlDbType.VarChar).Value = actionUserId;
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Task deleting failed. Please try again.");
                }
            }
        }

        public List<Task> ReadAll()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadAllTasks";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            List<Task> entities = new List<Task>();
                            while (reader.Read())
                            {
                                Task task = new Task(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), new TaskStatus(reader.GetString(5), reader.GetString(6)), new Project(reader.GetString(7), string.Empty, string.Empty, string.Empty), reader.GetString(8), reader.GetString(9));
                                task.Users = ReadTaskUsersByTaskId(task.TaskId);
                                entities.Add(task);
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

        public Task ReadById(string id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadTaskById";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@TaskId", System.Data.SqlDbType.VarChar).Value = id;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Task task = new Task(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), new TaskStatus(reader.GetString(5), reader.GetString(6)), new Project(reader.GetString(7), string.Empty, string.Empty, string.Empty), reader.GetString(8), reader.GetString(9));
                                task.Users = ReadTaskUsersByTaskId(task.TaskId);
                                return task;
                            }
                            throw new Exception("Task not found.");
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Failed to fetch data. Please try again.");
                }
            }
        }

        public void Update(Task entity, string actionUserId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                Task existingEntity = ReadById(entity.TaskId);
                try
                {
                    string procedure = "UpdateTask";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        if (entity.Users == null || existingEntity.Users == null || entity.Users.Count == 0 || existingEntity.Users.Count == 0)
                        {
                            throw new Exception("Updating or existing users list null or empty.");
                        }
                        List<string> insertedUserIds = FilterMissingUserIds(entity.Users, existingEntity.Users);
                        List<string> deletedUserIds = FilterMissingUserIds(existingEntity.Users, entity.Users);

                        DataTable insertedUserIdList = new DataTable("InsertedUserIdList");
                        insertedUserIdList.Columns.Add("UserId", typeof(string));
                        foreach (string userId in insertedUserIds)
                        {
                            insertedUserIdList.Rows.Add(userId);
                        }

                        DataTable deletedUserIdList = new DataTable("DeletedUserIdList");
                        deletedUserIdList.Columns.Add("UserId", typeof(string));
                        foreach (string userId in deletedUserIds)
                        {
                            deletedUserIdList.Rows.Add(userId);
                        }

                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@TaskTitle", System.Data.SqlDbType.VarChar).Value = entity.TaskTitle;
                        sqlCommand.Parameters.Add("@TaskDescription", System.Data.SqlDbType.VarChar).Value = entity.TaskDescription;
                        sqlCommand.Parameters.Add("@StartDate", System.Data.SqlDbType.Date).Value = entity.StartDate;
                        sqlCommand.Parameters.Add("@EndDate", System.Data.SqlDbType.Date).Value = entity.EndDate;
                        sqlCommand.Parameters.Add("@TaskStatusId", System.Data.SqlDbType.VarChar).Value = entity.TaskStatus.TaskStatusId;
                        sqlCommand.Parameters.Add("@InsertedUsers", System.Data.SqlDbType.Structured).Value = insertedUserIdList;
                        sqlCommand.Parameters.Add("@DeletedUsers", System.Data.SqlDbType.Structured).Value = deletedUserIdList;
                        sqlCommand.Parameters.Add("@TaskId", System.Data.SqlDbType.VarChar).Value = entity.TaskId;
                        sqlCommand.Parameters.Add("@CurrentBatchNo", System.Data.SqlDbType.VarChar).Value = entity.CurrentBatchNo;
                        sqlCommand.Parameters.Add("@ActionUserId", System.Data.SqlDbType.VarChar).Value = actionUserId;

                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Task creation failed. Please try again.");
                }
            }
        }

        private List<User> ReadTaskUsersByTaskId(string id) {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadUserTasksByTaskId";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@TaskId", System.Data.SqlDbType.VarChar).Value = id;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            List<User> users = new List<User>();
                            if (reader.Read())
                            {
                                users.Add(new User(reader.GetString(1), reader.GetString(2), string.Empty, reader.GetString(3), new UserGroup(string.Empty, string.Empty), reader.GetString(4), reader.GetString(5)));
                            }
                            return users;
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Failed to fetch data. Please try again.");
                }
            }
        }

        // Returns ids in subject array but not in reference array
        private List<string> FilterMissingUserIds(List<User> subject, List<User> reference) {
            List<string> ids = new List<string>();
            bool found = false;
            foreach (User subjectUser in subject) {
                foreach (User referenceUser in reference) {
                    if (subjectUser.UserId.Equals(referenceUser.UserId)) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    ids.Add(subjectUser.UserId);
                }
            }
            return ids;
        }
    }
}
