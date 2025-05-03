using Microsoft.Data.SqlClient;
using ProjectManagementSystem.Models;
using Task = ProjectManagementSystem.Models.Task;
using TaskStatus = ProjectManagementSystem.Models.TaskStatus;

namespace ProjectManagementSystem.Repositories
{
    public class ProjectRepository : IRepository<Project, string>
    {
        private string? _connection;
        public ProjectRepository(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        public void Create(Project entity, string actionUserId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "CreateProject";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@ProjectName", System.Data.SqlDbType.VarChar).Value = entity.ProjectName;
                        sqlCommand.Parameters.Add("@ActionUserId", System.Data.SqlDbType.VarChar).Value = actionUserId;
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Project creation failed. Please try again.");
                }
            }
        }

        public void Delete(Project entity, string actionUserId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "DeleteProject";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@ProjectId", System.Data.SqlDbType.VarChar).Value = entity.ProjectId;
                        sqlCommand.Parameters.Add("@CurrentBatchNo", System.Data.SqlDbType.VarChar).Value = entity.CurrentBatchNo;
                        sqlCommand.Parameters.Add("@ActionUserId", System.Data.SqlDbType.VarChar).Value = actionUserId;
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Project deleting failed. Please try again.");
                }
            }
        }

        public List<Project> ReadAll()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadAllProjects";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            List<Project> entities = new List<Project>();
                            while (reader.Read())
                            {
                                Project project = new Project(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                                project.Tasks = ReadTasksByProjectId(project.ProjectId);
                                entities.Add(project);
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

        public Project ReadById(string id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadProjectById";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@ProjectId", System.Data.SqlDbType.VarChar).Value = id;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Project project = new Project(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                                project.Tasks = ReadTasksByProjectId(project.ProjectId);
                                return project;
                            }
                            throw new Exception("Project not found.");
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Failed to fetch data. Please try again.");
                }
            }
        }

        public void Update(Project entity, string actionUserId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "UpdateProject";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@ProjectName", System.Data.SqlDbType.VarChar).Value = entity.ProjectName;
                        sqlCommand.Parameters.Add("@ProjectId", System.Data.SqlDbType.VarChar).Value = entity.ProjectId;
                        sqlCommand.Parameters.Add("@CurrentBatchNo", System.Data.SqlDbType.VarChar).Value = entity.CurrentBatchNo;
                        sqlCommand.Parameters.Add("@ActionUserId", System.Data.SqlDbType.VarChar).Value = actionUserId;
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Project updating failed. Please try again.");
                }
            }
        }

        public Audit<Project> ReadAuditByCurrentBatchNo(string currentBatchNo)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadProjectShadowByCurrentBatchNo";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@CurrentBatchNo", System.Data.SqlDbType.VarChar).Value = currentBatchNo;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Audit<Project>(reader.GetString(5), reader.GetDateTime(6), reader.GetString(7), new Project(reader.GetString(0), reader.GetString(1), reader.GetString(3), reader.GetString(4)));
                            }
                            throw new Exception("Audit not found.");
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Failed to fetch data. Please try again.");
                }
            }
        }

        private List<Task> ReadTasksByProjectId(string id) {
            using (SqlConnection sqlConnection = new SqlConnection(_connection))
            {
                try
                {
                    string procedure = "ReadTasksByProjectId";
                    using (SqlCommand sqlCommand = new SqlCommand(procedure, sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@ProjectId", System.Data.SqlDbType.VarChar).Value = id;
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            List<Task> entities = new List<Task>();
                            while (reader.Read())
                            {
                                Task task = new Task(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), new TaskStatus(reader.GetString(5), reader.GetString(6)), new Project(reader.GetString(7), string.Empty, string.Empty, string.Empty), reader.GetString(8), reader.GetString(9));
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
    }
}
