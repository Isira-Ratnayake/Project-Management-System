using System.Diagnostics.CodeAnalysis;

namespace ProjectManagementSystem.Models
{
    public abstract class Versioned
    {
        public required string OriginalBatchNo { get; set; }
        public required string CurrentBatchNo { get; set; }

        [SetsRequiredMembers]
        public Versioned(string originalBatchNo, string currentBatchNo)
        {
            OriginalBatchNo = originalBatchNo;
            CurrentBatchNo = currentBatchNo;
        }
    }
}
