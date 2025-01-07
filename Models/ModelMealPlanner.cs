using System.ComponentModel.DataAnnotations;

namespace Licenta2.Models
{
    public class ModelMealPlanner
    {
        [Key]
        public int Id { get; set; }  // Primary Key

        public string UserId { get; set; }  // Foreign Key, should be string to match Identity User ID

        public ModelUser User { get; set; }  // Navigation property

        public ICollection<ModelMealPlannerEntry> Entries { get; set; }  // Navigation property for related entries

        public ModelMealPlanner()
        {
            Entries = new List<ModelMealPlannerEntry>();
        }
    }
}
