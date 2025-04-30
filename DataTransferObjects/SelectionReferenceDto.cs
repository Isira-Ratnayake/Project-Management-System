namespace ProjectManagementSystem.DataTransferObjects
{
    public class SelectionReferenceDto
    {
        public string? Id { get; set; }
        public string? Description { get; set; }

        public SelectionReferenceDto(string? id, string? description) {
            Id = id;
            Description = description;
        }

        public SelectionReferenceDto() { }
    }
}
