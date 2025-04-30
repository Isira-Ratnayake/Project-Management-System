namespace ProjectManagementSystem.DataTransferObjects
{
    public class AuthoritiesDto
    {
        public bool IsCreate {  get; set; }
        public bool IsListAll {  get; set; }
        public bool IsUpdate {  get; set; }
        public bool IsDelete {  get; set; }

        public AuthoritiesDto() { }
    }
}
