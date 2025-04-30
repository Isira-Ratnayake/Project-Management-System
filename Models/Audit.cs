namespace ProjectManagementSystem.Models
{
    public class Audit<T> where T : Versioned
    {
        public string Action {  get; set; }
        public DateTime ActionDateTime { get; set; }
        public string ActionUserId { get; set; }
        public T Image { get; set; }

        public Audit(string action, DateTime actionDateTime, string actionUserId, T image)
        {
            Action = action;
            ActionDateTime = actionDateTime;
            ActionUserId = actionUserId;
            Image = image;
        }
    }
}
