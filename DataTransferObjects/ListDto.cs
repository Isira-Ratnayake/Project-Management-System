namespace ProjectManagementSystem.DataTransferObjects
{
    public abstract class ListDto<T> where T : VersionedDto
    {
        public AuthoritiesDto? Authorities { get; set; }
        public List<T>? Objects { get; set; }

        public ListDto(AuthoritiesDto authorities, List<T> objects) {
            Authorities = authorities;
            Objects = objects;
        }

        public ListDto() { }
    }
}
