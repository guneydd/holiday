using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Holidays.Api.Models;

public class HolidaysCount {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    public string Country { get; set; }
    public int Year { get; set; }
    public int Count { get; set; }
}
