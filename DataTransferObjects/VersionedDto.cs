namespace ProjectManagementSystem.DataTransferObjects
{
    public abstract class VersionedDto
    {
        public string? BatchNo { get; set; }
        public string? Created { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }
        public AuthoritiesDto? Authorities { get; set; }

        protected VersionedDto(string? batchNo, string? created, string? createdBy, string? lastModified, string? lastModifiedBy, AuthoritiesDto? authorities) {
            BatchNo = batchNo;
            Created = created;
            CreatedBy = createdBy;
            LastModified = lastModified;
            LastModifiedBy = lastModifiedBy;
            Authorities = authorities;
        }
        protected VersionedDto() { }
    }
}
